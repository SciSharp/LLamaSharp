using System;

namespace LLama
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

        /// <summary>
        /// Whether there's already a config for native library.
        /// </summary>
        public bool Initialied { get; private set; }
        internal Description Desc { get; private set; }

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
        /// Load a specified native library as backend for LLamaSharp
        /// </summary>
        /// <param name="libraryPath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void WithLibrary(string libraryPath)
        {
            var config = GetInstance();
            if (config.Initialied)
            {
                throw new InvalidOperationException("NativeLibraryConfig could be configured only once before any call to llama model apis.");
            }
            config.Desc = new Description(libraryPath);
        }

        /// <summary>
        /// Ass rules to match a suitable library from installed LLamaSharp backend.
        /// </summary>
        /// <param name="useCuda"></param>
        /// <param name="avxLevel"></param>
        /// <param name="allowFallback">Whether to allow fall-back when your hardware doesn't support your configuration.</param>
        /// <param name="skipCheck">Whether to skip the check when fallback is allowed. 
        /// It's especially useful when your cuda library is not in the default path. </param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void WithMatchRule(bool useCuda = true, AvxLevel avxLevel = AvxLevel.Avx2, bool allowFallback = true, bool skipCheck = false)
        {
            if(allowFallback && skipCheck)
            {
                throw new ArgumentException("Cannot skip the check when fallback is allowed.");
            }
            var config = GetInstance();
            if (config.Initialied)
            {
                throw new InvalidOperationException("NativeLibraryConfig could be configured only once before any call to llama model apis.");
            }
            config.Desc = new Description(UseCuda: useCuda, AvxLevel: avxLevel, AllowFallback: allowFallback, SkipCheck: skipCheck);
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
            Desc = new Description();
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
        internal record Description(string Path = "", bool UseCuda = true, AvxLevel AvxLevel = AvxLevel.Avx2, bool AllowFallback = true, bool SkipCheck = false);
    }
#endif
            }
