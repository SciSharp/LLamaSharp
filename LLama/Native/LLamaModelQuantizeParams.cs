using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// Quantizer parameters used in the native API
    /// </summary>
    /// <remarks>llama_model_quantize_params</remarks>
    [StructLayout(LayoutKind.Sequential)]
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
            get => Convert.ToBoolean(_allow_requantize);
            set => _allow_requantize = Convert.ToSByte(value);
        }
        private sbyte _allow_requantize;

        /// <summary>
        /// quantize output.weight
        /// </summary>
        public bool quantize_output_tensor
        {
            get => Convert.ToBoolean(_quantize_output_tensor);
            set => _quantize_output_tensor = Convert.ToSByte(value);
        }
        private sbyte _quantize_output_tensor;

        /// <summary>
        /// only copy tensors - ftype, allow_requantize and quantize_output_tensor are ignored
        /// </summary>
        public bool only_copy
        {
            get => Convert.ToBoolean(_only_copy);
            set => _only_copy = Convert.ToSByte(value);
        }
        private sbyte _only_copy;

        /// <summary>
        /// disable k-quant mixtures and quantize all tensors to the same type
        /// </summary>
        public bool pure
        {
            get => Convert.ToBoolean(_pure);
            set => _pure = Convert.ToSByte(value);
        }
        private sbyte _pure;

        /// <summary>
        /// pointer to importance matrix data
        /// </summary>
        public IntPtr imatrix;
    }
}
