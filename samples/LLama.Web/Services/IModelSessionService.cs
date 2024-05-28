using LLama.Web.Common;
using LLama.Web.Models;

namespace LLama.Web.Services;

public interface IModelSessionService
{
    /// <summary>
    /// Gets the ModelSession with the specified Id.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <returns>The ModelSession if exists, otherwise null</returns>
    Task<ModelSession> GetAsync(string sessionId);

    /// <summary>
    /// Gets all ModelSessions
    /// </summary>
    /// <returns>A collection oa all Model instances</returns>
    Task<IEnumerable<ModelSession>> GetAllAsync();

    /// <summary>
    /// Creates a new ModelSession
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="sessionConfig">The session configuration.</param>
    /// <param name="inferenceOptions">The default inference configuration, will be used for all inference where no infer configuration is supplied.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">
    /// Session with id {sessionId} already exists
    /// or
    /// Failed to create model session
    /// </exception>
    Task<ModelSession> CreateAsync(string sessionId, ISessionConfig sessionConfig, InferenceOptions inferenceOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the session
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <returns></returns>
    Task<bool> CloseAsync(string sessionId);

    /// <summary>
    /// Runs inference on the current ModelSession
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="inferenceConfig">The inference configuration, if null session default is used</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="System.Exception">Inference is already running for this session</exception>
    IAsyncEnumerable<TokenModel> InferAsync(string sessionId, string prompt, InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs inference on the current ModelSession
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="inferenceOptions">The inference configuration, if null session default is used</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Streaming async result of <see cref="System.String" /></returns>
    /// <exception cref="System.Exception">Inference is already running for this session</exception>
    IAsyncEnumerable<string> InferTextAsync(string sessionId, string prompt, InferenceOptions inferenceOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queues inference on the current ModelSession
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="inferenceOptions">The inference configuration, if null session default is used</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Completed inference result as string</returns>
    /// <exception cref="System.Exception">Inference is already running for this session</exception>
    Task<string> InferTextCompleteAsync(string sessionId, string prompt, InferenceOptions inferenceOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels the current inference action.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <returns></returns>
    Task<bool> CancelAsync(string sessionId);
}
