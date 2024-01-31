using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// Base class for all llama handles to native resources
    /// </summary>
    public abstract class SafeLLamaHandleBase
        : SafeHandle
    {
        private protected SafeLLamaHandleBase()
            : base(IntPtr.Zero, ownsHandle: true)
        {
        }

        private protected SafeLLamaHandleBase(IntPtr handle, bool ownsHandle)
            : base(IntPtr.Zero, ownsHandle)
        {
            SetHandle(handle);
        }

        /// <inheritdoc />
        public override bool IsInvalid => handle == IntPtr.Zero;

        /// <inheritdoc />
        public override string ToString() => handle.ToString();
    }
}
