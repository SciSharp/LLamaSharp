using LLama.Abstractions;
using LLama.Web.Common;
using LLama.Web.Models;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;

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
        private readonly IModelCacheService _modelCacheService;
        private readonly ConcurrentDictionary<string, ModelSession> _modelSessions;


        public ConnectionSessionService(ILogger<ConnectionSessionService> logger, IOptions<LLamaOptions> options, IModelCacheService modelCacheService)
        {
            _logger = logger;
            _options = options.Value;
            _modelCacheService = modelCacheService;
            _modelSessions = new ConcurrentDictionary<string, ModelSession>();
        }

        public Task<ModelSession> GetAsync(string sessionId)
        {
            _modelSessions.TryGetValue(sessionId, out var modelSession);
            return Task.FromResult(modelSession);
        }

        public async Task<IServiceResult<ModelSession>> CreateAsync(string sessionId, LLamaExecutorType executorType, string modelName, string promptName, string parameterName)
        {
            var modelOption = _options.Models.FirstOrDefault(x => x.Name == modelName);
            if (modelOption is null)
                return ServiceResult.FromError<ModelSession>($"Model option '{modelName}' not found");

            var promptOption = _options.Prompts.FirstOrDefault(x => x.Name == promptName);
            if (promptOption is null)
                return ServiceResult.FromError<ModelSession>($"Prompt option '{promptName}' not found");

            var parameterOption = _options.Parameters.FirstOrDefault(x => x.Name == parameterName);
            if (parameterOption is null)
                return ServiceResult.FromError<ModelSession>($"Parameter option '{parameterName}' not found");

            return await CreateAsync(sessionId, executorType, modelOption, promptOption, parameterOption);
        }


        public async Task<IServiceResult<ModelSession>> CreateAsync(string sessionId, LLamaExecutorType executorType, ModelOptions modelOption, PromptOptions promptOption, ParameterOptions parameterOption)
        {
            // Remove existing connections session
            await RemoveAsync(sessionId);
         
            //Max instance
            var currentInstances = _modelSessions.Count(x => x.Value.ModelName == modelOption.Name);
            if (modelOption.MaxInstances > -1 && currentInstances >= modelOption.MaxInstances)
                return ServiceResult.FromError<ModelSession>("Maximum model instances reached");

            // Create Model/Context
            var llamaModelContext = await CreateModelContext(sessionId, modelOption);

            // Create executor
            ILLamaExecutor executor = executorType switch
            {
                LLamaExecutorType.Interactive => new InteractiveExecutor(llamaModelContext),
                LLamaExecutorType.Instruct => new InstructExecutor(llamaModelContext),
                LLamaExecutorType.Stateless => new StatelessExecutor(llamaModelContext),
                _ => default
            };

            // Create session
            var modelSession = new ModelSession(executor, modelOption, promptOption, parameterOption);
            if (!_modelSessions.TryAdd(sessionId, modelSession))
                return ServiceResult.FromError<ModelSession>("Failed to create model session");

            return ServiceResult.FromValue(modelSession);
        }

        public async IAsyncEnumerable<ResponseFragment> InferAsync(string sessionId, string prompt, CancellationTokenSource cancellationTokenSource)
        {
            var modelSession = await GetAsync(sessionId);
            if (modelSession is null)
                yield break;

            // Create unique response id
            var responseId = Guid.NewGuid().ToString();

            // Send begin of response
            var stopwatch = Stopwatch.GetTimestamp();
            yield return new ResponseFragment
            {
                Id = responseId,
                IsFirst = true
            };

            // Send content of response
            await foreach (var fragment in modelSession.InferAsync(prompt, cancellationTokenSource))
            {
                yield return new ResponseFragment
                {
                    Id = responseId,
                    Content = fragment
                };
            }

            // Send end of response
            var elapsedTime = Stopwatch.GetElapsedTime(stopwatch);
            var signature = modelSession.IsInferCanceled()
                  ? $"Inference cancelled after {elapsedTime.TotalSeconds:F0} seconds"
                  : $"Inference completed in {elapsedTime.TotalSeconds:F0} seconds";
            yield return new ResponseFragment
            {
                Id = responseId,
                IsLast = true,
                Content = signature,
                IsCancelled = modelSession.IsInferCanceled(),
                Elapsed = (int)elapsedTime.TotalMilliseconds
            };
        }


        public Task<bool> RemoveAsync(string sessionId)
        {
            if (_modelSessions.TryRemove(sessionId, out var modelSession))
            {
                modelSession.CancelInfer();
                modelSession.Dispose();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> CancelAsync(string sessionId)
        {
            if (_modelSessions.TryGetValue(sessionId, out var modelSession))
            {
                modelSession.CancelInfer();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private async Task<LLamaModelContext> CreateModelContext(string sessionId, ModelOptions modelOption)
        {
            // Create model
            var llamaModel = await _modelCacheService.Get(modelOption.Name)
                          ?? await _modelCacheService.Create(modelOption);
            if (llamaModel is null)
                throw new Exception($"Failed to create model, modelName: {modelOption.Name}");

            //Create context
            var llamaModelContext = await llamaModel.GetContext(sessionId)
                                 ?? await llamaModel.CreateContext(sessionId);
            if (llamaModelContext is null)
                throw new Exception($"Failed to create model, connectionId: {sessionId}");

            return llamaModelContext;
        }
    }
}
