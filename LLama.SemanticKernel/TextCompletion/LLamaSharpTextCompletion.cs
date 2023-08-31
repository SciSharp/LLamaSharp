using LLama;
using LLama.Abstractions;
using Microsoft.SemanticKernel.AI.TextCompletion;

namespace Microsoft.SemanticKernel.Connectors.AI.LLama.TextCompletion;

public sealed class LLamaSharpTextCompletion : ITextCompletion
{
    public ILLamaExecutor executor;

    public LLamaSharpTextCompletion(ILLamaExecutor executor)
    {
        this.executor = executor;
    }

    public async Task<IReadOnlyList<ITextResult>> GetCompletionsAsync(string text, CompleteRequestSettings requestSettings, CancellationToken cancellationToken = default)
    {
        var result = executor.InferAsync(text, requestSettings.ToLLamaSharpInferenceParams(), cancellationToken);
        return await Task.FromResult(new List<ITextResult> { new LLamaTextResult(result) }.AsReadOnly()).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<ITextStreamingResult> GetStreamingCompletionsAsync(string text, CompleteRequestSettings requestSettings, CancellationToken cancellationToken = default)
    {
        var result = executor.InferAsync(text, requestSettings.ToLLamaSharpInferenceParams(), cancellationToken);
        yield return new LLamaTextResult(result);
    }
}
