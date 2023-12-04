using System;
using LLama.Native;

namespace LLama.Sampling.Tokens;

using llama_token = Int32;

/// <summary>
/// Processes token logits before sampling, applying penalties to certain tokens
/// </summary>
public interface ITokenDataProcessor
    : IDisposable
{
    /// <summary>
    /// Process token logits in a LLamaTokenDataArray
    /// </summary>
    /// <param name="ctx">The context this is operating in</param>
    /// <param name="tokens">The token data array to process</param>
    /// <param name="lastTokens">The most recent tokens output</param>
    /// <returns>LLamaTokenDataArray, created from logits</returns>
    void ProcessTokens(SafeLLamaContextHandle ctx, LLamaTokenDataArray tokens, ReadOnlySpan<llama_token> lastTokens);

    /// <summary>
    /// Inform this process when a token is accepted by the model
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="token"></param>
    void AcceptToken(SafeLLamaContextHandle ctx, int token);

    /// <summary>
    /// Reset all internal sampling state
    /// </summary>
    void Reset();
}