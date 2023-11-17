using LLama;
using Microsoft.SemanticKernel.AI.Embeddings;

namespace LLamaSharp.SemanticKernel.TextEmbedding;

public sealed class LLamaSharpEmbeddingGeneration : ITextEmbeddingGeneration
{
    private LLamaEmbedder _embedder;

    private readonly Dictionary<string, string> _attributes = new();

    public IReadOnlyDictionary<string, string> Attributes => this._attributes;

    public LLamaSharpEmbeddingGeneration(LLamaEmbedder embedder)
    {
        _embedder = embedder;
    }

    /// <inheritdoc/>
    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
    {
        var embeddings = data.Select(text => new ReadOnlyMemory<float>(_embedder.GetEmbeddings(text))).ToList();
        return await Task.FromResult(embeddings);
    }
}
