using LLama.Abstractions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// A native library compiled with cublas/cuda.
    /// </summary>
    public class NativeLibraryWithCuda : INativeLibrary
    {
        private int _majorCudaVersion;
        private NativeLibraryName _libraryName;
        private AvxLevel _avxLevel;
        private bool _skipCheck;

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
        public NativeLibraryWithCuda(int majorCudaVersion, NativeLibraryName libraryName, bool skipCheck)
        {
            _majorCudaVersion = majorCudaVersion;
            _libraryName = libraryName;
            _skipCheck = skipCheck;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            // TODO: Avx level is ignored now, needs to be implemented in the future.
            if (systemInfo.OSPlatform == OSPlatform.Windows || systemInfo.OSPlatform == OSPlatform.Linux || _skipCheck)
            {
                if (_majorCudaVersion == -1 && _skipCheck)
                {
                    // Currently only 11 and 12 are supported.
                    foreach(var cuda12LibraryPath in GetCudaPaths(systemInfo, 12, logCallback))
                    {
                        if (cuda12LibraryPath is not null)
                        {
                            yield return cuda12LibraryPath;
                        }
                    }
                    foreach (var cuda11LibraryPath in GetCudaPaths(systemInfo, 11, logCallback))
                    {
                        if (cuda11LibraryPath is not null)
                        {
                            yield return cuda11LibraryPath;
                        }
                    }
                }
                else if (_majorCudaVersion != -1)
                {
                    foreach (var cudaLibraryPath in GetCudaPaths(systemInfo, _majorCudaVersion, logCallback))
                    {
                        if (cudaLibraryPath is not null)
                        {
                            yield return cudaLibraryPath;
                        }
                    }
                }
            }
        }

        private IEnumerable<string> GetCudaPaths(SystemInfo systemInfo, int cudaVersion, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            NativeLibraryUtils.GetPlatformPathParts(systemInfo.OSPlatform, out var os, out var fileExtension, out var libPrefix);
            yield return $"runtimes/{os}/native/cuda{cudaVersion}/{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
#if NETSTANDARD
            // For .NET framework, the path might exclude `runtimes`.
            yield return $"{os}/native/cuda{cudaVersion}/{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
#endif
        }
    }
}
