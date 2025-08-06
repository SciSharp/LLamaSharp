using System;

namespace LLama.Native
{
    /// <summary>
    /// Called by llama.cpp with a progress value between 0 and 1
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="ctx"></param>
    /// <returns>If the provided progress_callback returns true, model loading continues.
    /// If it returns false, model loading is immediately aborted.</returns>
    /// <remarks>llama_progress_callback</remarks>
    public delegate bool LlamaProgressCallback(float progress, IntPtr ctx);

    /// <summary>
    /// A C# representation of the llama.cpp `llama_context_params` struct
    /// </summary>
    /// <remarks>changing the default values of parameters marked as [EXPERIMENTAL] may cause crashes or incorrect results in certain configurations
    /// https://github.com/ggerganov/llama.cpp/pull/7544</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct LLamaContextParams
    {
        /// <summary>
        /// text context, 0 = from model
        /// </summary>
        public uint n_ctx;

        /// <summary>
        /// logical maximum batch size that can be submitted to llama_decode
        /// </summary>
        public uint n_batch;

        /// <summary>
        /// physical maximum batch size
        /// </summary>
        public uint n_ubatch;

        /// <summary>
        /// max number of sequences (i.e. distinct states for recurrent models)
        /// </summary>
        public uint n_seq_max;

        /// <summary>
        /// number of threads to use for generation
        /// </summary>
        public int n_threads;

        /// <summary>
        /// number of threads to use for batch processing
        /// </summary>
        public int n_threads_batch;

        /// <summary>
        /// RoPE scaling type, from `enum llama_rope_scaling_type` 
        /// </summary>
        public RopeScalingType rope_scaling_type;

        /// <summary>
        /// whether to pool (sum) embedding results by sequence id
        /// </summary>
        public LLamaPoolingType llama_pooling_type;

        /// <summary>
        /// Attention type to use for embeddings
        /// </summary>
        public LLamaAttentionType attention_type;
        
        /// <summary>
        /// RoPE base frequency, 0 = from model
        /// </summary>
        public float rope_freq_base;

        /// <summary>
        /// RoPE frequency scaling factor, 0 = from model
        /// </summary>
        public float rope_freq_scale;

        /// <summary>
        /// YaRN extrapolation mix factor, negative = from model
        /// </summary>
        public float yarn_ext_factor;

        /// <summary>
        /// YaRN magnitude scaling factor
        /// </summary>
        public float yarn_attn_factor;

        /// <summary>
        /// YaRN low correction dim
        /// </summary>
        public float yarn_beta_fast;

        /// <summary>
        /// YaRN high correction dim
        /// </summary>
        public float yarn_beta_slow;
        
        /// <summary>
        /// YaRN original context size
        /// </summary>
        public uint yarn_orig_ctx;

        /// <summary>
        /// defragment the KV cache if holes/size &gt; defrag_threshold, Set to &lt;= 0 to disable (default)
        /// </summary>
        public float defrag_threshold;

        //todo: implement cb_eval callback support
        /// <summary>
        /// ggml_backend_sched_eval_callback
        /// </summary>
        public IntPtr cb_eval;

        //todo: implement cb_eval callback support
        /// <summary>
        /// User data passed into cb_eval
        /// </summary>
        public IntPtr cb_eval_user_data;

        /// <summary>
        /// data type for K cache. <b>EXPERIMENTAL</b>
        /// </summary>
        public GGMLType type_k;

        /// <summary>
        /// data type for V cache. <b>EXPERIMENTAL</b>
        /// </summary>
        public GGMLType type_v;

        //todo: implement abort callback support
        /// <summary>
        /// ggml_abort_callback
        /// </summary>
        public IntPtr abort_callback;

        //todo: implement abort callback support
        /// <summary>
        /// User data passed into abort_callback
        /// </summary>
        public IntPtr abort_callback_user_data;

        /// <summary>
        /// if true, extract embeddings (together with logits)
        /// </summary>
        public bool embeddings
        { 
            readonly get => Convert.ToBoolean(_embeddings);
            set => _embeddings = Convert.ToSByte(value);
        }
        private sbyte _embeddings;

        /// <summary>
        /// whether to offload the KQV ops (including the KV cache) to GPU
        /// </summary>
        public bool offload_kqv
        {
            readonly get => Convert.ToBoolean(_offload_kqv);
            set => _offload_kqv = Convert.ToSByte(value);
        }
        private sbyte _offload_kqv;

        /// <summary>
        /// whether to use flash attention. <b>EXPERIMENTAL</b>
        /// </summary>
        public bool flash_attention
        {
            readonly get => Convert.ToBoolean(_flash_attention);
            set => _flash_attention = Convert.ToSByte(value);
        }
        private sbyte _flash_attention;

        /// <summary>
        /// whether to measure performance timings
        /// </summary>
        public bool no_perf
        {
            readonly get => Convert.ToBoolean(_no_perf);
            set => _no_perf = Convert.ToSByte(value);
        }
        private sbyte _no_perf;

        /// <summary>
        /// offload host tensor operations to device
        /// </summary>
        public bool op_offload
        {
            readonly get => Convert.ToBoolean(_op_offload);
            set => _op_offload = Convert.ToSByte(value);
        }
        private sbyte _op_offload;

        /// <summary>
        /// use full-size SWA cache (https://github.com/ggml-org/llama.cpp/pull/13194#issuecomment-2868343055)
        /// NOTE: setting to false when n_seq_max > 1 can cause bad performance in some cases
        ///       ref: https://github.com/ggml-org/llama.cpp/pull/13845#issuecomment-2924800573
        /// </summary>
        public bool swa_full
        {
            readonly get => Convert.ToBoolean(_swa_full);
            set => _swa_full = Convert.ToSByte(value);
        }
        private sbyte _swa_full;

        /// <summary>
        /// use a unified buffer across the input sequences when computing the attention.
        /// try to disable when n_seq_max > 1 for improved performance when the sequences do not share a large prefix
        /// <br />
        /// ref: https://github.com/ggml-org/llama.cpp/pull/14363
        /// </summary>
        public bool kv_unified
        {
            readonly get => Convert.ToBoolean(_kv_unified);
            set => _kv_unified = Convert.ToSByte(value);
        }
        private sbyte _kv_unified;

        /// <summary>
        /// Get the default LLamaContextParams
        /// </summary>
        /// <returns></returns>
        public static LLamaContextParams Default()
        {
            return llama_context_default_params();

            [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
            static extern LLamaContextParams llama_context_default_params();
        }
    }
}

