using LLama;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using System.Runtime.CompilerServices;

namespace LLamaSharp.SemanticKernel.ChatCompletion;

/// <summary>
/// LLamaSharp ChatCompletion
/// </summary>
public sealed class LLamaSharpChatCompletion : IChatCompletion
{
    private const string UserRole = "user:";
    private const string AssistantRole = "assistant:";
    private ChatSession session;
    private ChatRequestSettings defaultRequestSettings;

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

    public LLamaSharpChatCompletion(InteractiveExecutor model, ChatRequestSettings? defaultRequestSettings = default)
    {
        this.session = new ChatSession(model)
            .WithHistoryTransform(new HistoryTransform())
            .WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(new string[] { UserRole, AssistantRole }));
        this.defaultRequestSettings = defaultRequestSettings ??= GetDefaultSettings();
    }

    public LLamaSharpChatCompletion(ChatSession session, ChatRequestSettings? defaultRequestSettings = default)
    {
        this.session = session;
        this.defaultRequestSettings = defaultRequestSettings ??= GetDefaultSettings();
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

        // This call is not awaited because LLamaSharpChatResult accepts an IAsyncEnumerable.
        var result = this.session.ChatAsync(chat.ToLLamaSharpChatHistory(), settings.ToLLamaSharpInferenceParams(), cancellationToken);

        return Task.FromResult<IReadOnlyList<IChatResult>>(new List<IChatResult> { new LLamaSharpChatResult(result) }.AsReadOnly());
    }

    /// <inheritdoc/>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously.
    public async IAsyncEnumerable<IChatStreamingResult> GetStreamingChatCompletionsAsync(ChatHistory chat, AIRequestSettings? requestSettings = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
#pragma warning restore CS1998
    {
        var settings = requestSettings != null
            ? ChatRequestSettings.FromRequestSettings(requestSettings)
            : defaultRequestSettings;

        // This call is not awaited because LLamaSharpChatResult accepts an IAsyncEnumerable.
        var result = this.session.ChatAsync(chat.ToLLamaSharpChatHistory(), settings.ToLLamaSharpInferenceParams(), cancellationToken);

        yield return new LLamaSharpChatResult(result);
    }
}
