using LLama.Abstractions;
using System;
using System.Collections.Generic;

namespace LLama.Native
{
    /// <summary>
    /// When you are using .NET standard2.0 and no explicit library path was set with
    /// <see cref="NativeLibraryConfig.WithLibrary"/>, automatic native library loading is not supported.
    /// This class will be returned in <see cref="NativeLibraryConfig.DryRun(out INativeLibrary)"/> in that case.
    /// </summary>
    public class UnknownNativeLibrary: INativeLibrary
    {
        /// <inheritdoc/>
        public NativeLibraryMetadata? Metadata => null;

        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null)
        {
            throw new NotSupportedException("This class is only a placeholder and should not be used to load native library.");
        }
    }
}
