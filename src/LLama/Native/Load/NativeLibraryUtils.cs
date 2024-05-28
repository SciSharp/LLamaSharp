using LLama.Abstractions;
using LLama.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    internal static class NativeLibraryUtils
    {
        /// <summary>
        /// Try to load libllama/llava_shared, using CPU feature detection to try and load a more specialised DLL if possible
        /// </summary>
        /// <returns>The library handle to unload later, or IntPtr.Zero if no library was loaded</returns>
        internal static IntPtr TryLoadLibrary(NativeLibraryConfig config, out INativeLibrary? loadedLibrary)
        {
#if NET6_0_OR_GREATER
            var description = config.CheckAndGatherDescription();
            var systemInfo = SystemInfo.Get();
            Log($"Loading library: '{config.NativeLibraryName.GetLibraryName()}'", LLamaLogLevel.Debug, config.LogCallback);

            // Get platform specific parts of the path (e.g. .so/.dll/.dylib, libName prefix or not)
            NativeLibraryUtils.GetPlatformPathParts(systemInfo.OSPlatform, out var os, out var ext, out var libPrefix);
            Log($"Detected OS Platform: '{systemInfo.OSPlatform}'", LLamaLogLevel.Info, config.LogCallback);
            Log($"Detected OS string: '{os}'", LLamaLogLevel.Debug, config.LogCallback);
            Log($"Detected extension string: '{ext}'", LLamaLogLevel.Debug, config.LogCallback);
            Log($"Detected prefix string: '{libPrefix}'", LLamaLogLevel.Debug, config.LogCallback);

            // Set the flag to ensure this config can no longer be modified
            config.LibraryHasLoaded = true;

            // Show the configuration we're working with
            Log(description.ToString(), LLamaLogLevel.Info, config.LogCallback);

            // Get the libraries ordered by priority from the selecting policy.
            var libraries = config.SelectingPolicy.Apply(description, systemInfo, config.LogCallback);

            foreach (var library in libraries)
            {
                // Prepare the local library file and get the path.
                var paths = library.Prepare(systemInfo, config.LogCallback);
                foreach (var path in paths)
                {
                    Log($"Got relative library path '{path}' from local with {library.Metadata}, trying to load it...", LLamaLogLevel.Debug, config.LogCallback);

                    var result = TryLoad(path, description.SearchDirectories, config.LogCallback);
                    if (result != IntPtr.Zero)
                    {
                        loadedLibrary = library;
                        return result;
                    }
                }
            }

            // If fallback is allowed, we will make the last try (the default system loading) when calling the native api.
            // Otherwise we throw an exception here.
            if (!description.AllowFallback)
            {
                throw new RuntimeError("Failed to load the native library. Please check the log for more information.");
            }
            loadedLibrary = null;
#else
            loadedLibrary = new UnknownNativeLibrary();
#endif

            Log($"No library was loaded before calling native apis. " +
                $"This is not an error under netstandard2.0 but needs attention with net6 or higher.", LLamaLogLevel.Warning, config.LogCallback);
            return IntPtr.Zero;

#if NET6_0_OR_GREATER
            // Try to load a DLL from the path.
            // Returns null if nothing is loaded.
            static IntPtr TryLoad(string path, IEnumerable<string> searchDirectories, NativeLogConfig.LLamaLogCallback? logCallback)
            {
                var fullPath = TryFindPath(path, searchDirectories);
                Log($"Found full path file '{fullPath}' for relative path '{path}'", LLamaLogLevel.Debug, logCallback);
                if (NativeLibrary.TryLoad(fullPath, out var handle))
                {
                    Log($"Successfully loaded '{fullPath}'", LLamaLogLevel.Info, logCallback);
                    return handle;
                }

                Log($"Failed Loading '{fullPath}'", LLamaLogLevel.Info, logCallback);
                return IntPtr.Zero;
            }
#endif
        }

        // Try to find the given file in any of the possible search paths
        private static string TryFindPath(string filename, IEnumerable<string> searchDirectories)
        {
            // Try the configured search directories in the configuration
            foreach (var path in searchDirectories)
            {
                var candidate = Path.Combine(path, filename);
                if (File.Exists(candidate))
                    return candidate;
            }

            // Try a few other possible paths
            var possiblePathPrefix = new[] {
                    AppDomain.CurrentDomain.BaseDirectory,
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ""
                };

            foreach (var path in possiblePathPrefix)
            {
                var candidate = Path.Combine(path, filename);
                if (File.Exists(candidate))
                    return candidate;
            }

            return filename;
        }

        private static void Log(string message, LLamaLogLevel level, NativeLogConfig.LLamaLogCallback? logCallback)
        {
            if (!message.EndsWith("\n"))
                message += "\n";

            logCallback?.Invoke(level, message);
        }

#if NET6_0_OR_GREATER
        public static void GetPlatformPathParts(OSPlatform platform, out string os, out string fileExtension, out string libPrefix)
        {
            if (platform == OSPlatform.Windows)
            {
                os = "win-x64";
                fileExtension = ".dll";
                libPrefix = "";
                return;
            }

            if (platform == OSPlatform.Linux)
            {
                os = "linux-x64";
                fileExtension = ".so";
                libPrefix = "lib";
                return;
            }

            if (platform == OSPlatform.OSX)
            {
                fileExtension = ".dylib";

                os = System.Runtime.Intrinsics.Arm.ArmBase.Arm64.IsSupported
                    ? "osx-arm64"
                    : "osx-x64";
                libPrefix = "lib";
            }
            else
            {
                throw new RuntimeError("Your operating system is not supported, please open an issue in LLamaSharp.");
            }
        }
#endif
    }
}
