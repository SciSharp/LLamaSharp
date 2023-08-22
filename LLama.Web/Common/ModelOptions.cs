using LLama.Abstractions;

namespace LLama.Web.Common
{
    public class ModelOptions
        : IModelParams
    {
        public string Name { get; set; }
        public int MaxInstances { get; set; }


        /// <summary>
        /// Model context size (n_ctx)
        /// </summary>
        public int ContextSize { get; set; } = 512;
        /// <summary>
        /// the GPU that is used for scratch and small tensors
        /// </summary>
        public int MainGpu { get; set; } = 0;
        /// <summary>
        /// if true, reduce VRAM usage at the cost of performance
        /// </summary>
        public bool LowVram { get; set; } = false;
        /// <summary>
        /// Number of layers to run in VRAM / GPU memory (n_gpu_layers)
        /// </summary>
        public int GpuLayerCount { get; set; } = 20;
        /// <summary>
        /// Seed for the random number generator (seed)
        /// </summary>
        public int Seed { get; set; } = 1686349486;
        /// <summary>
        /// Use f16 instead of f32 for memory kv (memory_f16)
        /// </summary>
        public bool UseFp16Memory { get; set; } = true;
        /// <summary>
        /// Use mmap for faster loads (use_mmap)
        /// </summary>
        public bool UseMemorymap { get; set; } = true;
        /// <summary>
        /// Use mlock to keep model in memory (use_mlock)
        /// </summary>
        public bool UseMemoryLock { get; set; } = false;
        /// <summary>
        /// Compute perplexity over the prompt (perplexity)
        /// </summary>
        public bool Perplexity { get; set; } = false;
        /// <summary>
        /// Model path (model)
        /// </summary>
        public string ModelPath { get; set; }
        /// <summary>
        /// model alias
        /// </summary>
        public string ModelAlias { get; set; } = "unknown";
        /// <summary>
        /// lora adapter path (lora_adapter)
        /// </summary>
        public string LoraAdapter { get; set; } = string.Empty;
        /// <summary>
        /// base model path for the lora adapter (lora_base)
        /// </summary>
        public string LoraBase { get; set; } = string.Empty;
        /// <summary>
        /// Number of threads (-1 = autodetect) (n_threads)
        /// </summary>
        public int Threads { get; set; } = Math.Max(Environment.ProcessorCount / 2, 1);
        /// <summary>
        /// batch size for prompt processing (must be >=32 to use BLAS) (n_batch)
        /// </summary>
        public int BatchSize { get; set; } = 512;

        /// <summary>
        /// Whether to convert eos to newline during the inference.
        /// </summary>
        public bool ConvertEosToNewLine { get; set; } = false;

        /// <summary>
        /// Whether to use embedding mode. (embedding) Note that if this is set to true, 
        /// The LLamaModel won't produce text response anymore.
        /// </summary>
        public bool EmbeddingMode { get; set; } = false;

        /// <summary>
        /// how split tensors should be distributed across GPUs
        /// </summary>
        public float[] TensorSplits { get; set; }

        /// <summary>
        /// Grouped-Query Attention
        /// </summary>
        public int GroupedQueryAttention { get; set; } = 1;

        /// <summary>
        /// RMS Norm Epsilon
        /// </summary>
        public float RmsNormEpsilon { get; set; } = 5e-6f;

        /// <summary>
        /// RoPE base frequency
        /// </summary>
        public float RopeFrequencyBase { get; set; } = 10000.0f;

        /// <summary>
        /// RoPE frequency scaling factor
        /// </summary>
        public float RopeFrequencyScale { get; set; } = 1.0f;

        /// <summary>
        /// Use experimental mul_mat_q kernels
        /// </summary>
        public bool MulMatQ { get; set; }

        /// <summary>
        /// The encoding to use for models
        /// </summary>
        public string Encoding { get; set; } = "UTF-8";
    }
}
