using LLama;
using LLama.Abstractions;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using LLama.Web.Common;

namespace LLama.Web.Models;

public class ModelSession
{
    private readonly string _sessionId;
    private readonly LLamaModel _model;
    private readonly LLamaContext _context;
    private readonly ILLamaExecutor _executor;
    private readonly ISessionConfig _sessionConfig;
    private readonly ITextStreamTransform _outputTransform;
    private readonly InferenceOptions _defaultInferenceConfig;
    private readonly ChatHistory _chatHistory = new();
    private readonly string? _systemPrompt;
    private readonly string? _templateUserMarker;
    private readonly object _historyLock = new();

    private CancellationTokenSource _cancellationTokenSource;

    public ModelSession(LLamaModel model, LLamaContext context, string sessionId, ISessionConfig sessionConfig, InferenceOptions inferenceOptions = null)
    {
        _model = model;
        _context = context;
        _sessionId = sessionId;
        _sessionConfig = sessionConfig;
        _defaultInferenceConfig = inferenceOptions ?? new InferenceOptions
        {
            SamplingPipeline = new DefaultSamplingPipeline()
        };
        _outputTransform = CreateOutputFilter();
        _executor = CreateExecutor();
        _systemPrompt = _sessionConfig.Prompt;
        _templateUserMarker = ResolveTemplateUserMarker();

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

        var inferenceStream = _executor.InferAsync(message, inferenceParams, _cancellationTokenSource.Token);
        if (_outputTransform is not null)
            return _outputTransform.TransformAsync(inferenceStream);

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
        var antiPrompts = _sessionConfig.GetAntiPrompts();
        if (!string.IsNullOrWhiteSpace(_templateUserMarker))
        {
            if (!antiPrompts.Contains(_templateUserMarker))
                antiPrompts.Add(_templateUserMarker);

            var trimmedMarker = _templateUserMarker.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedMarker) && !antiPrompts.Contains(trimmedMarker))
                antiPrompts.Add(trimmedMarker);
        }

        inferenceParams.AntiPrompts = antiPrompts;
        return inferenceParams;
    }

    private ITextStreamTransform CreateOutputFilter()
    {
        var outputFilters = _sessionConfig.GetOutputFilters();
        if (outputFilters.Count > 0)
            return new LLamaTransforms.KeywordTextOutputStreamTransform(outputFilters);

        return null;
    }

    private ILLamaExecutor CreateExecutor()
    {
        return _sessionConfig.ExecutorType switch
        {
            LLamaExecutorType.Interactive => _model.MtmdWeights is null
                ? new InteractiveExecutor(_context)
                : new InteractiveExecutor(_context, _model.MtmdWeights),
            LLamaExecutorType.Instruct => _model.MtmdWeights is null
                ? new InstructExecutor(_context)
                : new InstructExecutor(_context, _model.MtmdWeights),
            LLamaExecutorType.Stateless => new StatelessExecutor(_model.LLamaWeights, _context.Params),
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

    private string? ResolveTemplateUserMarker()
    {
        const string userMarkerA = "__LLAMA_USER_A__";
        const string assistantMarkerA = "__LLAMA_ASSISTANT_A__";
        const string userMarkerB = "__LLAMA_USER_B__";

        try
        {
            var template = new LLamaTemplate(_model.LLamaWeights.NativeHandle)
            {
                AddAssistant = false
            };
            template.Add("user", userMarkerA);
            template.Add("assistant", assistantMarkerA);
            template.Add("user", userMarkerB);

            var rendered = LLamaTemplate.Encoding.GetString(template.Apply());
            var assistantIndex = rendered.IndexOf(assistantMarkerA, StringComparison.Ordinal);
            var userIndex = rendered.IndexOf(userMarkerB, StringComparison.Ordinal);
            if (assistantIndex < 0 || userIndex <= assistantIndex)
                return null;

            var between = rendered.Substring(assistantIndex + assistantMarkerA.Length, userIndex - (assistantIndex + assistantMarkerA.Length));
            return string.IsNullOrWhiteSpace(between) ? null : between;
        }
        catch
        {
            return null;
        }
    }
}
