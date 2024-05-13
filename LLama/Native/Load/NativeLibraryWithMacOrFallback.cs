using LLama.Abstractions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// A native library compiled on Mac, or fallbacks from all other libraries in the selection.
    /// </summary>
    public class NativeLibraryWithMacOrFallback : INativeLibrary
    {
        private NativeLibraryName _libraryName;
        private bool _skipCheck;

        /// <inheritdoc/>
        public NativeLibraryMetadata? Metadata
        {
            get
            {
                return new NativeLibraryMetadata(_libraryName, false, AvxLevel.None);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="libraryName"></param>
        /// <param name="skipCheck"></param>
        public NativeLibraryWithMacOrFallback(NativeLibraryName libraryName, bool skipCheck)
        {
            _libraryName = libraryName;
            _skipCheck = skipCheck;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            return GetPaths(systemInfo, AvxLevel.None, logCallback);
        }

        private IEnumerable<string> GetPaths(SystemInfo systemInfo, AvxLevel avxLevel, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            NativeLibraryUtils.GetPlatformPathParts(systemInfo.OSPlatform, out var os, out var fileExtension, out var libPrefix);
            string relativePath;
            if (systemInfo.OSPlatform == OSPlatform.OSX)
            {
                yield return $"runtimes/{os}/native/{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
#if NETSTANDARD
                // For .NET framework, the path might exclude `runtimes`.
                yield return $"{os}/native/{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
#endif
            }
            else
            {
                var avxStr = NativeLibraryConfig.AvxLevelToString(AvxLevel.None);
                if (!string.IsNullOrEmpty(avxStr))
                    avxStr += "/";

                yield return $"runtimes/{os}/native/{avxStr}{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
#if NETSTANDARD
                // For .NET framework, the path might exclude `runtimes`.
                yield return $"{os}/native/{avxStr}{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
#endif
            }
        }
    }
}
