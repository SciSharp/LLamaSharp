using LLama.Abstractions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LLama.Native
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// A native library compiled with avx support but without cuda/cublas.
    /// </summary>
    public class NativeLibraryWithAvx : INativeLibrary
    {
        private NativeLibraryName _libraryName;
        private AvxLevel _avxLevel;
        private bool _skipCheck;

        /// <inheritdoc/>
        public NativeLibraryMetadata? Metadata
        {
            get
            {
                return new NativeLibraryMetadata(_libraryName, false, _avxLevel);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="libraryName"></param>
        /// <param name="avxLevel"></param>
        /// <param name="skipCheck"></param>
        public NativeLibraryWithAvx(NativeLibraryName libraryName, AvxLevel avxLevel, bool skipCheck)
        {
            _libraryName = libraryName;
            _avxLevel = avxLevel;
            _skipCheck = skipCheck;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            if (systemInfo.OSPlatform != OSPlatform.Windows && systemInfo.OSPlatform != OSPlatform.Linux && !_skipCheck)
            {
                // Not supported on systems other than Windows and Linux.
                return [];
            }
            var path = GetAvxPath(systemInfo, _avxLevel, logCallback);
            return path is null ? [] : [path];
        }

        private string? GetAvxPath(SystemInfo systemInfo, AvxLevel avxLevel, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            NativeLibraryUtils.GetPlatformPathParts(systemInfo.OSPlatform, out var os, out var fileExtension, out var libPrefix);
            var avxStr = NativeLibraryConfig.AvxLevelToString(avxLevel);
            if (!string.IsNullOrEmpty(avxStr))
                avxStr += "/";
            var relativePath = $"runtimes/{os}/native/{avxStr}{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
            return relativePath;
        }
    }
#endif
}
