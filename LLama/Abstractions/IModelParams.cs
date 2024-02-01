using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using LLama.Native;

namespace LLama.Abstractions
{
    /// <summary>
    /// The parameters for initializing a LLama model.
    /// </summary>
    public interface IModelParams
    {
        /// <summary>
        /// main_gpu interpretation depends on split_mode:
        /// <list type="bullet">
        ///     <item>
        ///         <term>None</term>
        ///         <description>The GPU that is used for the entire mode.</description>
        ///     </item>
        ///     <item>
        ///         <term>Row</term>
        ///         <description>The GPU that is used for small tensors and intermediate results.</description>
        ///     </item>
        ///     <item>
        ///         <term>Layer</term>
        ///         <description>Ignored.</description>
        ///     </item>
        /// </list>
        /// </summary>
        int MainGpu { get; set; }

        /// <summary>
        /// How to split the model across multiple GPUs
        /// </summary>
        GPUSplitMode SplitMode { get; }

        /// <summary>
        /// Number of layers to run in VRAM / GPU memory (n_gpu_layers)
        /// </summary>
        int GpuLayerCount { get; }

        /// <summary>
        /// Use mmap for faster loads (use_mmap)
        /// </summary>
        bool UseMemorymap { get; }

        /// <summary>
        /// Use mlock to keep model in memory (use_mlock)
        /// </summary>
        bool UseMemoryLock { get; }

        /// <summary>
        /// Model path (model)
        /// </summary>
        string ModelPath { get; }

        /// <summary>
        /// how split tensors should be distributed across GPUs
        /// </summary>
        TensorSplitsCollection TensorSplits { get; }

        /// <summary>
        /// Load vocab only (no weights)
        /// </summary>
        bool VocabOnly { get; }

        /// <summary>
        /// List of LoRA adapters to apply
        /// </summary>
        AdapterCollection LoraAdapters { get; }

        /// <summary>
        /// base model path for the lora adapter (lora_base)
        /// </summary>
        string LoraBase { get; }

        /// <summary>
        /// Override specific metadata items in the model
        /// </summary>
        List<MetadataOverride> MetadataOverrides { get; }
    }

    /// <summary>
    /// A LoRA adapter to apply to a model
    /// </summary>
    /// <param name="Path">Path to the LoRA file</param>
    /// <param name="Scale">Strength of this LoRA</param>
    public readonly record struct LoraAdapter(string Path, float Scale);

