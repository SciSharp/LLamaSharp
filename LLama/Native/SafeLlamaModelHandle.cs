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
        internal SafeLlamaModelHandle(IntPtr handle)
            : base(handle)
        {
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
    }
}
