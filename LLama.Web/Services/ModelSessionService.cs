using LLama.Web.Async;
using LLama.Web.Common;
using LLama.Web.Models;
using LLama.Native;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace LLama.Web.Services;

/// <summary>
/// Example service for handling a model session for the lifetime of a WebSocket connection.
/// Each WebSocket connection creates its own session and context, allowing you to use multiple tabs to compare prompts and results.
/// </summary>
public class ModelSessionService : IModelSessionService
{
    private readonly AsyncGuard<string> _sessionGuard;
    private readonly IModelService _modelService;
    private readonly IAttachmentService _attachmentService;
    private readonly ILogger<ModelSessionService> _logger;
    private readonly ConcurrentDictionary<string, ModelSession> _modelSessions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelSessionService"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    /// <param name="attachmentService">The attachment service.</param>
    public ModelSessionService(IModelService modelService, IAttachmentService attachmentService, ILogger<ModelSessionService> logger)
    {
        _modelService = modelService;
        _attachmentService = attachmentService;
        _logger = logger;
        _sessionGuard = new AsyncGuard<string>();
        _modelSessions = new ConcurrentDictionary<string, ModelSession>();
    }

    /// <summary>
    /// Gets the ModelSession with the specified ID.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <returns>The ModelSession if it exists; otherwise, null.</returns>
    public Task<ModelSession> GetAsync(string sessionId)
    {
        return Task.FromResult(_modelSessions.TryGetValue(sessionId, out var session) ? session : null);
    }

