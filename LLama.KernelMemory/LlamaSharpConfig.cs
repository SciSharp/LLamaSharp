using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLamaSharp.KernelMemory
{
    /// <summary>
    /// Represents the configuration for LLamaSharp. Available properties are `ModelPath`, `ContextSize`, `Seed`, `GpuLayerCount`.
    /// </summary>
    public class LLamaSharpConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LLamaSharpConfig"/> class.
        /// </summary>
        /// <param name="modelPath">The path to the model file.</param>
        public LLamaSharpConfig(string modelPath)
        {
            ModelPath = modelPath;
        }

        /// <summary>
        /// Gets or sets the path to the model file.
        /// </summary>
        public string ModelPath { get; set; }

        /// <summary>
        /// Gets or sets the size of the context.
        /// </summary>
        public uint? ContextSize { get; set; }

        /// <summary>
        /// Gets or sets the seed value.
        /// </summary>
        public uint? Seed { get; set; }

        /// <summary>
        /// Gets or sets the number of GPU layers.
        /// </summary>
        public int? GpuLayerCount { get; set; }


        /// <summary>
        /// Set the default inference parameters.
        /// </summary>
        public InferenceParams? DefaultInferenceParams { get; set; }
    }
}
