using LLama.Abstractions;
using LLama.Native;

namespace LLama.Experimental.Native
{
#if NET6_0_OR_GREATER
    public class AutoDownloadedLibraries
    {
        public class Cuda: INativeLibrary
        {
            private NativeLibraryWithCuda _cudaLibrary;
            private NativeLibraryDownloadSettings _settings;

            public Cuda(NativeLibraryWithCuda cudaLibrary, NativeLibraryDownloadSettings settings)
            {
                _cudaLibrary = cudaLibrary;
                _settings = settings;
            }

            public NativeLibraryMetadata? Metadata => _cudaLibrary.Metadata;

            public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null)
            {
                foreach(var relativePath in _cudaLibrary.Prepare(systemInfo, logCallback))
                {
                    yield return relativePath;
                    var path = NativeLibraryDownloader.DownloadLibraryFile(_settings, relativePath, logCallback).Result;
                    if (path is not null)
                    {
                        yield return path;
                    }
                }
            }
        }

        public class Avx : INativeLibrary
        {
            private NativeLibraryWithAvx _avxLibrary;
            private NativeLibraryDownloadSettings _settings;

            public Avx(NativeLibraryWithAvx avxLibrary, NativeLibraryDownloadSettings settings)
            {
                _avxLibrary = avxLibrary;
                _settings = settings;
            }

            public NativeLibraryMetadata? Metadata => _avxLibrary.Metadata;

            public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null)
            {
                foreach (var relativePath in _avxLibrary.Prepare(systemInfo, logCallback))
                {
                    yield return relativePath;
                    var path = NativeLibraryDownloader.DownloadLibraryFile(_settings, relativePath, logCallback).Result;
                    if (path is not null)
                    {
                        yield return path;
                    }
                }
            }
        }

        public class MacOrFallback : INativeLibrary
        {
            private NativeLibraryWithMacOrFallback _macLibrary;
            private NativeLibraryDownloadSettings _settings;

            public MacOrFallback(NativeLibraryWithMacOrFallback macLibrary, NativeLibraryDownloadSettings settings)
            {
                _macLibrary = macLibrary;
                _settings = settings;
            }

            public NativeLibraryMetadata? Metadata => _macLibrary.Metadata;

            public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback = null)
            {
                foreach (var relativePath in _macLibrary.Prepare(systemInfo, logCallback))
                {
                    yield return relativePath;
                    var path = NativeLibraryDownloader.DownloadLibraryFile(_settings, relativePath, logCallback).Result;
                    if (path is not null)
                    {
                        yield return path;
                    }
                }
            }
        }
    }
#endif
}
