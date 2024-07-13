using LLama.Abstractions;
using System.Collections.Generic;

namespace LLama.Native
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// A native library compiled with vulkan.
    /// </summary>
    public class NativeLibraryWithVulkan : INativeLibrary
    {
        private string? _vulkanVersion;
        private NativeLibraryName _libraryName;
        private AvxLevel _avxLevel;
        private bool _skipCheck;

        /// <inheritdoc/>
        public NativeLibraryMetadata? Metadata
        {
            get
            {
                return new NativeLibraryMetadata(_libraryName, false, true, _avxLevel);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vulkanVersion"></param>
        /// <param name="libraryName"></param>
        /// <param name="skipCheck"></param>
        public NativeLibraryWithVulkan(string? vulkanVersion, NativeLibraryName libraryName, bool skipCheck)
        {
            _vulkanVersion = vulkanVersion;
            _libraryName = libraryName;
            _skipCheck = skipCheck;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Prepare(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            // TODO: Avx level is ignored now, needs to be implemented in the future.
            if (systemInfo.OSPlatform == OSPlatform.Windows || systemInfo.OSPlatform == OSPlatform.Linux || _skipCheck)
            {
                if(systemInfo.VulkanVersion != null)
                {
                    var vulkanLibraryPath = GetVulkanPath(systemInfo, logCallback);
                    if (vulkanLibraryPath is not null)
                    {
                        yield return vulkanLibraryPath;
                    }
                }
            }
        }
        
        private string? GetVulkanPath(SystemInfo systemInfo, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            NativeLibraryUtils.GetPlatformPathParts(systemInfo.OSPlatform, out var os, out var fileExtension, out var libPrefix);
            var relativePath = $"runtimes/{os}/native/vulkan/{libPrefix}{_libraryName.GetLibraryName()}{fileExtension}";
            return relativePath;
        }
    }
#endif
}
