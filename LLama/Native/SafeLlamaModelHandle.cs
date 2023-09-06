using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LLama.Exceptions;
using LLama.Extensions;

namespace LLama.Native
{
    /// <summary>
    /// A reference to a set of llama model weights
    /// </summary>
    public sealed class SafeLlamaModelHandle
        : SafeLLamaHandleBase
    {
        /// <summary>
        /// Total number of tokens in vocabulary of this model
        /// </summary>
        public int VocabCount { get; }

        /// <summary>
        /// Total number of tokens in the context
        /// </summary>
        public int ContextSize { get; }

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize { get; }

        internal SafeLlamaModelHandle(IntPtr handle)
            : base(handle)
        {
            VocabCount = NativeApi.llama_model_n_vocab(this);
            ContextSize = NativeApi.llama_model_n_ctx(this);
            EmbeddingSize = NativeApi.llama_model_n_embd(this);
        }

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            NativeApi.llama_free_model(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        /// <summary>
        /// Load a model from the given file path into memory
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="lparams"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public static SafeLlamaModelHandle LoadFromFile(string modelPath, LLamaContextParams lparams)
        {
            var model_ptr = NativeApi.llama_load_model_from_file(modelPath, lparams);
            if (model_ptr == IntPtr.Zero)
                throw new RuntimeError($"Failed to load model {modelPath}.");

            return new SafeLlamaModelHandle(model_ptr);
        }

        #region LoRA
        /// <summary>
        /// Apply a LoRA adapter to a loaded model
        /// </summary>
        /// <param name="lora"></param>
        /// <param name="modelBase">A path to a higher quality model to use as a base for the layers modified by the
        /// adapter. Can be NULL to use the current loaded model.</param>
        /// <param name="threads"></param>
        /// <exception cref="RuntimeError"></exception>
        public void ApplyLoraFromFile(string lora, string? modelBase = null, int threads = -1)
        {
            var err = NativeApi.llama_model_apply_lora_from_file(
                this,
                lora,
                string.IsNullOrEmpty(modelBase) ? null : modelBase,
                threads
            );

            if (err != 0)
                throw new RuntimeError("Failed to apply lora adapter.");
        }
        #endregion

        #region tokenize
        /// <summary>
        /// Convert a single llama token into bytes
        /// </summary>
        /// <param name="llama_token">Token to decode</param>
        /// <param name="dest">A span to attempt to write into. If this is too small nothing will be written</param>
        /// <returns>The size of this token. **nothing will be written** if this is larger than `dest`</returns>
        public int TokenToSpan(int llama_token, Span<byte> dest)
        {
            unsafe
            {
                fixed (byte* destPtr = dest)
                {
                    var length = NativeApi.llama_token_to_piece_with_model(this, llama_token, destPtr, dest.Length);
                    return Math.Abs(length);
                }
            }
        }

        /// <summary>
        /// Convert a single llama token into a string
        /// </summary>
        /// <param name="llama_token"></param>
        /// <param name="encoding">Encoding to use to decode the bytes into a string</param>
        /// <returns></returns>
        public string TokenToString(int llama_token, Encoding encoding)
        {
            unsafe
            {
                var length = NativeApi.llama_token_to_piece_with_model(this, llama_token, null, 0);
                if (length == 0)
                    return "";

                Span<byte> bytes = stackalloc byte[-length];

                fixed (byte* bytePtr = bytes)
                {
                    var written = NativeApi.llama_token_to_piece_with_model(this, llama_token, bytePtr, bytes.Length);
                    Debug.Assert(written == bytes.Length);

                    return encoding.GetString(bytePtr, bytes.Length);
                }
            }
        }

        /// <summary>
        /// Append a single llama token to a string builder
        /// </summary>
        /// <param name="llama_token">Token to decode</param>
        /// <param name="encoding"></param>
        /// <param name="dest">string builder to append the result to</param>
        public void TokenToString(int llama_token, Encoding encoding, StringBuilder dest)
        {
            unsafe
            {
                var length = NativeApi.llama_token_to_piece_with_model(this, llama_token, null, 0);
                if (length == 0)
                    return;

                Span<byte> bytes = stackalloc byte[-length];
                fixed (byte* bytePtr = bytes)
                {
                    // Decode into bytes
                    var written = NativeApi.llama_token_to_piece_with_model(this, llama_token, bytePtr, bytes.Length);
                    Debug.Assert(written == bytes.Length);

                    // Decode into chars
                    var charCount = encoding.GetCharCount(bytePtr, bytes.Length);
                    Span<char> chars = stackalloc char[charCount];
                    fixed (char* charPtr = chars)
                        encoding.GetChars(bytePtr, bytes.Length, charPtr, chars.Length);

                    // Write it to the output
                    for (var i = 0; i < chars.Length; i++)
                        dest.Append(chars[i]);
                }
            }
        }

        /// <summary>
        /// Convert a sequence of tokens into characters. If there 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="dest"></param>
        /// <param name="encoding"></param>
        /// <returns>The section of the span which has valid data in it.
        /// If there was insufficient space in the output span this will be
        /// filled with as many characters as possible, starting from the _last_ token.
        /// </returns>
        internal Span<char> TokensToSpan(IReadOnlyList<int> tokens, Span<char> dest, Encoding encoding)
        {
            // Rent an array to detokenize into
            var tokenBytesArr = ArrayPool<byte>.Shared.Rent(16);
            var tokenCharsArr = ArrayPool<char>.Shared.Rent(16);
            try
            {
                var totalCharacters = 0;
                var unused = dest;

                for (var i = tokens.Count - 1; i >= 0; i--)
                {
                    var token = tokens[i];

                    // Get bytes for this token
                    var tokenBytes = TokenToBytes(ref tokenBytesArr, token, this);

                    // Get chars for this token
                    var tokenChars = BytesToChars(ref tokenCharsArr, tokenBytes, encoding);

                    // Trim down number of characters if there are too many
                    if (tokenChars.Length > unused.Length)
                        tokenChars = tokenChars.Slice(tokenChars.Length - unused.Length, unused.Length);

                    // Copy characters
                    tokenChars.CopyTo(unused.Slice(unused.Length - tokenChars.Length, tokenChars.Length));
                    unused = unused.Slice(0, unused.Length - tokenChars.Length);
                    totalCharacters += tokenChars.Length;

                    // Break out if we've run out of space
                    if (unused.Length == 0)
                        break;
                }

                return dest.Slice(dest.Length - totalCharacters, totalCharacters);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(tokenBytesArr);
                ArrayPool<char>.Shared.Return(tokenCharsArr);
            }
            
            // vvv Local Functions vvv

            static Span<byte> TokenToBytes(ref byte[] bytes, int token, SafeLlamaModelHandle model)
            {
                // Try to get bytes, if that fails we known the length
                var l = model.TokenToSpan(token, bytes);

                // Array was too small, get a bigger one
                if (l < 0)
                {
                    ArrayPool<byte>.Shared.Return(bytes);
                    bytes = ArrayPool<byte>.Shared.Rent(-l * 2);

                    // Get bytes, this time it can't fail
                    l = model.TokenToSpan(token, bytes);
                }

                Debug.Assert(l >= 0);
                return new Span<byte>(bytes, 0, l);
            }

            static Span<char> BytesToChars(ref char[] chars, ReadOnlySpan<byte> bytes, Encoding encoding)
            {
                var count = encoding.GetCharCount(bytes);
                if (count > chars.Length)
                {
                    ArrayPool<char>.Shared.Return(chars);
                    chars = ArrayPool<char>.Shared.Rent(count * 2);
                }

                encoding.GetChars(bytes, chars);
                return chars.AsSpan(0, count);
            }
        }

        /// <summary>
        /// Convert a string of text into tokens
        /// </summary>
        /// <param name="text"></param>
        /// <param name="add_bos"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public int[] Tokenize(string text, bool add_bos, Encoding encoding)
        {
            // Convert string to bytes, adding one extra byte to the end (null terminator)
            var bytesCount = encoding.GetByteCount(text);
            var bytes = new byte[bytesCount + 1];
            unsafe
            {
                fixed (char* charPtr = text)
                fixed (byte* bytePtr = &bytes[0])
                {
                    encoding.GetBytes(charPtr, text.Length, bytePtr, bytes.Length);
                }
            }

            unsafe
            {
                fixed (byte* bytesPtr = &bytes[0])
                {
                    // Tokenize once with no output, to get the token count. Output will be negative (indicating that there was insufficient space)
                    var count = -NativeApi.llama_tokenize_with_model(this, bytesPtr, (int*)IntPtr.Zero, 0, add_bos);

                    // Tokenize again, this time outputting into an array of exactly the right size
                    var tokens = new int[count];
                    fixed (int* tokensPtr = &tokens[0])
                    {
                        NativeApi.llama_tokenize_with_model(this, bytesPtr, tokensPtr, count, add_bos);
                        return tokens;
                    }
                }
            }
        }
        #endregion

        #region context
        /// <summary>
        /// Create a new context for this model
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        public SafeLLamaContextHandle CreateContext(LLamaContextParams @params)
        {
            return SafeLLamaContextHandle.Create(this, @params);
        }
        #endregion
    }
}
