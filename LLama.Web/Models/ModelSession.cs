using LLama;
using LLama.Abstractions;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using LLama.Web.Common;
using Microsoft.Extensions.Logging;

namespace LLama.Web.Models;

public class ModelSession
{
    private sealed record TemplateMarkers(string? AssistantEndMarker, string? AssistantToUserMarker);

    private static readonly HashSet<string> LegacyDefaultAntiPrompts = new(StringComparer.Ordinal)
    {
        "User:",
        "<|im_end|>",
        "<|eot_id|>",
        "<|endoftext|>"
    };

    private static readonly HashSet<string> LegacyDefaultOutputFilters = new(StringComparer.Ordinal)
    {
        "User:",
        "Assistant:",
        "<|im_end|>",
        "<|eot_id|>",
        "<|endoftext|>"
    };

    private readonly string _sessionId;
    private readonly LLamaModel _model;
    private readonly LLamaContext _context;
    private readonly ILLamaExecutor _executor;
    private readonly ISessionConfig _sessionConfig;
    private readonly ILogger? _logger;
    private readonly List<string> _configuredAntiPrompts;
    private readonly List<string> _configuredOutputFilters;
    private readonly ITextStreamTransform _outputTransform;
    private readonly InferenceOptions _defaultInferenceConfig;
    private readonly ChatHistory _chatHistory = new();
    private readonly string? _systemPrompt;
    private readonly TemplateMarkers _templateMarkers;
    private readonly object _historyLock = new();

    private CancellationTokenSource _cancellationTokenSource;

    public ModelSession(LLamaModel model, LLamaContext context, string sessionId, ISessionConfig sessionConfig, InferenceOptions inferenceOptions = null, ILogger? logger = null)
    {
        _model = model;
        _context = context;
        _sessionId = sessionId;
        _sessionConfig = sessionConfig;
        _logger = logger;
        _defaultInferenceConfig = inferenceOptions ?? new InferenceOptions
        {
            SamplingPipeline = new DefaultSamplingPipeline()
        };
        _systemPrompt = _sessionConfig.Prompt;
        _configuredAntiPrompts = _sessionConfig.GetAntiPrompts();
        _configuredOutputFilters = _sessionConfig.GetOutputFilters();
        _executor = CreateExecutor();
        _templateMarkers = ResolveTemplateMarkers();
        _outputTransform = CreateOutputFilter();
        _logger?.LogInformation(
            "Session {SessionId} created for model {ModelName} using {ExecutorType}. Template markers: assistant-end={AssistantEndMarker}; assistant-to-user={AssistantToUserMarker}.",
            _sessionId,
            _sessionConfig.Model,
            _sessionConfig.ExecutorType,
            EscapeForLog(_templateMarkers.AssistantEndMarker),
            EscapeForLog(_templateMarkers.AssistantToUserMarker));

        if (_sessionConfig.ExecutorType != LLamaExecutorType.Stateless && !string.IsNullOrWhiteSpace(_systemPrompt))
            _chatHistory.AddMessage(AuthorRole.System, _systemPrompt);
    }

    /// <summary>
    /// Gets the session identifier.
    /// </summary>
    public string SessionId => _sessionId;

    /// <summary>
    /// Gets the name of the model.
    /// </summary>
    public string ModelName => _sessionConfig.Model;

    /// <summary>
    /// Gets the context.
    /// </summary>
    public LLamaContext Context => _context;

    /// <summary>
    /// Gets the session configuration.
    /// </summary>
    public ISessionConfig SessionConfig => _sessionConfig;

    /// <summary>
    /// Gets the inference parameters.
    /// </summary>
    public InferenceOptions InferenceParams => _defaultInferenceConfig;

    /// <summary>
    /// Returns true if the executor has multimodal support enabled.
    /// </summary>
    public bool IsMultiModal => _executor.IsMultiModal;

    /// <summary>
    /// Returns true if the multimodal model supports audio inputs.
    /// </summary>
    public bool SupportsAudio => _executor.ClipModel?.SupportsAudio ?? false;

    /// <summary>
    /// Returns true if the multimodal model supports vision inputs.
    /// </summary>
    public bool SupportsVision => _executor.ClipModel?.SupportsVision ?? false;

