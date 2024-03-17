using LLama.Exceptions;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Collections.Generic;

namespace LLama.Native
{
    public static partial class NativeApi
    {
        static NativeApi()
        {
            // Overwrite the Dll import resolver for this assembly. The resolver gets
            // called by the runtime every time that a call into a DLL is required. The
            // resolver returns the loaded DLL handle. This allows us to take control of
            // which llama.dll is used.
            SetDllImportResolver();

            // Immediately make a call which requires loading the llama DLL. This method call
            // can't fail unless the DLL hasn't been loaded.
            try
            {
                llama_empty_call();
            }
            catch (DllNotFoundException)
            {
                throw new RuntimeError("The native library cannot be correctly loaded. It could be one of the following reasons: \n" +
                    "1. No LLamaSharp backend was installed. Please search LLamaSharp.Backend and install one of them. \n" +
                    "2. You are using a device with only CPU but installed cuda backend. Please install cpu backend instead. \n" +
                    "3. One of the dependency of the native library is missed. Please use `ldd` on linux, `dumpbin` on windows and `otool`" +
                    "to check if all the dependency of the native library is satisfied. Generally you could find the libraries under your output folder.\n" +
                    "4. Try to compile llama.cpp yourself to generate a libllama library, then use `LLama.Native.NativeLibraryConfig.WithLibrary` " +
                    "to specify it at the very beginning of your code. For more informations about compilation, please refer to LLamaSharp repo on github.\n");
            }

            // Init llama.cpp backend
            llama_backend_init();
        }

#if NET5_0_OR_GREATER
        private static IntPtr _loadedLlamaHandle;
        private static IntPtr _loadedLlavaSharedHandle;
#endif

        private static void SetDllImportResolver()
        {
            // NativeLibrary is not available on older runtimes. We'll have to depend on
            // the normal runtime dll resolution there.
#if NET5_0_OR_GREATER
            NativeLibrary.SetDllImportResolver(typeof(NativeApi).Assembly, (name, _, _) =>
            {
                if (name == "llama")
                {
                    // If we've already loaded llama return the handle that was loaded last time.
                    if (_loadedLlamaHandle != IntPtr.Zero)
                        return _loadedLlamaHandle;

                    // Try to load a preferred library, based on CPU feature detection
                    _loadedLlamaHandle = TryLoadLibraries(LibraryName.Llama);
                    return _loadedLlamaHandle;
                }

                if (name == "llava_shared")
                {
                    // If we've already loaded llava return the handle that was loaded last time.
                    if (_loadedLlavaSharedHandle != IntPtr.Zero)
                        return _loadedLlavaSharedHandle;

                    // Try to load a preferred library, based on CPU feature detection
                    _loadedLlavaSharedHandle = TryLoadLibraries(LibraryName.LlavaShared);
                    return _loadedLlavaSharedHandle;
                }

                // Return null pointer to indicate that nothing was loaded.
                return IntPtr.Zero;
            });
#endif
        }

        private static void Log(string message, LLamaLogLevel level)
        {
            if (!enableLogging)
                return;

            if ((int)level > (int)logLevel)
                return;

            var fg = Console.ForegroundColor;
            var bg = Console.BackgroundColor;
            try
            {
                ConsoleColor color;
                string levelPrefix;
                if (level == LLamaLogLevel.Debug)
                {
                    color = ConsoleColor.Cyan;
                    levelPrefix = "[Debug]";
                }
                else if (level == LLamaLogLevel.Info)
                {
                    color = ConsoleColor.Green;
                    levelPrefix = "[Info]";
                }
                else if (level == LLamaLogLevel.Error)
                {
                    color = ConsoleColor.Red;
                    levelPrefix = "[Error]";
                }
                else
                {
                    color = ConsoleColor.Yellow;
                    levelPrefix = "[UNK]";
                }

                Console.ForegroundColor = color;
                Console.WriteLine($"{loggingPrefix} {levelPrefix} {message}");
            }
            finally
            {
                Console.ForegroundColor = fg;
                Console.BackgroundColor = bg;
            }
        }

