using LLama;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

namespace LLamaSharp.SemanticKernel.TextEmbedding;

public sealed class LLamaSharpEmbeddingGeneration
    : IEmbeddingGenerator, ITextEmbeddingGenerationService, IDisposable
{
    private readonly LLamaEmbedder _embedder;

    private readonly Dictionary<string, object?> _attributes = new();

    public IReadOnlyDictionary<string, object?> Attributes => _attributes;

    public LLamaSharpEmbeddingGeneration(LLamaEmbedder embedder)
    {
        _embedder = embedder;
    }

    /// <inheritdoc/>
    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        var result = new List<ReadOnlyMemory<float>>();

        foreach (var item in data)
            result.Add((await _embedder.GetEmbeddings(item, cancellationToken)).First());

        return result;
    }

    /// <inheritdoc/>
    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_embedder is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
