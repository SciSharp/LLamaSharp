using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LLama.Native;
using LLama.Sampling.Logits;
using LLama.Sampling.Selection;
using LLama.Sampling.Tokens;

namespace LLama.Sampling;

/// <summary>
/// Convert a span of logits into a single sampled token
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

/// <summary>
/// Simple implementation of `ISamplingPipeline`, applies processors in order every time
/// </summary>
public sealed class ConfigurableSamplingPipeline
    : ISamplingPipeline
{
    /// <summary>
    /// Logit processors to apply in this pipeline
    /// </summary>
    public IList<ILogitProcessor> LogitProcessors { get; } = new List<ILogitProcessor>();

    /// <summary>
    /// Logits values which will not be changed by the logit processors
    /// </summary>
    public IList<int> ProtectedLogits { get; } = new List<int>();

    /// <summary>
    /// Token data processors to apply in this pipeline
    /// </summary>
    public IList<ITokenDataProcessor> TokenDataProcessors { get; } = new List<ITokenDataProcessor>();

    /// <summary>
    /// The selector to choose the final token
    /// </summary>
    public ITokenSelector Selector { get; set; } = new StandardSelection();

    /// <inheritdoc />
    public int Sample(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<int> lastTokens)
    {
        var savedLogitsCount = ProtectedLogits.Count;
        var savedLogitValues = ArrayPool<float>.Shared.Rent(savedLogitsCount);
        var savedLogitIndices = ArrayPool<int>.Shared.Rent(savedLogitsCount);
        try
        {
            // Save the values of protected logits
            for (var i = 0; i < ProtectedLogits.Count; i++)
            {
                savedLogitValues[i] = logits[ProtectedLogits[i]];
                savedLogitIndices[i] = ProtectedLogits[i];
            }

            // Modify raw logits
            foreach (var logitProcessor in LogitProcessors)
                logitProcessor.ProcessLogits(ctx, logits, lastTokens);

            // Restore the values of protected logits
            for (var i = 0; i < savedLogitsCount; i++)
                logits[savedLogitIndices[i]] = savedLogitValues[i];
        }
        finally
        {
            ArrayPool<float>.Shared.Return(savedLogitValues);
            ArrayPool<int>.Shared.Return(savedLogitIndices);
        }

        // Convert logits into token candidates
        var candidates_p = LLamaTokenDataArray.Create(logits);

        // Process token candidates
        foreach (var tokenDataProcessor in TokenDataProcessors)
            tokenDataProcessor.ProcessTokens(ctx, candidates_p, lastTokens);

        // Select a token
        var token = Selector.Select(ctx, candidates_p, lastTokens);

        // Tell processors what was selected
        foreach (var logitProcessor in LogitProcessors)
            logitProcessor.AcceptToken(ctx, token);
        foreach (var tokenDataProcessor in TokenDataProcessors)
            tokenDataProcessor.AcceptToken(ctx, token);

        return token;
    }

    /// <inheritdoc />
    public void Reset()
    {
        foreach (var logitProcessor in LogitProcessors)
            logitProcessor.Reset();
        foreach (var tokenDataProcessor in TokenDataProcessors)
            tokenDataProcessor.Reset();

        Selector.Reset();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var logitProcessor in LogitProcessors)
            logitProcessor.Dispose();
        foreach (var tokenDataProcessor in TokenDataProcessors)
            tokenDataProcessor.Dispose();

        Selector.Dispose();
    }
}