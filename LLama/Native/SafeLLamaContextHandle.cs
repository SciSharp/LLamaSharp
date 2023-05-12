using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Native
{
    public class SafeLLamaContextHandle: SafeLLamaHandleBase
    {
        protected SafeLLamaContextHandle()
        {
        }

        public SafeLLamaContextHandle(IntPtr handle)
            : base(handle)
        {
        }

        protected override bool ReleaseHandle()
        {
            NativeApi.llama_free(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }
    }
}
