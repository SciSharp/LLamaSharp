using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace LLama.Native
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Allows configuration of the native llama.cpp libraries to load and use.
    /// All configuration must be done before using **any** other LLamaSharp methods!
    /// </summary>
    public sealed partial class NativeLibraryConfig
    {
        private string? _libraryPath;
        private string? _libraryPathLLava;

        private bool _useCuda = true;
        private AvxLevel _avxLevel;
        private bool _allowFallback = true;
        private bool _skipCheck = false;
        private bool _logging = false;
        private LLamaLogLevel _logLevel = LLamaLogLevel.Info;

        /// <summary>
        /// search directory -> priority level, 0 is the lowest.
        /// </summary>
        private readonly List<string> _searchDirectories = new List<string>();

        #region configurators
        /// <summary>
        /// Load a specified native library as backend for LLamaSharp.
        /// When this method is called, all the other configurations will be ignored.
        /// </summary>
        /// <param name="llamaPath">The full path to the llama library to load.</param>
        /// <param name="llavaPath">The full path to the llava library to load.</param>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfig WithLibrary(string? llamaPath, string? llavaPath)
        {
            ThrowIfLoaded();

            _libraryPath = llamaPath;
            _libraryPathLLava = llavaPath;
            return this;
        }

        /// <summary>
        /// Configure whether to use cuda backend if possible.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfig WithCuda(bool enable = true)
        {
            ThrowIfLoaded();

            _useCuda = enable;
            return this;
        }

        /// <summary>
        /// Configure the prefferred avx support level of the backend.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfig WithAvx(AvxLevel level)
        {
            ThrowIfLoaded();

            _avxLevel = level;
            return this;
        }

        /// <summary>
        /// Configure whether to allow fallback when there's no match for preferred settings.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfig WithAutoFallback(bool enable = true)
        {
            ThrowIfLoaded();

            _allowFallback = enable;
            return this;
        }

        /// <summary>
        /// Whether to skip the check when you don't allow fallback. This option 
        ///  may be useful under some complex conditions. For example, you're sure 
        ///  you have your cublas configured but LLamaSharp take it as invalid by mistake.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfig SkipCheck(bool enable = true)
        {
            ThrowIfLoaded();

            _skipCheck = enable;
            return this;
        }

        /// <summary>
        /// Whether to output the logs to console when loading the native library with your configuration.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfig WithLogs(bool enable)
        {
            ThrowIfLoaded();

            _logging = enable;
            return this;
        }

        /// <summary>
        /// Enable console logging with the specified log logLevel.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfig WithLogs(LLamaLogLevel logLevel = LLamaLogLevel.Info)
        {
            ThrowIfLoaded();

            _logging = true;
            _logLevel = logLevel;
            return this;
        }

        /// <summary>
        /// Add self-defined search directories. Note that the file stucture of the added 
        /// directories must be the same as the default directory. Besides, the directory 
        /// won't be used recursively.
        /// </summary>
        /// <param name="directories"></param>
        /// <returns></returns>
        public NativeLibraryConfig WithSearchDirectories(IEnumerable<string> directories)
        {
            ThrowIfLoaded();

            _searchDirectories.AddRange(directories);
            return this;
        }

        /// <summary>
        /// Add self-defined search directories. Note that the file stucture of the added 
        /// directories must be the same as the default directory. Besides, the directory 
        /// won't be used recursively.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public NativeLibraryConfig WithSearchDirectory(string directory)
        {
            ThrowIfLoaded();

            _searchDirectories.Add(directory);
            return this;
        }
        #endregion

        internal static Description CheckAndGatherDescription(LibraryName library)
        {
            if (Instance._allowFallback && Instance._skipCheck)
                throw new ArgumentException("Cannot skip the check when fallback is allowed.");

            var path = library switch
            {
                LibraryName.Llama => Instance._libraryPath,
                LibraryName.LlavaShared => Instance._libraryPathLLava,
                _ => throw new ArgumentException($"Unknown library name '{library}'", nameof(library)),
            };

            return new Description(
                path,
                library,
                Instance._useCuda,
                Instance._avxLevel,
                Instance._allowFallback,
                Instance._skipCheck,
                Instance._logging,
                Instance._logLevel,
                Instance._searchDirectories.Concat(new[] { "./" }).ToArray()
            );
        }

        internal static string AvxLevelToString(AvxLevel level)
        {
            return level switch
            {
                AvxLevel.None => string.Empty,
                AvxLevel.Avx => "avx",
                AvxLevel.Avx2 => "avx2",
                AvxLevel.Avx512 => "avx512",
                _ => throw new ArgumentException($"Unknown AvxLevel '{level}'")
            };
        }

        /// <summary>
        /// Private constructor prevents new instances of this class being created
        /// </summary>
        private NativeLibraryConfig()
        {
            // Automatically detect the highest supported AVX level
            if (System.Runtime.Intrinsics.X86.Avx.IsSupported)
                _avxLevel = AvxLevel.Avx;
            if (System.Runtime.Intrinsics.X86.Avx2.IsSupported)
                _avxLevel = AvxLevel.Avx2;

            if (CheckAVX512())
                _avxLevel = AvxLevel.Avx512;
        }

        private static bool CheckAVX512()
        {
            if (!System.Runtime.Intrinsics.X86.X86Base.IsSupported)
                return false;

            // ReSharper disable UnusedVariable (ebx is used when < NET8)
            var (_, ebx, ecx, _) = System.Runtime.Intrinsics.X86.X86Base.CpuId(7, 0);
            // ReSharper restore UnusedVariable

            var vnni = (ecx & 0b_1000_0000_0000) != 0;

#if NET8_0_OR_GREATER
            var f = System.Runtime.Intrinsics.X86.Avx512F.IsSupported;
            var bw = System.Runtime.Intrinsics.X86.Avx512BW.IsSupported;
            var vbmi = System.Runtime.Intrinsics.X86.Avx512Vbmi.IsSupported;
#else
            var f = (ebx & (1 << 16)) != 0;
            var bw = (ebx & (1 << 30)) != 0;
            var vbmi = (ecx & 0b_0000_0000_0010) != 0;
#endif

            return vnni && vbmi && bw && f;
        }

        /// <summary>
        /// Avx support configuration
        /// </summary>
        public enum AvxLevel
        {
            /// <summary>
            /// No AVX
            /// </summary>
            None,

            /// <summary>
            /// Advanced Vector Extensions (supported by most processors after 2011)
            /// </summary>
            Avx,

            /// <summary>
            /// AVX2 (supported by most processors after 2013)
            /// </summary>
            Avx2,

            /// <summary>
            /// AVX512 (supported by some processors after 2016, not widely supported)
            /// </summary>
            Avx512,
        }

        internal record Description(string? Path, LibraryName Library, bool UseCuda, AvxLevel AvxLevel, bool AllowFallback, bool SkipCheck, bool Logging, LLamaLogLevel LogLevel, string[] SearchDirectories)
        {
            public override string ToString()
            {
                string avxLevelString = AvxLevel switch
                {
                    AvxLevel.None => "NoAVX",
                    AvxLevel.Avx => "AVX",
                    AvxLevel.Avx2 => "AVX2",
                    AvxLevel.Avx512 => "AVX512",
                    _ => "Unknown"
                };

                string searchDirectoriesString = "{ " + string.Join(", ", SearchDirectories) + " }";

                return $"NativeLibraryConfig Description:\n" +
                       $"- LibraryName: {Library}\n" +
                       $"- Path: '{Path}'\n" +
                       $"- PreferCuda: {UseCuda}\n" +
                       $"- PreferredAvxLevel: {avxLevelString}\n" +
                       $"- AllowFallback: {AllowFallback}\n" +
                       $"- SkipCheck: {SkipCheck}\n" +
                       $"- Logging: {Logging}\n" +
                       $"- LogLevel: {LogLevel}\n" +
                       $"- SearchDirectories and Priorities: {searchDirectoriesString}";
            }
        }
    }