    /// <summary>
    /// Gets the MTMD clip model, if available.
    /// </summary>
    public MtmdWeights ClipModel => _executor.ClipModel;

    /// <summary>
    /// Returns true if the session should preserve chat history.
    /// </summary>
    public bool StoresHistory => _sessionConfig.ExecutorType != LLamaExecutorType.Stateless;

    /// <summary>
    /// Queue media embeddings for the next inference call.
    /// </summary>
    public void QueueEmbeds(IEnumerable<SafeMtmdEmbed> embeds)
    {
        if (embeds is null)
            return;

        foreach (var embed in embeds)
            _executor.Embeds.Add(embed);
    }

    /// <summary>
    /// Initializes the prompt.
    /// </summary>
    /// <param name="inferenceConfig">The inference configuration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal async Task InitializePrompt(InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default)
    {
        if (_sessionConfig.ExecutorType == LLamaExecutorType.Stateless)
            return;

        if (_chatHistory.Messages.Count == 0)
            return;

        if (_executor is StatefulExecutorBase statefulExecutor)
        {
            var prompt = FormatChatHistory(_chatHistory, addAssistant: false);
            if (!string.IsNullOrWhiteSpace(prompt))
                await statefulExecutor.PrefillPromptAsync(prompt, cancellationToken);
        }
    }

    /// <summary>
    /// Runs inference on the model context
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inferenceConfig">The inference configuration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    internal IAsyncEnumerable<string> InferAsync(string message, InferenceOptions inferenceConfig = null, CancellationToken cancellationToken = default)
    {
        var inferenceParams = ConfigureInferenceParams(inferenceConfig);
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _logger?.LogInformation(
            "Session {SessionId} starting inference. Prompt={Prompt}. DecodeSpecialTokens={DecodeSpecialTokens}. AntiPrompts=[{AntiPrompts}]. OutputFilters=[{OutputFilters}].",
            _sessionId,
            EscapeForLog(message, 320),
            inferenceParams.DecodeSpecialTokens,
            JoinForLog(inferenceParams.AntiPrompts),
            JoinForLog(GetEffectiveOutputFilters()));

        var inferenceStream = LogStreamAsync(
            _executor.InferAsync(message, inferenceParams, _cancellationTokenSource.Token),
            "raw",
            _cancellationTokenSource.Token);
        if (_outputTransform is not null)
            return LogStreamAsync(_outputTransform.TransformAsync(inferenceStream), "filtered", _cancellationTokenSource.Token);

        return inferenceStream;
    }

    public void CancelInfer()
    {
        _cancellationTokenSource?.Cancel();
    }

    public bool IsInferCanceled()
    {
        return _cancellationTokenSource.IsCancellationRequested;
    }

    /// <summary>
    /// Builds a prompt for the current chat session using the model template.
    /// </summary>
    /// <param name="userContent">The user content to include in the prompt.</param>
    /// <returns>The formatted prompt to send to the model.</returns>
    internal string BuildPrompt(string userContent)
    {
        userContent ??= string.Empty;

        if (_sessionConfig.ExecutorType == LLamaExecutorType.Stateless)
            return BuildStatelessPrompt(userContent);

        return BuildChatDelta(userContent);
    }

    /// <summary>
    /// Adds the assistant response to the chat history.
    /// </summary>
    /// <param name="assistantContent">Assistant response content.</param>
    internal void AddAssistantMessage(string assistantContent)
    {
        if (_sessionConfig.ExecutorType == LLamaExecutorType.Stateless)
            return;

        lock (_historyLock)
        {
            _chatHistory.AddMessage(AuthorRole.Assistant, assistantContent ?? string.Empty);
        }
    }

    /// <summary>
    /// Configures the inference parameters.
    /// </summary>
    /// <param name="inferenceConfig">The inference configuration.</param>
    private IInferenceParams ConfigureInferenceParams(InferenceOptions inferenceConfig)
    {
        var inferenceParams = inferenceConfig ?? _defaultInferenceConfig;
        var antiPrompts = GetEffectiveAntiPrompts();
        AddTemplateMarkers(antiPrompts);

        inferenceParams.AntiPrompts = antiPrompts;
        inferenceParams.DecodeSpecialTokens = inferenceParams.DecodeSpecialTokens || ShouldDecodeSpecialTokens(antiPrompts);
        return inferenceParams;
    }

