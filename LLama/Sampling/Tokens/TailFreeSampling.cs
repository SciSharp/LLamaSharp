using System;
using LLama.Native;

namespace LLama.Sampling.Tokens;

/// <summary>
/// Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.
/// </summary>
public sealed class TailFreeSampling
    : ITokenDataProcessor
{
    /// <summary>
    /// Z value for tail free sampling
    /// </summary>
    public float Z { get; set; }

    /// <summary>
    /// Minimum number of tokens to keep
    /// </summary>
    public ulong MinKeep { get; set; } = 1;

    /// <inheritdoc />
    public void ProcessTokens(SafeLLamaContextHandle ctx, LLamaTokenDataArray tokens, ReadOnlySpan<int> lastTokens)
    {
        tokens.TailFree(ctx, Z, MinKeep);
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