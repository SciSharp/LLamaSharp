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
        /// text context
        /// </summary>
        public int n_ctx;
        /// <summary>
        /// number of layers to store in VRAM
        /// </summary>
        public int n_gpu_layers;
        /// <summary>
        /// RNG seed, -1 for random
        /// </summary>
        public int seed;

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

        /// <summary>
        /// called with a progress value between 0 and 1, pass NULL to disable
        /// </summary>
        public IntPtr progress_callback;
        /// <summary>
        /// context pointer passed to the progress callback
        /// </summary>
        public IntPtr progress_callback_user_data;
    }
}
