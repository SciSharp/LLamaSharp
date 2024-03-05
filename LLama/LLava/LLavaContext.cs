using LLama.Native;
using System;
using System.Runtime.InteropServices;

namespace LLama.LLava
{
    [StructLayout(LayoutKind.Sequential)]
    public class LLavaContext
    {
        public IntPtr ClipContext;
        public SafeLLamaContextHandle LLamaContext;
        public SafeLlamaModelHandle model;
    }
}
