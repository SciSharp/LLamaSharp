using LLama.Abstractions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LLama.Native
{
#if NET6_0_OR_GREATER
    /// <inheritdoc/>
    public class DefaultNativeLibrarySelectingPolicy: INativeLibrarySelectingPolicy
    {
        /// <inheritdoc/>
        public IEnumerable<INativeLibrary> Select(NativeLibraryConfig.Description description, SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            List<INativeLibrary> results = new();

            // Show the configuration we're working with
            Log(description.ToString(), LLamaLogLevel.Info, logCallback);

            // If a specific path is requested, only use it, no fall back.
            if (!string.IsNullOrEmpty(description.Path))
            {
                yield return new NativeLibraryFromPath(description.Path);
            }
            else
            {
                if (description.UseCuda)
                {
                    yield return new NativeLibraryWithCuda(systemInfo.CudaMajorVersion, description.Library, description.SkipCheck, description.DownloadSettings);
                }

                if(!description.UseCuda || description.AllowFallback)
                {
                    if (description.AllowFallback)
                    {
                        // Try all of the AVX levels we can support.
                        if (description.AvxLevel >= AvxLevel.Avx512)
                            yield return new NativeLibraryWithAvx(description.Library, AvxLevel.Avx512, description.SkipCheck, description.DownloadSettings);

                        if (description.AvxLevel >= AvxLevel.Avx2)
                            yield return new NativeLibraryWithAvx(description.Library, AvxLevel.Avx2, description.SkipCheck, description.DownloadSettings);

                        if (description.AvxLevel >= AvxLevel.Avx)
                            yield return new NativeLibraryWithAvx(description.Library, AvxLevel.Avx, description.SkipCheck, description.DownloadSettings);

                        yield return new NativeLibraryWithAvx(description.Library, AvxLevel.None, description.SkipCheck, description.DownloadSettings);
                    }
                    else
                    {
                        yield return new NativeLibraryWithAvx(description.Library, description.AvxLevel, description.SkipCheck, description.DownloadSettings);
                    }
                }

                if(systemInfo.OSPlatform == OSPlatform.OSX || description.AllowFallback)
                {
                    yield return new NativeLibraryWithCpuOrMac(description.Library, description.SkipCheck, description.DownloadSettings);
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
