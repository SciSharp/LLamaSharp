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
        private NativeLibraryDownloadSettings _downloadSettings;

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
        /// <param name="downloadSettings"></param>
        public NativeLibraryWithAvx(NativeLibraryName libraryName, AvxLevel avxLevel, bool skipCheck, NativeLibraryDownloadSettings downloadSettings)
        {
            _libraryName = libraryName;
            _avxLevel = avxLevel;
            _skipCheck = skipCheck;
            _downloadSettings = downloadSettings;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, bool fromRemote, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            if (systemInfo.OSPlatform != OSPlatform.Windows && systemInfo.OSPlatform != OSPlatform.Linux && !_skipCheck)
            {
                // Not supported on systems other than Windows and Linux.
                return [];
            }
            var path = GetAvxPath(systemInfo, _avxLevel, fromRemote, logCallback);
            return path is null ? [] : [path];
        }

        private string? GetAvxPath(SystemInfo systemInfo, AvxLevel avxLevel, bool fromRemote, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            NativeLibraryUtils.GetPlatformPathParts(systemInfo.OSPlatform, out var os, out var fileExtension, out var libPrefix);
            var avxStr = NativeLibraryConfig.AvxLevelToString(avxLevel);
            if (!string.IsNullOrEmpty(avxStr))
                avxStr += "/";
            var relativePath = $"runtimes/{os}/native/{avxStr}{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";

            if (fromRemote)
            {
                // Download and return the local path.
                // We make it sychronize because we c'd better not use async method when loading library later.
                return NativeLibraryDownloadManager.DownloadLibraryFile(_downloadSettings, relativePath, logCallback).Result;
            }
            else
            {
                return relativePath;
            }
        }
    }
#endif
}
