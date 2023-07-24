using LLama.Web.Common;
using LLama.Web.Models;

namespace LLama.Web.Hubs
{
    public interface ISessionClient
    {
        Task OnStatus(string connectionId, SessionConnectionStatus status);
        Task OnResponse(ResponseFragment fragment);
        Task OnError(string error);
    }
}
