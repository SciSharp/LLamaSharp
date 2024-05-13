using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LLama.Abstractions;
using Microsoft.Extensions.Logging;

namespace LLama.Native
{
    /// <summary>
    /// Allows configuration of the native llama.cpp libraries to load and use.
    /// <c>All configuration must be done before using **any** other LLamaSharp methods!</c>
    /// </summary>
    public sealed partial class NativeLibraryConfig
    {
        private string? _libraryPath;

        private bool _useCuda = true;
        private AvxLevel _avxLevel;
        private bool _allowFallback = true;
        private bool _skipCheck = false;

        /// <summary>
        /// search directory -> priority level, 0 is the lowest.
        /// </summary>
        private readonly List<string> _searchDirectories = new List<string>();

#if NETSTANDARD
        internal static bool LLavaDisabled { get; private set; } = false;
        internal static bool DynamicLoadingDisabled { get; private set; } = false;

        /// <summary>
        /// Disable the llava library. If this method is called, the llava library will not be loaded.
        /// If the API related with LLava is called, An exception will be thrown.
        /// <c>This method is only available with .NET standard 2.0.</c>
        /// </summary>
        public void DisableLLava() => LLavaDisabled = true;

        /// <summary>
        /// Disable the dynamic loading. It might fix some weird behaviors of native API calling and might slightly improve the performance.
        /// However, if the dynamic loading is disabled, the native library can only be loaded from the default path, with no flexibility.
        /// <c>This method is only available with .NET standard 2.0.</c>
        /// </summary>
        public void DisableDynamicLoading() => DynamicLoadingDisabled = true;
#endif

        internal INativeLibrarySelectingPolicy SelectingPolicy { get; private set; } = new DefaultNativeLibrarySelectingPolicy();

        #region configurators
        /// <summary>
        /// Load a specified native library as backend for LLamaSharp.
        /// When this method is called, all the other configurations will be ignored.
        /// </summary>
        /// <param name="libraryPath">The full path to the native library to load.</param>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfig WithLibrary(string? libraryPath)
        {
            ThrowIfLoaded();

            _libraryPath = libraryPath;
            return this;
        }

        /// <summary>
        /// Configure whether to use cuda backend if possible. Default is true.
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
        /// Default value is detected automatically due to your operating system.
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
        /// Configure whether to allow fallback when there's no match for preferred settings. Default is true.
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
        ///  you have your cublas configured but LLamaSharp take it as invalid by mistake. Default is false;
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
        /// Add self-defined search directories. Note that the file structure of the added 
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
        /// Add self-defined search directories. Note that the file structure of the added 
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

        /// <summary>
        /// Set the policy which decides how to select the desired native libraries and order them by priority. 
        /// By default we use <see cref="DefaultNativeLibrarySelectingPolicy"/>.
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public NativeLibraryConfig WithSelectingPolicy(INativeLibrarySelectingPolicy policy)
        {
            ThrowIfLoaded();

            SelectingPolicy = policy;
            return this;
        }

        #endregion

        internal Description CheckAndGatherDescription()
        {
            if (_allowFallback && _skipCheck)
                throw new ArgumentException("Cannot skip the check when fallback is allowed.");

            var path = _libraryPath;
            var assemblyDirectoryName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            return new Description(
                path,
                NativeLibraryName,
                _useCuda,
                _avxLevel,
                _allowFallback,
                _skipCheck,
                _searchDirectories.Concat(
                    assemblyDirectoryName is null || assemblyDirectoryName == AppDomain.CurrentDomain.BaseDirectory ?
                        new[] { AppDomain.CurrentDomain.BaseDirectory }
                      : new[] { AppDomain.CurrentDomain.BaseDirectory, assemblyDirectoryName }
                ).ToArray()
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
        private NativeLibraryConfig(NativeLibraryName nativeLibraryName)
        {
            NativeLibraryName = nativeLibraryName;

#if NETSTANDARD
            // In .NET standard2.0 we don't have a way to get the system avx level so we set it as avx2 by default.
            _avxLevel = AvxLevel.Avx2;
#else
            // Automatically detect the highest supported AVX level
            if (System.Runtime.Intrinsics.X86.Avx.IsSupported)
                _avxLevel = AvxLevel.Avx;
            if (System.Runtime.Intrinsics.X86.Avx2.IsSupported)
                _avxLevel = AvxLevel.Avx2;

            if (CheckAVX512())
                _avxLevel = AvxLevel.Avx512;
#endif
        }

#if !NETSTANDARD
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
#endif

        /// <summary>
        /// The description of the native library configurations that's already specified.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Library"></param>
        /// <param name="UseCuda"></param>
        /// <param name="AvxLevel"></param>
        /// <param name="AllowFallback"></param>
        /// <param name="SkipCheck"></param>
        /// <param name="SearchDirectories"></param>
        public record Description(string? Path, NativeLibraryName Library, bool UseCuda, AvxLevel AvxLevel, bool AllowFallback, bool SkipCheck, 
            string[] SearchDirectories)
        {
            /// <inheritdoc/>
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
                       $"- SearchDirectories and Priorities: {searchDirectoriesString}";
            }
        }
    }

