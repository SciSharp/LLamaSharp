using LLama.Abstractions;
using System.Text;
using System.Text.Json.Serialization;
using LLama.Native;
using System.Collections.Generic;
using System;

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
        public GPUSplitMode SplitMode { get; set; } = GPUSplitMode.None;

        /// <inheritdoc />
        public int GpuLayerCount { get; set; } = 20;

        /// <inheritdoc />
        public uint Seed { get; set; } = 0xFFFFFFFF;

        /// <inheritdoc />
        public bool UseMemorymap { get; set; } = true;

        /// <inheritdoc />
        public bool UseMemoryLock { get; set; }

        /// <inheritdoc />
        public string ModelPath { get; set; }

        /// <inheritdoc />
        public AdapterCollection LoraAdapters { get; set; } = new();

        /// <inheritdoc />
        public string LoraBase { get; set; } = string.Empty;

        /// <inheritdoc />
        public uint? Threads { get; set; } = GetNumPhysicalCores();

        /// <inheritdoc />
        public uint? BatchThreads { get; set; }

        /// <inheritdoc />
        public uint BatchSize { get; set; } = 512;

        /// <inheritdoc />
        public bool EmbeddingMode { get; set; }

        /// <inheritdoc />
        public TensorSplitsCollection TensorSplits { get; set; } = new();

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
        public bool VocabOnly { get; set; }

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

        private static uint GetNumPhysicalCores()
        {
            int n_cores = Environment.ProcessorCount;
            return (uint)(n_cores > 0 ? (n_cores <= 4 ? n_cores : n_cores / 2) : 4);
        }
    }
}
