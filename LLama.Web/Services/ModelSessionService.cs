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
    private readonly ConcurrentDictionary<string, ModelSession> _modelSessions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelSessionService"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    /// <param name="attachmentService">The attachment service.</param>
    public ModelSessionService(IModelService modelService, IAttachmentService attachmentService)
    {
        _modelService = modelService;
        _attachmentService = attachmentService;
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
        var modelSession = new ModelSession(model, context, sessionId, sessionConfig, inferenceConfig);
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
            return await _modelService.RemoveContext(modelSession.ModelName, sessionId);
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
        var embeds = new List<SafeMtmdEmbed>();

        try
        {
            if (request.AttachmentIds != null && request.AttachmentIds.Count > 0)
            {
                var mediaMarker = GetMediaMarker();
                var attachments = _attachmentService.GetAttachments(sessionId, request.AttachmentIds);

                foreach (var attachment in attachments)
                {
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

                            embeds.Add(await LoadEmbedAsync(modelSession, attachment, cancellationToken));
                            AppendMedia(promptBuilder, attachment, "Image", mediaMarker);
                            break;
                        case AttachmentKind.Audio:
                            if (!modelSession.IsMultiModal || !modelSession.SupportsAudio)
                                throw new Exception("This model does not support audio inputs.");

                            embeds.Add(await LoadEmbedAsync(modelSession, attachment, cancellationToken));
                            AppendMedia(promptBuilder, attachment, "Audio", mediaMarker);
                            break;
                    }
                }
            }

            if (embeds.Count > 0)
                modelSession.QueueEmbeds(embeds);

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

    private static void AppendMedia(StringBuilder builder, AttachmentInfo attachment, string mediaLabel, string mediaMarker)
    {
        builder.AppendLine();
        builder.AppendLine($"[{mediaLabel}: {attachment.FileName}]");
        builder.AppendLine(mediaMarker);
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

    private static async Task<SafeMtmdEmbed> LoadEmbedAsync(ModelSession modelSession, AttachmentInfo attachment, CancellationToken cancellationToken)
    {
        if (!File.Exists(attachment.FilePath))
            throw new FileNotFoundException("Attachment not found.", attachment.FilePath);

        var data = await File.ReadAllBytesAsync(attachment.FilePath, cancellationToken);
        var clipModel = modelSession.ClipModel;
        if (clipModel is null)
            throw new Exception("Multimodal model is not available for this session.");

        var embed = clipModel.LoadMedia(data);
        if (embed == null)
            throw new Exception("Failed to prepare multimodal embedding.");

        return embed;
    }

    private sealed record PreparedPrompt(string Prompt);
}
