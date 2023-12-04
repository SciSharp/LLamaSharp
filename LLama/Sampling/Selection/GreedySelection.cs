using System;
using LLama.Native;

namespace LLama.Sampling.Selection;

/// <summary>
/// Select the most likely token
/// </summary>
public sealed class GreedySelection
    : ITokenSelector
{
    /// <inheritdoc />
    public int Select(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<int> lastTokens)
    {
        return candidates.SampleTokenGreedy(ctx);
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