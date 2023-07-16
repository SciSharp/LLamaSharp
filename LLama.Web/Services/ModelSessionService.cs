using LLama.Web.Models;
using System.Collections.Concurrent;

namespace LLama.Web.Services
{
    public class ModelSessionService : IModelSessionService
    {
        private readonly ILogger<ModelSessionService> _logger;
        private readonly ConcurrentDictionary<string, ModelSession> _modelSessions;

        public ModelSessionService(ILogger<ModelSessionService> logger)
        {
            _logger = logger;
            _modelSessions = new ConcurrentDictionary<string, ModelSession>();
        }

        public Task<ModelSession>  GetAsync(string connectionId)
        {
            _modelSessions.TryGetValue(connectionId, out var modelSession);
            return Task.FromResult(modelSession);
        }

        public Task<ModelSession> CreateAsync(string connectionId, ModelOptions modelOption, PromptOptions promptOption, ParameterOptions parameterOption)
        {
            //TODO: Max instance etc
            var modelSession = new ModelSession(connectionId, modelOption, promptOption, parameterOption);
            if (!_modelSessions.TryAdd(connectionId, modelSession))
            {
                _logger.Log(LogLevel.Error, "[CreateAsync] - Failed to create model session, Connection: {0}", connectionId);
                return Task.FromResult<ModelSession>(default);
            }
            return Task.FromResult(modelSession);
        }

        public Task RemoveAsync(string connectionId)
        {
            if (_modelSessions.TryRemove(connectionId, out var modelSession))
            {
                _logger.Log(LogLevel.Information, "[RemoveAsync] - Removed model session, Connection: {0}", connectionId);
                modelSession.Dispose();
            }
            return Task.CompletedTask;
        }

        public Task CancelAsync(string connectionId)
        {
            if (_modelSessions.TryGetValue(connectionId, out var modelSession))
            {
                _logger.Log(LogLevel.Information, "[CancelAsync] - Canceled model session, Connection: {0}", connectionId);
                modelSession.CancelInfer();
            }
            return Task.CompletedTask;
        }

    }
}
