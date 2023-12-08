using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LLama.Native;

namespace LLama.Sampling;

/// <summary>
/// Convert a span of logits into a single sampled token. This interface can be implemented to completely customise the sampling process.
/// </summary>
public interface ISamplingPipeline
    : IDisposable
{
    /// <summary>
    /// Sample a single token from the given logits
    /// </summary>
    /// <param name="ctx">The context being sampled from</param>
    /// <param name="logits">The logits produced by the model</param>
    /// <param name="lastTokens">A span of tokens recently returned by the model</param>
    /// <returns></returns>
    int Sample(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<int> lastTokens);

    /// <summary>
    /// Reset all internal state of the sampling pipeline
    /// </summary>
    void Reset();
}

/// <summary>
/// Extensions methods for ISamplingPipeline
/// </summary>
public static class ISamplingPipelineExtensions
{
    /// <summary>
    /// Sample a single token from the given logits
    /// </summary>
    /// <param name="pipeline"></param>
    /// <param name="ctx">The context being sampled from</param>
    /// <param name="logits">The logits produced by the model</param>
    /// <param name="lastTokens">A list of tokens recently returned by the model</param>
    /// <returns></returns>
    public static int Sample(this ISamplingPipeline pipeline, SafeLLamaContextHandle ctx, Span<float> logits, List<int> lastTokens)
    {
#if NET5_0_OR_GREATER
        var span = CollectionsMarshal.AsSpan(lastTokens);
        return pipeline.Sample(ctx, logits, span);
#else
        var copy = ArrayPool<int>.Shared.Rent(lastTokens.Count);
        try
        {
            lastTokens.CopyTo(copy);
            return pipeline.Sample(ctx, logits, copy.AsSpan(0, copy.Length));
        }
        finally
        {
            ArrayPool<int>.Shared.Return(copy);
        }
#endif
    }
}