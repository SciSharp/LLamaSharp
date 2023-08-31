using LLama;
using Microsoft.SemanticKernel.AI.Embeddings;

namespace Microsoft.SemanticKernel.Connectors.AI.LLama.TextEmbedding;

public sealed class LLamaSharpEmbeddingGeneration : ITextEmbeddingGeneration
{
    private LLamaEmbedder _embedder;

    public LLamaSharpEmbeddingGeneration(LLamaEmbedder embedder)
    {
        _embedder = embedder;
    }

    /// <inheritdoc/>
    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
    {
        var result = data.Select(text => new ReadOnlyMemory<float>(_embedder.GetEmbeddings(text))).ToList();
        return await Task.FromResult(result).ConfigureAwait(false);
    }
}
