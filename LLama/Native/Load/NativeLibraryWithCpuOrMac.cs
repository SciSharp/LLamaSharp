using LLama.Abstractions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LLama.Native
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// A native library compiled on Mac, or fallbacks from all other libraries in the selection.
    /// </summary>
    public class NativeLibraryWithCpuOrMac
        : INativeLibrary
    {
        private NativeLibraryName _libraryName;
        private bool _skipCheck;
        private NativeLibraryDownloadSettings _downloadSettings;

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
        /// <param name="downloadSettings"></param>
        public NativeLibraryWithCpuOrMac(NativeLibraryName libraryName, bool skipCheck, NativeLibraryDownloadSettings downloadSettings)
        {
            _libraryName = libraryName;
            _skipCheck = skipCheck;
            _downloadSettings = downloadSettings;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, bool fromRemote, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            var path = GetPath(systemInfo, AvxLevel.None, fromRemote, logCallback);
            return path is null ?[] : [path];
        }

        private string? GetPath(SystemInfo systemInfo, AvxLevel avxLevel, bool fromRemote, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            NativeLibraryUtils.GetPlatformPathParts(systemInfo.OSPlatform, out var os, out var fileExtension, out var libPrefix);
            string relativePath;
            if (systemInfo.OSPlatform == OSPlatform.OSX)
            {
                relativePath = $"runtimes/{os}/native/{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
            }
            else
            {
                var avxStr = NativeLibraryConfig.AvxLevelToString(AvxLevel.None);
                if (!string.IsNullOrEmpty(avxStr))
                    avxStr += "/";

                relativePath = $"runtimes/{os}/native/{avxStr}{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
            }

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
