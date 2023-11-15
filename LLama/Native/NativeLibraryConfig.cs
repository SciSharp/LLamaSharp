using System;

namespace LLama.Native
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// Allows configuration of the native llama.cpp libraries to load and use.
    /// All configuration must be done before using **any** other LLamaSharp methods!
    /// </summary>
    public sealed class NativeLibraryConfig
    {
        private static readonly Lazy<NativeLibraryConfig> _instance = new(() => new NativeLibraryConfig());

        /// <summary>
        /// Get the config instance
        /// </summary>
        public static NativeLibraryConfig Instance => _instance.Value;

        /// <summary>
        /// Whether there's already a config for native library.
        /// </summary>
        public static bool LibraryHasLoaded { get; internal set; } = false;

        private string _libraryPath = string.Empty;
        private bool _useCuda = true;
        private AvxLevel _avxLevel;
        private bool _allowFallback = true;
        private bool _skipCheck = false;
        private bool _logging = false;

        private static void ThrowIfLoaded()
        {
            if (LibraryHasLoaded)
                throw new InvalidOperationException("NativeLibraryConfig must be configured before using **any** other LLamaSharp methods!");
        }

        /// <summary>
        /// Load a specified native library as backend for LLamaSharp.
        /// When this method is called, all the other configurations will be ignored.
        /// </summary>
        /// <param name="libraryPath"></param>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfig WithLibrary(string libraryPath)
        {
            ThrowIfLoaded();

            _libraryPath = libraryPath;
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
        public NativeLibraryConfig WithLogs(bool enable = true)
        {
            ThrowIfLoaded();

            _logging = enable;
            return this;
        }

        internal static Description CheckAndGatherDescription()
        {
            if (Instance._allowFallback && Instance._skipCheck)
            {
                throw new ArgumentException("Cannot skip the check when fallback is allowed.");
            }
            return new Description(Instance._libraryPath, Instance._useCuda, Instance._avxLevel, Instance._allowFallback, Instance._skipCheck, Instance._logging);
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
#if NET8_0_OR_GREATER
            if (System.Runtime.Intrinsics.X86.Avx512F.IsSupported)
                _avxLevel = AvxLevel.Avx512;
#endif
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

        internal record Description(string Path, bool UseCuda, AvxLevel AvxLevel, bool AllowFallback, bool SkipCheck, bool Logging);
    }
#endif
        }
