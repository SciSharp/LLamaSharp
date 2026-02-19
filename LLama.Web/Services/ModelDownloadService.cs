using System.Collections.Concurrent;
using LLama.Web.Common;
using LLama.Web.Hubs;
using LLama.Web.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace LLama.Web.Services;

public class ModelDownloadService : IModelDownloadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly HttpClient _httpClient;
    private readonly LLamaOptions _options;
    private readonly IHubContext<SessionConnectionHub, ISessionClient> _hubContext;
    private readonly string _modelsRoot;
    private readonly string _downloadsRoot;
    private readonly object _syncLock = new();
    private readonly ConcurrentDictionary<string, List<ModelAssetStatus>> _assetsByModel = new();

    private Task _downloadTask;
    private bool _started;
    private bool _initialized;

    public ModelDownloadService(IWebHostEnvironment environment, IHttpClientFactory httpClientFactory, IOptions<LLamaOptions> options, IHubContext<SessionConnectionHub, ISessionClient> hubContext)
    {
        _environment = environment;
        _httpClient = httpClientFactory.CreateClient(nameof(ModelDownloadService));
        _options = options.Value;
        _hubContext = hubContext;

        var appDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appRoot = Path.Combine(appDataRoot, "LLama.Web");
        _modelsRoot = Path.Combine(appRoot, "Models");
        _downloadsRoot = Path.Combine(appRoot, "Downloads");
    }

    public string ModelsRoot => _modelsRoot;
    public string DownloadsRoot => _downloadsRoot;

    public void StartDownloads(CancellationToken cancellationToken)
    {
        lock (_syncLock)
        {
            if (_started)
                return;

            _started = true;
            EnsureInitialized();
            _downloadTask = Task.Run(() => DownloadAllAsync(cancellationToken), cancellationToken);
        }
    }

    public Task WaitForDownloadsAsync(CancellationToken cancellationToken)
    {
        lock (_syncLock)
        {
            return _downloadTask ?? Task.CompletedTask;
        }
    }

    public bool IsModelReady(string modelName)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            return false;

        if (!_assetsByModel.TryGetValue(modelName, out var assets))
            return false;

        return assets.All(asset => asset.State == ModelDownloadState.Completed);
    }

    public IReadOnlyList<ModelDownloadSnapshot> GetSnapshots()
    {
        EnsureInitialized();
        var snapshots = new List<ModelDownloadSnapshot>();
        foreach (var kvp in _assetsByModel)
        {
            var assets = kvp.Value;
            var snapshot = new ModelDownloadSnapshot
            {
                ModelName = kvp.Key,
                Ready = assets.All(asset => asset.State == ModelDownloadState.Completed)
            };

            foreach (var asset in assets)
                snapshot.Assets.Add(ToSnapshot(asset));

            snapshots.Add(snapshot);
        }

        return snapshots;
    }

    private void EnsureInitialized()
    {
        lock (_syncLock)
        {
            if (_initialized)
                return;

            InitializeAssets();
            _initialized = true;
        }

        _ = BroadcastSnapshotAsync();
    }

    private void InitializeAssets()
    {
        Directory.CreateDirectory(_modelsRoot);
        Directory.CreateDirectory(_downloadsRoot);
        foreach (var model in _options.Models ?? new List<ModelOptions>())
        {
            var assets = new List<ModelAssetStatus>();

            var modelPath = ResolvePath(_modelsRoot, model.ModelPath);
            model.ModelPath = modelPath;
            assets.Add(CreateAssetStatus(model.Name, ModelAssetKind.Model, modelPath, model.ModelDownloadUrl));

            if (!string.IsNullOrWhiteSpace(model.MmprojPath))
            {
                var mmprojPath = ResolvePath(_modelsRoot, model.MmprojPath);
                model.MmprojPath = mmprojPath;
                assets.Add(CreateAssetStatus(model.Name, ModelAssetKind.Mmproj, mmprojPath, model.MmprojDownloadUrl));
            }

            _assetsByModel[model.Name] = assets;
        }
    }

    private ModelAssetStatus CreateAssetStatus(string modelName, ModelAssetKind kind, string localPath, string downloadUrl)
    {
        var status = new ModelAssetStatus
        {
            ModelName = modelName,
            AssetKind = kind,
            LocalPath = localPath,
            DownloadUrl = downloadUrl,
            FileName = Path.GetFileName(localPath),
            State = ModelDownloadState.Queued
        };

        if (File.Exists(localPath))
        {
            var info = new FileInfo(localPath);
            status.TotalBytes = info.Length;
            status.BytesReceived = info.Length;
            status.State = ModelDownloadState.Completed;
        }
        else if (string.IsNullOrWhiteSpace(downloadUrl))
        {
            status.State = ModelDownloadState.MissingUrl;
            status.Error = "Download URL is missing.";
        }

        return status;
    }

    private async Task DownloadAllAsync(CancellationToken cancellationToken)
    {
        foreach (var assets in _assetsByModel.Values)
        {
            foreach (var asset in assets)
            {
                if (asset.State == ModelDownloadState.Completed)
                    continue;

                if (asset.State == ModelDownloadState.MissingUrl)
                {
                    await BroadcastProgressAsync(asset);
                    continue;
                }

                await DownloadAssetAsync(asset, cancellationToken);
            }
        }
    }

    private async Task DownloadAssetAsync(ModelAssetStatus asset, CancellationToken cancellationToken)
    {
        asset.State = ModelDownloadState.Downloading;
        asset.BytesReceived = 0;
        asset.Error = null;

        await BroadcastProgressAsync(asset);

        var directory = Path.GetDirectoryName(asset.LocalPath);
        if (string.IsNullOrWhiteSpace(directory))
            directory = _environment.ContentRootPath;
        Directory.CreateDirectory(directory);

        var tempFileName = $"{asset.ModelName}-{asset.AssetKind}-{asset.FileName}.download";
        var tempPath = Path.Combine(_downloadsRoot, tempFileName);
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, asset.DownloadUrl);
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            asset.TotalBytes = response.Content.Headers.ContentLength;

            var lastReport = DateTimeOffset.UtcNow;

            await using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 64, useAsync: true);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var buffer = new byte[1024 * 64];
            int read;
            while ((read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                asset.BytesReceived += read;

                if (DateTimeOffset.UtcNow - lastReport > TimeSpan.FromMilliseconds(250))
                {
                    lastReport = DateTimeOffset.UtcNow;
                    await BroadcastProgressAsync(asset);
                }
            }

            if (File.Exists(asset.LocalPath))
                File.Delete(asset.LocalPath);

            File.Move(tempPath, asset.LocalPath);

            asset.State = ModelDownloadState.Completed;
            asset.BytesReceived = asset.TotalBytes ?? asset.BytesReceived;
            asset.Error = null;
        }
        catch (Exception ex)
        {
            asset.State = ModelDownloadState.Failed;
            asset.Error = ex.Message;
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
        finally
        {
            await BroadcastProgressAsync(asset);
        }
    }

    private static string ResolvePath(string contentRoot, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        return Path.IsPathRooted(path)
            ? path
            : Path.GetFullPath(Path.Combine(contentRoot, path));
    }

    private Task BroadcastProgressAsync(ModelAssetStatus asset)
    {
        var progress = new ModelDownloadProgress
        {
            ModelName = asset.ModelName,
            FileName = asset.FileName,
            AssetKind = asset.AssetKind,
            State = asset.State,
            TotalBytes = asset.TotalBytes,
            BytesReceived = asset.BytesReceived,
            Error = asset.Error
        };

        return _hubContext.Clients.All.OnModelDownloadProgress(progress);
    }

    private Task BroadcastSnapshotAsync()
    {
        return _hubContext.Clients.All.OnModelDownloadSnapshot(GetSnapshots());
    }

    private static ModelAssetSnapshot ToSnapshot(ModelAssetStatus asset)
    {
        return new ModelAssetSnapshot
        {
            ModelName = asset.ModelName,
            FileName = asset.FileName,
            AssetKind = asset.AssetKind,
            State = asset.State,
            TotalBytes = asset.TotalBytes,
            BytesReceived = asset.BytesReceived,
            Error = asset.Error
        };
    }

    private sealed class ModelAssetStatus
    {
        public string ModelName { get; set; }
        public string FileName { get; set; }
        public string LocalPath { get; set; }
        public string DownloadUrl { get; set; }
        public ModelAssetKind AssetKind { get; set; }
        public ModelDownloadState State { get; set; }
        public long? TotalBytes { get; set; }
        public long BytesReceived { get; set; }
        public string Error { get; set; }
    }
}
