using System;
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

        public int ContextSize { get; set; }

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
    }
}
