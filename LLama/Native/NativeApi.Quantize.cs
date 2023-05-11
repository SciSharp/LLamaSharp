using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Native
{
    internal partial class NativeApi
    {
        [DllImport(libraryName)]
        public static extern bool ggml_custom_quantize(string src_filename, string dst_filename,
            string ftype_str, int nthread, bool print_info);
    }
}
