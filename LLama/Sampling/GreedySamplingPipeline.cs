using System;
using LLama.Native;

namespace LLama.Sampling;

/// <summary>
/// A sampling pipeline which always selects the most likely token
/// </summary>
public class GreedySamplingPipeline
    : BaseSamplingPipeline
{
    /// <inheritdoc />
    protected override ReadOnlySpan<float> ProcessLogits(SafeLLamaContextHandle ctx, ReadOnlySpan<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
    {
        return logits;
    }

    /// <inheritdoc />
    protected override LLamaToken ProcessTokenDataArray(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<LLamaToken> lastTokens)
    {
        return candidates.SampleTokenGreedy(ctx);
    }

    /// <inheritdoc />
    public override ISamplingPipeline Clone()
    {
        return new GreedySamplingPipeline
        {
            Grammar = Grammar?.Clone()
        };
    }
}