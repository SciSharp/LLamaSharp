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
        public bool allow_requantize
        {
            get => Utils.SignedByteToBool(_allow_requantize);
            set => _allow_requantize = Utils.BoolToSignedByte(value);
        }
        private sbyte _allow_requantize;

        /// <summary>
        /// quantize output.weight
        /// </summary>
        public bool quantize_output_tensor
        {
            get => Utils.SignedByteToBool(_quantize_output_tensor);
            set => _quantize_output_tensor = Utils.BoolToSignedByte(value);
        }
        private sbyte _quantize_output_tensor;
    }
}
