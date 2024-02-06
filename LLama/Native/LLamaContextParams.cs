using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// Called by llama.cpp with a progress value between 0 and 1
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="ctx"></param>
    /// <remarks>llama_progress_callback</remarks>
    public delegate bool LlamaProgressCallback(float progress, IntPtr ctx);

    /// <summary>
    /// A C# representation of the llama.cpp `llama_context_params` struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LLamaContextParams
    {
        /// <summary>
        /// RNG seed, -1 for random
        /// </summary>
        public uint seed;

        /// <summary>
        /// text context, 0 = from model
        /// </summary>
        public uint n_ctx;

        /// <summary>
        /// prompt processing batch size
        /// </summary>
        public uint n_batch;

        /// <summary>
        /// number of threads to use for generation
        /// </summary>
        public uint n_threads;

        /// <summary>
        /// number of threads to use for batch processing
        /// </summary>
        public uint n_threads_batch;

        /// <summary>
        /// RoPE scaling type, from `enum llama_rope_scaling_type` 
        /// </summary>
        public RopeScalingType rope_scaling_type;        
        
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
        /// ggml_backend_sched_eval_callback
        /// </summary>
        public IntPtr cb_eval;

        /// <summary>
        /// User data passed into cb_eval
        /// </summary>
        public IntPtr cb_eval_user_data;

        /// <summary>
        /// data type for K cache
        /// </summary>
        public GGMLType type_k;

        /// <summary>
        /// data type for V cache
        /// </summary>
        public GGMLType type_v;

        /// <summary>
        /// Deprecated!
        /// </summary>
        private sbyte _mul_mat_q;

        /// <summary>
        /// Deprecated!
        /// </summary>
        private sbyte _logits_all;

        /// <summary>
        /// embedding mode only
        /// </summary>
        public bool embedding
        { 
            readonly get => Convert.ToBoolean(_embedding);
            set => _embedding = Convert.ToSByte(value);
        }
        private sbyte _embedding;

        /// <summary>
        /// whether to offload the KQV ops (including the KV cache) to GPU
        /// </summary>
        public bool offload_kqv
        {
            readonly get => Convert.ToBoolean(_offload_kqv);
            set => _offload_kqv = Convert.ToSByte(value);
        }
        private sbyte _offload_kqv;
    }
}

