using LLama.Abstractions;
using LLamaSharp.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;

namespace LLamaSharp.SemanticKernel.TextCompletion;

public sealed class LLamaSharpTextCompletion : ITextCompletion
{
    public ILLamaExecutor executor;

    public LLamaSharpTextCompletion(ILLamaExecutor executor)
    {
        this.executor = executor;
    }

    public async Task<IReadOnlyList<ITextResult>> GetCompletionsAsync(string text, AIRequestSettings? requestSettings, CancellationToken cancellationToken = default)
    {
        var settings = (ChatRequestSettings)requestSettings;
        var result = executor.InferAsync(text, settings?.ToLLamaSharpInferenceParams(), cancellationToken);
        return await Task.FromResult(new List<ITextResult> { new LLamaTextResult(result) }.AsReadOnly()).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<ITextStreamingResult> GetStreamingCompletionsAsync(string text, AIRequestSettings? requestSettings, CancellationToken cancellationToken = default)
    {
        var settings = (ChatRequestSettings)requestSettings;
        var result = executor.InferAsync(text, settings?.ToLLamaSharpInferenceParams(), cancellationToken);
        yield return new LLamaTextResult(result);
    }
}
