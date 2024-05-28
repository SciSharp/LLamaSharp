using LLama.Web.Async;
using LLama.Web.Common;
using LLama.Web.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LLama.Web.Services;

/// <summary>
/// Example Service for handling a model session for a websockets connection lifetime
/// Each websocket connection will create its own unique session and context allowing you to use multiple tabs to compare prompts etc
/// </summary>
public class ModelSessionService : IModelSessionService
{
    private readonly AsyncGuard<string> _sessionGuard;
    private readonly IModelService _modelService;
    private readonly ConcurrentDictionary<string, ModelSession> _modelSessions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelSessionService{T}"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    /// <param name="modelSessionStateService">The model session state service.</param>
    public ModelSessionService(IModelService modelService)
    {
        _modelService = modelService;
        _sessionGuard = new AsyncGuard<string>();
        _modelSessions = new ConcurrentDictionary<string, ModelSession>();
    }

    /// <summary>
    /// Gets the ModelSession with the specified Id.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <returns>The ModelSession if exists, otherwise null</returns>
    public Task<ModelSession> GetAsync(string sessionId)
    {
        return Task.FromResult(_modelSessions.TryGetValue(sessionId, out var session) ? session : null);
    }

    /// <summary>
    /// Gets all ModelSessions
    /// </summary>
    /// <returns>A collection oa all Model instances</returns>
    public Task<IEnumerable<ModelSession>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<ModelSession>>(_modelSessions.Values);
    }

    /// <summary>
    /// Creates a new ModelSession
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="sessionConfig">The session configuration.</param>
    /// <param name="inferenceConfig">The default inference configuration, will be used for all inference where no infer configuration is supplied.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">
    /// Session with id {sessionId} already exists
    /// or
    /// Failed to create model session
    /// </exception>
    public async Task<ModelSession> CreateAsync(string sessionId, ISessionConfig sessionConfig, InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default)
    {
        if (_modelSessions.TryGetValue(sessionId, out _))
            throw new Exception($"Session with id {sessionId} already exists");

        // Create context
        var (model, context) = await _modelService.GetOrCreateModelAndContext(sessionConfig.Model, sessionId);

        // Create session
        var modelSession = new ModelSession(model, context, sessionId, sessionConfig, inferenceConfig);
        if (!_modelSessions.TryAdd(sessionId, modelSession))
            throw new Exception($"Failed to create model session");

        // Run initial Prompt
        await modelSession.InitializePrompt(inferenceConfig, cancellationToken);
        return modelSession;

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
            return await _modelService.RemoveContext(modelSession.ModelName, sessionId);
        }
        return false;
    }

    /// <summary>
    /// Runs inference on the current ModelSession
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="inferenceConfig">The inference configuration, if null session default is used</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="System.Exception">Inference is already running for this session</exception>
    public async IAsyncEnumerable<TokenModel> InferAsync(string sessionId, string prompt, InferenceOptions inferenceConfig = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!_sessionGuard.Guard(sessionId))
            throw new Exception($"Inference is already running for this session");

        try
        {
            if (!_modelSessions.TryGetValue(sessionId, out var modelSession))
                yield break;

            // Send begin of response
            var stopwatch = Stopwatch.GetTimestamp();
            yield return new TokenModel(default, default, TokenType.Begin);

            // Send content of response
            await foreach (var token in modelSession.InferAsync(prompt, inferenceConfig, cancellationToken).ConfigureAwait(false))
            {
                yield return new TokenModel(default, token);
            }

            // Send end of response
            var elapsedTime = GetElapsed(stopwatch);
            var endTokenType = modelSession.IsInferCanceled() ? TokenType.Cancel : TokenType.End;
            var signature = endTokenType == TokenType.Cancel
                  ? $"Cancelled after {elapsedTime / 1000:F0} seconds"
                  : $"Completed in {elapsedTime / 1000:F0} seconds";
            yield return new TokenModel(default, signature, endTokenType);
        }
        finally
        {
            _sessionGuard.Release(sessionId);
        }
    }

    /// <summary>
    /// Runs inference on the current ModelSession
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="inferenceConfig">The inference configuration, if null session default is used</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Streaming async result of <see cref="System.String" /></returns>
    /// <exception cref="System.Exception">Inference is already running for this session</exception>
    public IAsyncEnumerable<string> InferTextAsync(string sessionId, string prompt, InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default)
    {
        async IAsyncEnumerable<string> InferTextInternal()
        {
            await foreach (var token in InferAsync(sessionId, prompt, inferenceConfig, cancellationToken).ConfigureAwait(false))
            {
                if (token.TokenType ==  TokenType.Content)
                    yield return token.Content;
            }
        }
        return InferTextInternal();
    }

    /// <summary>
    /// Runs inference on the current ModelSession
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="inferenceConfig">The inference configuration, if null session default is used</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Completed inference result as string</returns>
    /// <exception cref="System.Exception">Inference is already running for this session</exception>
    public async Task<string> InferTextCompleteAsync(string sessionId, string prompt, InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default)
    {
        var inferResult = await InferAsync(sessionId, prompt, inferenceConfig, cancellationToken)
            .Where(x => x.TokenType == TokenType.Content)
            .Select(x => x.Content)
            .ToListAsync(cancellationToken: cancellationToken);

        return string.Concat(inferResult);
    }

    /// <summary>
    /// Cancels the current inference action.
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