    /// <summary>
    /// Gets all model sessions.
    /// </summary>
    /// <returns>A collection of all model sessions.</returns>
    public Task<IEnumerable<ModelSession>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<ModelSession>>(_modelSessions.Values);
    }

    /// <summary>
    /// Creates a new model session.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="sessionConfig">The session configuration.</param>
    /// <param name="inferenceConfig">The default inference configuration, used when no inference configuration is supplied.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">
    /// Session with ID {sessionId} already exists
    /// or
    /// Failed to create model session
    /// </exception>
    public async Task<ModelSession> CreateAsync(string sessionId, ISessionConfig sessionConfig, InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default)
    {
        if (_modelSessions.TryGetValue(sessionId, out _))
            throw new Exception($"Session with id {sessionId} already exists");

        // Create context.
        var (model, context) = await _modelService.GetOrCreateModelAndContext(sessionConfig.Model, sessionId);

        // Create session.
        var modelSession = new ModelSession(model, context, sessionId, sessionConfig, inferenceConfig, _logger);
        if (!_modelSessions.TryAdd(sessionId, modelSession))
            throw new Exception($"Failed to create model session");

        // Run initial prompt.
        await modelSession.InitializePrompt(inferenceConfig, cancellationToken);
        return modelSession;

    }

    /// <summary>
    /// Closes the session.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <returns></returns>
    public async Task<bool> CloseAsync(string sessionId)
    {
        if (_modelSessions.TryRemove(sessionId, out var modelSession))
        {
            modelSession.CancelInfer();
            try
            {
                return await _modelService.RemoveContext(modelSession.ModelName, sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove context '{ContextName}' from model '{ModelName}' during session close.", sessionId, modelSession.ModelName);
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Runs inference on the current model session.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="request">The prompt request.</param>
    /// <param name="inferenceConfig">The inference configuration; if null, the session default is used.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="System.Exception">Inference is already running for this session</exception>
    public async IAsyncEnumerable<TokenModel> InferAsync(string sessionId, PromptRequest request, InferenceOptions inferenceConfig = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!_sessionGuard.Guard(sessionId))
            throw new Exception($"Inference is already running for this session");

        try
        {
            if (!_modelSessions.TryGetValue(sessionId, out var modelSession))
                yield break;

            var preparedPrompt = await PreparePromptAsync(sessionId, modelSession, request, cancellationToken);
            var formattedPrompt = modelSession.BuildPrompt(preparedPrompt.Prompt);

            // Send start of response.
            var stopwatch = Stopwatch.GetTimestamp();
            yield return new TokenModel(default, default, TokenType.Begin);

            var responseBuilder = new StringBuilder();

            // Send response content.
            await foreach (var token in modelSession.InferAsync(formattedPrompt, inferenceConfig, cancellationToken).ConfigureAwait(false))
            {
                responseBuilder.Append(token);
                yield return new TokenModel(default, token);
            }

            if (!modelSession.IsInferCanceled() && modelSession.StoresHistory)
                modelSession.AddAssistantMessage(responseBuilder.ToString());

            // Send end of response.
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
    /// Runs inference on the current model session.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="inferenceConfig">The inference configuration; if null, the session default is used.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Streaming async result of <see cref="System.String" /></returns>
    /// <exception cref="System.Exception">Inference is already running for this session</exception>
    public IAsyncEnumerable<string> InferTextAsync(string sessionId, string prompt, InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default)
    {
        async IAsyncEnumerable<string> InferTextInternal()
        {
            await foreach (var token in InferAsync(sessionId, new PromptRequest { Prompt = prompt }, inferenceConfig, cancellationToken).ConfigureAwait(false))
            {
                if (token.TokenType ==  TokenType.Content)
                    yield return token.Content;
            }
        }
        return InferTextInternal();
    }

    /// <summary>
    /// Runs inference on the current model session.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="prompt">The prompt.</param>
    /// <param name="inferenceConfig">The inference configuration; if null, the session default is used.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Completed inference result as string</returns>
    /// <exception cref="System.Exception">Inference is already running for this session</exception>
    public async Task<string> InferTextCompleteAsync(string sessionId, string prompt, InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default)
    {
        var inferResult = await InferAsync(sessionId, new PromptRequest { Prompt = prompt }, inferenceConfig, cancellationToken)
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
    /// Gets the model capabilities for the specified session.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <returns>The model capabilities.</returns>
    public Task<ModelCapabilities> GetCapabilitiesAsync(string sessionId)
    {
        if (_modelSessions.TryGetValue(sessionId, out var modelSession))
        {
            return Task.FromResult(new ModelCapabilities
            {
                SupportsText = true,
                SupportsVision = modelSession.SupportsVision,
                SupportsAudio = modelSession.SupportsAudio
            });
        }

        return Task.FromResult(new ModelCapabilities
        {
            SupportsText = true,
            SupportsVision = false,
            SupportsAudio = false
        });
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

    private async Task<PreparedPrompt> PreparePromptAsync(string sessionId, ModelSession modelSession, PromptRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            return new PreparedPrompt(string.Empty);

        var promptBuilder = new StringBuilder(request.Prompt ?? string.Empty);
        var mediaPrefixBuilder = new StringBuilder();
        var embeds = new List<SafeMtmdEmbed>();

        try
        {
            if (request.AttachmentIds != null && request.AttachmentIds.Count > 0)
            {
                var mediaMarker = GetMediaMarker();
                var attachments = _attachmentService.GetAttachments(sessionId, request.AttachmentIds);

                _logger.LogInformation(
                    "Preparing prompt for session {SessionId} with {RequestedCount} attachment id(s); resolved {ResolvedCount}: [{ResolvedAttachments}]",
                    sessionId,
                    request.AttachmentIds.Count,
                    attachments.Count,
                    string.Join(", ", attachments.Select(a => $"{a.Id}:{a.Kind}:{a.ContentType}:{a.SizeBytes}:{a.FileName}")));

                foreach (var attachment in attachments)
                {
                    _logger.LogInformation(
                        "Processing attachment {AttachmentId} for session {SessionId}: kind={Kind}, contentType={ContentType}, size={SizeBytes}, file={FileName}",
                        attachment.Id,
                        sessionId,
                        attachment.Kind,
                        attachment.ContentType,
                        attachment.SizeBytes,
                        attachment.FileName);

                    switch (attachment.Kind)
                    {
                        case AttachmentKind.Pdf:
                            AppendPdf(promptBuilder, attachment);
                            break;
                        case AttachmentKind.Word:
                            AppendWord(promptBuilder, attachment);
                            break;
                        case AttachmentKind.Image:
                            if (!modelSession.IsMultiModal)
                                throw new Exception("This model does not support multimodal inputs.");

                            embeds.Add(await LoadEmbedAsync(sessionId, modelSession, attachment, cancellationToken));
                            AppendMedia(modelSession, promptBuilder, mediaPrefixBuilder, mediaMarker);
                            break;
                        case AttachmentKind.Audio:
                            if (!modelSession.IsMultiModal || !modelSession.SupportsAudio)
                                throw new Exception("This model does not support audio inputs.");

                            embeds.Add(await LoadEmbedAsync(sessionId, modelSession, attachment, cancellationToken));
                            AppendMedia(modelSession, promptBuilder, mediaPrefixBuilder, mediaMarker);
                            break;
                    }
                }
            }

            if (embeds.Count > 0)
                modelSession.QueueEmbeds(embeds);

            if (mediaPrefixBuilder.Length > 0)
                promptBuilder.Insert(0, mediaPrefixBuilder.ToString());

            return new PreparedPrompt(promptBuilder.ToString());
        }
        catch
        {
            foreach (var embed in embeds)
                embed.Dispose();

            throw;
        }
    }

    private static void AppendPdf(StringBuilder builder, AttachmentInfo attachment)
    {
        if (string.IsNullOrWhiteSpace(attachment.ExtractedText))
            return;

        builder.AppendLine();
        builder.AppendLine($"[PDF: {attachment.FileName}]");
        builder.AppendLine(attachment.ExtractedText.Trim());
        if (attachment.ExtractedTextTruncated)
            builder.AppendLine("[PDF text truncated]");
    }

    private static void AppendMedia(ModelSession modelSession, StringBuilder builder, StringBuilder mediaPrefixBuilder, string mediaMarker)
    {
        if (ShouldPrefixMediaMarker(modelSession))
        {
            mediaPrefixBuilder.Append(mediaMarker);
            return;
        }

        if (builder.Length > 0 && !char.IsWhiteSpace(builder[^1]))
            builder.Append(' ');

        builder.Append(mediaMarker);
    }

    private static bool ShouldPrefixMediaMarker(ModelSession modelSession)
    {
        return modelSession.ModelName.Contains("Qwen2.5-Omni", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetMediaMarker()
    {
        var mtmdParameters = MtmdContextParams.Default();
        return mtmdParameters.MediaMarker ?? NativeApi.MtmdDefaultMarker() ?? "<media>";
    }

    private static void AppendWord(StringBuilder builder, AttachmentInfo attachment)
    {
        builder.AppendLine();
        builder.AppendLine($"[Word: {attachment.FileName}]");

        if (string.IsNullOrWhiteSpace(attachment.ExtractedText))
        {
            builder.AppendLine("[Word text could not be extracted]");
            return;
        }

        builder.AppendLine(attachment.ExtractedText.Trim());
        if (attachment.ExtractedTextTruncated)
            builder.AppendLine("[Word text truncated]");
    }

    private async Task<SafeMtmdEmbed> LoadEmbedAsync(string sessionId, ModelSession modelSession, AttachmentInfo attachment, CancellationToken cancellationToken)
    {
        if (!File.Exists(attachment.FilePath))
            throw new FileNotFoundException("Attachment not found.", attachment.FilePath);

        var clipModel = modelSession.ClipModel;
        if (clipModel is null)
            throw new Exception("Multimodal model is not available for this session.");

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            SafeMtmdEmbed? embed;
            if (attachment.Kind == AttachmentKind.Audio)
            {
                var data = await File.ReadAllBytesAsync(attachment.FilePath, cancellationToken);
                _logger.LogInformation(
                    "Loading audio attachment {AttachmentId} for session {SessionId} into MTMD from buffer: file={FileName}, contentType={ContentType}, bytes={ByteCount}, model={ModelName}",
                    attachment.Id,
                    sessionId,
                    attachment.FileName,
                    attachment.ContentType,
                    data.Length,
                    modelSession.ModelName);
                embed = clipModel.LoadMediaStandalone(data);
            }
            else
            {
                _logger.LogInformation(
                    "Loading media attachment {AttachmentId} for session {SessionId} into MTMD from file: file={FileName}, contentType={ContentType}, path={FilePath}, model={ModelName}",
                    attachment.Id,
                    sessionId,
                    attachment.FileName,
                    attachment.ContentType,
                    attachment.FilePath,
                    modelSession.ModelName);
                embed = clipModel.LoadMediaStandalone(attachment.FilePath);
            }

            if (embed == null)
            {
                _logger.LogWarning(
                    "MTMD returned null embed for attachment {AttachmentId} in session {SessionId}: kind={Kind}, contentType={ContentType}, file={FileName}",
                    attachment.Id,
                    sessionId,
                    attachment.Kind,
                    attachment.ContentType,
                    attachment.FileName);
                throw new Exception("Failed to prepare multimodal embedding.");
            }

            _logger.LogInformation(
                "MTMD embed prepared for attachment {AttachmentId} in session {SessionId}: isAudio={IsAudio}, nx={Nx}, ny={Ny}, byteCount={ByteCount}",
                attachment.Id,
                sessionId,
                embed.IsAudio,
                embed.Nx,
                embed.Ny,
                embed.ByteCount);

            return embed;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to load MTMD embed for attachment {AttachmentId} in session {SessionId}: kind={Kind}, contentType={ContentType}, file={FileName}, path={FilePath}",
                attachment.Id,
                sessionId,
                attachment.Kind,
                attachment.ContentType,
                attachment.FileName,
                attachment.FilePath);
            throw;
        }
    }

    private sealed record PreparedPrompt(string Prompt);
}
