using System;
using LLama.Native;

namespace LLama.Sampling.Selection;

/// <summary>
/// Select from all possible tokens according to their probability
/// </summary>
public sealed class StandardSelection
    : ITokenSelector
{
    /// <inheritdoc />
    public int Select(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<int> lastTokens)
    {
        return candidates.SampleToken(ctx);
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