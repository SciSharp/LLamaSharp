using System;
using System.Collections.Generic;
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
    /// <param name="ctx"></param>
    /// <param name="logits"></param>
    /// <param name="lastTokens"></param>
    /// <returns></returns>
    int Sample(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<int> lastTokens);

    /// <summary>
    /// Reset all internal state of the sampling pipeline
    /// </summary>
    void Reset();
}

/// <summary>
/// Simple implementation of `ISamplingPipeline`, applies processors in order every time
/// </summary>
public sealed class BasicSamplingPipeline
    : ISamplingPipeline
{
    /// <summary>
    /// Logit processors to apply in this pipeline
    /// </summary>
    public IList<ILogitProcessor> LogitProcessors { get; } = new List<ILogitProcessor>();

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
        // Modify raw logits
        foreach (var logitProcessor in LogitProcessors)
            logitProcessor.ProcessLogits(ctx, logits, lastTokens);

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