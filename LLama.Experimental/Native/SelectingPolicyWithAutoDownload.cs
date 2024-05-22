using LLama.Abstractions;
using LLama.Native;

namespace LLama.Experimental.Native
{
#if NET6_0_OR_GREATER
    public class SelectingPolicyWithAutoDownload: INativeLibrarySelectingPolicy
    {
        private DefaultNativeLibrarySelectingPolicy _defaultPolicy = new();
        private NativeLibraryDownloadSettings _downloadSettings;

        internal SelectingPolicyWithAutoDownload(NativeLibraryDownloadSettings downloadSettings)
        {
            _downloadSettings = downloadSettings;
        }

        public IEnumerable<INativeLibrary> Apply(NativeLibraryConfig.Description description, SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            Log($"""
                Auto-download of native library has been enabled, with the following settings:
                {_downloadSettings}
                """, LLamaLogLevel.Info, logCallback);
            foreach(var library in _defaultPolicy.Apply(description, systemInfo, logCallback))
            {
                if(library is NativeLibraryWithCuda cudaLibrary)
                {
                    yield return new AutoDownloadedLibraries.Cuda(cudaLibrary, _downloadSettings);
                }
                else if(library is NativeLibraryWithAvx avxLibrary)
                {
                    yield return new AutoDownloadedLibraries.Avx(avxLibrary, _downloadSettings);
                }
                else if(library is NativeLibraryWithMacOrFallback macLibrary)
                {
                    yield return new AutoDownloadedLibraries.MacOrFallback(macLibrary, _downloadSettings);
                }
                else if(library is NativeLibraryFromPath)
                {
                    yield return library; // No need to download
                }
                else
                {
                    Log($"Unknown native library type of auto-download: {library.GetType()}. " +
                        $"Please ignore this warning if you are using self-defined native library", LLamaLogLevel.Warning, logCallback);
                    yield return library;
                }
            }
        }

        private void Log(string message, LLamaLogLevel level, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            if (!message.EndsWith("\n"))
                message += "\n";

            logCallback?.Invoke(level, message);
        }
    }
#endif
}
