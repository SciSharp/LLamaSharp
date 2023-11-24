using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Orchestration;
using System.Runtime.CompilerServices;
using System.Text;

namespace LLamaSharp.SemanticKernel.ChatCompletion;

internal sealed class LLamaSharpChatResult : IChatResult, IChatStreamingResult
{
    private readonly ModelResult _modelResult;
    private readonly IAsyncEnumerable<string> _stream;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    public LLamaSharpChatResult(IAsyncEnumerable<string> stream)
    {
        _stream = stream;
        this._modelResult = new ModelResult(stream);
    }

    public ModelResult ModelResult => this._modelResult;

    /// <inheritdoc/>
    public async Task<ChatMessage> GetChatMessageAsync(CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();
        await foreach (var token in _stream)
        {
            sb.Append(token);
        }
        return await Task.FromResult(new LLamaSharpChatMessage(AuthorRole.Assistant, sb.ToString())).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<ChatMessage> GetStreamingChatMessageAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var token in _stream)
        {
            yield return new LLamaSharpChatMessage(AuthorRole.Assistant, token);
        }
    }
}
