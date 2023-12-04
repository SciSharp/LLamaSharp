using System;
using LLama.Native;

namespace LLama.Sampling.Tokens;

/// <summary>
/// Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
/// Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.
/// </summary>
public sealed class RepetitionPenalty
    : ITokenDataProcessor
{
    private float _alphaFreq;
    private float _alphaPresence;

    /// <summary>
    /// Repetition penalty, as described in https://arxiv.org/abs/1909.05858
    /// </summary>
    public float RepeatPenalty { get; set; } = 1.1f;

    /// <summary>
    /// Frequency penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br />
    /// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text
    /// so far, decreasing the model's likelihood to repeat the same line verbatim.
    /// </summary>
    public float AlphaFrequency
    {
        get => _alphaFreq;
        set
        {
            if (value < -2)
                throw new ArgumentOutOfRangeException(nameof(value), "AlphaFrequency must be greater than -2");
            if (value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), "AlphaFrequency must be less than 2");
            _alphaFreq = value;
        }
    }

    /// <summary>
    /// Presence penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br />
    /// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the
    /// text so far, increasing the model's likelihood to talk about new topics.
    /// </summary>
    public float AlphaPresence
    {
        get => _alphaPresence;
        set
        {
            if (value < -2)
                throw new ArgumentOutOfRangeException(nameof(value), "AlphaFrequency must be greater than -2");
            if (value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), "AlphaFrequency must be less than 2");
            _alphaPresence = value;
        }
    }

    /// <inheritdoc />
    public void ProcessTokens(SafeLLamaContextHandle ctx, LLamaTokenDataArray tokens, ReadOnlySpan<int> lastTokens)
    {
        tokens.RepetitionPenalty(ctx, lastTokens, RepeatPenalty, AlphaFrequency, AlphaPresence);
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