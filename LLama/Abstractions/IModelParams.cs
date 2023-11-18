using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
}