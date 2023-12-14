using LLama;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

namespace LLamaSharp.SemanticKernel.TextEmbedding;

public sealed class LLamaSharpEmbeddingGeneration : ITextEmbeddingGenerationService
{
    private LLamaEmbedder _embedder;

    private readonly Dictionary<string, object?> _attributes = new();

    public IReadOnlyDictionary<string, object?> Attributes => this._attributes;

    public LLamaSharpEmbeddingGeneration(LLamaEmbedder embedder)
    {
        _embedder = embedder;
    }

    /// <inheritdoc/>
    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, Kernel? kernel = null, CancellationToken cancellationToken = default)
    {
        var embeddings = data.Select(text => new ReadOnlyMemory<float>(_embedder.GetEmbeddings(text))).ToList();
        return await Task.FromResult(embeddings);
    }
}
