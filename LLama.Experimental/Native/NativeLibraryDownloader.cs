using HuggingfaceHub;

namespace LLama.Native
{
    internal class NativeLibraryDownloader
    {
        /// <summary>
        /// Download the library file
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="remoteFilePath"></param>
        /// <param name="logCallback"></param>
        /// <returns>The local path of the file if successful otherwise null.</returns>
        public static async Task<string?> DownloadLibraryFile(NativeLibraryDownloadSettings settings, string remoteFilePath, NativeLogConfig.LLamaLogCallback? logCallback = null)
        {
            if (settings.LocalDir is null)
            {
                // Null local directory is not expected here (it will make things more complex if we want to handle it).
                // It should always be set when gathering the description.
                throw new Exception("Auto-download is enabled for native library but the `LocalDir` is null. " +
                    "It's an unexpected behavior and please report an issue to LLamaSharp.");
            }
            HFGlobalConfig.DefaultDownloadTimeout = settings.Timeout;

            HashSet<string> endpointSet = new([settings.Endpoint]);
            if (settings.EndpointFallbacks is not null)
            {
                foreach (var endpoint in settings.EndpointFallbacks)
                {
                    endpointSet.Add(endpoint);
                }
            }
            var endpoints = endpointSet.ToArray();

            Dictionary<string, string> exceptionMap = new();
            foreach(var endpoint in endpoints)
            {
                logCallback?.Invoke(LLamaLogLevel.Debug, $"Downloading the native library file '{remoteFilePath}' from {endpoint} with repo = {settings.RepoId}, tag = {settings.Tag}");
                var path = await HFDownloader.DownloadFileAsync(settings.RepoId, remoteFilePath, revision: settings.Tag, cacheDir: settings.CacheDir,
                    localDir: settings.LocalDir, token: settings.Token, endpoint: endpoint);
                if (path is not null)
                {
                    logCallback?.Invoke(LLamaLogLevel.Debug, $"Successfully downloaded the native library file to {path}");
                    return path;
                }
                else
                {
                    logCallback?.Invoke(LLamaLogLevel.Warning, "The download failed without an explicit error, please check your configuration or report an issue to LLamaSharp.");
                }
            }

            // means that the download finally fails.
            return null;
        }
    }

    /// <summary>
    /// Settings for downloading the native library.
    /// </summary>
    public class NativeLibraryDownloadSettings
    {
        /// <summary>
        /// The endpoint to download from, by default the official site of HuggingFace.
        /// </summary>
        public string Endpoint { get; private set; } = "https://huggingface.co";

        /// <summary>
        /// Endpoints to fallback to if downloading with the main endpoint fails.
        /// 
        /// Generally this is an option for those countries or regions where the main endpoint is blocked.
        /// You should not put too many endpoints here, as it will slow down the downloading process.
        /// </summary>
        public string[]? EndpointFallbacks { get; private set; } = null;

        /// <summary>
        /// The version of the library to download. Please use LLamaSharp version in format `[major].[minor].[patch]` as tag 
        /// or go to https://huggingface.co/AsakusaRinne/LLamaSharpNative 
        /// to see all available tags, or use your own repo and tags.
        /// </summary>
        public string Tag { get; private set; } = string.Empty;

        /// <summary>
        /// The repo id to download the native library files.
        /// </summary>
        public string RepoId { get; private set; } = "AsakusaRinne/LLamaSharpNative";

        /// <summary>
        /// The directory to cache the downloaded files. If you only want to make the downloaded files appear in a directory, 
        /// regardless of whether the file will have a copy in another place, please set <see cref="LocalDir"/> instead.
        /// </summary>
        public string CacheDir { get; private set; }

        /// <summary>
        /// If provided, the downloaded file will be placed under this directory, 
        /// either as a symlink (default) or a regular file.
        /// </summary>
        public string? LocalDir { get; private set; } = null;

        /// <summary>
        /// If you are using your own private repo as remote source, you could set the token to get the access.
        /// </summary>
        public string? Token { get; private set; } = null;

        /// <summary>
        /// The timeout (second) of the native library file download.
        /// </summary>
        public int Timeout { get; private set; } = 10;

