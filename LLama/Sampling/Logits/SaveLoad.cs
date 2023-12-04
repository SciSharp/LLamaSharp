using System;
using System.Collections.Generic;
using LLama.Native;

namespace LLama.Sampling.Logits;

/// <summary>
/// Save certain logit values
/// </summary>
public sealed class SaveLogitValues
    : ILogitProcessor
{
    private readonly Dictionary<int, float> _saved = new();

    /// <summary>
    /// Logits to save
    /// </summary>
    public ISet<int> Logits { get; } = new HashSet<int>();

    /// <summary>
    /// Saved logit values
    /// </summary>
    public IReadOnlyDictionary<int, float> Values => _saved;

    /// <inheritdoc />
    public void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<int> lastTokens)
    {
        _saved.Clear();
        foreach (var logit in Logits)
            _saved[logit] = logits[logit];
    }

    /// <inheritdoc />
    public void AcceptToken(SafeLLamaContextHandle ctx, int token)
    {
    }

    /// <inheritdoc />
    public void Reset()
    {
        _saved.Clear();
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }

    /// <summary>
    /// Get a logit processor that overwrite the logit values with the values saved here
    /// </summary>
    /// <returns></returns>
    public ILogitProcessor GetWriter()
    {
        return new LoadLogitValues(_saved);
    }
}

/// <summary>
/// Overwrite certain logit values
/// </summary>
public sealed class LoadLogitValues
    : ILogitProcessor
{
    /// <summary>
    /// Logits to overwrite, token -> logit
    /// </summary>
    public IDictionary<int, float> Values { get; }

    /// <summary>
    /// Create a new LoadLogitValues
    /// </summary>
    /// <param name="values">Source for values to overwrite</param>
    public LoadLogitValues(Dictionary<int, float>? values = null)
    {
        Values = values ?? new Dictionary<int, float>();
    }

    /// <inheritdoc />
    public void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<int> lastTokens)
    {
        foreach (var logit in Values)
            logits[logit.Key] = logit.Value;
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