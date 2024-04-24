using LLama.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Abstractions
{
    /// <summary>
    /// Descriptor of a native library.
    /// </summary>
    public interface INativeLibrary
    {
        /// <summary>
        /// Metadata of this library.
        /// </summary>
        NativeLibraryMetadata? Metadata { get; }

        /// <summary>
        /// Prepare the native library file and returns the local path of it.
        /// If it's a relative path, LLamaSharp will search the path in the search directies you set.
        /// </summary>
        /// <param name="systemInfo">The system information of the current machine.</param>
        /// <param name="logCallback">The log callback.</param>
        /// <returns>
        /// The relative paths of the library. You could return multiple paths to try them one by one. If no file is available, please return an empty array.
        /// </returns>
        IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null);
    }
}
