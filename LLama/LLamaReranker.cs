using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using LLama.Abstractions;
using LLama.Exceptions;
using LLama.Native;
using Microsoft.Extensions.Logging;

namespace LLama;

/// <summary>
/// Get rank scores between prompt and documents 
/// </summary>
public sealed partial class LLamaReranker
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
    /// Create a new reranker, using the given LLamaWeights
    /// </summary>
    /// <param name="weights"></param>
    /// <param name="params"></param>
    /// <param name="logger"></param>
    public LLamaReranker(LLamaWeights weights, IContextParams @params, ILogger? logger = null)
    {
        if (@params.UBatchSize != @params.BatchSize)
            throw new ArgumentException("For non-causal models, batch size must be equal to ubatch size", nameof(@params));
        if (weights.NativeHandle is { HasEncoder: true, HasDecoder: true })
            throw new NotSupportedException("Computing rank in encoder-decoder models is not supported");
        if (@params.PoolingType != LLamaPoolingType.Rank)
            throw new NotSupportedException("Computing rank score, PoolingType must be equal to LLamaPoolingType.Rank");
        Context = weights.CreateContext(@params, logger);
        NativeApi.llama_set_embeddings(Context.NativeHandle, true);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Context.Dispose();
    }

    /// <summary>
    /// Retrieve relevance scores for input and documents by reranking, execute once.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="documents"></param>
    /// <param name="normalize">Whether to normalize the score to the range (0, 1)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="RuntimeError"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public async Task<IReadOnlyList<float>> GetRelevanceScores(string input, IReadOnlyList<string> documents, bool normalize = false, CancellationToken cancellationToken = default)
    {
        List<float> scores = new List<float>(documents.Count);
        var inputTokens = Context.Tokenize(input);
        var batch = new LLamaBatch();
        var clearFlag = 0;

        for(var idx = 0; idx < documents.Count; idx++)
        {
            var docTokens = Context.Tokenize(documents[idx] ?? "");
            LLamaToken[] tokens = [.. inputTokens, .. docTokens];

            if (batch.TokenCount + tokens.Length > Context.ContextSize)
            {
                scores.AddRange(await CalcRelevanceScores(batch, normalize, cancellationToken));
                batch.Clear();
                clearFlag = idx;
            }

            for (var i = 0; i < tokens.Length; i++)
                batch.Add(tokens[i], i, (LLamaSeqId)(idx - clearFlag), true);
        }
        if (batch.LogitPositionCount > 0)
        {
            scores.AddRange(await CalcRelevanceScores(batch, normalize, cancellationToken));
            batch.Clear();
        }

        return scores;
    }

    /// <summary>
    /// Retrieve relevance score for input and document by reranking
    /// </summary>
    /// <param name="input"></param>
    /// <param name="document"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="normalize">Whether to normalize the score to the range (0, 1)</param>
    /// <returns></returns>
    /// <exception cref="RuntimeError"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public async Task<(float Score, int Tokens)> GetRelevanceScoreWithTokenCount(string input, string document, bool normalize = false, CancellationToken cancellationToken = default)
    {
        var inputTokens = Context.Tokenize(input);
        var docTokens = Context.Tokenize(document);
        LLamaToken[] tokens = [..inputTokens, ..docTokens];
        var batch = new LLamaBatch();
        for (var i = 0; i < tokens.Length; i++)
            batch.Add(tokens[i], i, LLamaSeqId.Zero, true);

        // clear previous kv_cache values
        Context.NativeHandle.MemoryClear();

        // Check if we should cancel the work, just before doing anything expensive (encode/decode)
        cancellationToken.ThrowIfCancellationRequested();

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

        var score = Context.NativeHandle.GetEmbeddingsSeq(LLamaSeqId.Zero)[0];

        Context.NativeHandle.MemoryClear();

        return (normalize ? Sigmoid(score) : score, tokens.Length);
    }

    private async Task<IReadOnlyList<float>> CalcRelevanceScores(LLamaBatch batch, bool normalize = false, CancellationToken cancellationToken = default)
    {
        var (logicCap, _) = batch.GetLogitPositions()[batch.LogitPositionCount - 1];
        var seqNum = logicCap.Value + 1;
        List<float> scores = new List<float>(seqNum);
        // clear previous kv_cache values
        Context.NativeHandle.MemoryClear();

        // Check if we should cancel the work, just before doing anything expensive (encode/decode)
        cancellationToken.ThrowIfCancellationRequested();

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

        for (var seq = 0; seq < seqNum; seq++)
        {
            var score = Context.NativeHandle.GetEmbeddingsSeq((LLamaSeqId)seq)[0];
            scores.Add(normalize ? Sigmoid(score) : score);
        }

        Context.NativeHandle.MemoryClear();

        return scores;
    }

    private float Sigmoid(float x)
    {
        return (float)(1 / (1 + Math.Exp(-x)));
    }
}
