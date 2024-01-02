using System;
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
        public int VocabCount => NativeApi.llama_n_vocab(this);

        /// <summary>
        /// Total number of tokens in the context
        /// </summary>
        public int ContextSize => NativeApi.llama_n_ctx_train(this);

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => NativeApi.llama_n_embd(this);

        /// <summary>
        /// Get the size of this model in bytes
        /// </summary>
        public ulong SizeInBytes => NativeApi.llama_model_size(this);

        /// <summary>
        /// Get the number of parameters in this model
        /// </summary>
        public ulong ParameterCount => NativeApi.llama_model_n_params(this);

        /// <summary>
        /// Get the number of metadata key/value pairs
        /// </summary>
        /// <returns></returns>
        public int MetadataCount => NativeApi.llama_model_meta_count(this);

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            NativeApi.llama_free_model(DangerousGetHandle());
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
        public static SafeLlamaModelHandle LoadFromFile(string modelPath, LLamaModelParams lparams)
        {
            var model_ptr = NativeApi.llama_load_model_from_file(modelPath, lparams);
            if (model_ptr == null)
                throw new RuntimeError($"Failed to load model {modelPath}.");

            return model_ptr;
        }

        #region LoRA

        /// <summary>
        /// Apply a LoRA adapter to a loaded model
        /// </summary>
        /// <param name="lora"></param>
        /// <param name="scale"></param>
        /// <param name="modelBase">A path to a higher quality model to use as a base for the layers modified by the
        /// adapter. Can be NULL to use the current loaded model.</param>
        /// <param name="threads"></param>
        /// <exception cref="RuntimeError"></exception>
        public void ApplyLoraFromFile(string lora, float scale, string? modelBase = null, int? threads = null)
        {
            var err = NativeApi.llama_model_apply_lora_from_file(
                this,
                lora,
                scale,
                string.IsNullOrEmpty(modelBase) ? null : modelBase,
                threads ?? Math.Max(1, Environment.ProcessorCount / 2)
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
            var length = NativeApi.llama_token_to_piece(this, llama_token, dest);
            return Math.Abs(length);
        }

        /// <summary>
        /// Convert a sequence of tokens into characters.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="dest"></param>
        /// <param name="encoding"></param>
        /// <returns>The section of the span which has valid data in it.
        /// If there was insufficient space in the output span this will be
        /// filled with as many characters as possible, starting from the _last_ token.
        /// </returns>
        [Obsolete("Use a StreamingTokenDecoder instead")]
        internal Span<char> TokensToSpan(IReadOnlyList<int> tokens, Span<char> dest, Encoding encoding)
        {
            var decoder = new StreamingTokenDecoder(encoding, this);

            foreach (var token in tokens)
                decoder.Add(token);

            var str = decoder.Read();

            if (str.Length < dest.Length)
            {
                str.AsSpan().CopyTo(dest);
                return dest.Slice(0, str.Length);
            }
            else
            {
                str.AsSpan().Slice(str.Length - dest.Length).CopyTo(dest);
                return dest;
            }
        }

        /// <summary>
        /// Convert a string of text into tokens
        /// </summary>
        /// <param name="text"></param>
        /// <param name="add_bos"></param>
        /// <param name="encoding"></param>
        /// <param name="special">Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.</param>
        /// <returns></returns>
        public int[] Tokenize(string text, bool add_bos, bool special, Encoding encoding)
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
                    var count = -NativeApi.llama_tokenize(this, bytesPtr, bytesCount, (int*)IntPtr.Zero, 0, add_bos, special);

                    // Tokenize again, this time outputting into an array of exactly the right size
                    var tokens = new int[count];
                    fixed (int* tokensPtr = &tokens[0])
                    {
                        NativeApi.llama_tokenize(this, bytesPtr, bytesCount, tokensPtr, count, add_bos, special);
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

        #region metadata
        /// <summary>
        /// Get the metadata key for the given index
        /// </summary>
        /// <param name="index">The index to get</param>
        /// <returns>The key, null if there is no such key or if the buffer was too small</returns>
        public Memory<byte>? MetadataKeyByIndex(int index)
        {
            int keyLength;
            unsafe
            {
                // Check if the key exists, without getting any bytes of data
                keyLength = NativeApi.llama_model_meta_key_by_index(this, index, Array.Empty<byte>());
                if (keyLength < 0)
                    return null;
            }

            // get a buffer large enough to hold it
            var buffer = new byte[keyLength + 1];
            keyLength = NativeApi.llama_model_meta_key_by_index(this, index, buffer);
            Debug.Assert(keyLength >= 0);

            return buffer.AsMemory().Slice(0, keyLength);
        }

        /// <summary>
        /// Get the metadata value for the given index
        /// </summary>
        /// <param name="index">The index to get</param>
        /// <returns>The value, null if there is no such value or if the buffer was too small</returns>
        public Memory<byte>? MetadataValueByIndex(int index)
        {
            // Check if the key exists, without getting any bytes of data
            var valueLength = NativeApi.llama_model_meta_val_str_by_index(this, index, Array.Empty<byte>());
            if (valueLength < 0)
                return null;

            // get a buffer large enough to hold it
            var buffer = new byte[valueLength + 1];
            valueLength = NativeApi.llama_model_meta_val_str_by_index(this, index, buffer);
            Debug.Assert(valueLength >= 0);

            return buffer.AsMemory().Slice(0, valueLength);
        }

        internal IReadOnlyDictionary<string, string> ReadMetadata()
        {
            var result = new Dictionary<string, string>();

            for (var i = 0; i < MetadataCount; i++)
            {
                var keyBytes = MetadataKeyByIndex(i);
                if (keyBytes == null)
                    continue;
                var key = Encoding.UTF8.GetStringFromSpan(keyBytes.Value.Span);

                var valBytes = MetadataValueByIndex(i);
                if (valBytes == null)
                    continue;
                var val = Encoding.UTF8.GetStringFromSpan(valBytes.Value.Span);

                result[key] = val;
            }

            return result;
        }
        #endregion
    }
}
