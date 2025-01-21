using System.Collections.Generic;
using LLama.Abstractions;

namespace LLama.Native
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// A native library compiled on Mac, or fallbacks from all other libraries in the selection.
    /// </summary>
    public class NativeLibraryWithMacOrFallback
        : INativeLibrary
    {
        private readonly NativeLibraryName _libraryName;

        /// <inheritdoc/>
        public NativeLibraryMetadata Metadata => new(_libraryName, false, false, AvxLevel.None);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="libraryName"></param>
        public NativeLibraryWithMacOrFallback(NativeLibraryName libraryName)
        {
            _libraryName = libraryName;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            yield return GetPath(systemInfo);
        }

        private string GetPath(SystemInfo systemInfo)
        {
            NativeLibraryUtils.GetPlatformPathParts(systemInfo.OSPlatform, out var os, out var fileExtension, out var libPrefix);
            string relativePath;
            if (systemInfo.OSPlatform == OSPlatform.OSX)
            {
                var rosettaStr = os == "osx-x64" && !System.Runtime.Intrinsics.X86.Avx.IsSupported ? "rosetta2/" : "";
                relativePath = $"runtimes/{os}/native/{rosettaStr}{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
            }
            else
            {
                relativePath = $"runtimes/{os}/native/{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
            }

            return relativePath;
        }
    }
#endif
}
