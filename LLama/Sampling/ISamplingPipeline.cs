using System;
using LLama.Native;

namespace LLama.Sampling;

/// <summary>
/// Convert a span of logits into a single sampled token. This interface can be implemented to completely customise the sampling process.
/// </summary>
public interface ISamplingPipeline
    : IDisposable
{
    /// <summary>
    /// Sample a single token from the given context at the given position
    /// </summary>
    /// <param name="ctx">The context being sampled from</param>
    /// <param name="index">Position to sample logits from</param>
    /// <returns></returns>
    LLamaToken Sample(SafeLLamaContextHandle ctx, int index);

    /// <summary>
    /// Reset all internal state of the sampling pipeline
    /// </summary>
    void Reset();

    /// <summary>
    /// Update the pipeline, with knowledge that a particular token was just accepted
    /// </summary>
    /// <param name="token"></param>
    void Accept(LLamaToken token);
}

/// <summary>
/// Extension methods for <see cref="ISamplingPipeline"/>
/// </summary>
public static class ISamplingPipelineExtensions
{
    /// <summary>
    /// Sample a single token from the given context at the given position
    /// </summary>
    /// <param name="pipe"></param>
    /// <param name="ctx">The context being sampled from</param>
    /// <param name="index">Position to sample logits from</param>
    /// <returns></returns>
    public static LLamaToken Sample(this ISamplingPipeline pipe, LLamaContext ctx, int index)
    {
        return pipe.Sample(ctx.NativeHandle, index);
    }
}