using LLama.Sampling;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.ChatCompletion;
using AuthorRole = LLama.Common.AuthorRole;

namespace LLamaSharp.SemanticKernel;

public static class ExtensionMethods
{
    public static LLama.Common.ChatHistory ToLLamaSharpChatHistory(this ChatHistory chatHistory, bool ignoreCase = true)
    {
        if (chatHistory is null)
        {
            throw new ArgumentNullException(nameof(chatHistory));
        }

        var history = new LLama.Common.ChatHistory();

        foreach (var chat in chatHistory)
        {
            if (!Enum.TryParse<AuthorRole>(chat.Role.Label, ignoreCase, out var role))
                role = AuthorRole.Unknown;

            history.AddMessage(role, chat.Content ?? "");
        }

        return history;
    }

    public static LLama.Common.ChatHistory ToLLamaSharpChatHistory(this IEnumerable<ChatMessage> messages, bool ignoreCase = true)
    {
        if (messages is null)
        {
            throw new ArgumentNullException(nameof(messages));
        }

        var history = new LLama.Common.ChatHistory();

        foreach (var chat in messages)
        {
            if (!Enum.TryParse<AuthorRole>(chat.Role.Value, ignoreCase, out var role))
                role = AuthorRole.Unknown;

            history.AddMessage(role, chat.Text ?? "");
        }

        return history;
    }

    /// <summary>
    /// Convert LLamaSharpPromptExecutionSettings to LLamaSharp InferenceParams
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <returns></returns>
    internal static LLama.Common.InferenceParams ToLLamaSharpInferenceParams(this LLamaSharpPromptExecutionSettings requestSettings)
    {
        if (requestSettings is null)
        {
            throw new ArgumentNullException(nameof(requestSettings));
        }

        var antiPrompts = new List<string>(requestSettings.StopSequences)
        {
            $"{AuthorRole.User}:",
            $"{AuthorRole.Assistant}:",
            $"{AuthorRole.System}:"
        };
        return new LLama.Common.InferenceParams
        {
            AntiPrompts = antiPrompts,
            MaxTokens = requestSettings.MaxTokens ?? -1,

            SamplingPipeline = new DefaultSamplingPipeline()
            {
                Temperature = (float)requestSettings.Temperature,
                TopP = (float)requestSettings.TopP,
                PresencePenalty = (float)requestSettings.PresencePenalty,
                FrequencyPenalty = (float)requestSettings.FrequencyPenalty,
            }
        };
    }
}
