using LLama;
using LLama.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Services;
using System;
using System.IO;
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
    private LLamaSharpPromptExecutionSettings defaultRequestSettings;
    private readonly IHistoryTransform historyTransform;
    private readonly ITextStreamTransform outputTransform;

    private readonly Dictionary<string, object?> _attributes = new();
    private readonly bool _isStatefulExecutor;

    public IReadOnlyDictionary<string, object?> Attributes => this._attributes;

    static LLamaSharpPromptExecutionSettings GetDefaultSettings()
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
        this._model = model;
        this._isStatefulExecutor = this._model is StatefulExecutorBase;
        this.defaultRequestSettings = defaultRequestSettings ?? GetDefaultSettings();
        this.historyTransform = historyTransform ?? new HistoryTransform();
        this.outputTransform = outputTransform ?? new KeywordTextOutputStreamTransform(new[] { $"{LLama.Common.AuthorRole.User}:",
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
           : defaultRequestSettings;

        string prompt = this._getFormattedPrompt(chatHistory);
        var result = _model.InferAsync(prompt, settings.ToLLamaSharpInferenceParams(), cancellationToken);

        var output = outputTransform.TransformAsync(result);

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
          : defaultRequestSettings;

        string prompt = this._getFormattedPrompt(chatHistory);
        var result = _model.InferAsync(prompt, settings.ToLLamaSharpInferenceParams(), cancellationToken);

        var output = outputTransform.TransformAsync(result);

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
    private string _getFormattedPrompt(ChatHistory chatHistory){
        string prompt;
        if (this._isStatefulExecutor){
            InteractiveExecutorState state = (InteractiveExecutorState)((StatefulExecutorBase)this._model).GetStateData();
            if (state.IsPromptRun)
            {
                prompt = historyTransform.HistoryToText(chatHistory.ToLLamaSharpChatHistory());
            }
            else
            {
                ChatHistory temp_history = new();
                temp_history.AddUserMessage(chatHistory.Last().Content);
                prompt = historyTransform.HistoryToText(temp_history.ToLLamaSharpChatHistory());
            }
        }
        else
        {
            prompt = historyTransform.HistoryToText(chatHistory.ToLLamaSharpChatHistory());
        }

        return prompt;
    }
}
