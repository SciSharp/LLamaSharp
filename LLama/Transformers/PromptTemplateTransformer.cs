using System;
using System.Text;
using LLama.Abstractions;
using LLama.Common;

namespace LLama.Transformers;

/// <summary>
/// A prompt formatter that will use llama.cpp's template formatter
/// If your model is not supported, you will need to define your own formatter according the cchat prompt specification for your model
/// </summary>
public class PromptTemplateTransformer(LLamaWeights model, 
    bool withAssistant = true) : IHistoryTransform
{
    private readonly LLamaWeights _model = model;
    private readonly bool _withAssistant = withAssistant;

    /// <inheritdoc />
    public string HistoryToText(ChatHistory history)
    {
        var template = new LLamaTemplate(_model.NativeHandle)
        {
            AddAssistant = _withAssistant,
        };

        // encode each message and return the final prompt
        foreach (var message in history.Messages)
        {
            template.Add(message.AuthorRole.ToString().ToLowerInvariant(), message.Content);
        }
        return ToModelPrompt(template);
    }

    /// <inheritdoc />
    public ChatHistory TextToHistory(AuthorRole role, string text)
    {
        return new ChatHistory([new ChatHistory.Message(role, text)]);
    }

    /// <inheritdoc />
    public IHistoryTransform Clone()
    {
        // need to preserve history?
        return new PromptTemplateTransformer(_model);
    }

    #region utils
    /// <summary>
    /// Apply the template to the messages and return the resulting prompt as a string
    /// </summary>
    /// 
    /// <returns>The formatted template string as defined by the model</returns>
    public static string ToModelPrompt(LLamaTemplate template)
    {
        // Apply the template to update state and get data length
        var templateBuffer = template.Apply();

        // convert the resulting buffer to a string
#if NET6_0_OR_GREATER
        return LLamaTemplate.Encoding.GetString(templateBuffer);
#endif

        // need the ToArray call for netstandard -- avoided in newer runtimes
        return LLamaTemplate.Encoding.GetString(templateBuffer.ToArray());
    }
    #endregion utils
}
