using System;
using LLama.Native;

namespace LLama.Sampling.Tokens;

/// <summary>
/// Sample with temperature.
/// As temperature increases, the prediction becomes more diverse but also vulnerable to hallucinations -- generating tokens that are sensible but not factual
/// </summary>
public sealed class TemperatureSampling
    : ITokenDataProcessor
{
    /// <summary>
    /// Temperature value to apply
    /// </summary>
    public float Temperature { get; set; } = 0.5f;

    /// <inheritdoc />
    public void ProcessTokens(SafeLLamaContextHandle ctx, LLamaTokenDataArray tokens, ReadOnlySpan<int> lastTokens)
    {
        tokens.Temperature(ctx, Temperature);
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