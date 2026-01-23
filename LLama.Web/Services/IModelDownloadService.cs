using LLama.Web.Models;

namespace LLama.Web.Services;

public interface IModelDownloadService
{
    string ModelsRoot { get; }
    string DownloadsRoot { get; }
    void StartDownloads(CancellationToken cancellationToken);
    Task WaitForDownloadsAsync(CancellationToken cancellationToken);
    IReadOnlyList<ModelDownloadSnapshot> GetSnapshots();
    bool IsModelReady(string modelName);
}
