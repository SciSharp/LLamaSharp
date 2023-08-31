using LLama;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.Connectors.AI.LLama.ChatCompletion;

/// <summary>
/// LLamaSharp ChatCompletion
/// </summary>
public sealed class LLamaSharpChatCompletion : IChatCompletion
{
    private const string UserRole = "user:";
    private const string AssistantRole = "assistant:";
    private ChatSession session;

    public LLamaSharpChatCompletion(InteractiveExecutor model)
    {
        this.session = new ChatSession(model)
            .WithHistoryTransform(new HistoryTransform())
            .WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(new string[] { UserRole, AssistantRole }));
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
    public async Task<IReadOnlyList<IChatResult>> GetChatCompletionsAsync(ChatHistory chat, ChatRequestSettings? requestSettings = null, CancellationToken cancellationToken = default)
    {
        requestSettings ??= new ChatRequestSettings()
        {
            MaxTokens = 256,
            Temperature = 0,
            TopP = 0,
            StopSequences = new List<string> { }
        };

        var result = this.session.ChatAsync(chat.ToLLamaSharpChatHistory(), requestSettings.ToLLamaSharpInferenceParams(), cancellationToken);

        return new List<IChatResult> { new LLamaSharpChatResult(result) }.AsReadOnly();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<IChatStreamingResult> GetStreamingChatCompletionsAsync(ChatHistory chat, ChatRequestSettings? requestSettings = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        requestSettings ??= new ChatRequestSettings()
        {
            MaxTokens = 256,
            Temperature = 0,
            TopP = 0,
            StopSequences = new List<string> { }
        };

        var result = this.session.ChatAsync(chat.ToLLamaSharpChatHistory(), requestSettings.ToLLamaSharpInferenceParams(), cancellationToken);

        yield return new LLamaSharpChatResult(result);
    }
}
