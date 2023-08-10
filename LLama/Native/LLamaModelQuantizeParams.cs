using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// Quantizer parameters used in the native API
    /// </summary>
    public struct LLamaModelQuantizeParams
    {
        /// <summary>
        /// number of threads to use for quantizing, if &lt;=0 will use std::thread::hardware_concurrency()
        /// </summary>
        public int nthread;

        /// <summary>
        /// quantize to this llama_ftype
        /// </summary>
        public LLamaFtype ftype;

        /// <summary>
        /// allow quantizing non-f32/f16 tensors
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool allow_requantize;

        /// <summary>
        /// quantize output.weight
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool quantize_output_tensor;
    }
}
