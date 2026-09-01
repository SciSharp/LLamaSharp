using System;
using Microsoft.Win32.SafeHandles;
using LLama.Native;

namespace LLama.Speculative
{
    /// <summary>
    /// A safe handle that securely wraps the unmanaged native pointer for a <c>llama_speculative_context</c>.
    /// <para>This class guarantees that the native memory allocated by the speculative engine is safely freed when the object is disposed or finalized.</para>
    /// </summary>
    public sealed class SafeLLamaSpeculativeContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeLLamaSpeculativeContextHandle"/> class.
        /// </summary>
        /// <param name="handle">The unmanaged <c>IntPtr</c> returned by <c>llama_speculative_init</c>.</param>
        public SafeLLamaSpeculativeContextHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        /// <summary>
        /// Safely releases the unmanaged resources associated with the speculative context.
        /// </summary>
        /// <returns><c>true</c> if the handle was released successfully.</returns>
        protected override bool ReleaseHandle()
        {
            NativeApi.llama_speculative_free(handle);
            return true;
        }
    }
}