﻿using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.TextCompletion;

namespace LLamaSharp.SemanticKernel;

internal static class ExtensionMethods
{
    internal static global::LLama.Common.ChatHistory ToLLamaSharpChatHistory(this ChatHistory chatHistory)
    {
        if (chatHistory is null)
        {
            throw new ArgumentNullException(nameof(chatHistory));
        }

        var history = new global::LLama.Common.ChatHistory();

        foreach (var chat in chatHistory)
        {
            var role = Enum.TryParse<global::LLama.Common.AuthorRole>(chat.Role.Label, out var _role) ? _role : global::LLama.Common.AuthorRole.Unknown;
            history.AddMessage(role, chat.Content);
        }

        return history;
    }

    /// <summary>
    /// Convert ChatRequestSettings to LLamaSharp InferenceParams
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <returns></returns>
    internal static global::LLama.Common.InferenceParams ToLLamaSharpInferenceParams(this ChatRequestSettings requestSettings)
    {
        if (requestSettings is null)
        {
            throw new ArgumentNullException(nameof(requestSettings));
        }

        var antiPrompts = new List<string>(requestSettings.StopSequences) { AuthorRole.User.ToString() + ":" };
        return new global::LLama.Common.InferenceParams
        {
            Temperature = (float)requestSettings.Temperature,
            TopP = (float)requestSettings.TopP,
            PresencePenalty = (float)requestSettings.PresencePenalty,
            FrequencyPenalty = (float)requestSettings.FrequencyPenalty,
            AntiPrompts = antiPrompts,
            MaxTokens = requestSettings.MaxTokens ?? -1
        };
    }

    /// <summary>
    /// Convert CompleteRequestSettings to LLamaSharp InferenceParams
    /// </summary>
    /// <param name="requestSettings"></param>
    /// <returns></returns>
    internal static global::LLama.Common.InferenceParams ToLLamaSharpInferenceParams(this CompleteRequestSettings requestSettings)
    {
        if (requestSettings is null)
        {
            throw new ArgumentNullException(nameof(requestSettings));
        }

        return new global::LLama.Common.InferenceParams
        {
            Temperature = (float)requestSettings.Temperature,
            TopP = (float)requestSettings.TopP,
            PresencePenalty = (float)requestSettings.PresencePenalty,
            FrequencyPenalty = (float)requestSettings.FrequencyPenalty,
            AntiPrompts = requestSettings.StopSequences,
            MaxTokens = requestSettings.MaxTokens ?? -1
        };
    }
}
