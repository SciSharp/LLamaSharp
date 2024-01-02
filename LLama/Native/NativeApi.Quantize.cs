using System.Runtime.InteropServices;

namespace LLama.Native
{
    public static partial class NativeApi
    {
        /// <summary>
        /// Returns 0 on success
        /// </summary>
        /// <param name="fname_inp"></param>
        /// <param name="fname_out"></param>
        /// <param name="param"></param>
        /// <remarks>not great API - very likely to change</remarks>
        /// <returns>Returns 0 on success</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int llama_model_quantize(string fname_inp, string fname_out, LLamaModelQuantizeParams* param);
    }
}
