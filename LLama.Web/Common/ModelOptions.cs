using System.Text;
using LLama.Abstractions;
using LLama.Native;

namespace LLama.Web.Common
{
    public class ModelOptions
        : ILLamaParams
    {
        /// <summary>
        /// Model friendly name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Max context insta=nces allowed per model
        /// </summary>
        public int MaxInstances { get; set; }

        /// <summary>
        /// Model context size (n_ctx)
        /// </summary>
        public uint ContextSize { get; set; } = 512;

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
        public uint Seed { get; set; } = 1686349486;

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
        /// List of LoRAs to apply
        /// </summary>
        public AdapterCollection LoraAdapters { get; set; } = new();

        /// <summary>

        /// base model path for the lora adapter (lora_base)
        /// </summary>
        public string LoraBase { get; set; } = string.Empty;

        /// <summary>
        /// Number of threads (null = autodetect) (n_threads)
        /// </summary>
        public uint? Threads { get; set; }

        /// <summary>
        /// Number of threads to use for batch processing (null = autodetect) (n_threads)
        /// </summary>
        public uint? BatchThreads { get; set; }

        /// <summary>
        /// batch size for prompt processing (must be >=32 to use BLAS) (n_batch)
        /// </summary>
        public uint BatchSize { get; set; } = 512;

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
        public TensorSplitsCollection TensorSplits { get; set; } = new();

        /// <summary>
        /// RoPE base frequency
        /// </summary>
        public float? RopeFrequencyBase { get; set; }

        /// <summary>
        /// RoPE frequency scaling factor
        /// </summary>
        public float? RopeFrequencyScale { get; set; }

        /// <inheritdoc />
        public float? YarnExtrapolationFactor { get; set; }

        /// <inheritdoc />
        public float? YarnAttentionFactor { get; set; }

        /// <inheritdoc />
        public float? YarnBetaFast { get; set; }

        /// <inheritdoc />
        public float? YarnBetaSlow { get; set; }

        /// <inheritdoc />
        public uint? YarnOriginalContext { get; set; }

        /// <inheritdoc />
        public RopeScalingType? YarnScalingType { get; set; }

        /// <summary>
        /// Use experimental mul_mat_q kernels
        /// </summary>
        public bool MulMatQ { get; set; }

        /// <summary>
        /// The encoding to use for models
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Load vocab only (no weights)
        /// </summary>
        public bool VocabOnly { get; set; }
    }
}