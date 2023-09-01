using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace LLamaSharp.SemanticKernel.ChatCompletion;

/// <summary>
/// LLamaSharp Chat Message
/// </summary>
public class LLamaSharpChatMessage : ChatMessageBase
{
    /// <inheritdoc/>
    public LLamaSharpChatMessage(AuthorRole role, string content) : base(role, content)
    {
    }
}