    public sealed partial class NativeLibraryConfig
    {
        /// <summary>
        /// Set configurations for all the native libraries, including LLama and LLava
        /// </summary>
        [Obsolete("Please use NativeLibraryConfig.All instead, or set configurations for NativeLibraryConfig.LLama and NativeLibraryConfig.LLavaShared respectively.")]
        public static NativeLibraryConfigContainer Instance => All;

        /// <summary>
        /// Set configurations for all the native libraries, including LLama and LLava
        /// </summary>
        public static NativeLibraryConfigContainer All { get; }

        /// <summary>
        /// Configuration for LLama native library
        /// </summary>
        public static NativeLibraryConfig LLama { get; }

        /// <summary>
        /// Configuration for LLava native library
        /// </summary>
        public static NativeLibraryConfig LLava { get; }


        /// <summary>
        /// The current version.
        /// </summary>
        public static string CurrentVersion => VERSION; // This should be changed before publishing new version. TODO: any better approach?

        private const string COMMIT_HASH = "f7001c";
        private const string VERSION = "master";

        /// <summary>
        /// Get the llama.cpp commit hash of the current version.
        /// </summary>
        /// <returns></returns>
        public static string GetNativeLibraryCommitHash() => COMMIT_HASH;

        static NativeLibraryConfig()
        {
            LLama = new(NativeLibraryName.LLama);
            LLava = new(NativeLibraryName.LLava);
            All = new(LLama, LLava);
        }

        /// <summary>
        /// Check if the native library has already been loaded. Configuration cannot be modified if this is true.
        /// </summary>
        public bool LibraryHasLoaded { get; internal set; }

        internal NativeLibraryName NativeLibraryName { get; }

        internal NativeLogConfig.LLamaLogCallback? LogCallback { get; private set; } = null;

        private void ThrowIfLoaded()
        {
            if (LibraryHasLoaded)
                throw new InvalidOperationException("The library has already loaded, you can't change the configurations. " +
                    "Please finish the configuration setting before any call to LLamaSharp native APIs." +
                    "Please use NativeLibraryConfig.DryRun if you want to see whether it's loaded " +
                    "successfully but still have chance to modify the configurations.");
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

            // Redirect to llama_log_set. This will wrap the logger in a delegate and bind that as the log callback instead.
            NativeLogConfig.llama_log_set(logger);

            return this;
        }
#if !NETSTANDARD
        /// <summary>
        /// Try to load the native library with the current configurations, 
        /// but do not actually set it to <see cref="NativeApi"/>.
        /// 
        /// You can still modify the configuration after this calling but only before any call from <see cref="NativeApi"/>.
        /// </summary>
        /// <param name="loadedLibrary">
        /// The loaded library. When the loading failed, it will be null. 
        /// </param>
        /// <param name="libraryPath">
        /// The path of the loaded library. When the loading failed, it will be null. 
        /// </param>
        /// <returns>Whether the running is successful.</returns>
        public bool DryRun(out INativeLibrary? loadedLibrary, out string? libraryPath)
        {
            LogCallback?.Invoke(LLamaLogLevel.Debug, $"Beginning dry run for {this.NativeLibraryName.GetLibraryName()}...");
            return NativeLibraryUtils.TryLoadLibrary(this, out loadedLibrary, out libraryPath) != IntPtr.Zero;
        }
#endif
    }

    /// <summary>
    /// A class to set same configurations to multiple libraries at the same time.
    /// </summary>
    public sealed class NativeLibraryConfigContainer
    {
        private NativeLibraryConfig[] _configs;

        internal NativeLibraryConfigContainer(params NativeLibraryConfig[] configs)
        {
            _configs = configs;
        }

        #region configurators

        /// <summary>
        /// Load a specified native library as backend for LLamaSharp.
        /// When this method is called, all the other configurations will be ignored.
        /// </summary>
        /// <param name="llamaPath">The full path to the llama library to load.</param>
        /// <param name="llavaPath">The full path to the llava library to load.</param>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfigContainer WithLibrary(string? llamaPath, string? llavaPath)
        {
            foreach(var config in _configs)
            {
                if(config.NativeLibraryName == NativeLibraryName.LLama && llamaPath is not null)
                {
                    config.WithLibrary(llamaPath);
                }
                if(config.NativeLibraryName == NativeLibraryName.LLava && llavaPath is not null)
                {
                    config.WithLibrary(llavaPath);
                }
            }

            return this;
        }

