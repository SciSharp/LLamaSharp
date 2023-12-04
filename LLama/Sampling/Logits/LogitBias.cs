using System;
using System.Collections.Generic;
using LLama.Native;

namespace LLama.Sampling.Logits;

/// <summary>
/// Add a bias directly to logit values
/// </summary>
public sealed class LogitBias
    : ILogitProcessor
{
    /// <summary>
    /// Biases to apply, token -> bias
    /// </summary>
    public IDictionary<int, float> Biases { get; } = new Dictionary<int, float>();

    /// <inheritdoc />
    public void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<int> lastTokens)
    {
        foreach (var kvp in Biases)
            logits[kvp.Key] += kvp.Value;
    }

    /// <inheritdoc />
    public void AcceptToken(SafeLLamaContextHandle ctx, int token)
    {
    }

    /// <inheritdoc />
    public void Reset()
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}