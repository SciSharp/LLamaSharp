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
            try
            {
                // Try to load libraries, paths are set based on configuration combined with hardware detection.
                // On netstandard2.0, this always returns false.
                if (TryLoadLibrariesWithImportResolver())
                    return;
                // if we get here, we've failed to load, and it's up to the native loader to give it one last shot.
                llama_backend_init(false);
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
            
        }

        private static void Log(string message, LogLevel level)
        {
            if (!enableLogging) return;
            Debug.Assert(level is LogLevel.Information or LogLevel.Error or LogLevel.Warning);
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
        

        
        private static bool TryLoadLibrariesWithImportResolver()
        {
#if NET6_0_OR_GREATER
            var configuration = NativeLibraryConfig.CheckAndGatherDescription();
            enableLogging = configuration.Logging;
            // We move the flag to avoid loading library when the variable is called else where.
            NativeLibraryConfig.LibraryHasLoaded = true;
            LlamaModuleInitializer.SearchPaths.AddRange(EnumerateSearchPaths(configuration));


            while (LlamaModuleInitializer.SearchPaths.Count > 0)
            {
                try
                {
                    llama_backend_init(false);
                    // Init complete. Make sure we only search that path (or the system default search paths) going forward.
                    LlamaModuleInitializer.SearchPaths.Clear();
                    var libraryLoadedFrom = LlamaModuleInitializer.CurrentPath;
                    if (!string.IsNullOrWhiteSpace(LlamaModuleInitializer.CurrentPath))
                    {
                        Log($"Successfully loaded and initialized llama, using libraries found {libraryLoadedFrom}", LogLevel.Information);
                        LlamaModuleInitializer.SearchPaths.Add(LlamaModuleInitializer.CurrentPath);
                    }
                    else
                    {
                        Log($"Successfully loaded and initialized llama", LogLevel.Information);
                    }


                    return true;
                }
                catch // Handle any exception, like if we've loaded OpenCL, but it's not installed or doesn't work.
                {
                    // Unload any libraries we've loaded so we can try again, if there are any paths left
                    LlamaModuleInitializer.Reset();
                    if (string.IsNullOrWhiteSpace(LlamaModuleInitializer.CurrentPath))
                    {
                        // We searched through all paths, and didn't find anything, so we're done now.
                        LlamaModuleInitializer.SearchPaths.Clear();
                    }
                    else
                    {
                        // Remove the path we just tried to load from
                        LlamaModuleInitializer.SearchPaths.Remove(LlamaModuleInitializer.CurrentPath);
                    }
                }
            }

            Log("Failed to load libraries via DllImportResolver, falling back to .NET default library loading logic!", LogLevel.Warning);
#else
            Log("Skipping trying to load libraries manually due to netstandard2.0", LogLevel.Debug);
#endif
            return false;
        }

#if NET6_0_OR_GREATER

        private static IEnumerable<string> EnumerateSearchPaths(NativeLibraryConfig.Description configuration)
        {
            var prefix = $"runtimes/{RuntimeInformation.RuntimeIdentifier}/native";
            if (!string.IsNullOrWhiteSpace(configuration.Path))
            {
                yield return Path.GetFileNameWithoutExtension(configuration.Path);
                if (!configuration.AllowFallback)
                    yield break;
            }

            foreach (var path in configuration.SearchDirectories)
            {
                yield return path;
            }

            if (configuration.UseCuda && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // no cuda on macos
            {
                int cudaVersion = GetCudaMajorVersion();

                // TODO: load cuda library with avx
                if (cudaVersion == -1 && !configuration.AllowFallback)
                {
                    // if check skipped, we just try to load cuda libraries one by one.
                    if (configuration.SkipCheck)
                    {
                        yield return Path.Combine(prefix, "cuda12");
                        yield return Path.Combine(prefix, "cuda11");
                    }
                    else
                    {
                        throw new RuntimeError("Configured to load a cuda library but no cuda detected on your device.");
                    }
                }
                else if (cudaVersion == 11)
                {
                    Log($"Detected cuda major version {cudaVersion}.", LogLevel.Information);
                    yield return Path.Combine(prefix, "cuda11");
                }
                else if (cudaVersion == 12)
                {
                    Log($"Detected cuda major version {cudaVersion}.", LogLevel.Information);
                    yield return Path.Combine(prefix, "cuda12");
                }
                else if (cudaVersion > 0)
                {
                    if(!configuration.AllowFallback)
                        throw new RuntimeError($"Cuda version {cudaVersion} hasn't been supported by LLamaSharp, please open an issue for it.");
                    // Fallback allowed, just complain.
                    Log($"Incompatible cuda version {cudaVersion} found, please open an issue with LlamaSharp to investigate supporting it.", LogLevel.Warning);
                }
                // otherwise no cuda detected but allow fallback
            }
            
            if (configuration.UseOpenCL && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                yield return Path.Combine(prefix, "clblast");
                if (!configuration.AllowFallback)
                    yield break;
            }
            var avxLevel = configuration.AvxLevel;
            if (RuntimeInformation.ProcessArchitecture > Architecture.X64)
            {
                // AVX is x86/64 specific.
                avxLevel = NativeLibraryConfig.AvxLevel.None;
            }
            if (!configuration.AllowFallback)
            {
                yield return Path.Combine(prefix, NativeLibraryConfig.AvxLevelToString(avxLevel));
                yield break;
            }
            // We don't need to check for OS X, because OS X x64 does support AVX, and arm does not.
            // So the arch check above kills two birds with one stone so to speak.
                
            while (avxLevel > NativeLibraryConfig.AvxLevel.None)
            {
                yield return Path.Combine(prefix, NativeLibraryConfig.AvxLevelToString(avxLevel));
                avxLevel--;
            }

            yield return prefix;
        }

#endif

        internal const string libraryName = "llama";
        private const string cudaVersionFile = "version.json";
        private const string loggingPrefix = "[LLamaSharp Native]";
        private static bool enableLogging = false;
    }
}
