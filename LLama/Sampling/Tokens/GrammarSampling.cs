using System;
using LLama.Grammars;
using LLama.Native;

namespace LLama.Sampling.Tokens;

/// <summary>
/// Apply a grammar to prevent sampling tokens which do not match the grammar
/// </summary>
public sealed class GrammarSampling
    : ITokenDataProcessor
{
    private SafeLLamaGrammarHandle? _handle;

    /// <summary>
    /// Grammar to use for sampling
    /// </summary>
    public Grammar? Grammar { get; set; }

    /// <summary>
    /// Create a new 
    /// </summary>
    /// <param name="grammar"></param>
    public GrammarSampling(Grammar grammar)
    {
        Grammar = grammar;
    }

    /// <inheritdoc />
    public void Reset()
    {
        _handle?.Dispose();
        _handle = null;
    }

    /// <inheritdoc />
    public void ProcessTokens(SafeLLamaContextHandle ctx, LLamaTokenDataArray tokens, ReadOnlySpan<int> lastTokens)
    {
        // Create a new grammar instance if necessary
        _handle ??= Grammar?.CreateInstance();

        // Apply it
        if (_handle != null)
            tokens.ApplyGrammar(ctx, _handle);
    }

    /// <inheritdoc />
    public void AcceptToken(SafeLLamaContextHandle ctx, int token)
    {
        _handle?.AcceptToken(ctx, token);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _handle?.Dispose();
        _handle = null;
    }
}