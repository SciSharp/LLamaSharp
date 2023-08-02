using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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
        [MarshalAs(UnmanagedType.LPArray)]
        public float[] tensor_split;

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
        [MarshalAs(UnmanagedType.I1)]
        public bool low_vram;

        /// <summary>
        /// use fp16 for KV cache
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool f16_kv;

        /// <summary>
        /// the llama_eval() call computes all logits, not just the last one
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool logits_all;

        /// <summary>
        /// only load the vocabulary, no weights
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] 
        public bool vocab_only;

        /// <summary>
        /// use mmap if possible
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] 
        public bool use_mmap;

        /// <summary>
        /// force system to keep model in RAM
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] 
        public bool use_mlock;

        /// <summary>
        /// embedding mode only
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] 
        public bool embedding;
    }

    public struct TensorSplits
    {
        public float Item1;
    }
}
