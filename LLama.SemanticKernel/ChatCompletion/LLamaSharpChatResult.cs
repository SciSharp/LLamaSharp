using Microsoft.SemanticKernel.AI.ChatCompletion;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.SemanticKernel.Connectors.AI.LLama.ChatCompletion;

internal sealed class LLamaSharpChatResult : IChatStreamingResult
{
    private readonly IAsyncEnumerable<string> _stream;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    public LLamaSharpChatResult(IAsyncEnumerable<string> stream)
    {
        _stream = stream;
    }
    /// <inheritdoc/>
    public async Task<ChatMessageBase> GetChatMessageAsync(CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();
        await foreach (var token in _stream)
        {
            sb.Append(token);
        }
        return await Task.FromResult(new LLamaSharpChatMessage(AuthorRole.Assistant, sb.ToString())).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<ChatMessageBase> GetStreamingChatMessageAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var token in _stream)
        {
            yield return new LLamaSharpChatMessage(AuthorRole.Assistant, token);
        }
    }
}
