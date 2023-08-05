using System;
using LLama.Exceptions;

namespace LLama.Native
{
    /// <summary>
    /// A safe wrapper around a llama_context
    /// </summary>
    public class SafeLLamaContextHandle
        : SafeLLamaHandleBase
    {
        /// <summary>
        /// This field guarantees that a reference to the model is held for as long as this handle is held
        /// </summary>
        private SafeLlamaModelHandle? _model;

        /// <summary>
        /// Create a new SafeLLamaContextHandle
        /// </summary>
        /// <param name="handle">pointer to an allocated llama_context</param>
        /// <param name="model">the model which this context was created from</param>
        public SafeLLamaContextHandle(IntPtr handle, SafeLlamaModelHandle model)
            : base(handle)
        {
            // Increment the model reference count while this context exists
            _model = model;
            var success = false;
            _model.DangerousAddRef(ref success);
            if (!success)
                throw new RuntimeError("Failed to increment model refcount");
        }

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            // Decrement refcount on model
            _model?.DangerousRelease();
            _model = null;

            NativeApi.llama_free(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        /// <summary>
        /// Create a new llama_state for the given model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="lparams"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public static SafeLLamaContextHandle Create(SafeLlamaModelHandle model, LLamaContextParams lparams)
        {
            var ctx_ptr = NativeApi.llama_new_context_with_model(model, lparams);
            if (ctx_ptr == IntPtr.Zero)
                throw new RuntimeError("Failed to create context from model");

            return new(ctx_ptr, model);
        }
    }
}
