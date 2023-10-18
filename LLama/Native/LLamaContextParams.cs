using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// Called by llama.cpp with a progress value between 0 and 1
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="ctx"></param>
    public delegate void LlamaProgressCallback(float progress, IntPtr ctx);

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
        /// text context
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
        /// ref: https://github.com/ggerganov/llama.cpp/pull/2054
        /// RoPE base frequency
        /// </summary>
        public float rope_freq_base;

        /// <summary>
        /// ref: https://github.com/ggerganov/llama.cpp/pull/2054
        /// RoPE frequency scaling factor
        /// </summary>
        public float rope_freq_scale; 

        /// <summary>
        /// if true, use experimental mul_mat_q kernels
        /// </summary>
        public bool mul_mat_q
        { 
            readonly get => Convert.ToBoolean(_mul_mat_q);
            set => _mul_mat_q = Convert.ToSByte(value);
        }
        private sbyte _mul_mat_q;

        /// <summary>
        /// use fp16 for KV cache
        /// </summary>
        public bool f16_kv
        { 
            readonly get => Convert.ToBoolean(_f16_kv);
            set => _f16_kv = Convert.ToSByte(value);
        }
        private sbyte _f16_kv;

        /// <summary>
        /// the llama_eval() call computes all logits, not just the last one
        /// </summary>
        public bool logits_all
        { 
            readonly get => Convert.ToBoolean(_logits_all);
            set => _logits_all = Convert.ToSByte(value);
        }
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
    }
}

