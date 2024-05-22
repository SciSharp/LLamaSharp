using LLama.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Abstractions
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Decides the selected native library that should be loaded according to the configurations.
    /// </summary>
    public interface INativeLibrarySelectingPolicy
    {
        /// <summary>
        /// Select the native library.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="systemInfo">The system information of the current machine.</param>
        /// <param name="logCallback">The log callback.</param>
        /// <returns>The information of the selected native library files, in order by priority from the beginning to the end.</returns>
        IEnumerable<INativeLibrary> Apply(NativeLibraryConfig.Description description, SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null);
    }
#endif
}
