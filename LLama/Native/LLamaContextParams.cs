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
        public int seed;

        /// <summary>
        /// text context
        /// </summary>
        public int n_ctx;

        /// <summary>
        /// prompt processing batch size
        /// </summary>
        public int n_batch;

        /// <summary>
        /// number of layers to store in VRAM
        /// </summary>
        public int n_gpu_layers;

        /// <summary>
        /// the GPU that is used for scratch and small tensors
        /// </summary>
        public int main_gpu;

        /// <summary>
        /// how to split layers across multiple GPUs
        /// </summary>
        public nint tensor_split;

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
        /// called with a progress value between 0 and 1, pass NULL to disable
        /// </summary>
        public IntPtr progress_callback;

        /// <summary>
        /// context pointer passed to the progress callback
        /// </summary>
        public IntPtr progress_callback_user_data;

        /// <summary>
        /// if true, reduce VRAM usage at the cost of performance
        /// </summary>
        public bool low_vram
        { 
            readonly get => Convert.ToBoolean(_low_vram);
            set => _low_vram = Convert.ToSByte(value);
        }
        private sbyte _low_vram;

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
        /// only load the vocabulary, no weights
        /// </summary>
        public bool vocab_only
        { 
            readonly get => Convert.ToBoolean(_vocab_only);
            set => _vocab_only = Convert.ToSByte(value);
        }
        private sbyte _vocab_only;

        /// <summary>
        /// use mmap if possible
        /// </summary>
        public bool use_mmap
        { 
            readonly get => Convert.ToBoolean(_use_mmap);
            set => _use_mmap = Convert.ToSByte(value);
        }
        private sbyte _use_mmap;

        /// <summary>
        /// force system to keep model in RAM
        /// </summary>
        public bool use_mlock
        { 
            readonly get => Convert.ToBoolean(_use_mlock);
            set => _use_mlock = Convert.ToSByte(value);
        }
        private sbyte _use_mlock;

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

