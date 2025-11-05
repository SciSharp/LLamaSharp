using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LLama.Abstractions;
using LLama.Exceptions;
using LLama.Native;
using Microsoft.Extensions.Logging;

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
    public int EmbeddingSize { get; private set; }

    /// <summary>
    /// LLama Context
    /// </summary>
    /// <remarks>
    /// If the context was not provided externally, the returned context will be in a disposed state.
    /// </remarks>
    public LLamaContext Context { get; private set; }

    private readonly LLamaWeights? _weights;
    private readonly IContextParams _params;
    private readonly ILogger? _logger;
    private readonly bool _hasExternalContext;
    private readonly LLamaSeqIdManager? _lamaSeqIdManager;

    /// <summary>
    /// Create a new embedder, using the given <see cref="LLamaWeights"/>.
    /// This will create and dispose a new <see cref="LLamaContext"/> for each embedding request.
    /// If you want to manage the context lifetime yourself, consider using the other constructor that takes a <see cref="LLamaContext"/>.
    /// </summary>
    /// <param name="weights">weights to use for generating embeddings. The weights must be for a model that supports embeddings (i.e. it must have an encoder or a decoder, but not both).</param>
    /// <param name="params">context parameters to use when creating the context</param>
    /// <param name="logger">optional logger</param>
    /// <exception cref="ArgumentException">raised if the provided context has batch size different from ubatch size</exception>
    /// <exception cref="NotSupportedException">raised if the provided context is for an encoder-decoder model</exception>
    public LLamaEmbedder(LLamaWeights weights, IContextParams @params, ILogger? logger = null)
    {
        if (@params.UBatchSize != @params.BatchSize)
            throw new ArgumentException("For non-causal models, batch size must be equal to ubatch size", nameof(@params));
        if (weights.NativeHandle is { HasEncoder: true, HasDecoder: true })
            throw new NotSupportedException("Computing embeddings in encoder-decoder models is not supported");

        Context = weights.CreateContext(@params, logger);
        EmbeddingSize = Context.EmbeddingSize;
        Context.Dispose();
        _weights = weights;
        _params = @params;
        _logger = logger;
        _hasExternalContext = false;
        _lamaSeqIdManager = null;
    }

    /// <summary>
    /// Creates a new embedder using the given <see cref="LLamaContext"/>.
    /// The caller is responsible for managing the lifetime of the context, and must ensure that the context remains valid
    /// for the entire lifetime of this <see cref="LLamaEmbedder"/>. The context will not be disposed when this embedder is disposed.
    /// </summary>
    /// <param name="context">context to use for generating embeddings. The context must be configured with a model that supports embeddings (i.e. it must have an encoder or a decoder, but not both).</param>
    /// <param name="logger">optional logger</param>
    /// <exception cref="ArgumentException">raised if the provided context has batch size different from ubatch size</exception>
    /// <exception cref="NotSupportedException">raised if the provided context is for an encoder-decoder model</exception>
    public LLamaEmbedder(LLamaContext context, ILogger? logger = null)
    {
        if (context.Params.UBatchSize != context.Params.BatchSize)
            throw new ArgumentException("For non-causal models, batch size must be equal to ubatch size", nameof(context));

        if (context.NativeHandle.ModelHandle is { HasEncoder: true, HasDecoder: true })
            throw new NotSupportedException("Computing embeddings in encoder-decoder models is not supported");

        Context = context;
        EmbeddingSize = Context.EmbeddingSize;
        NativeApi.llama_set_embeddings(Context.NativeHandle, true);
        _params = context.Params;
        _logger = logger;
        _hasExternalContext = true;
        _lamaSeqIdManager = new LLamaSeqIdManager(context.Params.SeqMax);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_hasExternalContext && !Context.NativeHandle.IsClosed)
            Context.Dispose();
        _lamaSeqIdManager?.Dispose();
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
        if (!_hasExternalContext)
        {
            if (!Context.NativeHandle.IsClosed)
                Context.Dispose();

            Context = _weights!.CreateContext(_params, _logger);
            NativeApi.llama_set_embeddings(Context.NativeHandle, true);
        }

        using var seqId = await Context.AcquireSequenceIdAsync(removeMemoryOnRelease: true, cancellationToken: cancellationToken);
        // Add all the tokens to the batch
        var tokens = Context.Tokenize(input, special: true);
        if (tokens.Length > Context.ContextSize)
            throw new ArgumentException(
                $"Embedding prompt is longer than the context window ({tokens.Length} > {Context.ContextSize})",
                nameof(input));

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
            batch.AddRange(tokens.AsSpan(i, n_eval), n_past, seqId, true);
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
            results.Add(Context.NativeHandle.GetEmbeddingsSeq(seqId).ToArray());
        }

        // Normalize the embeddings vector
        // https://github.com/ggerganov/llama.cpp/blob/2891c8aa9af17f4ff636ff3868bc34ff72b56e25/examples/embedding/embedding.cpp#L92
        foreach (var embedding in results)
        {
            embedding.EuclideanNormalization();
        }

        if (!_hasExternalContext)
            Context.Dispose();

        return (results, tokens.Length);
    }
}