#endif

    public sealed partial class NativeLibraryConfig
    {
        /// <summary>
        /// Get the config instance
        /// </summary>
        public static NativeLibraryConfig Instance { get; } = new();

        /// <summary>
        /// Check if the native library has already been loaded. Configuration cannot be modified if this is true.
        /// </summary>
        public static bool LibraryHasLoaded { get; internal set; }

        internal NativeLogConfig.LLamaLogCallback? LogCallback;
        internal ILogger? LoggerCallback;

        private static void ThrowIfLoaded()
        {
            if (LibraryHasLoaded)
                throw new InvalidOperationException("NativeLibraryConfig must be configured before using **any** other LLamaSharp methods!");
        }

        /// <summary>
        /// Set the log callback that will be used for all llama.cpp log messages
        /// </summary>
        /// <param name="callback"></param>
        /// <exception cref="NotImplementedException"></exception>
        public NativeLibraryConfig WithLogCallback(NativeLogConfig.LLamaLogCallback? callback)
        {
            ThrowIfLoaded();

            LogCallback = callback;
            LoggerCallback = null;
            return this;
        }

        /// <summary>
        /// Set the log callback that will be used for all llama.cpp log messages
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="NotImplementedException"></exception>
        public NativeLibraryConfig WithLogCallback(ILogger? logger)
        {
            ThrowIfLoaded();

            LogCallback = null;
            LoggerCallback = logger;
            return this;
        }
    }

    internal enum LibraryName
    {
        Llama,
        LlavaShared
    }

    internal static class LibraryNameExtensions
    {
        public static string GetLibraryName(this LibraryName name)
        {
            switch (name)
            {
                case LibraryName.Llama:
                    return NativeApi.libraryName;
                case LibraryName.LlavaShared:
                    return NativeApi.llavaLibraryName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(name), name, null);
            }
        }
    }
}
