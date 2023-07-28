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

        public async Task<IServiceResult<ModelSession>> CreateAsync(LLamaExecutorType executorType, string connectionId, string modelName, string promptName, string parameterName)
        {
            // Remove existing connections session
            await RemoveAsync(connectionId);


            var modelOption = _options.Models.FirstOrDefault(x => x.Name == modelName);
            if (modelOption is null)
                return ServiceResult.FromError<ModelSession>($"Model option '{modelName}' not found");

            var promptOption = _options.Prompts.FirstOrDefault(x => x.Name == promptName);
            if (promptOption is null)
                return ServiceResult.FromError<ModelSession>($"Prompt option '{promptName}' not found");

            var parameterOption = _options.Parameters.FirstOrDefault(x => x.Name == parameterName);
            if (parameterOption is null)
                return ServiceResult.FromError<ModelSession>($"Parameter option '{parameterName}' not found");


            //Max instance
            var currentInstances = _modelSessions.Count(x => x.Value.ModelName == modelOption.Name);
            if (modelOption.MaxInstances > -1 && currentInstances >= modelOption.MaxInstances)
                return ServiceResult.FromError<ModelSession>("Maximum model instances reached");

            // Create model
            var llamaModel = new LLamaModel(modelOption);

            //Create context
            var llamaModelContext = new LLamaModelContext(llamaModel);

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
            if (!_modelSessions.TryAdd(connectionId, modelSession))
                return ServiceResult.FromError<ModelSession>("Failed to create model session");

            return ServiceResult.FromValue(modelSession);
        }

        public async IAsyncEnumerable<ResponseFragment> InferAsync(string connectionId, string prompt, CancellationTokenSource cancellationTokenSource)
        {
            var modelSession = await GetAsync(connectionId);
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
