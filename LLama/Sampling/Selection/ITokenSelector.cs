using System;
using LLama.Native;

namespace LLama.Sampling.Selection;

/// <summary>
/// Select a single token from a set of possibilities
/// </summary>
public interface ITokenSelector
    : IDisposable
{
    /// <summary>
    /// Select a single token
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="candidates"></param>
    /// <param name="lastTokens"></param>
    /// <returns></returns>
    int Select(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<int> lastTokens);

    /// <summary>
    /// Reset the state
    /// </summary>
    void Reset();
}