using LLama.Abstractions;
using LLamaSharp.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;
using System.Runtime.CompilerServices;

namespace LLamaSharp.SemanticKernel.TextCompletion;

public sealed class LLamaSharpTextCompletion : ITextCompletion
{
    public ILLamaExecutor executor;

    private readonly Dictionary<string, string> _attributes = new();

    public IReadOnlyDictionary<string, string> Attributes => this._attributes;

    public LLamaSharpTextCompletion(ILLamaExecutor executor)
    {
        this.executor = executor;
    }

    public async Task<IReadOnlyList<ITextResult>> GetCompletionsAsync(string text, AIRequestSettings? requestSettings, CancellationToken cancellationToken = default)
    {
        var settings = ChatRequestSettings.FromRequestSettings(requestSettings);
        var result = executor.InferAsync(text, settings?.ToLLamaSharpInferenceParams(), cancellationToken);
        return await Task.FromResult(new List<ITextResult> { new LLamaTextResult(result) }.AsReadOnly()).ConfigureAwait(false);
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously.
    public async IAsyncEnumerable<ITextStreamingResult> GetStreamingCompletionsAsync(string text, AIRequestSettings? requestSettings,[EnumeratorCancellation] CancellationToken cancellationToken = default)
#pragma warning restore CS1998
    {
        var settings = ChatRequestSettings.FromRequestSettings(requestSettings);
        var result = executor.InferAsync(text, settings?.ToLLamaSharpInferenceParams(), cancellationToken);
        yield return new LLamaTextResult(result);
    }
}
