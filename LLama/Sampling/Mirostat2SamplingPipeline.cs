using System;
using LLama.Native;

namespace LLama.Sampling;

/// <summary>
/// A sampling pipeline which uses mirostat (v2) to select tokens
/// </summary>
public class Mirostat2SamplingPipeline
    : BaseSamplingPipeline
{
    private const float DEFAULT_TAU = 5;

    private float _mu = DEFAULT_TAU * 2;
    /// <summary>
    /// Currently learned mu value
    /// </summary>
    public float Mu => _mu;

    private float _tau = DEFAULT_TAU;
    /// <summary>
    /// target entropy
    /// </summary>
    public float Tau
    {
        get => _tau;
        set
        {
            _tau = value;
            _mu = value * 2;
        }
    }

    /// <summary>
    /// learning rate
    /// </summary>
    public float Eta { get; set; } = 0.1f;

    /// <inheritdoc />
    protected override void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
    {
    }

    /// <inheritdoc />
    protected override LLamaToken ProcessTokenDataArray(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<LLamaToken> lastTokens)
    {
        return candidates.SampleTokenMirostat2(ctx, Tau, Eta, ref _mu);
    }

    /// <inheritdoc />
    public override void Reset()
    {
        base.Reset();

        _mu = Tau * 2;
    }

    /// <inheritdoc />
    public override ISamplingPipeline Clone()
    {
        return new Mirostat2SamplingPipeline
        {
            Grammar = Grammar?.Clone(),

            _mu = _mu,
            _tau = _tau,
            Eta = Eta
        };
    }
}