        /// <summary>
        /// Configure whether to use cuda backend if possible.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfigContainer WithCuda(bool enable = true)
        {
            foreach(var config in _configs)
            {
                config.WithCuda(enable);
            }
            return this;
        }

        /// <summary>
        /// Configure the prefferred avx support level of the backend.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfigContainer WithAvx(AvxLevel level)
        {
            foreach (var config in _configs)
            {
                config.WithAvx(level);
            }
            return this;
        }

        /// <summary>
        /// Configure whether to allow fallback when there's no match for preferred settings.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfigContainer WithAutoFallback(bool enable = true)
        {
            foreach (var config in _configs)
            {
                config.WithAutoFallback(enable);
            }
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
        public NativeLibraryConfigContainer SkipCheck(bool enable = true)
        {
            foreach (var config in _configs)
            {
                config.SkipCheck(enable);
            }
            return this;
        }

        /// <summary>
        /// Add self-defined search directories. Note that the file structure of the added 
        /// directories must be the same as the default directory. Besides, the directory 
        /// won't be used recursively.
        /// </summary>
        /// <param name="directories"></param>
        /// <returns></returns>
        public NativeLibraryConfigContainer WithSearchDirectories(IEnumerable<string> directories)
        {
            foreach (var config in _configs)
            {
                config.WithSearchDirectories(directories);
            }
            return this;
        }

        /// <summary>
        /// Add self-defined search directories. Note that the file structure of the added 
        /// directories must be the same as the default directory. Besides, the directory 
        /// won't be used recursively.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public NativeLibraryConfigContainer WithSearchDirectory(string directory)
        {
            foreach (var config in _configs)
            {
                config.WithSearchDirectory(directory);
            }
            return this;
        }

        /// <summary>
        /// Set the policy which decides how to select the desired native libraries and order them by priority. 
        /// By default we use <see cref="DefaultNativeLibrarySelectingPolicy"/>.
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public NativeLibraryConfigContainer WithSelectingPolicy(INativeLibrarySelectingPolicy policy)
        {
            foreach (var config in _configs)
            {
                config.WithSelectingPolicy(policy);
            }
            return this;
        }

        /// <summary>
        /// Set the log callback that will be used for all llama.cpp log messages
        /// </summary>
        /// <param name="callback"></param>
        /// <exception cref="NotImplementedException"></exception>
        public NativeLibraryConfigContainer WithLogCallback(NativeLogConfig.LLamaLogCallback? callback)
        {
            foreach (var config in _configs)
            {
                config.WithLogCallback(callback);
            }
            return this;
        }

        /// <summary>
        /// Set the log callback that will be used for all llama.cpp log messages
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="NotImplementedException"></exception>
        public NativeLibraryConfigContainer WithLogCallback(ILogger? logger)
        {
            foreach (var config in _configs)
            {
                config.WithLogCallback(logger);
            }
            return this;
        }

        #endregion

#if !NETSTANDARD
        /// <summary>
        /// Try to load the native library with the current configurations, 
        /// but do not actually set it to <see cref="NativeApi"/>.
        /// 
        /// You can still modify the configuration after this calling but only before any call from <see cref="NativeApi"/>.
        /// </summary>
        /// <returns>Whether the running is successful.</returns>
        public bool DryRun(out INativeLibrary? loadedLLamaNativeLibrary, out INativeLibrary? loadedLLavaNativeLibrary)
        {
            bool success = true;
            foreach(var config in _configs)
            {
                success &= config.DryRun(out var loadedLibrary, out var _);
                if(config.NativeLibraryName == NativeLibraryName.LLama)
                {
                    loadedLLamaNativeLibrary = loadedLibrary;
                }
                else if(config.NativeLibraryName == NativeLibraryName.LLava)
                {
                    loadedLLavaNativeLibrary = loadedLibrary;
                }
                else
                {
                    throw new Exception("Unknown native library config during the dry run.");
                }
            }
            loadedLLamaNativeLibrary = loadedLLavaNativeLibrary = null;
            return success;
        }
#endif
    }

    /// <summary>
    /// The name of the native library
    /// </summary>
    public enum NativeLibraryName
    {
        /// <summary>
        /// The native library compiled from llama.cpp.
        /// </summary>
        LLama,
        /// <summary>
        /// The native library compiled from the LLaVA example of llama.cpp.
        /// </summary>
        LLava
    }

    internal static class LibraryNameExtensions
    {
        public static string GetLibraryName(this NativeLibraryName name)
        {
            switch (name)
            {
                case NativeLibraryName.LLama:
                    return NativeApi.libraryName;
                case NativeLibraryName.LLava:
                    return NativeApi.llavaLibraryName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(name), name, null);
            }
        }
    }
}
