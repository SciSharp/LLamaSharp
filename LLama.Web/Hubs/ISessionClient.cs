using LLama.Web.Common;
using LLama.Web.Models;

namespace LLama.Web.Hubs
{
    public interface ISessionClient
    {
        Task OnStatus(string connectionId, SessionConnectionStatus status);
        Task OnError(string error);
        Task OnModelDownloadSnapshot(IReadOnlyList<ModelDownloadSnapshot> snapshots);
        Task OnModelDownloadProgress(ModelDownloadProgress progress);
        Task OnStorageInfo(StorageInfo info);
    }
}
