using LLama.Abstractions;
using LLama.Web.Common;
using LLama.Web.Models;

namespace LLama.Web.Services
{
    public interface IModelSessionService
    {
        Task<bool> CloseAsync(string sessionId);
        Task<bool> CancelAsync(string sessionId);
        Task<ModelSession> CreateAsync(string sessionId, SessionConfig sessionConfig, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default);
        IAsyncEnumerable<InferTokenModel> InferAsync(string sessionId, string prompt, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default);
    }
}
