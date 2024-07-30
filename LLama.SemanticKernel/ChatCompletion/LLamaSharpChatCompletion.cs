using LLama;
using LLama.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Runtime.CompilerServices;
using System.Text;
using static LLama.InteractiveExecutor;
using static LLama.LLamaTransforms;

namespace LLamaSharp.SemanticKernel.ChatCompletion;

/// <summary>
/// LLamaSharp ChatCompletion
/// </summary>
public sealed class LLamaSharpChatCompletion : IChatCompletionService
{
    private readonly ILLamaExecutor _model;
    private readonly LLamaSharpPromptExecutionSettings _defaultRequestSettings;
    private readonly IHistoryTransform _historyTransform;
    private readonly ITextStreamTransform _outputTransform;

    private readonly Dictionary<string, object?> _attributes = new();
    private readonly bool _isStatefulExecutor;

    public IReadOnlyDictionary<string, object?> Attributes => _attributes;

    private static LLamaSharpPromptExecutionSettings GetDefaultSettings()
    {
        return new LLamaSharpPromptExecutionSettings
        {
            MaxTokens = 256,
            Temperature = 0,
            TopP = 0,
            StopSequences = new List<string>()
        };
    }

    public LLamaSharpChatCompletion(ILLamaExecutor model,
        LLamaSharpPromptExecutionSettings? defaultRequestSettings = default,
        IHistoryTransform? historyTransform = null,
        ITextStreamTransform? outputTransform = null)
    {
        _model = model;
        _isStatefulExecutor = _model is StatefulExecutorBase;
        _defaultRequestSettings = defaultRequestSettings ?? GetDefaultSettings();
        _historyTransform = historyTransform ?? new HistoryTransform();
        _outputTransform = outputTransform ?? new KeywordTextOutputStreamTransform(new[] { $"{LLama.Common.AuthorRole.User}:",
                                                                                            $"{LLama.Common.AuthorRole.Assistant}:",
                                                                                            $"{LLama.Common.AuthorRole.System}:"});
    }

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
    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        var settings = executionSettings != null
           ? LLamaSharpPromptExecutionSettings.FromRequestSettings(executionSettings)
           : _defaultRequestSettings;

        var prompt = _getFormattedPrompt(chatHistory);
        var result = _model.InferAsync(prompt, settings.ToLLamaSharpInferenceParams(), cancellationToken);

        var output = _outputTransform.TransformAsync(result);

        var sb = new StringBuilder();
        await foreach (var token in output)
        {
            sb.Append(token);
        }

        return new List<ChatMessageContent> { new(AuthorRole.Assistant, sb.ToString()) }.AsReadOnly();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var settings = executionSettings != null
          ? LLamaSharpPromptExecutionSettings.FromRequestSettings(executionSettings)
          : _defaultRequestSettings;

        var prompt = _getFormattedPrompt(chatHistory);
        var result = _model.InferAsync(prompt, settings.ToLLamaSharpInferenceParams(), cancellationToken);

        var output = _outputTransform.TransformAsync(result);

        await foreach (var token in output)
        {
            yield return new StreamingChatMessageContent(AuthorRole.Assistant, token);
        }
    }

    /// <summary>
    /// Return either the entire formatted chatHistory or just the most recent message based on
    /// whether the model extends StatefulExecutorBase or not.
    /// </summary>
    /// <param name="chatHistory"></param>
    /// <returns>The formatted prompt</returns>
    private string _getFormattedPrompt(ChatHistory chatHistory)
    {
        string prompt;
        if (_isStatefulExecutor)
        {
            var state = (InteractiveExecutorState)((StatefulExecutorBase)_model).GetStateData();
            if (state.IsPromptRun)
            {
                prompt = _historyTransform.HistoryToText(chatHistory.ToLLamaSharpChatHistory());
            }
            else
            {
                ChatHistory tempHistory = new();
                tempHistory.AddUserMessage(chatHistory.Last().Content ?? "");
                prompt = _historyTransform.HistoryToText(tempHistory.ToLLamaSharpChatHistory());
            }
        }
        else
        {
            prompt = _historyTransform.HistoryToText(chatHistory.ToLLamaSharpChatHistory());
        }

        return prompt;
    }
}
