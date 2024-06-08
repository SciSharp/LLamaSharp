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
        return template.ToModelPrompt();
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
}
