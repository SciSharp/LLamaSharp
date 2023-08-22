using LLama.Abstractions;
using LLama.Web.Common;
using LLama.Web.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LLama.Web.Services
{
    /// <summary>
    /// Example Service for handling a model session for a websockets connection lifetime
    /// Each websocket connection will create its own unique session and context allowing you to use multiple tabs to compare prompts etc
    public class ModelSessionService : IModelSessionService
    {
        private readonly IModelService _modelService;
        private readonly ConcurrentDictionary<string, ModelSession> _modelSessions;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelSessionService"/> class.
        /// </summary>
        /// <param name="modelService">The model service.</param>
        /// <param name="modelSessionStateService">The model session state service.</param>
        public ModelSessionService(IModelService modelService)
        {
            _modelService = modelService;
            _modelSessions = new ConcurrentDictionary<string, ModelSession>();
        }


        /// <summary>
        /// Creates a new ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="sessionConfig">The session configuration.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Session with id {sessionId} already exists
        /// or
        /// Failed to create model session
        /// </exception>
        public async Task<ModelSession> CreateAsync(string sessionId, SessionConfig sessionConfig, IInferenceParams inferenceParams = null, CancellationToken cancellationToken = default)
        {
            if (_modelSessions.TryGetValue(sessionId, out var existingSession))
                return existingSession;

            var model = await _modelService.GetModel(sessionConfig.Model);
            if (model is null)
                throw new Exception($"Unable to locate model");

            // Create context
            var context = await _modelService.GetOrCreateModelAndContext(sessionConfig.Model, sessionId.ToString());

            // Create session
            var modelSession = new ModelSession(model, context, sessionConfig, inferenceParams);
            if (!_modelSessions.TryAdd(sessionId, modelSession))
                throw new Exception($"Failed to create model session");

            // Run initial Prompt
            await modelSession.InitializePrompt(inferenceParams, cancellationToken);
            return modelSession;

        }


        /// <summary>
        /// Runs inference on the current ModelSession
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="prompt">The prompt.</param>
        /// <param name="inferenceParams">The inference parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Inference is already running for this session</exception>
        public async IAsyncEnumerable<InferTokenModel> InferAsync(string sessionId, string prompt, IInferenceParams inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!_modelSessions.TryGetValue(sessionId, out var modelSession))
                yield break;


            // Send begin of response
            var stopwatch = Stopwatch.GetTimestamp();
            yield return new InferTokenModel(default, default, default, InferTokenType.Begin, GetElapsed(stopwatch));

            // Send content of response
            await foreach (var token in modelSession.InferAsync(prompt, inferenceParams, cancellationToken))
                yield return new InferTokenModel(default, default, token, InferTokenType.Content, GetElapsed(stopwatch));

            // Send end of response
            var elapsedTime = GetElapsed(stopwatch);
            var endTokenType = modelSession.IsInferCanceled() ? InferTokenType.Cancel : InferTokenType.End;
            var signature = endTokenType == InferTokenType.Cancel
                  ? $"Inference cancelled after {elapsedTime / 1000:F0} seconds"
                  : $"Inference completed in {elapsedTime / 1000:F0} seconds";
            yield return new InferTokenModel(default, default, signature, endTokenType, elapsedTime);
        }


        /// <summary>
        /// Closes the session
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        public async Task<bool> CloseAsync(string sessionId)
        {
            if (_modelSessions.TryRemove(sessionId, out var modelSession))
            {
                modelSession.CancelInfer();
                return await _modelService.RemoveContext(modelSession.ModelName, sessionId.ToString());
            }
            return false;
        }


        /// <summary>
        /// Cancels the current action.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        public Task<bool> CancelAsync(string sessionId)
        {
            if (_modelSessions.TryGetValue(sessionId, out var modelSession))
            {
                modelSession.CancelInfer();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Gets the elapsed time in milliseconds.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns></returns>
        private static int GetElapsed(long timestamp)
        {
            return (int)Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;
        }
    }
}
