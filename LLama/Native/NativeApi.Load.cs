using LLama.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace LLama.Native
{
    public static partial class NativeApi
    {
        static NativeApi()
        {
            // Try to load a preferred library, based on CPU feature detection
            TryLoadLibrary();

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
            llama_backend_init(false);
        }

        private static void Log(string message, LogLevel level)
        {
            if (!enableLogging)
                return;

            if ((int)level < (int)logLevel)
                return;

            ConsoleColor color;
            string levelPrefix;
            if (level == LogLevel.Information)
            {
                color = ConsoleColor.Green;
                levelPrefix = "[Info]";
            }
            else if (level == LogLevel.Error)
            {
                color = ConsoleColor.Red;
                levelPrefix = "[Error]";
            }
            else
            {
                color = ConsoleColor.Yellow;
                levelPrefix = "[Error]";
            }
            Console.ForegroundColor = color;
            Console.WriteLine($"{loggingPrefix} {levelPrefix} {message}");
            Console.ResetColor();
        }

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

#if NET6_0_OR_GREATER
        private static string GetAvxLibraryPath(NativeLibraryConfig.AvxLevel avxLevel, string prefix, string suffix, string libraryNamePrefix)
        {
            var avxStr = NativeLibraryConfig.AvxLevelToString(avxLevel);
            if (!string.IsNullOrEmpty(avxStr))
            {
                avxStr += "/";
            }
            return $"{prefix}{avxStr}{libraryNamePrefix}{libraryName}{suffix}";
        }

        private static List<string> GetLibraryTryOrder(NativeLibraryConfig.Description configuration)
        {
            OSPlatform platform;
            string prefix, suffix, libraryNamePrefix;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                platform = OSPlatform.Windows;
                prefix = "runtimes/win-x64/native/";
                suffix = ".dll";
                libraryNamePrefix = "";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platform = OSPlatform.Linux;
                prefix = "runtimes/linux-x64/native/";
                suffix = ".so";
                libraryNamePrefix = "lib";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                platform = OSPlatform.OSX;
                suffix = ".dylib";

                prefix = System.Runtime.Intrinsics.Arm.ArmBase.Arm64.IsSupported
                       ? "runtimes/osx-arm64/native/"
                       : "runtimes/osx-x64/native/";
                libraryNamePrefix = "lib";
            }
            else
            {
                throw new RuntimeError("Your system plarform is not supported, please open an issue in LLamaSharp.");
            }
            Log($"Detected OS Platform: {platform}", LogLevel.Information);

            List<string> result = new();
            if (configuration.UseCuda && (platform == OSPlatform.Windows || platform == OSPlatform.Linux)) // no cuda on macos
            {
                int cudaVersion = GetCudaMajorVersion();

                // TODO: load cuda library with avx
                if (cudaVersion == -1 && !configuration.AllowFallback)
                {
                    // if check skipped, we just try to load cuda libraries one by one.
                    if (configuration.SkipCheck)
                    {
                        result.Add($"{prefix}cuda12/{libraryNamePrefix}{libraryName}{suffix}");
                        result.Add($"{prefix}cuda11/{libraryNamePrefix}{libraryName}{suffix}");
                    }
                    else
                    {
                        throw new RuntimeError("Configured to load a cuda library but no cuda detected on your device.");
                    }
                }
                else if (cudaVersion == 11)
                {
                    Log($"Detected cuda major version {cudaVersion}.", LogLevel.Information);
                    result.Add($"{prefix}cuda11/{libraryNamePrefix}{libraryName}{suffix}");
                }
                else if (cudaVersion == 12)
                {
                    Log($"Detected cuda major version {cudaVersion}.", LogLevel.Information);
                    result.Add($"{prefix}cuda12/{libraryNamePrefix}{libraryName}{suffix}");
                }
                else if (cudaVersion > 0)
                {
                    throw new RuntimeError($"Cuda version {cudaVersion} hasn't been supported by LLamaSharp, please open an issue for it.");
                }
                // otherwise no cuda detected but allow fallback
            }

            // use cpu (or mac possibly with metal)
            if (!configuration.AllowFallback && platform != OSPlatform.OSX)
            {
                result.Add(GetAvxLibraryPath(configuration.AvxLevel, prefix, suffix, libraryNamePrefix));
            }
            else if (platform != OSPlatform.OSX) // in macos there's absolutely no avx
            {
                if (configuration.AvxLevel >= NativeLibraryConfig.AvxLevel.Avx512)
                    result.Add(GetAvxLibraryPath(NativeLibraryConfig.AvxLevel.Avx512, prefix, suffix, libraryNamePrefix));

                if (configuration.AvxLevel >= NativeLibraryConfig.AvxLevel.Avx2)
                    result.Add(GetAvxLibraryPath(NativeLibraryConfig.AvxLevel.Avx2, prefix, suffix, libraryNamePrefix));

                if (configuration.AvxLevel >= NativeLibraryConfig.AvxLevel.Avx)
                    result.Add(GetAvxLibraryPath(NativeLibraryConfig.AvxLevel.Avx, prefix, suffix, libraryNamePrefix));

                result.Add(GetAvxLibraryPath(NativeLibraryConfig.AvxLevel.None, prefix, suffix, libraryNamePrefix));
            }

            if (platform == OSPlatform.OSX)
            {
                result.Add($"{prefix}{libraryNamePrefix}{libraryName}{suffix}");
            }

            return result;
        }
#endif

        /// <summary>
        /// Try to load libllama, using CPU feature detection to try and load a more specialised DLL if possible
        /// </summary>
        /// <returns>The library handle to unload later, or IntPtr.Zero if no library was loaded</returns>
        private static IntPtr TryLoadLibrary()
        {
#if NET6_0_OR_GREATER
            var configuration = NativeLibraryConfig.CheckAndGatherDescription();
            enableLogging = configuration.Logging;
            logLevel = configuration.LogLevel;
            // We move the flag to avoid loading library when the variable is called else where.
            NativeLibraryConfig.LibraryHasLoaded = true;
            Log(configuration.ToString(), LogLevel.Information);

            if (!string.IsNullOrEmpty(configuration.Path))
            {
                // When loading the user specified library, there's no fallback.
                var success = NativeLibrary.TryLoad(configuration.Path, out var result);
                if (!success)
                {
                    throw new RuntimeError($"Failed to load the native library [{configuration.Path}] you specified.");
                }
                Log($"Successfully loaded the library [{configuration.Path}] specified by user", LogLevel.Information);
                return result;
            }

            var libraryTryLoadOrder = GetLibraryTryOrder(configuration);

            var preferredPaths = configuration.SearchDirectories;
            var possiblePathPrefix = new[] {
                AppDomain.CurrentDomain.BaseDirectory,
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ""
            };

            string TryFindPath(string filename)
            {
                foreach (var path in preferredPaths)
                {
                    if (File.Exists(Path.Combine(path, filename)))
                    {
                        return Path.Combine(path, filename);
                    }
                }

                foreach (var path in possiblePathPrefix)
                {
                    if (File.Exists(Path.Combine(path, filename)))
                    {
                        return Path.Combine(path, filename);
                    }
                }

                return filename;
            }

            foreach (var libraryPath in libraryTryLoadOrder)
            {
                var fullPath = TryFindPath(libraryPath);
                var result = TryLoad(fullPath, true);
                if (result is not null && result != IntPtr.Zero)
                {
                    Log($"{fullPath} is selected and loaded successfully.", LogLevel.Information);
                    return (IntPtr)result;
                }

                Log($"Tried to load {fullPath} but failed.", LogLevel.Information);
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
                $"This is not an error under netstandard2.0 but needs attention with net6 or higher.", LogLevel.Warning);
            return IntPtr.Zero;

#if NET6_0_OR_GREATER
            // Try to load a DLL from the path if supported. Returns null if nothing is loaded.
            static IntPtr? TryLoad(string path, bool supported = true)
            {
                if (!supported)
                    return null;

                if (NativeLibrary.TryLoad(path, out var handle))
                    return handle;

                return null;
            }
#endif
        }

        internal const string libraryName = "llama";
        internal const string llavaLibName = "llava_shared";
        private const string cudaVersionFile = "version.json";
        private const string loggingPrefix = "[LLamaSharp Native]";
        private static bool enableLogging = false;
        private static LLamaLogLevel logLevel = LLamaLogLevel.Info;
    }
}
