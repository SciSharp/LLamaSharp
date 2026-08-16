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
        /// <returns>Returns 0 on success</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint llama_model_quantize(string fname_inp, string fname_out, ref LLamaModelQuantizeParams param);

        /// <summary>
        /// Get the model file type (quantization) as a string, e.g. "Q8_0" or "Q4_K - Medium"
        /// </summary>
        /// <param name="ftype"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern string llama_ftype_name(LLamaFtype ftype);
    }
}
