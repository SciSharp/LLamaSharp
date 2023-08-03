using LLama.Abstractions;
using LLama.Web.Common;
using LLama.Web.Models;

namespace LLama.Web.Services
{
    public interface IModelSessionService
    {
        Task<ModelSession> GetAsync(string sessionId);
        Task<IServiceResult<ModelSession>> CreateAsync(string sessionId, LLamaExecutorType executorType, string modelName, string promptName, string parameterName);
        Task<IServiceResult<ModelSession>> CreateAsync(string sessionId, LLamaExecutorType executorType, ModelOptions modelOption, PromptOptions promptOption, ParameterOptions parameterOption);
        Task<bool> RemoveAsync(string sessionId);
        Task<bool> CancelAsync(string sessionId);
    }


}
