using System;
using System.Text;
using LLama.Exceptions;

namespace LLama.Native
{
    /// <summary>
    /// A reference to a set of llama model weights
    /// </summary>
    public class SafeLlamaModelHandle
        : SafeLLamaHandleBase
    {
        /// <summary>
        /// Total number of tokens in vocabulary of this model
        /// </summary>
        public int VocabCount { get; set; }

        /// <summary>
        /// Total number of tokens in the context
        /// </summary>
        public int ContextSize { get; set; }

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingCount { get; set; }

        internal SafeLlamaModelHandle(IntPtr handle)
            : base(handle)
        {
            VocabCount = NativeApi.llama_n_vocab_from_model(this);
            ContextSize = NativeApi.llama_n_ctx_from_model(this);
            EmbeddingCount = NativeApi.llama_n_embd_from_model(this);
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
        /// Convert a single llama token into string bytes
        /// </summary>
        /// <param name="llama_token"></param>
        /// <returns></returns>
        public ReadOnlySpan<byte> TokenToSpan(int llama_token)
        {
            unsafe
            {
                var bytes = new ReadOnlySpan<byte>(NativeApi.llama_token_to_str_with_model(this, llama_token), int.MaxValue);
                var terminator = bytes.IndexOf((byte)0);
                return bytes.Slice(0, terminator);
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
            var span = TokenToSpan(llama_token);

            if (span.Length == 0)
                return "";

            unsafe
            {
                fixed (byte* ptr = &span[0])
                {
                    return encoding.GetString(ptr, span.Length);
                }
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
                        count = NativeApi.llama_tokenize_with_model(this, bytesPtr, tokensPtr, count, add_bos);
                        return tokens;
                    }
                }
            }
        }
        #endregion
    }
}
