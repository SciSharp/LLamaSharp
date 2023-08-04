using LLama.Abstractions;
using LLama.Web.Common;
using LLama.Web.Models;

namespace LLama.Web.Services
{
    public interface IModelSessionService
    {
        Task<ModelSession> GetAsync(string sessionId);
        Task<IServiceResult<ModelSession>> CreateAsync(string sessionId, CreateSessionModel sessionModel);
        Task<bool> RemoveAsync(string sessionId);
        Task<bool> CancelAsync(string sessionId);
    }


}
