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
    protected override void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
    {
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