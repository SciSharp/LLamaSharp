using LLama.Web.Models;

namespace LLama.Web.Hubs
{
    public interface ISessionClient
    {
        Task OnStatus(string status, string data = null);
        Task OnResponse(ResponseFragment fragment);
        Task OnError(string error);
    }
}
