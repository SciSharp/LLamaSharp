using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LLama.Native;
using Microsoft.Extensions.AI;

namespace LLama;

public partial class LLamaEmbedder
    : IEmbeddingGenerator<string, Embedding<float>>
{
    private EmbeddingGeneratorMetadata? _metadata;

    /// <inheritdoc />
    object? IEmbeddingGenerator.GetService(Type serviceType, object? serviceKey)
    {
        if (serviceKey is null)
        {
            if (serviceType == typeof(EmbeddingGeneratorMetadata))
            {
                return _metadata ??= new(
                    nameof(LLamaEmbedder),
                    defaultModelId: Context.NativeHandle.ModelHandle.ReadMetadata().TryGetValue("general.name", out var name) ? name : null,
                    defaultModelDimensions: EmbeddingSize);
            }

            if (serviceType?.IsInstanceOfType(Context) is true)
            {
                return Context;
            }

            if (serviceType?.IsInstanceOfType(this) is true)
            {
                return this;
            }
        }

        return null;
    }

    /// <inheritdoc />
    async Task<GeneratedEmbeddings<Embedding<float>>> IEmbeddingGenerator<string, Embedding<float>>.GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options, CancellationToken cancellationToken)
    {
        if (Context.NativeHandle.PoolingType == LLamaPoolingType.None)
        {
            throw new NotSupportedException($"Embedding generation is not supported with {nameof(LLamaPoolingType)}.{nameof(LLamaPoolingType.None)}.");
        }

        GeneratedEmbeddings<Embedding<float>> results = new() 
        {
            Usage = new() { InputTokenCount = 0 },
        };
        
        foreach (var value in values)
        {
            var (embeddings, tokenCount) = await GetEmbeddingsWithTokenCount(value, cancellationToken).ConfigureAwait(false);
            Debug.Assert(embeddings.Count == 1, "Should be one and only one embedding when pooling is enabled.");

            results.Usage.InputTokenCount += tokenCount;
            results.Add(new Embedding<float>(embeddings[0]) { CreatedAt = DateTime.UtcNow });
        }

        results.Usage.TotalTokenCount = results.Usage.InputTokenCount;

        return results;
    }
}