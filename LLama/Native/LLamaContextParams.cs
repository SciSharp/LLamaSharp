using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    public delegate void LlamaProgressCallback(float progress, IntPtr ctx);

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
        /// grouped-query attention (TEMP - will be moved to model hparams)
        /// </summary>
        public int n_gqa;

        /// <summary>
        /// rms norm epsilon (TEMP - will be moved to model hparams)
        /// </summary>
        public float rms_norm_eps;

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
            get => Utils.SignedByteToBool(_low_vram);
            set => _low_vram = Utils.BoolToSignedByte(value);
        }
        private sbyte _low_vram;

        /// <summary>
        /// if true, use experimental mul_mat_q kernels
        /// </summary>
        public bool mul_mat_q
        {
            get => Utils.SignedByteToBool(_mul_mat_q);
            set => _mul_mat_q = Utils.BoolToSignedByte(value);
        }
        private sbyte _mul_mat_q;

        /// <summary>
        /// use fp16 for KV cache
        /// </summary>
        public bool f16_kv
        {
            get => Utils.SignedByteToBool(_f16_kv);
            set => _f16_kv = Utils.BoolToSignedByte(value);
        }
        private sbyte _f16_kv;

        /// <summary>
        /// the llama_eval() call computes all logits, not just the last one
        /// </summary>
        public bool logits_all
        {
            get => Utils.SignedByteToBool(_logits_all);
            set => _logits_all = Utils.BoolToSignedByte(value);
        }
        private sbyte _logits_all;

        /// <summary>
        /// only load the vocabulary, no weights
        /// </summary>
        public bool vocab_only
        {
            get => Utils.SignedByteToBool(_vocab_only);
            set => _vocab_only = Utils.BoolToSignedByte(value);
        }
        private sbyte _vocab_only;

        /// <summary>
        /// use mmap if possible
        /// </summary>
        public bool use_mmap
        {
            get => Utils.SignedByteToBool(_use_mmap);
            set => _use_mmap = Utils.BoolToSignedByte(value);
        }
        private sbyte _use_mmap;

        /// <summary>
        /// force system to keep model in RAM
        /// </summary>
        public bool use_mlock
        {
            get => Utils.SignedByteToBool(_use_mlock);
            set => _use_mlock = Utils.BoolToSignedByte(value);
        }
        private sbyte _use_mlock;

        /// <summary>
        /// embedding mode only
        /// </summary>
        public bool embedding
        {
            get => Utils.SignedByteToBool(_embedding);
            set => _embedding = Utils.BoolToSignedByte(value);
        }
        private sbyte _embedding;
    }
}

