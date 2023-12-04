using System;
using LLama.Native;

namespace LLama.Sampling.Tokens;

/// <summary>
/// Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
/// </summary>
public sealed class LocallyTypicalSampling
    : ITokenDataProcessor
{
    /// <summary>
    /// P value for locally typical sampling
    /// </summary>
    public float P { get; set; }

    /// <summary>
    /// Minimum number of tokens to keep
    /// </summary>
    public ulong MinKeep { get; set; } = 1;

    /// <inheritdoc />
    public void ProcessTokens(SafeLLamaContextHandle ctx, LLamaTokenDataArray tokens, ReadOnlySpan<int> lastTokens)
    {
        tokens.LocallyTypical(ctx, P, MinKeep);
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