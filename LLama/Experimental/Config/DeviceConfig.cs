using LLama.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Config
{
    /// <summary>
    /// Device configuration for using LLM.
    /// </summary>
    public class DeviceConfig
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
        public int MainGpu { get; set; } = 0;

        /// <summary>
        /// How to split the model across multiple GPUs
        /// </summary>
        public GPUSplitMode SplitMode { get; set; } = GPUSplitMode.None;

        /// <summary>
        /// Number of layers to run in VRAM / GPU memory (n_gpu_layers)
        /// </summary>
        public int GpuLayerCount { get; set; } = 20;

        // TODO: Add a static method/property "Auto" to return a default DeviceConfig
    }
}
