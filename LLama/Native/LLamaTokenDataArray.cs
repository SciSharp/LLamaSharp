using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LLamaTokenDataArray
    {
        public Memory<LLamaTokenData> data;
        public ulong size;
        [MarshalAs(UnmanagedType.I1)]
        public bool sorted;

        public LLamaTokenDataArray(LLamaTokenData[] data, ulong size, bool sorted)
        {
            this.data = data;
            this.size = size;
            this.sorted = sorted;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct LLamaTokenDataArrayNative
    {
        public IntPtr data;
        public ulong size;
        public bool sorted;
    }
}
