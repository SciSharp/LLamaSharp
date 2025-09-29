using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;

namespace LLama;

public partial class LLamaEmbedder
    : IEmbeddingGenerator<string, Embedding<float>>
{
    private EmbeddingGeneratorMetadata? _metadata;

    /// <inheritdoc />
    object? IEmbeddingGenerator.GetService(Type serviceType, object? serviceKey)
    {
        if (serviceKey is not null)
        {
            return null;
        }
        
        if (_hasExternalContext && serviceType == typeof(EmbeddingGeneratorMetadata))
        {
            return _metadata ??= new(
                nameof(LLamaEmbedder),
                defaultModelId: Context.NativeHandle.ModelHandle.ReadMetadata().TryGetValue("general.name", out var name) ? name : null,
                defaultModelDimensions: EmbeddingSize);
        }

        if (_hasExternalContext && serviceType?.IsInstanceOfType(Context) is true)
        {
            return Context;
        }

        if (serviceType?.IsInstanceOfType(this) is true)
        {
            return this;
        }

        return null;
    }

    /// <inheritdoc />
    async Task<GeneratedEmbeddings<Embedding<float>>> IEmbeddingGenerator<string, Embedding<float>>.GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options, CancellationToken cancellationToken)
    {
        GeneratedEmbeddings<Embedding<float>> results = new() 
        {
            Usage = new() { InputTokenCount = 0 },
        };
        
        foreach (var value in values)
        {
            var (embeddings, tokenCount) = await GetEmbeddingsWithTokenCount(value, cancellationToken).ConfigureAwait(false);
            Debug.Assert(embeddings.Count == 1, "Should be one and only one embedding returned from LLama for a single input string.");

            results.Usage.InputTokenCount += tokenCount;
            results.Add(new Embedding<float>(embeddings[0]) { CreatedAt = DateTime.UtcNow });
        }

        results.Usage.TotalTokenCount = results.Usage.InputTokenCount;

        return results;
    }
}