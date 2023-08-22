using LLama.Web.Common;

namespace LLama.Web.Hubs
{
    public interface ISessionClient
    {
        Task OnStatus(string connectionId, SessionConnectionStatus status);
        Task OnError(string error);
    }
}
