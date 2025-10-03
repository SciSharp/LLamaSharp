using System;
using System.Collections.Generic;
using System.Linq;
using LLama.Abstractions;
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

        private bool _useCuda = true;
        private bool _useVulkan = true;
        private AvxLevel _avxLevel;
        private bool _allowFallback = true;
        private bool _skipCheck = false;

        /// <summary>
        /// search directory -> priority level, 0 is the lowest.
        /// </summary>
        private readonly List<string> _searchDirectories = new List<string>();

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
        /// Configure whether to use vulkan backend if possible. Default is true.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfig WithVulkan(bool enable = true)
        {
            ThrowIfLoaded();

            _useVulkan = enable;
            return this;
        }

        /// <summary>
        /// Configure the preferred AVX support level of the backend. 
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


            return new Description(
                path,
                NativeLibraryName,
                _useCuda,
                _useVulkan,
                _avxLevel,
                _allowFallback,
                _skipCheck,
                _searchDirectories.Concat([ "./" ]).ToArray()
            );
        }

        internal static string AvxLevelToString(AvxLevel level)
        {
            return level switch
            {
                AvxLevel.None => "noavx",
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
        /// The description of the native library configurations that's already specified.
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Library"></param>
        /// <param name="UseCuda"></param>
        /// <param name="AvxLevel"></param>
        /// <param name="AllowFallback"></param>
        /// <param name="SkipCheck"></param>
        /// <param name="SearchDirectories"></param>
        /// <param name="UseVulkan"></param>
        public record Description(string? Path, NativeLibraryName Library, bool UseCuda, bool UseVulkan, AvxLevel AvxLevel, bool AllowFallback, bool SkipCheck, 
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
                       $"- PreferVulkan: {UseVulkan}\n" +
                       $"- PreferredAvxLevel: {avxLevelString}\n" +
                       $"- AllowFallback: {AllowFallback}\n" +
                       $"- SkipCheck: {SkipCheck}\n" +
                       $"- SearchDirectories and Priorities: {searchDirectoriesString}";
            }
        }
    }
#endif

    /// <summary>
    /// Global configuration handle for the Native (cpp) library.
    /// </summary>
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

        static NativeLibraryConfig()
        {
            LLama = new(NativeLibraryName.LLama);
            LLava = new(NativeLibraryName.LLava);
            All = new(LLama, LLava);
        }

#if NETSTANDARD2_0
        private NativeLibraryConfig(NativeLibraryName nativeLibraryName)
        {
            NativeLibraryName = nativeLibraryName;
        }
#endif

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

        /// <summary>
        /// Try to load the native library with the current configurations, 
        /// but do not actually set it to <see cref="NativeApi"/>.
        /// 
        /// You can still modify the configuration after this calling but only before any call from <see cref="NativeApi"/>.
        /// </summary>
        /// <param name="loadedLibrary">
        /// The loaded livrary. When the loading failed, this will be null. 
        /// However if you are using .NET standard2.0, this will never return null.
        /// </param>
        /// <returns>Whether the running is successful.</returns>
        public bool DryRun(out INativeLibrary? loadedLibrary)
        {
            LogCallback?.Invoke(LLamaLogLevel.Debug, $"Beginning dry run for {NativeLibraryName.GetLibraryName()}...");
            return NativeLibraryUtils.TryLoadLibrary(this, out loadedLibrary) != IntPtr.Zero;
        }
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

        /// <summary>
        /// Do an action for all the configs in this container.
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<NativeLibraryConfig> action)
        {
            foreach (var config in _configs)
            {
                action(config);
            }
        }

        #region configurators

#if NET6_0_OR_GREATER
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
        /// Configure whether to use vulkan backend if possible.
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if `LibraryHasLoaded` is true.</exception>
        public NativeLibraryConfigContainer WithVulkan(bool enable = true)
        {
            foreach(var config in _configs)
            {
                config.WithVulkan(enable);
            }
            return this;
        }

        /// <summary>
        /// Configure the preferred AVX support level of the backend.
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
#endif

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
                success &= config.DryRun(out var loadedLibrary);
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
