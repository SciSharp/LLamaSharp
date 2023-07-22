using LLama.Abstractions;
using LLama.Web.Models;

namespace LLama.Web.Services
{
    public interface IModelSessionService
    {
        Task<ModelSession> GetAsync(string connectionId);
        Task<ModelSession> CreateAsync(string connectionId, ILLamaExecutor executor, ModelOptions modelOption, PromptOptions promptOption, ParameterOptions parameterOption);
        Task RemoveAsync(string connectionId);
        Task CancelAsync(string connectionId);
    }


}
