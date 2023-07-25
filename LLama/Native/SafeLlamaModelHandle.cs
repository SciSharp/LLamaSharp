using System;
using LLama.Exceptions;

namespace LLama.Native
{
    public class SafeLlamaModelHandle
        : SafeLLamaHandleBase
    {
        public SafeLlamaModelHandle(IntPtr handle)
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

        public static SafeLlamaModelHandle LoadFromFile(string modelPath, LLamaContextParams lparams)
        {
            var model_ptr = NativeApi.llama_load_model_from_file(modelPath, lparams);
            if (model_ptr == null)
                throw new RuntimeError($"Failed to load model {modelPath}.");

            return new SafeLlamaModelHandle(model_ptr);
        }
    }
}
