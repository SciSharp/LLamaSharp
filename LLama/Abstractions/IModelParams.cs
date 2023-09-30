using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Number of threads (-1 = autodetect) (n_threads)
        /// </summary>
        int Threads { get; set; }

        /// <summary>
        /// how split tensors should be distributed across GPUs
        /// </summary>
        float[]? TensorSplits { get; set; }

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
}