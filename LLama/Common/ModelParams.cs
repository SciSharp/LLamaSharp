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

        /// <inheritdoc />
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
