using System;
using LLama.Native;

namespace LLama.Sampling.Selection;

/// <summary>
/// Select a token using Mirostat sampling.
/// Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966.
/// </summary>
public sealed class MirostatSelection
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
    /// The number of tokens considered in the estimation of `s_hat`. This is an arbitrary value that is used to calculate `s_hat`, which in turn
    /// helps to calculate the value of `k`. In the paper, they use `m = 100`, but you can experiment with different values to see how it affects
    /// the performance of the algorithm.
    /// </summary>
    public int M { get; set; }

    /// <summary>
    /// Create a new Mirostat 2.0 sampler
    /// </summary>
    /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text.
    /// A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text</param>
    /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word.
    /// A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
    /// <param name="m">The number of tokens considered in the estimation of `s_hat`. This is an arbitrary value that is used to calculate `s_hat`, which in turn
    /// helps to calculate the value of `k`. In the paper, they use `m = 100`, but you can experiment with different values to see how it affects
    /// the performance of the algorithm.</param>
    public MirostatSelection(float tau, float eta, int m = 100)
    {
        Tau = tau;
        Eta = eta;
        M = m;
    }

    /// <inheritdoc />
    public int Select(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<int> lastTokens)
    {
        return candidates.SampleTokenMirostat(ctx, Tau, Eta, M, ref _mu);
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