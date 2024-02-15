using System;
using System.Buffers;
using System.Collections.Generic;
using LLama.Native;

namespace LLama.Sampling;

/// <summary>
/// Base class for implementing custom sampling pipelines. This provides a helpful framework for implementing `ISamplingPipeline`.
/// </summary>
public abstract class BaseSamplingPipeline
    : ISamplingPipeline
{
    private int _savedLogitsCount;
    private (LLamaToken index, float logit)[]? _savedLogits;

    /// <inheritdoc/>
    public LLamaToken Sample(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
    {
        var protectedLogits = GetProtectedTokens(ctx);
        _savedLogitsCount = protectedLogits.Count;
        _savedLogits = ArrayPool<(LLamaToken, float)>.Shared.Rent(_savedLogitsCount);
        try
        {
            // Save the values of protected logits
            for (var i = 0; i < protectedLogits.Count; i++)
            {
                var index = protectedLogits[i];
                var value = logits[(int)index];
                _savedLogits[i] = (index, value);
            }

            // Process raw logits
            ProcessLogits(ctx, logits, lastTokens);

            // Automatically restore saved logit values after processing
            RestoreProtectedTokens(logits);

            // Convert logits into token candidates
            var candidates = LLamaTokenDataArray.Create(logits);

            // Process token data array
            return ProcessTokenDataArray(ctx, candidates, lastTokens);
        }
        finally
        {
            ArrayPool<(LLamaToken, float)>.Shared.Return(_savedLogits);
            _savedLogits = null;
            _savedLogitsCount = 0;
        }
    }

    /// <inheritdoc />
    public abstract void Accept(SafeLLamaContextHandle ctx, LLamaToken token);

    #region protected tokens
    /// <summary>
    /// Get all of the "protected" tokens that cannot be changed by ProcessLogits
    /// </summary>
    /// <returns></returns>
    protected abstract IReadOnlyList<LLamaToken> GetProtectedTokens(SafeLLamaContextHandle ctx);

    /// <summary>
    /// Restore the value of the "protected" tokens which were saved before the call to ProcessLogits
    /// </summary>
    /// <param name="logits"></param>
    protected void RestoreProtectedTokens(Span<float> logits)
    {
        if (_savedLogits == null)
            return;

        // The array may be bigger than necessary, get a span of the valid bit
        var saved = _savedLogits.AsSpan(0, _savedLogitsCount);

        // Restore the values of protected logits
        for (var i = 0; i < saved.Length; i++)
            logits[(int)saved[i].index] = saved[i].logit;
    }

    /// <summary>
    /// Restore the value of the "protected" tokens which were saved before the call to ProcessLogits
    /// </summary>
    /// <param name="candidates"></param>
    protected void RestoreProtectedTokens(LLamaTokenDataArray candidates)
    {
        if (_savedLogits == null || _savedLogits.Length == 0)
            return;

        candidates.OverwriteLogits(_savedLogits.AsSpan(0, _savedLogitsCount));
    }
    #endregion

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