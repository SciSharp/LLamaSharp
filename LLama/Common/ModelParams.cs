using LLama.Abstractions;
using System.Text;
using System.Text.Json.Serialization;
using LLama.Native;
using System.Collections.Generic;

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
        public GPUSplitMode? SplitMode { get; set; }

        /// <inheritdoc />
        public List<TensorBufferOverride> TensorBufferOverrides { get; set; } = new();

        /// <inheritdoc />
        public int GpuLayerCount { get; set; } = 20;

        /// <inheritdoc />
        public uint SeqMax { get; set; } = 1;

        /// <inheritdoc />
        public bool UseMemorymap { get; set; } = true;

        /// <inheritdoc />
        public bool UseMemoryLock { get; set; }

        /// <inheritdoc />
        public string ModelPath { get; set; }

        /// <inheritdoc />
        public int? Threads { get; set; }

        /// <inheritdoc />
        public int? BatchThreads { get; set; }

        /// <inheritdoc />
        public uint BatchSize { get; set; } = 512;

        /// <inheritdoc />
        public uint UBatchSize { get; set; } = 512;

        /// <inheritdoc />
        public bool Embeddings { get; set; }

        /// <inheritdoc />
        public TensorSplitsCollection TensorSplits { get; set; } = new();

        /// <inheritdoc />
        public bool CheckTensors { get; }

        /// <inheritdoc />
        public List<MetadataOverride> MetadataOverrides { get; set; } = new();

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

        /// <inheritdoc />
        public GGMLType? TypeK { get; set; }

        /// <inheritdoc />
        public GGMLType? TypeV { get; set; }

        /// <inheritdoc />
        public bool NoKqvOffload { get; set; }

        /// <inheritdoc />

        public bool FlashAttention { get; set; }

        /// <inheritdoc />
        public float? DefragThreshold { get; set; }

        /// <inheritdoc />
        public LLamaPoolingType PoolingType { get; set; } = LLamaPoolingType.Unspecified;

        /// <inheritdoc />
        public LLamaAttentionType AttentionType { get; set; } = LLamaAttentionType.Unspecified;

        /// <inheritdoc />
        public bool VocabOnly { get; set; }

        /// <inheritdoc />
        public bool? OpOffload { get; set; }

        /// <inheritdoc />
        public bool? SwaFull { get; set; }

        /// <inheritdoc />
        public bool? KVUnified { get; set; }

        /// <summary>
        /// `Encoding` cannot be directly JSON serialized, instead store the name as a string which can
        /// </summary>
        [JsonPropertyName("Encoding")]
        [JsonInclude]
        private string EncodingName { get; set; } = Encoding.UTF8.WebName;

        /// <inheritdoc />
        [JsonIgnore]
        public Encoding Encoding
        {
            get => Encoding.GetEncoding(EncodingName);
            set => EncodingName = value.WebName;
        }

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
    }
}
