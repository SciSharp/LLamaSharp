using System;

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
        /// output tensor type
        /// </summary>
        public GGMLType output_tensor_type;

        /// <summary>
        /// token embeddings tensor type
        /// </summary>
        public GGMLType token_embedding_type;

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
        /// quantize all tensors to the default type
        /// </summary>
        public bool pure
        {
            get => Convert.ToBoolean(_pure);
            set => _pure = Convert.ToSByte(value);
        }
        private sbyte _pure;

        /// <summary>
        /// quantize to the same number of shards
        /// </summary>
        public bool keep_split
        {
            get => Convert.ToBoolean(_keep_split);
            set => _keep_split = Convert.ToSByte(value);
        }
        private sbyte _keep_split;

        /// <summary>
        /// pointer to importance matrix data
        /// </summary>
        public IntPtr imatrix;

        /// <summary>
        /// pointer to vector containing overrides
        /// </summary>
        public IntPtr kv_overrides;

        /// <summary>
        /// pointer to vector containing tensor types
        /// </summary>
        public IntPtr tensor_types;

        /// <summary>
        /// Pointer to vector containing layer indices to prune
        /// </summary>
        public IntPtr prune_layers;

        /// <summary>
        /// Create a LLamaModelQuantizeParams with default values
        /// </summary>
        /// <returns></returns>
        public static LLamaModelQuantizeParams Default()
        {
            return llama_model_quantize_default_params();

            [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
            static extern LLamaModelQuantizeParams llama_model_quantize_default_params();
        }
    }
}
