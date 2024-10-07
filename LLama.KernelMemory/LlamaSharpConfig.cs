using LLama.Common;
using LLama.Native;

namespace LLamaSharp.KernelMemory
{
    /// <summary>
    /// Represents the configuration for LLamaSharp.
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
        /// Gets or sets the number of GPU layers.
        /// </summary>
        public int? GpuLayerCount { get; set; }

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
        /// <value></value>
        public int MainGpu { get; set; } = 0;

        /// <summary>
        /// How to split the model across multiple GPUs
        /// </summary>
        /// <value></value>
        public GPUSplitMode SplitMode { get; set; } = GPUSplitMode.None;

        /// <summary>
        /// Set the default inference parameters.
        /// </summary>
        public InferenceParams? DefaultInferenceParams { get; set; }
    }
}