    private ITextStreamTransform CreateOutputFilter()
    {
        var outputFilters = GetEffectiveOutputFilters();
        if (outputFilters.Count > 0)
            return new LLamaTransforms.KeywordTextOutputStreamTransform(outputFilters);

        return null;
    }

    private List<string> GetEffectiveAntiPrompts()
    {
        var antiPrompts = new List<string>(_configuredAntiPrompts);
        if (HasTemplateMarkers() && ContainsOnlyLegacyDefaults(antiPrompts, LegacyDefaultAntiPrompts))
            antiPrompts.Clear();

        return antiPrompts;
    }

    private List<string> GetEffectiveOutputFilters()
    {
        var outputFilters = new List<string>(_configuredOutputFilters);
        if (HasTemplateMarkers() && ContainsOnlyLegacyDefaults(outputFilters, LegacyDefaultOutputFilters))
            outputFilters.Clear();

        AddTemplateMarkers(outputFilters);
        return outputFilters;
    }

    private bool HasTemplateMarkers()
    {
        return !string.IsNullOrWhiteSpace(_templateMarkers.AssistantEndMarker)
            || !string.IsNullOrWhiteSpace(_templateMarkers.AssistantToUserMarker);
    }

    private void AddTemplateMarkers(List<string> values)
    {
        AddMarker(values, _templateMarkers.AssistantEndMarker);
        AddMarker(values, _templateMarkers.AssistantToUserMarker);
    }

    private static void AddMarker(List<string> values, string? marker)
    {
        if (string.IsNullOrWhiteSpace(marker))
            return;

        if (!values.Contains(marker))
            values.Add(marker);

        var trimmedMarker = marker.Trim();
        if (!string.IsNullOrWhiteSpace(trimmedMarker) && !values.Contains(trimmedMarker))
            values.Add(trimmedMarker);
    }

    private static bool ContainsOnlyLegacyDefaults(List<string> values, HashSet<string> legacyDefaults)
    {
        if (values.Count == 0)
            return false;

        foreach (var value in values)
        {
            if (!legacyDefaults.Contains(value))
                return false;
        }

        return true;
    }

