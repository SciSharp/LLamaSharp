using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Native
{
    public abstract class SafeLLamaHandleBase: SafeHandle
    {
        private protected SafeLLamaHandleBase()
            : base(IntPtr.Zero, ownsHandle: true)
        {
        }

        private protected SafeLLamaHandleBase(IntPtr handle)
            : base(IntPtr.Zero, ownsHandle: true)
        {
            SetHandle(handle);
        }

        private protected SafeLLamaHandleBase(IntPtr handle, bool ownsHandle)
            : base(IntPtr.Zero, ownsHandle)
        {
            SetHandle(handle);
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        public override string ToString()
            => $"0x{handle.ToString("x16")}";
    }
}
