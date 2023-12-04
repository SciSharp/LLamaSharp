using System;
using LLama.Native;

namespace LLama.Sampling.Tokens;

/// <summary>
/// Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
/// </summary>
public sealed class TopPSampling
    : ITokenDataProcessor
{
    /// <summary>
    /// P valies for TopP
    /// </summary>
    public float P { get; set; }

    /// <summary>
    /// Minimum number of tokens to keep
    /// </summary>
    public ulong MinKeep { get; set; } = 1;

    /// <inheritdoc />
    public void ProcessTokens(SafeLLamaContextHandle ctx, LLamaTokenDataArray tokens, ReadOnlySpan<int> lastTokens)
    {
        tokens.TopP(ctx, P, MinKeep);
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