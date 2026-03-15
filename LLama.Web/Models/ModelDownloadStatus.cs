namespace LLama.Web.Models;

public enum ModelAssetKind
{
    Model = 0,
    Mmproj = 1
}

public enum ModelDownloadState
{
    Queued = 0,
    Downloading = 1,
    Completed = 2,
    Failed = 3,
    MissingUrl = 4
}

public class ModelAssetSnapshot
{
    public string ModelName { get; set; }
    public string FileName { get; set; }
    public ModelAssetKind AssetKind { get; set; }
    public ModelDownloadState State { get; set; }
    public long? TotalBytes { get; set; }
    public long BytesReceived { get; set; }
    public string Error { get; set; }
}

public class ModelDownloadSnapshot
{
    public string ModelName { get; set; }
    public bool Ready { get; set; }
    public List<ModelAssetSnapshot> Assets { get; set; } = new();
}

public class ModelDownloadProgress
{
    public string ModelName { get; set; }
    public string FileName { get; set; }
    public ModelAssetKind AssetKind { get; set; }
    public ModelDownloadState State { get; set; }
    public long? TotalBytes { get; set; }
    public long BytesReceived { get; set; }
    public string Error { get; set; }
}
