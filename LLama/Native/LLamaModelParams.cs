using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// A C# representation of the llama.cpp `llama_model_params` struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct LLamaModelParams
    {
        /// <summary>
        /// // number of layers to store in VRAM
        /// </summary>
        public int n_gpu_layers;

        /// <summary>
        /// how to split the model across multiple GPUs
        /// </summary>
        public GPUSplitMode split_mode;

        /// <summary>
        /// the GPU that is used for scratch and small tensors
        /// </summary>
        public int main_gpu;

        /// <summary>
        /// how to split layers across multiple GPUs (size: <see cref="NativeApi.llama_max_devices"/>)
        /// </summary>
        public float* tensor_split;

        /// <summary>
        /// called with a progress value between 0 and 1, pass NULL to disable. If the provided progress_callback
        /// returns true, model loading continues. If it returns false, model loading is immediately aborted.
        /// </summary>
        public LlamaProgressCallback progress_callback;

        /// <summary>
        /// context pointer passed to the progress callback
        /// </summary>
        public void* progress_callback_user_data;

        /// <summary>
        /// override key-value pairs of the model meta data
        /// </summary>
        public LLamaModelMetadataOverride* kv_overrides;

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
    }
}
