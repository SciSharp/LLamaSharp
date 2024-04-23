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
        private AvxLevel _avxLevel;
        private bool _allowFallback = true;
        private bool _skipCheck = false;
        private bool _allowAutoDownload = false;
        private NativeLibraryDownloadSettings _downloadSettings = NativeLibraryDownloadSettings.Create();

        /// <summary>
        /// search directory -> priority level, 0 is the lowest.
        /// </summary>
        private readonly List<string> _searchDirectories = new List<string>();

        internal INativeLibrarySelectingPolicy SelectingPolicy { get; private set; } = new DefaultNativeLibrarySelectingPolicy();

        internal bool AllowAutoDownload => _allowAutoDownload;

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

        /// <summary>
        /// Set whether to download the best-matched native library file automatically if there's no backend or specified file to load.
        /// You could add a setting here to customize the behavior of the download.
        /// 
        /// If auto-download is enabled, please call <see cref="DryRun"/> after you have finished setting your configurations.
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public NativeLibraryConfig WithAutoDownload(bool enable = true, NativeLibraryDownloadSettings? settings = null)
        {
            ThrowIfLoaded();

            _allowAutoDownload = enable;
            if (settings is not null)
                _downloadSettings = settings;
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

            // Don't modify and pass the original object to `Description`, create a new one instead.
            // Also, we need to set the default local directory if the user does not.
            var defaultLocalDir = NativeLibraryDownloadSettings.GetDefaultLocalDir(GetCommitHash(_downloadSettings.Tag));
            var downloadSettings = NativeLibraryDownloadSettings.Create()
                .WithEndpoint(_downloadSettings.Endpoint).WithEndpointFallbacks(_downloadSettings.EndpointFallbacks ?? [])
                .WithRepoId(_downloadSettings.RepoId).WithToken(_downloadSettings.Token).WithTag(_downloadSettings.Tag)
                .WithTimeout(_downloadSettings.Timeout).WithLocalDir(_downloadSettings.LocalDir ?? defaultLocalDir);

            return new Description(
                path,
                NativeLibraryName,
                _useCuda,
                _avxLevel,
                _allowFallback,
                _skipCheck,
                _searchDirectories.Concat(new[] { "./" }).ToArray(), 
                _allowAutoDownload, 
                downloadSettings
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
            // This value should be changed when we're going to publish new release. (any better approach?)
            _downloadSettings = new NativeLibraryDownloadSettings().WithTag(GetCommitHash("master"));

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
        /// <param name="AllowAutoDownload"></param>
        /// <param name="DownloadSettings"></param>
        public record Description(string? Path, NativeLibraryName Library, bool UseCuda, AvxLevel AvxLevel, bool AllowFallback, bool SkipCheck, 
            string[] SearchDirectories, bool AllowAutoDownload, NativeLibraryDownloadSettings DownloadSettings)
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
                       $"- SearchDirectories and Priorities: {searchDirectoriesString}" + 
                       $"- AllowAutoDownload: {AllowAutoDownload}\n" + 
                       $"- DownloadSettings: {DownloadSettings}\n";
            }
        }
    }
#endif

    public sealed partial class NativeLibraryConfig
    {
        /// <summary>
        /// Set configurations for all the native libraries, including LLama and LLava
        /// </summary>
        [Obsolete("Please use NativeLibraryConfig.All instead, or set configurations for NativeLibraryConfig.LLama and NativeLibraryConfig.LLavaShared respectively.")]
        public static NativeLibraryConfigContainer Instance { get; }

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
        public static NativeLibraryConfig LLavaShared { get; }

        /// <summary>
        /// A dictionary mapping from version to corresponding llama.cpp commit hash.
        /// The version should be formatted int `[major].[minor].[patch]`. But there's an exceptance that you can 
        /// use `master` as a version to get the llama.cpp commit hash from the master branch.
        /// </summary>
        public static Dictionary<string, string> VersionMap { get; } = new Dictionary<string, string>() 
        // This value should be changed when we're going to publish new release. (any better approach?)
        {
            {"master", "f7001c"}
        };

        internal static string GetCommitHash(string version)
        {
            if(VersionMap.TryGetValue(version, out var hash))
            {
                return hash;
            }
            else
            {
                return version;
            }
        }

        static NativeLibraryConfig()
        {
            LLama = new(NativeLibraryName.Llama);
            LLavaShared = new(NativeLibraryName.LlavaShared);
            All = new(LLama, LLavaShared);
            Instance = All;
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

        /// <summary>
        /// Whether <see cref="DryRun"/> has been called.
        /// </summary>
        internal bool HasCalledDryRun { get; private set; } = false;

        internal NativeLibraryName NativeLibraryName { get; }

        internal NativeLogConfig.LLamaLogCallback? LogCallback { get; private set; } = null;

        private void ThrowIfLoaded()
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
        /// <returns>Whether the running is successful.</returns>
        public bool DryRun()
        {
            LogCallback?.Invoke(LLamaLogLevel.Debug, $"Beginning dry run for {this.NativeLibraryName.GetLibraryName()}...");
            HasCalledDryRun = true;
            return NativeLibraryUtils.TryLoadLibrary(this) != IntPtr.Zero;
        }
    }

    /// <summary>
    /// A class to set same configurations to multiple libraries at the same time.
    /// </summary>
    public sealed partial class NativeLibraryConfigContainer
    {
        private NativeLibraryConfig[] _configs;

        internal NativeLibraryConfigContainer(params NativeLibraryConfig[] configs)
        {
            _configs = configs;
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
                if(config.NativeLibraryName == NativeLibraryName.Llama && llamaPath is not null)
                {
                    config.WithLibrary(llamaPath);
                }
                if(config.NativeLibraryName == NativeLibraryName.LlavaShared && llavaPath is not null)
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
        /// Add self-defined search directories. Note that the file stucture of the added 
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
        /// Add self-defined search directories. Note that the file stucture of the added 
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
        /// Set whether to download the best-matched native library file automatically if there's no backend or specified file to load.
        /// You could add a setting here to customize the behavior of the download.
        /// 
        /// If auto-download is enabled, please call <see cref="DryRun"/> after you have finished setting your configurations.
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public NativeLibraryConfigContainer WithAutoDownload(bool enable = true, NativeLibraryDownloadSettings? settings = null)
        {
            foreach (var config in _configs)
            {
                config.WithAutoDownload(enable, settings);
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
        public bool DryRun()
        {
            return _configs.All(config => config.DryRun());
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
        Llama,
        /// <summary>
        /// The native library compiled from the LLaVA example of llama.cpp.
        /// </summary>
        LlavaShared
    }

    internal static class LibraryNameExtensions
    {
        public static string GetLibraryName(this NativeLibraryName name)
        {
            switch (name)
            {
                case NativeLibraryName.Llama:
                    return NativeApi.libraryName;
                case NativeLibraryName.LlavaShared:
                    return NativeApi.llavaLibraryName;
                default:
                    throw new ArgumentOutOfRangeException(nameof(name), name, null);
            }
        }
    }
}
