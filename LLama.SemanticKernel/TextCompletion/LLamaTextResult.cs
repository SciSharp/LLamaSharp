using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;
using System.Runtime.CompilerServices;
using System.Text;

namespace LLamaSharp.SemanticKernel.TextCompletion;

internal sealed class LLamaTextResult : ITextResult, ITextStreamingResult
{
    private readonly IAsyncEnumerable<string> _text;

    public LLamaTextResult(IAsyncEnumerable<string> text)
    {
        _text = text;
        ModelResult = new(text);
    }

    public ModelResult ModelResult { get; }

    public async Task<string> GetCompletionAsync(CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();
        await foreach (var token in _text)
        {
            sb.Append(token);
        }
        return await Task.FromResult(sb.ToString()).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<string> GetCompletionStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (string word in _text)
        {
            yield return word;
        }
    }
}
