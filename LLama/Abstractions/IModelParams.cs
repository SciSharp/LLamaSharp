using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using LLama.Common;
using LLama.Native;

namespace LLama.Abstractions
{
    /// <summary>
    /// The parameters for initializing a LLama model.
    /// </summary>
    public interface IModelParams
    {
        /// <summary>
        /// the GPU that is used for scratch and small tensors
        /// </summary>
        int MainGpu { get; set; }

        /// <summary>
        /// Number of layers to run in VRAM / GPU memory (n_gpu_layers)
        /// </summary>
        int GpuLayerCount { get; set; }

        /// <summary>
        /// Use mmap for faster loads (use_mmap)
        /// </summary>
        bool UseMemorymap { get; set; }

        /// <summary>
        /// Use mlock to keep model in memory (use_mlock)
        /// </summary>
        bool UseMemoryLock { get; set; }

        /// <summary>
        /// Model path (model)
        /// </summary>
        string ModelPath { get; set; }

        /// <summary>
        /// how split tensors should be distributed across GPUs
        /// </summary>
        TensorSplitsCollection TensorSplits { get; set; }

        /// <summary>
        /// Load vocab only (no weights)
        /// </summary>
        bool VocabOnly { get; set; }

        /// <summary>
        /// List of LoRA adapters to apply
        /// </summary>
        AdapterCollection LoraAdapters { get; }

        /// <summary>
        /// base model path for the lora adapter (lora_base)
        /// </summary>
        string LoraBase { get; set; }

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
    public abstract record MetadataOverride
    {
        /// <summary>
        /// Create a new override for an int key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MetadataOverride Create(string key, int value)
        {
            return new IntOverride(key, value);
        }

        /// <summary>
        /// Create a new override for a float key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MetadataOverride Create(string key, float value)
        {
            return new FloatOverride(key, value);
        }

        /// <summary>
        /// Create a new override for a boolean key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MetadataOverride Create(string key, bool value)
        {
            return new BoolOverride(key, value);
        }

        internal abstract void Write(ref LLamaModelMetadataOverride dest);

        /// <summary>
        /// Get the key being overriden by this override
        /// </summary>
        public abstract string Key { get; init; }

        private record IntOverride(string Key, int Value) : MetadataOverride
        {
            internal override void Write(ref LLamaModelMetadataOverride dest)
            {
                dest.Tag = LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_INT;
                dest.IntValue = Value;
            }
        }

        private record FloatOverride(string Key, float Value) : MetadataOverride
        {
            internal override void Write(ref LLamaModelMetadataOverride dest)
            {
                dest.Tag = LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_FLOAT;
                dest.FloatValue = Value;
            }
        }

        private record BoolOverride(string Key, bool Value) : MetadataOverride
        {
            internal override void Write(ref LLamaModelMetadataOverride dest)
            {
                dest.Tag = LLamaModelKvOverrideType.LLAMA_KV_OVERRIDE_BOOL;
                dest.BoolValue = Value ? -1 : 0;
            }
        }
    }

    public class MetadataOverrideConverter
        : JsonConverter<MetadataOverride>
    {
        /// <inheritdoc/>
        public override MetadataOverride Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
            //var arr = JsonSerializer.Deserialize<float[]>(ref reader, options) ?? Array.Empty<float>();
            //return new TensorSplitsCollection(arr);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, MetadataOverride value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
            //JsonSerializer.Serialize(writer, value.Splits, options);
        }
    }
}