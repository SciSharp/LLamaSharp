using LLama.Abstractions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LLama.Native
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// A native library compiled with cublas/cuda.
    /// </summary>
    public class NativeLibraryWithCuda : INativeLibrary
    {
        private int _majorCudaVersion;
        private NativeLibraryName _libraryName;
        private AvxLevel _avxLevel;
        private bool _skipCheck;
        private NativeLibraryDownloadSettings _downloadSettings;

        /// <inheritdoc/>
        public NativeLibraryMetadata? Metadata
        {
            get
            {
                return new NativeLibraryMetadata(_libraryName, true, _avxLevel);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="majorCudaVersion"></param>
        /// <param name="libraryName"></param>
        /// <param name="skipCheck"></param>
        /// <param name="downloadSettings"></param>
        public NativeLibraryWithCuda(int majorCudaVersion, NativeLibraryName libraryName, bool skipCheck, NativeLibraryDownloadSettings downloadSettings)
        {
            _majorCudaVersion = majorCudaVersion;
            _libraryName = libraryName;
            _skipCheck = skipCheck;
            _downloadSettings = downloadSettings;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, bool fromRemote, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            // TODO: Avx level is ignored now, needs to be implemented in the future.
            if (systemInfo.OSPlatform == OSPlatform.Windows || systemInfo.OSPlatform == OSPlatform.Linux || _skipCheck)
            {
                if (_majorCudaVersion == -1 && _skipCheck)
                {
                    // Currently only 11 and 12 are supported.
                    var cuda12LibraryPath = GetCudaPath(systemInfo, 12, fromRemote, logCallback);
                    if (cuda12LibraryPath is not null)
                    {
                        yield return cuda12LibraryPath;
                    }
                    var cuda11LibraryPath = GetCudaPath(systemInfo, 11, fromRemote, logCallback);
                    if (cuda11LibraryPath is not null)
                    {
                        yield return cuda11LibraryPath;
                    }
                }
                else if (_majorCudaVersion != -1)
                {
                    var cudaLibraryPath = GetCudaPath(systemInfo, _majorCudaVersion, fromRemote, logCallback);
                    if (cudaLibraryPath is not null)
                    {
                        yield return cudaLibraryPath;
                    }
                }
            }
        }

        private string? GetCudaPath(SystemInfo systemInfo, int cudaVersion, bool remote, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            NativeLibraryUtils.GetPlatformPathParts(systemInfo.OSPlatform, out var os, out var fileExtension, out var libPrefix);
            var relativePath = $"runtimes/{os}/native/cuda{cudaVersion}/{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
            if (remote)
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
