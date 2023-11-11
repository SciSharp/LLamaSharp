using LLama.Abstractions;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LLama.Native;

namespace LLama.Common
{
    /// <summary>
    /// The parameters for initializing a LLama model.
    /// </summary>
    public record ModelParams
        : ILLamaParams
    {
        /// <inheritdoc />
        public uint? ContextSize { get; set; }

        /// <inheritdoc />
        public int MainGpu { get; set; } = 0;

        /// <inheritdoc />
        public int GpuLayerCount { get; set; } = 20;

        /// <inheritdoc />
        public uint Seed { get; set; } = 0xFFFFFFFF;

        /// <inheritdoc />
        public bool UseFp16Memory { get; set; } = true;

        /// <inheritdoc />
        public bool UseMemorymap { get; set; } = true;

        /// <inheritdoc />
        public bool UseMemoryLock { get; set; }

        /// <inheritdoc />
        public bool Perplexity { get; set; }

        /// <inheritdoc />
        public string ModelPath { get; set; }

        /// <inheritdoc />
        public AdapterCollection LoraAdapters { get; set; } = new();

        /// <inheritdoc />
        public string LoraBase { get; set; } = string.Empty;

        /// <inheritdoc />
        public uint? Threads { get; set; }

        /// <inheritdoc />
        public uint? BatchThreads { get; set; }

        /// <inheritdoc />
        public uint BatchSize { get; set; } = 512;

        /// <inheritdoc />
        public bool EmbeddingMode { get; set; }

        /// <inheritdoc />
        [JsonConverter(typeof(TensorSplitsCollectionConverter))]
        public TensorSplitsCollection TensorSplits { get; set; } = new();

        /// <inheritdoc />
        public float? RopeFrequencyBase { get; set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool VocabOnly { get; set; }

        /// <inheritdoc />
        [JsonConverter(typeof(EncodingConverter))]
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelPath">The model path.</param>
        [JsonConstructor]
        public ModelParams(string modelPath)
        {
            ModelPath = modelPath;
        }

        private ModelParams()
        {
            // This constructor (default parameterless constructor) is used by Newtonsoft to deserialize!
            ModelPath = "";
        }

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
        /// <param name="embeddingMode">Whether to use embedding mode. (embedding) Note that if this is set to true, The LLamaModel won't produce text response anymore.</param>
        /// <param name="ropeFrequencyBase">RoPE base frequency.</param>
        /// <param name="ropeFrequencyScale">RoPE frequency scaling factor</param>
        /// <param name="mulMatQ">Use experimental mul_mat_q kernels</param>
        /// <param name="encoding">The encoding to use to convert text for the model</param>
        [Obsolete("Use object initializer to set all optional parameters")]
        public ModelParams(string modelPath, uint contextSize = 512, int gpuLayerCount = 20,
                           uint seed = 1337, bool useFp16Memory = true,
                           bool useMemorymap = true, bool useMemoryLock = false, bool perplexity = false,
                           string loraAdapter = "", string loraBase = "", int threads = -1, uint batchSize = 512,
                           bool embeddingMode = false,
                           float? ropeFrequencyBase = null, float? ropeFrequencyScale = null, bool mulMatQ = false,
                           string encoding = "UTF-8")
        {
            ContextSize = contextSize;
            GpuLayerCount = gpuLayerCount;
            Seed = seed;
            UseFp16Memory = useFp16Memory;
            UseMemorymap = useMemorymap;
            UseMemoryLock = useMemoryLock;
            Perplexity = perplexity;
            ModelPath = modelPath;
            LoraBase = loraBase;
            Threads = threads < 1 ? null : (uint)threads;
            BatchSize = batchSize;
            EmbeddingMode = embeddingMode;
            RopeFrequencyBase = ropeFrequencyBase;
            RopeFrequencyScale = ropeFrequencyScale;
            MulMatQ = mulMatQ;
            Encoding = Encoding.GetEncoding(encoding);
            LoraAdapters.Add(new LoraAdapter(loraAdapter, 1));
        }
    }

    internal class EncodingConverter
        : JsonConverter<Encoding>
    {
        public override Encoding? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var name = reader.GetString();
            if (name == null)
                return null;
            return Encoding.GetEncoding(name);
        }

        public override void Write(Utf8JsonWriter writer, Encoding value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.WebName);
        }
    }

    internal class TensorSplitsCollectionConverter
        : JsonConverter<TensorSplitsCollection>
    {
        public override TensorSplitsCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var arr = JsonSerializer.Deserialize<float[]>(ref reader, options) ?? Array.Empty<float>();
            return new TensorSplitsCollection(arr);
        }

        public override void Write(Utf8JsonWriter writer, TensorSplitsCollection value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Splits, options);
        }
    }
}