        #region CUDA version
        private static int GetCudaMajorVersion()
        {
            string? cudaPath;
            string version = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                cudaPath = Environment.GetEnvironmentVariable("CUDA_PATH");
                if (cudaPath is null)
                {
                    return -1;
                }
                version = GetCudaVersionFromPath(cudaPath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Try the default first
                cudaPath = "/usr/local/bin/cuda";
                version = GetCudaVersionFromPath(cudaPath);
                if (string.IsNullOrEmpty(version))
                {
                    cudaPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
                    if (cudaPath is null)
                    {
                        return -1;
                    }
                    foreach (var path in cudaPath.Split(':'))
                    {
                        version = GetCudaVersionFromPath(Path.Combine(path, ".."));
                        if (string.IsNullOrEmpty(version))
                        {
                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(version))
                return -1;

            version = version.Split('.')[0];
            if (int.TryParse(version, out var majorVersion))
                return majorVersion;

            return -1;
        }

        private static string GetCudaVersionFromPath(string cudaPath)
        {
            try
            {
                string json = File.ReadAllText(Path.Combine(cudaPath, cudaVersionFile));
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    JsonElement root = document.RootElement;
                    JsonElement cublasNode = root.GetProperty("libcublas");
                    JsonElement versionNode = cublasNode.GetProperty("version");
                    if (versionNode.ValueKind == JsonValueKind.Undefined)
                    {
                        return string.Empty;
                    }
                    return versionNode.GetString() ?? "";
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        #endregion

#if NET6_0_OR_GREATER
        private static IEnumerable<string> GetLibraryTryOrder(NativeLibraryConfig.Description configuration)
        {
            var loadingName = configuration.Library.GetLibraryName();
            Log($"Loading library: '{loadingName}'", LLamaLogLevel.Debug);

            // Get platform specific parts of the path (e.g. .so/.dll/.dylib, libName prefix or not)
            GetPlatformPathParts(out var platform, out var os, out var ext, out var libPrefix);
            Log($"Detected OS Platform: '{platform}'", LLamaLogLevel.Info);
            Log($"Detected OS string: '{os}'", LLamaLogLevel.Debug);
            Log($"Detected extension string: '{ext}'", LLamaLogLevel.Debug);
            Log($"Detected prefix string: '{libPrefix}'", LLamaLogLevel.Debug);

            if (configuration.UseCuda && (platform == OSPlatform.Windows || platform == OSPlatform.Linux))
            {
                var cudaVersion = GetCudaMajorVersion();
                Log($"Detected cuda major version {cudaVersion}.", LLamaLogLevel.Info);

                if (cudaVersion == -1 && !configuration.AllowFallback)
                {
                    // if check skipped, we just try to load cuda libraries one by one.
                    if (configuration.SkipCheck)
                    {
                        yield return GetCudaLibraryPath(loadingName, "cuda12");
                        yield return GetCudaLibraryPath(loadingName, "cuda11");
                    }
                    else
                    {
                        throw new RuntimeError("Configured to load a cuda library but no cuda detected on your device.");
                    }
                }
                else if (cudaVersion == 11)
                {
                    yield return GetCudaLibraryPath(loadingName, "cuda11");
                }
                else if (cudaVersion == 12)
                {
                    yield return GetCudaLibraryPath(loadingName, "cuda12");
                }
                else if (cudaVersion > 0)
                {
                    throw new RuntimeError($"Cuda version {cudaVersion} hasn't been supported by LLamaSharp, please open an issue for it.");
                }

                // otherwise no cuda detected but allow fallback
            }

            // Add the CPU/Metal libraries
            if (platform == OSPlatform.OSX)
            {
                // On Mac it's very simple, there's no AVX to consider.
                yield return GetMacLibraryPath(loadingName);
            }
            else
            {
                if (configuration.AllowFallback)
                {
                    // Try all of the AVX levels we can support.
                    if (configuration.AvxLevel >= NativeLibraryConfig.AvxLevel.Avx512)
                        yield return GetAvxLibraryPath(loadingName, NativeLibraryConfig.AvxLevel.Avx512);

                    if (configuration.AvxLevel >= NativeLibraryConfig.AvxLevel.Avx2)
                        yield return GetAvxLibraryPath(loadingName, NativeLibraryConfig.AvxLevel.Avx2);

                    if (configuration.AvxLevel >= NativeLibraryConfig.AvxLevel.Avx)
                        yield return GetAvxLibraryPath(loadingName, NativeLibraryConfig.AvxLevel.Avx);

                    yield return GetAvxLibraryPath(loadingName, NativeLibraryConfig.AvxLevel.None);
                }
                else
                {
                    // Fallback is not allowed - use the exact specified AVX level
                    yield return GetAvxLibraryPath(loadingName, configuration.AvxLevel);
                }
            }
        }

        private static string GetMacLibraryPath(string libraryName)
        {
            GetPlatformPathParts(out _, out var os, out var fileExtension, out var libPrefix);

            return $"runtimes/{os}/native/{libPrefix}{libraryName}{fileExtension}";
        }

        /// <summary>
        /// Given a CUDA version and some path parts, create a complete path to the library file
        /// </summary>
        /// <param name="libraryName">Library being loaded (e.g. "llama")</param>
        /// <param name="cuda">CUDA version (e.g. "cuda11")</param>
        /// <returns></returns>
        private static string GetCudaLibraryPath(string libraryName, string cuda)
        {
            GetPlatformPathParts(out _, out var os, out var fileExtension, out var libPrefix);

            return $"runtimes/{os}/native/{cuda}/{libPrefix}{libraryName}{fileExtension}";
        }

        /// <summary>
        /// Given an AVX level and some path parts, create a complete path to the library file
        /// </summary>
        /// <param name="libraryName">Library being loaded (e.g. "llama")</param>
        /// <param name="avx"></param>
        /// <returns></returns>
        private static string GetAvxLibraryPath(string libraryName, NativeLibraryConfig.AvxLevel avx)
        {
            GetPlatformPathParts(out _, out var os, out var fileExtension, out var libPrefix);

            var avxStr = NativeLibraryConfig.AvxLevelToString(avx);
            if (!string.IsNullOrEmpty(avxStr))
                avxStr += "/";

            return $"runtimes/{os}/native/{avxStr}{libPrefix}{libraryName}{fileExtension}";
        }

        private static void GetPlatformPathParts(out OSPlatform platform, out string os, out string fileExtension, out string libPrefix)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                platform = OSPlatform.Windows;
                os = "win-x64";
                fileExtension = ".dll";
                libPrefix = "";
                return;
            }
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platform = OSPlatform.Linux;
                os = "linux-x64";
                fileExtension = ".so";
                libPrefix = "lib";
                return;
            }
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                platform = OSPlatform.OSX;
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

        /// <summary>
        /// Try to load libllama/llava_shared, using CPU feature detection to try and load a more specialised DLL if possible
        /// </summary>
        /// <returns>The library handle to unload later, or IntPtr.Zero if no library was loaded</returns>
        private static IntPtr TryLoadLibraries(LibraryName lib)
        {
#if NET6_0_OR_GREATER
            var configuration = NativeLibraryConfig.CheckAndGatherDescription(lib);
            enableLogging = configuration.Logging;
            logLevel = configuration.LogLevel;

            // Set the flag to ensure the NativeLibraryConfig can no longer be modified
            NativeLibraryConfig.LibraryHasLoaded = true;

            // Show the configuration we're working with
            Log(configuration.ToString(), LLamaLogLevel.Info);

            // If a specific path is requested, load that or immediately fail
            if (!string.IsNullOrEmpty(configuration.Path))
            {
                if (!NativeLibrary.TryLoad(configuration.Path, out var handle))
                    throw new RuntimeError($"Failed to load the native library [{configuration.Path}] you specified.");

                Log($"Successfully loaded the library [{configuration.Path}] specified by user", LLamaLogLevel.Info);
                return handle;
            }

            // Get a list of locations to try loading (in order of preference)
            var libraryTryLoadOrder = GetLibraryTryOrder(configuration);

            foreach (var libraryPath in libraryTryLoadOrder)
            {
                var fullPath = TryFindPath(libraryPath);
                Log($"Trying '{fullPath}'", LLamaLogLevel.Debug);

                var result = TryLoad(fullPath);
                if (result != IntPtr.Zero)
                {
                    Log($"Loaded '{fullPath}'", LLamaLogLevel.Info);
                    return result;
                }

                Log($"Failed Loading '{fullPath}'", LLamaLogLevel.Info);
            }

            if (!configuration.AllowFallback)
            {
                throw new RuntimeError("Failed to load the library that match your rule, please" +
                    " 1) check your rule." +
                    " 2) try to allow fallback." +
                    " 3) or open an issue if it's expected to be successful.");
            }
#endif

            Log($"No library was loaded before calling native apis. " +
                $"This is not an error under netstandard2.0 but needs attention with net6 or higher.", LLamaLogLevel.Warning);
            return IntPtr.Zero;

#if NET6_0_OR_GREATER
            // Try to load a DLL from the path.
            // Returns null if nothing is loaded.
            static IntPtr TryLoad(string path)
            {
                if (NativeLibrary.TryLoad(path, out var handle))
                    return handle;

                return IntPtr.Zero;
            }

            // Try to find the given file in any of the possible search paths
            string TryFindPath(string filename)
            {
                // Try the configured search directories in the configuration
                foreach (var path in configuration.SearchDirectories)
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
#endif
        }

        internal const string libraryName = "llama";
        internal const string llavaLibraryName = "llava_shared";        
        private const string cudaVersionFile = "version.json";
        private const string loggingPrefix = "[LLamaSharp Native]";
        private static bool enableLogging = false;
        private static LLamaLogLevel logLevel = LLamaLogLevel.Info;
    }
}
