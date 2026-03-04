using System;

namespace LLama.Native
{
    /// <summary>
    /// A C# representation of the llama.cpp `llama_model_params` struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct LLamaModelParams
    {
        /// <summary>
        /// NULL-terminated list of devices to use for offloading (if NULL, all available devices are used)
        /// todo: add support for llama_model_params.devices
        /// </summary>
        private IntPtr devices;

        /// <summary>
        /// NULL-terminated list of buffer types to use for tensors that match a pattern
        /// </summary>
        public LLamaModelTensorBufferOverride* tensor_buft_overrides;

        /// <summary>
        /// // number of layers to store in VRAM
        /// </summary>
        public int n_gpu_layers;

        /// <summary>
        /// how to split the model across multiple GPUs
        /// </summary>
        public GPUSplitMode split_mode;

        /// <summary>
        /// the GPU that is used for the entire model when split_mode is LLAMA_SPLIT_MODE_NONE
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
#if NETSTANDARD2_0
        // this code is intended to be used when running LlamaSharp on NET Framework 4.8 (NET Standard 2.0) 
        // as NET Framework 4.8 does not play nice with the LlamaProgressCallback type
        public IntPtr progress_callback;
#else
        public LlamaProgressCallback? progress_callback;
#endif

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
        /// use direct io, takes precedence over use_mmap when supported
        /// </summary>
        public bool use_direct_io
        {
            readonly get => Convert.ToBoolean(_use_direct_io);
            set => _use_direct_io = Convert.ToSByte(value);
        }
        private sbyte _use_direct_io;

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
        /// validate model tensor data
        /// </summary>
        public bool check_tensors
        {
            readonly get => Convert.ToBoolean(_check_tensors);
            set => _check_tensors = Convert.ToSByte(value);
        }
        private sbyte _check_tensors;
        
        /// <summary>
        /// use extra buffer types (used for weight repacking) 
        /// </summary>
        public bool use_extra_bufts
        {
            readonly get => Convert.ToBoolean(_use_extra_bufts);
            set => _use_extra_bufts = Convert.ToSByte(value);
        }
        private sbyte _use_extra_bufts;

        /// <summary>
        /// bypass host buffer allowing extra buffers to be used
        /// </summary>
        public bool no_host
        {
            readonly get => Convert.ToBoolean(_no_host);
            set => _no_host = Convert.ToSByte(value);
        }
        private sbyte _no_host;

        /// <summary>
        /// only load metadata and simulate memory allocations
        /// </summary>
        public bool no_alloc
        {
            readonly get => Convert.ToBoolean(_no_alloc);
            set => _no_alloc = Convert.ToSByte(value);
        }
        private sbyte _no_alloc;
        /// <summary>
        /// Create a LLamaModelParams with default values
        /// </summary>
        /// <returns></returns>
        public static LLamaModelParams Default()
        {
            return llama_model_default_params();

            [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
            static extern LLamaModelParams llama_model_default_params();
        }
    }
}
