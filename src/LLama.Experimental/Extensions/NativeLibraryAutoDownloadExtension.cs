using LLama.Experimental.Native;

namespace LLama.Native
{
#if NET6_0_OR_GREATER
    public static class NativeLibraryAutoDownloadExtension
    {
        /// <summary>
        /// Set whether to download the best-matched native library file automatically if there's no backend or specified file to load.
        /// You could add a setting here to customize the behavior of the download.
        /// 
        /// * If auto-download is enabled, please call <see cref="NativeLibraryConfig.DryRun"/> after you have finished setting your configurations.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="enable"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static NativeLibraryConfig WithAutoDownload(this NativeLibraryConfig config, bool enable = true, NativeLibraryDownloadSettings? settings = null)
        {
            if (config.LibraryHasLoaded)
            {
                throw new Exception("The library has already loaded, you can't change the configurations. " +
                    "Please finish the configuration setting before any call to LLamaSharp native APIs." +
                    "Please use NativeLibraryConfig.DryRun if you want to see whether it's loaded successfully " +
                    "but still have chance to modify the configurations.");
            }
            if (enable)
            {
                if(settings is null)
                {
                    settings = NativeLibraryDownloadSettings.Create();
                }
                // Don't modify and pass the original object to `Description`, create a new one instead.
                // Also, we need to set the default local directory if the user does not.
                if (string.IsNullOrEmpty(settings.Tag))
                {
                    settings = settings.WithTag(GetNativeLibraryCommitHash());
                }
                var defaultLocalDir = NativeLibraryDownloadSettings.GetDefaultLocalDir(settings.Tag);
                settings = settings.WithLocalDir(settings.LocalDir ?? defaultLocalDir);

                // When using auto-download, this should be the only search directory.
                List<string> searchDirectoriesForDownload = [settings.LocalDir!];
                // unless extra search paths are added by the user.
                searchDirectoriesForDownload.AddRange(settings.ExtraSearchDirectories ?? []);
                config.WithSearchDirectories(searchDirectoriesForDownload);

                config.WithSelectingPolicy(new SelectingPolicyWithAutoDownload(settings));
            }
            return config;
        }

        private const string COMMIT_HASH = "a743d7";

        private static string GetNativeLibraryCommitHash() => COMMIT_HASH;

        /// <summary>
        /// Set whether to download the best-matched native library file automatically if there's no backend or specified file to load.
        /// You could add a setting here to customize the behavior of the download.
        /// 
        /// If auto-download is enabled, please call <see cref="NativeLibraryConfig.DryRun"/> after you have finished setting your configurations.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="enable"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static NativeLibraryConfigContainer WithAutoDownload(this NativeLibraryConfigContainer container,
            bool enable = true, NativeLibraryDownloadSettings? settings = null)
        {
            container.ForEach((config) => config.WithAutoDownload(enable, settings));
            return container;
        }
    }
#endif
}
