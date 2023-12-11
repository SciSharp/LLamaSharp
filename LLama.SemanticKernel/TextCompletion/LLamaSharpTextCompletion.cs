using LLama.Abstractions;
using LLamaSharp.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextGeneration;
using Microsoft.SemanticKernel.Services;
using System.Runtime.CompilerServices;
using System.Text;

namespace LLamaSharp.SemanticKernel.TextCompletion;

public sealed class LLamaSharpTextCompletion : ITextGenerationService
{
    public ILLamaExecutor executor;

    private readonly Dictionary<string, string> _attributes = new();

    public IReadOnlyDictionary<string, string> Attributes => this._attributes;

    IReadOnlyDictionary<string, object?> IAIService.Attributes => throw new NotImplementedException();

    public LLamaSharpTextCompletion(ILLamaExecutor executor)
    {
        this.executor = executor;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        var settings = ChatRequestSettings.FromRequestSettings(executionSettings);
        var result = executor.InferAsync(prompt, settings?.ToLLamaSharpInferenceParams(), cancellationToken);
        var sb = new StringBuilder();
        await foreach (var token in result)
        {
            sb.Append(token);
        }
        return new List<TextContent> { new(sb.ToString()) };
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var settings = ChatRequestSettings.FromRequestSettings(executionSettings);
        var result = executor.InferAsync(prompt, settings?.ToLLamaSharpInferenceParams(), cancellationToken);
        await foreach (var token in result)
        {
            yield return new StreamingTextContent(token);
        }
    }
}
