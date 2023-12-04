using System;
using LLama.Native;

namespace LLama.Sampling.Tokens;

/// <summary>
/// Minimum P sampling as described in https://github.com/ggerganov/llama.cpp/pull/3841
/// </summary>
public sealed class MinPSampling
    : ITokenDataProcessor
{
    /// <summary>
    /// All tokens with probability greater than this will be kept
    /// </summary>
    public float P { get; set; }

    /// <summary>
    /// Minimum number of tokens to keep
    /// </summary>
    public ulong MinKeep { get; set; } = 1;

    /// <inheritdoc />
    public void ProcessTokens(SafeLLamaContextHandle ctx, LLamaTokenDataArray tokens, ReadOnlySpan<int> lastTokens)
    {
        tokens.MinP(ctx, P, MinKeep);
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