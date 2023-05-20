using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Native
{
    public partial class NativeApi
    {
        /// <summary>
        /// Returns 0 on success
        /// </summary>
        /// <param name="fname_inp"></param>
        /// <param name="fname_out"></param>
        /// <param name="ftype"></param>
        /// <param name="nthread">how many threads to use. If <=0, will use std::thread::hardware_concurrency(), else the number given</param>
        /// <remarks>not great API - very likely to change</remarks>
        /// <returns>Returns 0 on success</returns>
        [DllImport(libraryName)]
        public static extern int llama_model_quantize(string fname_inp, string fname_out, LLamaFtype ftype, int nthread);
    }
}
