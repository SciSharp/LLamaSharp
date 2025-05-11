using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LLama.Abstractions;
using LLama.Exceptions;
using LLama.Native;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace LLama;

/// <summary>
/// Generate high dimensional embedding vectors from text
/// </summary>
public sealed partial class LLamaEmbedder
    : IDisposable
{
    /// <summary>
    /// Dimension of embedding vectors
    /// </summary>
    public int EmbeddingSize => Context.EmbeddingSize;

    /// <summary>
    /// LLama Context
    /// </summary>
    public LLamaContext Context { get; }

    /// <summary>
    /// Create a new embedder, using the given LLamaWeights
    /// </summary>
    /// <param name="weights"></param>
    /// <param name="params"></param>
    /// <param name="logger"></param>
    public LLamaEmbedder(LLamaWeights weights, IContextParams @params, ILogger? logger = null)
    {
        if (@params.UBatchSize != @params.BatchSize)
            throw new ArgumentException("For non-causal models, batch size must be equal to ubatch size", nameof(@params));
        if (weights.NativeHandle is { HasEncoder: true, HasDecoder: true })
            throw new NotSupportedException("Computing embeddings in encoder-decoder models is not supported");

        Context = weights.CreateContext(@params, logger);
        NativeApi.llama_set_embeddings(Context.NativeHandle, true);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Context.Dispose();
    }

    /// <summary>
    /// Get high dimensional embedding vectors for the given text. Depending on the pooling type used when constructing
    /// this <see cref="LLamaEmbedder"/> this may return an embedding vector per token, or one single embedding vector for the entire string.
    /// </summary>
    /// <remarks>Embedding vectors are not normalized, consider using one of the extensions in <see cref="SpanNormalizationExtensions"/>.</remarks>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="RuntimeError"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public async Task<IReadOnlyList<float[]>> GetEmbeddings(string input, CancellationToken cancellationToken = default) =>
        (await GetEmbeddingsWithTokenCount(input, cancellationToken).ConfigureAwait(false)).Embeddings;

    private async Task<(IReadOnlyList<float[]> Embeddings, int Tokens)> GetEmbeddingsWithTokenCount(string input, CancellationToken cancellationToken = default)
    {
        // Add all of the tokens to the batch
        var tokens = Context.Tokenize(input, special: true);
        if (tokens.Length > Context.ContextSize)
            throw new ArgumentException($"Embedding prompt is longer than the context window ({tokens.Length} > {Context.ContextSize})", nameof(input));

        // clear previous kv_cache values
        Context.NativeHandle.KvCacheClear();

        // Check if we should cancel the work, just before doing anything expensive (encode/decode)
        cancellationToken.ThrowIfCancellationRequested();

        // Evaluate prompt in batch-size chunks
        var n_past = 0;
        var batch = new LLamaBatch();
        var batchSize = (int)Context.Params.BatchSize;
        for (var i = 0; i < tokens.Length; i += batchSize)
        {
            var n_eval = tokens.Length - i;
            if (n_eval > batchSize)
                n_eval = batchSize;

            batch.Clear();
            batch.AddRange(tokens.AsSpan(i, n_eval), n_past, LLamaSeqId.Zero, true);
            n_past += n_eval;

            // Run model
            switch (Context.NativeHandle.ModelHandle.HasEncoder, Context.NativeHandle.ModelHandle.HasDecoder)
            {
                case (true, false):
                    {
                        var result = await Context.EncodeAsync(batch, cancellationToken);
                        if (result != EncodeResult.Ok)
                            throw new RuntimeError($"Failed to encode: {result}");
                        break;
                    }

                case (false, true):
                    {
                        var result = await Context.DecodeAsync(batch, cancellationToken);
                        if (result != DecodeResult.Ok)
                            throw new RuntimeError($"Failed to decode: {result}");
                        break;
                    }

                default:
                    throw new NotSupportedException("Unsupported model type");
            }
        }

        // Extract results
        var poolingType = Context.NativeHandle.PoolingType;
        var resultsCount = poolingType == LLamaPoolingType.None ? tokens.Length : 1;
        var results = new List<float[]>(resultsCount);

        if (poolingType == LLamaPoolingType.None)
        {
            var positions = batch.GetLogitPositions();
            foreach (var (_, pos) in positions)
                results.Add(Context.NativeHandle.GetEmbeddingsIth(pos).ToArray());
        }
        else
        {
            results.Add(Context.NativeHandle.GetEmbeddingsSeq(LLamaSeqId.Zero).ToArray());
        }

        // Normalize the embeddings vector
        // https://github.com/ggerganov/llama.cpp/blob/2891c8aa9af17f4ff636ff3868bc34ff72b56e25/examples/embedding/embedding.cpp#L92
        foreach (var embedding in results)
        {
            embedding.EuclideanNormalization();
        }

        Context.NativeHandle.KvCacheClear();

        return (results, tokens.Length);
    }
}