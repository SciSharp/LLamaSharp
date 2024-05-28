using LLama.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextGeneration;
using System.Runtime.CompilerServices;
using System.Text;

namespace LLamaSharp.SemanticKernel.TextCompletion;

public sealed class LLamaSharpTextCompletion : ITextGenerationService
{
    public ILLamaExecutor executor;

    private readonly Dictionary<string, object?> _attributes = new();

    public IReadOnlyDictionary<string, object?> Attributes => this._attributes;

    public LLamaSharpTextCompletion(ILLamaExecutor executor)
    {
        this.executor = executor;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        var settings = LLamaSharpPromptExecutionSettings.FromRequestSettings(executionSettings);
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
        var settings = LLamaSharpPromptExecutionSettings.FromRequestSettings(executionSettings);
        var result = executor.InferAsync(prompt, settings?.ToLLamaSharpInferenceParams(), cancellationToken);
        await foreach (var token in result)
        {
            yield return new StreamingTextContent(token);
        }
    }
}
