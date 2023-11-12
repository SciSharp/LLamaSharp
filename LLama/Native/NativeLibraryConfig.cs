using System;

namespace LLama.Native
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// A class about configurations when loading native libraries. 
    /// Note that it could be configured only once before any call to llama model apis.
    /// </summary>
    public class NativeLibraryConfig
    {
        private static NativeLibraryConfig? instance;
        private static readonly object lockObject = new object();
        public static NativeLibraryConfig Default
        {
            get
            {
                return GetInstance();
            }
        }

        /// <summary>
        /// Whether there's already a config for native library.
        /// </summary>
        public static bool LibraryHasLoaded { get; internal set; } = false;

        private string _libraryPath;
        private bool _useCuda;
        private AvxLevel _avxLevel;
        private bool _allowFallback;
        private bool _skipCheck;
        private bool _logging;

        internal static NativeLibraryConfig GetInstance()
        {
            if (instance is null)
            {
                lock (lockObject)
                {
                    if (instance is null)
                    {
                        instance = new NativeLibraryConfig();
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// Load a specified native library as backend for LLamaSharp.
        /// When this method is called, all the other configurations will be ignored.
        /// </summary>
        /// <param name="libraryPath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public NativeLibraryConfig WithLibrary(string libraryPath)
        {
            if (LibraryHasLoaded)
            {
                throw new InvalidOperationException("NativeLibraryConfig could be configured only once before any call to llama model apis.");
            }
            _libraryPath = libraryPath;
            return this;
        }

        /// <summary>
        /// Configure whether to use cuda backend if possible.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public NativeLibraryConfig WithCuda(bool enable = true)
        {
            if (LibraryHasLoaded)
            {
                throw new InvalidOperationException("NativeLibraryConfig could be configured only once before any call to llama model apis.");
            }
            _useCuda = enable;
            return this;
        }

        /// <summary>
        /// Configure the prefferred avx support level of the backend.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public NativeLibraryConfig WithAvx(AvxLevel level)
        {
            if (LibraryHasLoaded)
            {
                throw new InvalidOperationException("NativeLibraryConfig could be configured only once before any call to llama model apis.");
            }
            _avxLevel = level;
            return this;
        }

        /// <summary>
        /// Configure whether to allow fallback when there's not match for preffered settings.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public NativeLibraryConfig WithAutoFallback(bool enable = true)
        {
            if (LibraryHasLoaded)
            {
                throw new InvalidOperationException("NativeLibraryConfig could be configured only once before any call to llama model apis.");
            }
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
        /// <exception cref="InvalidOperationException"></exception>
        public NativeLibraryConfig SkipCheck(bool enable = true)
        {
            if (LibraryHasLoaded)
            {
                throw new InvalidOperationException("NativeLibraryConfig could be configured only once before any call to llama model apis.");
            }
            _skipCheck = enable;
            return this;
        }

        /// <summary>
        /// Whether to output the logs to console when loading the native library with your configuration.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public NativeLibraryConfig WithLogs(bool enable = true)
        {
            if (LibraryHasLoaded)
            {
                throw new InvalidOperationException("NativeLibraryConfig could be configured only once before any call to llama model apis.");
            }
            _logging = enable;
            return this;
        }

        internal static Description CheckAndGatherDescription()
        {
            if (Default._allowFallback && Default._skipCheck)
            {
                throw new ArgumentException("Cannot skip the check when fallback is allowed.");
            }
            return new Description(Default._libraryPath, Default._useCuda, Default._avxLevel, Default._allowFallback, Default._skipCheck, Default._logging);
        }

        internal static string AvxLevelToString(AvxLevel level)
        {
            return level switch
            {
                AvxLevel.None => string.Empty,
                AvxLevel.Avx => "avx",
                AvxLevel.Avx2 => "avx2",
#if NET8_0_OR_GREATER
                AvxLevel.Avx512 => "avx512"
#endif
                _ => throw new ArgumentException($"Cannot recognize Avx level {level}")
            };
        }


        private NativeLibraryConfig()
        {
            _libraryPath = string.Empty;
            _useCuda = true;
            _avxLevel = AvxLevel.Avx2;
            _allowFallback = true;
            _skipCheck = false;
            _logging = false;
        }

        /// <summary>
        /// Avx support configuration
        /// </summary>
        public enum AvxLevel
        {
            /// <inheritdoc />
            None = 0,
            /// <inheritdoc />
            Avx = 1,
            /// <inheritdoc />
            Avx2 = 2,
#if NET8_0_OR_GREATER
            /// <inheritdoc />
            Avx512 = 3,
#endif
        }
        internal record Description(string Path = "", bool UseCuda = true, AvxLevel AvxLevel = AvxLevel.Avx2,
            bool AllowFallback = true, bool SkipCheck = false, bool Logging = false);
    }
#endif
}