    /// <summary>
    /// A list of LoraAdapter objects
    /// </summary>
    public sealed class AdapterCollection
        : List<LoraAdapter>, IEquatable<AdapterCollection>
    {
        /// <inheritdoc />
        public bool Equals(AdapterCollection? other)
        {
            if (other == null)
                return false;

            return this.SequenceEqual(other);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as AdapterCollection);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                for (var i = 0; i < Count; i++)
                {
                    hash += this[i].GetHashCode();
                    hash *= 7823;
                }
                return hash;
            }
        }
    }


    /// <summary>
    /// A fixed size array to set the tensor splits across multiple GPUs
    /// </summary>
    [JsonConverter(typeof(TensorSplitsCollectionConverter))]
    public sealed class TensorSplitsCollection
        : IEnumerable<float>
    {
        internal readonly float[] Splits = new float[NativeApi.llama_max_devices()];

        /// <summary>
        /// The size of this array
        /// </summary>
        public int Length => Splits.Length;

        /// <summary>
        /// Get or set the proportion of work to do on the given device.
        /// </summary>
        /// <remarks>"[ 3, 2 ]" will assign 60% of the data to GPU 0 and 40% to GPU 1.</remarks>
        /// <param name="index"></param>
        /// <returns></returns>
        public float this[int index]
        {
            get => Splits[index];
            set => Splits[index] = value;
        }

        /// <summary>
        /// Create a new tensor splits collection, copying the given values
        /// </summary>
        /// <param name="splits"></param>
        /// <exception cref="ArgumentException"></exception>
        public TensorSplitsCollection(float[] splits)
        {
            if (splits.Length > Splits.Length)
                throw new ArgumentException($"Must supply at most {Splits.Length} tensor splits", nameof(splits));

            splits.CopyTo(Splits.AsSpan());
        }

        /// <summary>
        /// Create a new tensor splits collection with all values initialised to the default
        /// </summary>
        public TensorSplitsCollection()
        {
        }

        /// <summary>
        /// Set all values to zero
        /// </summary>
        public void Clear()
        {
            Array.Clear(Splits, 0, Splits.Length);
        }

        internal MemoryHandle Pin()
        {
            return Splits.AsMemory().Pin();
        }

        #region IEnumerator
        /// <inheritdoc />
        public IEnumerator<float> GetEnumerator()
        {
            return ((IEnumerable<float>)Splits).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Splits.GetEnumerator();
        }
        #endregion
    }

    /// <summary>
    /// A JSON converter for <see cref="TensorSplitsCollection"/>
    /// </summary>
    public class TensorSplitsCollectionConverter
        : JsonConverter<TensorSplitsCollection>
    {
        /// <inheritdoc/>
        public override TensorSplitsCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var arr = JsonSerializer.Deserialize<float[]>(ref reader, options) ?? Array.Empty<float>();
            return new TensorSplitsCollection(arr);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, TensorSplitsCollection value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Splits, options);
        }
    }


    /// <summary>
    /// An override for a single key/value pair in model metadata
    /// </summary>
    [JsonConverter(typeof(MetadataOverrideConverter))]
    public sealed record MetadataOverride
    {
        /// <summary>
        /// Get the key being overriden by this override
        /// </summary>
        public string Key { get; }

        internal LLamaModelKvOverrideType Type { get; }

        private readonly int _valueInt;
        private readonly float _valueFloat;
        private readonly bool _valueBool;

        /// <summary>
        /// Create a new override for an int key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public MetadataOverride(string key, int value)
        {
            Key = key;
            _valueInt = value;
            Type = LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_INT;
        }

        /// <summary>
        /// Create a new override for a float key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public MetadataOverride(string key, float value)
        {
            Key = key;
            _valueFloat = value;
            Type = LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_FLOAT;
        }

        /// <summary>
        /// Create a new override for a boolean key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public MetadataOverride(string key, bool value)
        {
            Key = key;
            _valueBool = value;
            Type = LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_BOOL;
        }

        internal void WriteValue(ref LLamaModelMetadataOverride dest)
        {
            switch (Type)
            {
                case LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_INT:
                    dest.IntValue = _valueInt;
                    break;
                case LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_FLOAT:
                    dest.FloatValue = _valueFloat;
                    break;
                case LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_BOOL:
                    dest.BoolValue = _valueBool ? -1L : 0;
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unknown {nameof(LLamaModelKvOverrideType)} value: {Type}");
            }
        }

        internal void WriteValue(Utf8JsonWriter writer)
        {
            switch (Type)
            {
                case LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_INT:
                    writer.WriteNumberValue(_valueInt);
                    break;
                case LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_FLOAT:
                    writer.WriteNumberValue(_valueFloat);
                    break;
                case LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_BOOL:
                    writer.WriteBooleanValue(_valueBool);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unknown {nameof(LLamaModelKvOverrideType)} value: {Type}");
            }
        }
    }

    /// <summary>
    /// A JSON converter for <see cref="MetadataOverride"/>
    /// </summary>
    public class MetadataOverrideConverter
        : JsonConverter<MetadataOverride>
    {
        /// <inheritdoc/>
        public override MetadataOverride Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var ktv = JsonSerializer.Deserialize<KeyTypeValue>(ref reader, options)!;

            return ((LLamaModelKvOverrideType)ktv.Type) switch
            {
                LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_INT => new MetadataOverride(ktv.Key, ktv.Value.GetInt32()),
                LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_FLOAT => new MetadataOverride(ktv.Key, ktv.Value.GetSingle()),
                LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_BOOL => new MetadataOverride(ktv.Key, ktv.Value.GetBoolean()),
                _ => throw new JsonException(),
            };
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, MetadataOverride value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            {
                writer.WriteNumber("Type", (int)value.Type);
                writer.WriteString("Key", value.Key);
                writer.WritePropertyName("Value");
                value.WriteValue(writer);
            }
            writer.WriteEndObject();
        }

        private record KeyTypeValue(int Type, string Key, JsonElement Value);
    }
}