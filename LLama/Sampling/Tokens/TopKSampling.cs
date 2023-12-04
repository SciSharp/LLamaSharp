using System;
using LLama.Native;

namespace LLama.Sampling.Tokens;

/// <summary>
/// Sample with TopK, removing all by the K most likely tokens.
/// Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
/// </summary>
public sealed class TopKSampling
    : ITokenDataProcessor
{
    /// <summary>
    /// Number of tokens to keep
    /// </summary>
    public int Count { get; set; }

    /// <inheritdoc />
    public void ProcessTokens(SafeLLamaContextHandle ctx, LLamaTokenDataArray tokens, ReadOnlySpan<int> lastTokens)
    {
        tokens.TopK(ctx, Count);
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