    private static bool ShouldDecodeSpecialTokens(IReadOnlyList<string> antiPrompts)
    {
        foreach (var antiPrompt in antiPrompts)
        {
            if (string.IsNullOrWhiteSpace(antiPrompt))
                continue;

            if (antiPrompt.Contains('<', StringComparison.Ordinal) || antiPrompt.Contains('>', StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    private ILLamaExecutor CreateExecutor()
    {
        return _sessionConfig.ExecutorType switch
        {
            LLamaExecutorType.Interactive => _model.MtmdWeights is null
                ? new InteractiveExecutor(_context, _logger)
                : new InteractiveExecutor(_context, _model.MtmdWeights, _logger),
            LLamaExecutorType.Instruct => _model.MtmdWeights is null
                ? new InstructExecutor(_context, logger: _logger)
                : new InstructExecutor(_context, _model.MtmdWeights, logger: _logger),
            LLamaExecutorType.Stateless => new StatelessExecutor(_model.LLamaWeights, _context.Params, _logger),
            _ => default
        };
    }

    private string BuildChatDelta(string userContent)
    {
        lock (_historyLock)
        {
            var pastPrompt = FormatChatHistory(_chatHistory, addAssistant: false);
            _chatHistory.AddMessage(AuthorRole.User, userContent);
            var fullPrompt = FormatChatHistory(_chatHistory, addAssistant: true);

            if (!fullPrompt.StartsWith(pastPrompt, StringComparison.Ordinal))
                return fullPrompt;

            var delta = fullPrompt[pastPrompt.Length..];
            if (pastPrompt.Length > 0 && pastPrompt.EndsWith('\n') && (delta.Length == 0 || delta[0] != '\n'))
                delta = "\n" + delta;

            return delta;
        }
    }

    private string BuildStatelessPrompt(string userContent)
    {
        var history = new ChatHistory();
        if (!string.IsNullOrWhiteSpace(_systemPrompt))
            history.AddMessage(AuthorRole.System, _systemPrompt);
        history.AddMessage(AuthorRole.User, userContent);

        return FormatChatHistory(history, addAssistant: true);
    }

    private string FormatChatHistory(ChatHistory history, bool addAssistant)
    {
        var template = new LLamaTemplate(_model.LLamaWeights.NativeHandle)
        {
            AddAssistant = addAssistant
        };

        foreach (var message in history.Messages)
            template.Add(message.AuthorRole.ToString().ToLowerInvariant(), message.Content);

        return LLamaTemplate.Encoding.GetString(template.Apply());
    }

    private TemplateMarkers ResolveTemplateMarkers()
    {
        const string userMarkerA = "__LLAMA_USER_A__";
        const string assistantMarkerA = "__LLAMA_ASSISTANT_A__";
        const string userMarkerB = "__LLAMA_USER_B__";

        try
        {
            var assistantTemplate = new LLamaTemplate(_model.LLamaWeights.NativeHandle)
            {
                AddAssistant = false
            };
            assistantTemplate.Add("user", userMarkerA);
            assistantTemplate.Add("assistant", assistantMarkerA);

            var assistantRendered = LLamaTemplate.Encoding.GetString(assistantTemplate.Apply());
            var assistantIndex = assistantRendered.IndexOf(assistantMarkerA, StringComparison.Ordinal);
            if (assistantIndex < 0)
                return new TemplateMarkers(null, null);

            var assistantEndMarker = assistantRendered[(assistantIndex + assistantMarkerA.Length)..];

            var conversationTemplate = new LLamaTemplate(_model.LLamaWeights.NativeHandle)
            {
                AddAssistant = false
            };
            conversationTemplate.Add("user", userMarkerA);
            conversationTemplate.Add("assistant", assistantMarkerA);
            conversationTemplate.Add("user", userMarkerB);

            var conversationRendered = LLamaTemplate.Encoding.GetString(conversationTemplate.Apply());
            var assistantConversationIndex = conversationRendered.IndexOf(assistantMarkerA, StringComparison.Ordinal);
            var userIndex = conversationRendered.IndexOf(userMarkerB, StringComparison.Ordinal);
            if (assistantConversationIndex < 0 || userIndex <= assistantConversationIndex)
                return new TemplateMarkers(NormalizeMarker(assistantEndMarker), null);

            var assistantToUserMarker = conversationRendered.Substring(
                assistantConversationIndex + assistantMarkerA.Length,
                userIndex - (assistantConversationIndex + assistantMarkerA.Length));

            return new TemplateMarkers(
                NormalizeMarker(assistantEndMarker),
                NormalizeMarker(assistantToUserMarker));
        }
        catch
        {
            _logger?.LogWarning("Session {SessionId} failed to resolve template markers for model {ModelName}.", _sessionId, _sessionConfig.Model);
            return new TemplateMarkers(null, null);
        }
    }

    private static string? NormalizeMarker(string? marker)
    {
        return string.IsNullOrWhiteSpace(marker) ? null : marker;
    }

    private async IAsyncEnumerable<string> LogStreamAsync(IAsyncEnumerable<string> stream, string stage, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chunkCount = 0;
        await foreach (var chunk in stream.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            chunkCount++;
            _logger?.LogInformation(
                "Session {SessionId} {Stage} chunk {ChunkCount}: {Chunk}",
                _sessionId,
                stage,
                chunkCount,
                EscapeForLog(chunk, 240));
            yield return chunk;
        }
    }

    private static string JoinForLog(IReadOnlyList<string>? values)
    {
        if (values is null || values.Count == 0)
            return string.Empty;

        return string.Join(", ", values.Select(value => EscapeForLog(value)));
    }

    private static string EscapeForLog(string? value, int maxLength = 120)
    {
        if (string.IsNullOrEmpty(value))
            return "<empty>";

        var escaped = value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\t", "\\t", StringComparison.Ordinal);

        if (escaped.Length <= maxLength)
            return escaped;

        return escaped[..maxLength] + "...";
    }
}
