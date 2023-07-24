using LLama.Abstractions;
using LLama.Web.Common;
using LLama.Web.Models;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Drawing;

namespace LLama.Web.Services
{
    /// <summary>
    /// Example Service for handling a model session for a websockets connection lifetime
    /// Each websocket connection will create its own unique session and context allowing you to use multiple tabs to compare prompts etc
    /// </summary>
    public class ConnectionSessionService : IModelSessionService
    {
        private readonly LLamaOptions _options;
        private readonly ILogger<ConnectionSessionService> _logger;
        private readonly ConcurrentDictionary<string, ModelSession> _modelSessions;

        public ConnectionSessionService(ILogger<ConnectionSessionService> logger, IOptions<LLamaOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _modelSessions = new ConcurrentDictionary<string, ModelSession>();
        }

        public Task<ModelSession> GetAsync(string connectionId)
        {
            _modelSessions.TryGetValue(connectionId, out var modelSession);
            return Task.FromResult(modelSession);
        }

        public Task<IServiceResult<ModelSession>> CreateAsync(LLamaExecutorType executorType, string connectionId, string modelName, string promptName, string parameterName)
        {
            var modelOption = _options.Models.FirstOrDefault(x => x.Name == modelName);
            if (modelOption is null)
                return Task.FromResult(ServiceResult.FromError<ModelSession>($"Model option '{modelName}' not found"));

            var promptOption = _options.Prompts.FirstOrDefault(x => x.Name == promptName);
            if (promptOption is null)
                return Task.FromResult(ServiceResult.FromError<ModelSession>($"Prompt option '{promptName}' not found"));

            var parameterOption = _options.Parameters.FirstOrDefault(x => x.Name == parameterName);
            if (parameterOption is null)
                return Task.FromResult(ServiceResult.FromError<ModelSession>($"Parameter option '{parameterName}' not found"));


            //Max instance
            var currentInstances = _modelSessions.Count(x => x.Value.ModelName == modelOption.Name);
            if (modelOption.MaxInstances > -1 && currentInstances >= modelOption.MaxInstances)
                return Task.FromResult(ServiceResult.FromError<ModelSession>("Maximum model instances reached"));

            // Create model
            var llamaModel = new LLamaModel(modelOption);

            // Create executor
            ILLamaExecutor executor = executorType switch
            {
                LLamaExecutorType.Interactive => new InteractiveExecutor(llamaModel),
                LLamaExecutorType.Instruct => new InstructExecutor(llamaModel),
                LLamaExecutorType.Stateless => new StatelessExecutor(llamaModel),
                _ => default
            };

            // Create session
            var modelSession = new ModelSession(executor, modelOption, promptOption, parameterOption);
            if (!_modelSessions.TryAdd(connectionId, modelSession))
                return Task.FromResult(ServiceResult.FromError<ModelSession>("Failed to create model session"));

            return Task.FromResult(ServiceResult.FromValue(modelSession));
        }

        public Task<bool> RemoveAsync(string connectionId)
        {
            if (_modelSessions.TryRemove(connectionId, out var modelSession))
            {
                modelSession.CancelInfer();
                modelSession.Dispose();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> CancelAsync(string connectionId)
        {
            if (_modelSessions.TryGetValue(connectionId, out var modelSession))
            {
                modelSession.CancelInfer();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
