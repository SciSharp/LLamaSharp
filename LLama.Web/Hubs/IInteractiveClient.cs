using LLama.Web.Models;

namespace LLama.Web.Hubs
{
    public interface IInteractiveClient
    {
        Task OnStatus(string status);
        Task OnResponse(ResponseFragment fragment);
        Task OnError(string error);
    }
}
