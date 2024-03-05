using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.LLava
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe
    public class LLavaImageEmbed
    {
        public float* embed;
        public int n_image_pos;
    }
}
