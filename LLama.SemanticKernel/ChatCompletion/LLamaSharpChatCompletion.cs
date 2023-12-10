using LLama;
using LLama.Abstractions;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using System.Runtime.CompilerServices;
using static LLama.LLamaTransforms;

namespace LLamaSharp.SemanticKernel.ChatCompletion;

/// <summary>
/// LLamaSharp ChatCompletion
/// </summary>
public sealed class LLamaSharpChatCompletion : IChatCompletion
{
    private readonly StatelessExecutor _model;
    private ChatRequestSettings defaultRequestSettings;
    private readonly IHistoryTransform historyTransform;
    private readonly ITextStreamTransform outputTransform;

    private readonly Dictionary<string, string> _attributes = new();

    public IReadOnlyDictionary<string, string> Attributes => this._attributes;

    static ChatRequestSettings GetDefaultSettings()
    {
        return new ChatRequestSettings
        {
            MaxTokens = 256,
            Temperature = 0,
            TopP = 0,
            StopSequences = new List<string>()
        };
    }

    public LLamaSharpChatCompletion(StatelessExecutor model,
        ChatRequestSettings? defaultRequestSettings = default,
        IHistoryTransform? historyTransform = null,
        ITextStreamTransform? outputTransform = null)
    {
        this._model = model;
        this.defaultRequestSettings = defaultRequestSettings ?? GetDefaultSettings();
        this.historyTransform = historyTransform ?? new HistoryTransform();
        this.outputTransform = outputTransform ?? new KeywordTextOutputStreamTransform(new[] { $"{LLama.Common.AuthorRole.User}:",
                                                                                            $"{LLama.Common.AuthorRole.Assistant}:",
                                                                                            $"{LLama.Common.AuthorRole.System}:"});
    }

    /// <inheritdoc/>
    public ChatHistory CreateNewChat(string? instructions = "")
    {
        var history = new ChatHistory();

        if (instructions != null && !string.IsNullOrEmpty(instructions))
        {
            history.AddSystemMessage(instructions);
        }

        return history;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<IChatResult>> GetChatCompletionsAsync(ChatHistory chat, AIRequestSettings? requestSettings = null, CancellationToken cancellationToken = default)
    {
        var settings = requestSettings != null
            ? ChatRequestSettings.FromRequestSettings(requestSettings)
            : defaultRequestSettings;
        var prompt = historyTransform.HistoryToText(chat.ToLLamaSharpChatHistory());

        var result = _model.InferAsync(prompt, settings.ToLLamaSharpInferenceParams(), cancellationToken);

        return Task.FromResult<IReadOnlyList<IChatResult>>(new List<IChatResult> { new LLamaSharpChatResult(outputTransform.TransformAsync(result)) }.AsReadOnly());
    }

    /// <inheritdoc/>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously.
    public async IAsyncEnumerable<IChatStreamingResult> GetStreamingChatCompletionsAsync(ChatHistory chat, AIRequestSettings? requestSettings = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
#pragma warning restore CS1998
    {
        var settings = requestSettings != null
            ? ChatRequestSettings.FromRequestSettings(requestSettings)
            : defaultRequestSettings;
        var prompt = historyTransform.HistoryToText(chat.ToLLamaSharpChatHistory());
        // This call is not awaited because LLamaSharpChatResult accepts an IAsyncEnumerable.
        var result = _model.InferAsync(prompt, settings.ToLLamaSharpInferenceParams(), cancellationToken);

        yield return new LLamaSharpChatResult(outputTransform.TransformAsync(result));
    }
}
