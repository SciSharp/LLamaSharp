using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Native
{
    internal struct GgmlInitParams
    {
        public ulong mem_size;
        public IntPtr mem_buffer;
        [MarshalAs(UnmanagedType.I1)]
        public bool no_alloc;
    }
}