        /// <summary>
        /// Extra search directories. They will only be used when finding files downloaded from remote.
        /// Generally it will be useful when you want to replace the downloading process with your custom implementation.
        /// If you are not sure how it works, please leave it empty.
        /// </summary>
        public string[]? ExtraSearchDirectories { get; private set; } = null;

        internal NativeLibraryDownloadSettings()
        {
            var home = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache");
            CacheDir = Path.Combine(home, "llama_sharp");
        }

        internal static string GetDefaultLocalDir(string tag)
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(home, ".llama_sharp", tag);
        }

        /// <summary>
        /// Create a <see cref="NativeLibraryDownloadSettings"/> with default settings.
        /// </summary>
        /// <returns></returns>
        public static NativeLibraryDownloadSettings Create()
        {
            return new NativeLibraryDownloadSettings();
        }

        /// <summary>
        /// Set the default endpoint to download file from.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public NativeLibraryDownloadSettings WithEndpoint(string endpoint)
        {
            Endpoint = endpoint;
            return this;
        }

        /// <summary>
        /// Set the endpoints to try when the download fails with the default endpoint.
        /// </summary>
        /// <param name="endpoints"></param>
        /// <returns></returns>
        public NativeLibraryDownloadSettings WithEndpointFallbacks(params string[] endpoints)
        {
            EndpointFallbacks = endpoints;
            return this;
        }

        /// <summary>
        /// Set the <see cref="Tag"/>
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public NativeLibraryDownloadSettings WithTag(string tag)
        {
            Tag = tag;
            return this;
        }

        /// <summary>
        /// Set the <see cref="RepoId"/>
        /// </summary>
        /// <param name="repoId"></param>
        /// <returns></returns>
        public NativeLibraryDownloadSettings WithRepoId(string repoId)
        {
            RepoId = repoId;
            return this;
        }

        /// <summary>
        /// Set the <see cref="CacheDir"/>. If you only want to make the downloaded files appear in a directory, 
        /// regardless of whether the file may have a copy in another place, please use <see cref="WithLocalDir(string)"/>instead.
        /// </summary>
        /// <param name="cacheDir"></param>
        /// <returns></returns>
        public NativeLibraryDownloadSettings WithCacheDir(string cacheDir)
        {
            CacheDir = cacheDir;
            return this;
        }

        /// <summary>
        /// Set the <see cref="LocalDir"/>
        /// </summary>
        /// <param name="localDir"></param>
        /// <returns></returns>
        public NativeLibraryDownloadSettings WithLocalDir(string localDir)
        {
            LocalDir = localDir;
            return this;
        }

        /// <summary>
        /// Set the <see cref="Token"/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public NativeLibraryDownloadSettings WithToken(string token)
        {
            Token = token;
            return this;
        }

        /// <summary>
        /// Set the <see cref="Timeout"/>
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public NativeLibraryDownloadSettings WithTimeout(int timeout)
        {
            Timeout = timeout;
            return this;
        }

        /// <summary>
        /// Set <see cref="ExtraSearchDirectories"/>. They will only be used when finding files downloaded from remote.
        /// Generally it will be useful when you want to replace the downloading process with your custom implementation.
        /// If you are not sure how it works, please ignore this method.
        /// </summary>
        /// <param name="directories"></param>
        /// <returns></returns>
        public NativeLibraryDownloadSettings WithExtraSearchDirectories(string[] directories)
        {
            ExtraSearchDirectories = directories;
            return this;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            // Token should be hidden when printing it.
            string hiddenToken = "";
            if (Token is not null)
            {
                if (Token.Length <= 10)
                {
                    hiddenToken = new string('*', Token.Length - 1) + Token.Last();
                }
                else
                {
                    hiddenToken += Token.Substring(0, 2);
                    hiddenToken += new string('*', Token.Length - 3);
                    hiddenToken += Token.Last();
                }
            }

            return $"(Endpoint = {Endpoint}, " +
                    $"EndpointFallbacks = {string.Join(", ", EndpointFallbacks ?? new string[0])}, " +
                    $"Tag = {Tag}, " +
                    $"RepoId = {RepoId}, " +
                    $"CacheDir = {CacheDir}, " +
                    $"LocalDir = {LocalDir}, " +
                    $"Token = {hiddenToken}, " +
                    $"Timeout = {Timeout}s)";
        }
    }
}
