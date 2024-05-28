using System;
using LLama.Native;

namespace LLama.Sampling;

/// <summary>
/// Base class for implementing custom sampling pipelines. This provides a helpful framework for implementing `ISamplingPipeline`.
/// </summary>
public abstract class BaseSamplingPipeline
    : ISamplingPipeline
{
    /// <summary>
    /// Grammar to constrain valid tokens
    /// </summary>
    public SafeLLamaGrammarHandle? Grammar { get; set; }
    
    private LLamaTokenData[]? _temporarySampling;

    /// <inheritdoc/>
    public LLamaToken Sample(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
    {
        // Apply processing to raw logit values
        ProcessLogits(ctx, logits, lastTokens);

        // Allocate some temporary space
        if (_temporarySampling == null || _temporarySampling.Length < logits.Length)
            _temporarySampling = new LLamaTokenData[logits.Length];

        // Process token data array to select a final token
        var candidates = LLamaTokenDataArray.Create(logits, _temporarySampling);
        candidates.ApplyGrammar(ctx, Grammar);
        return ProcessTokenDataArray(ctx, candidates, lastTokens);
    }

    /// <inheritdoc />
    public virtual void Accept(SafeLLamaContextHandle ctx, LLamaToken token)
    {
        Grammar?.AcceptToken(ctx, token);
    }

    /// <summary>
    /// Process the raw logit values
    /// </summary>
    /// <param name="ctx">The context being sampled from</param>
    /// <param name="logits">The logits produced by the model</param>
    /// <param name="lastTokens">A list of tokens recently returned by the model</param>
    protected abstract void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens);

    /// <summary>
    /// Process the LLamaTokenDataArray and select a single token
    /// </summary>
    /// <param name="ctx">The context being sampled from</param>
    /// <param name="candidates">The LLamaTokenDataArray data produced by the model</param>
    /// <param name="lastTokens">A list of tokens recently returned by the model</param>
    /// <returns></returns>
    protected abstract LLamaToken ProcessTokenDataArray(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<LLamaToken> lastTokens);

    /// <inheritdoc/>
    public virtual void Reset()
    {
    }

    /// <inheritdoc />
    public abstract ISamplingPipeline Clone();

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}