using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Common
{
    /// <summary>
    /// The parameters for initializing a LLama model.
    /// </summary>
    public class ModelParams
    {
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
        public float[] TensorSplits { get; set; } = new float[] { 0 };

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
        /// 
        /// </summary>
        /// <param name="modelPath">The model path.</param>
        /// <param name="contextSize">Model context size (n_ctx)</param>
        /// <param name="gpuLayerCount">Number of layers to run in VRAM / GPU memory (n_gpu_layers)</param>
        /// <param name="seed">Seed for the random number generator (seed)</param>
        /// <param name="useFp16Memory">Whether to use f16 instead of f32 for memory kv (memory_f16)</param>
        /// <param name="useMemorymap">Whether to use mmap for faster loads (use_mmap)</param>
        /// <param name="useMemoryLock">Whether to use mlock to keep model in memory (use_mlock)</param>
        /// <param name="perplexity">Thether to compute perplexity over the prompt (perplexity)</param>
        /// <param name="loraAdapter">Lora adapter path (lora_adapter)</param>
        /// <param name="loraBase">Base model path for the lora adapter (lora_base)</param>
        /// <param name="threads">Number of threads (-1 = autodetect) (n_threads)</param>
        /// <param name="batchSize">Batch size for prompt processing (must be >=32 to use BLAS) (n_batch)</param>
        /// <param name="convertEosToNewLine">Whether to convert eos to newline during the inference.</param>
        /// <param name="embeddingMode">Whether to use embedding mode. (embedding) Note that if this is set to true, The LLamaModel won't produce text response anymore.</param>
        public ModelParams(string modelPath, int contextSize = 512, int gpuLayerCount = 20,
                   int seed = 1337, bool useFp16Memory = true,
                   bool useMemorymap = true, bool useMemoryLock = false, bool perplexity = false,
                   string loraAdapter = "", string loraBase = "", int threads = -1, int batchSize = 512,
                   bool convertEosToNewLine = false, bool embeddingMode = false)
        {
            ContextSize = contextSize;
            GpuLayerCount = gpuLayerCount;
            Seed = seed;
            UseFp16Memory = useFp16Memory;
            UseMemorymap = useMemorymap;
            UseMemoryLock = useMemoryLock;
            Perplexity = perplexity;
            ModelPath = modelPath;
            LoraAdapter = loraAdapter;
            LoraBase = loraBase;
            Threads = threads == -1 ? Math.Max(Environment.ProcessorCount / 2, 1) : threads;
            BatchSize = batchSize;
            ConvertEosToNewLine = convertEosToNewLine;
            EmbeddingMode = embeddingMode;
        }
    }
}
