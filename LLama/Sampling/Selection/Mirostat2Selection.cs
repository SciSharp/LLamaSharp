using System;
using LLama.Native;

namespace LLama.Sampling.Selection;

/// <summary>
/// Select a token using Mirostat sampling.
/// Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966.
/// </summary>
public sealed class Mirostat2Selection
    : ITokenSelector
{
    private float _mu;

    /// <summary>
    /// Current value of Mu, updated based on the difference between target surprise and actual surprise
    /// </summary>
    public float Mu
    {
        get => _mu;
        set => _mu = value;
    }

    /// <summary>
    /// The target cross-entropy (or surprise) value you want to achieve for the generated text.
    /// A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text
    /// </summary>
    public float Tau { get; set; }

    /// <summary>
    /// The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word.
    /// A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.
    /// </summary>
    public float Eta { get; set; }

    /// <summary>
    /// Create a new Mirostat 2.0 sampler
    /// </summary>
    /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text.
    /// A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text</param>
    /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word.
    /// A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
    public Mirostat2Selection(float tau, float eta)
    {
        Tau = tau;
        Eta = eta;
    }

    /// <inheritdoc />
    public int Select(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<int> lastTokens)
    {
        return candidates.SampleTokenMirostat2(ctx, Tau, Eta, ref _mu);
    }

    /// <inheritdoc />
    public void Reset()
    {
        _mu = 2 * Tau;
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}