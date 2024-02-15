using System;
using System.Collections.Generic;
using LLama.Extensions;
using LLama.Native;

namespace LLama.Sampling;

/// <summary>
/// An implementation of ISamplePipeline which mimics the default llama.cpp sampling
/// </summary>
public sealed class DefaultSamplingPipeline
    : BaseSamplingPipeline
{
    /// <summary>
    /// Bias values to add to certain logits
    /// </summary>
    public Dictionary<int, float> LogitBias { get; } = new();

    /// <summary>
    /// Grammar to constrain valid tokens
    /// </summary>
    public SafeLLamaGrammarHandle? Grammar { get; set; }

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
    private float _alphaFreq = 0.1f;

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
    private float _alphaPresence = 0.1f;

    /// <summary>
    /// Temperature to apply (higher temperature is more "creative")
    /// </summary>
    public float Temperature { get; set; } = 0.75f;

    /// <summary>
    /// Number of tokens to keep in TopK sampling
    /// </summary>
    public int TopK { get; set; }

    /// <summary>
    /// Z value for tail free sampling
    /// </summary>
    public float TailFreeZ { get; set; }

    /// <summary>
    /// P value for locally typical sampling
    /// </summary>
    public float TypicalP { get; set; }

    /// <summary>
    /// P value for TopP sampling
    /// </summary>
    public float TopP { get; set; } = 1f;

    /// <summary>
    /// P value for MinP sampling
    /// </summary>
    public float MinP { get; set; }

    /// <summary>
    /// Whether the newline value should be protected from being modified by logit bias and repeat penalty
    /// </summary>
    public bool PenalizeNewline { get; set; } = false;

    private readonly LLamaToken[] _newlineToken = new LLamaToken[1];

    /// <inheritdoc />
    protected override IReadOnlyList<LLamaToken> GetProtectedTokens(SafeLLamaContextHandle ctx)
    {
        if (PenalizeNewline)
            return Array.Empty<LLamaToken>();

        _newlineToken[0] = NativeApi.llama_token_nl(ctx.ModelHandle);
        return _newlineToken;
    }

    /// <inheritdoc />
    protected override void ProcessLogits(SafeLLamaContextHandle ctx, Span<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
    {
        foreach (var (key, value) in LogitBias)
            logits[key] += value;
    }

    /// <inheritdoc />
    protected override LLamaToken ProcessTokenDataArray(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<LLamaToken> lastTokens)
    {
        // Apply penalties to candidates
        candidates.RepetitionPenalty(ctx, lastTokens, RepeatPenalty, AlphaFrequency, AlphaPresence);

        // Restore protected tokens, so they are not affected by repetition penalties
        RestoreProtectedTokens(candidates);

        // Apply the normal llama.cpp pipeline
        candidates.ApplyGrammar(ctx, Grammar);
        candidates.TopK(ctx, TopK);
        candidates.TailFree(ctx, TailFreeZ);
        candidates.LocallyTypical(ctx, TypicalP);
        candidates.TopP(ctx, TopP);
        candidates.MinP(ctx, MinP);
        candidates.Temperature(ctx, Temperature);
        var id = candidates.SampleToken(ctx);

        Grammar?.AcceptToken(ctx, id);
        return id;
    }

    public override void Accept(SafeLLamaContextHandle ctx, LLamaToken token)
    {
        Grammar?.AcceptToken(ctx, token);
    }

    /// <inheritdoc />
    public override ISamplingPipeline Clone()
    {
        var clone = new DefaultSamplingPipeline();

        foreach (var (k, v) in LogitBias)
            clone.LogitBias.Add(k, v);

        clone.Grammar = Grammar?.Clone();
        clone.RepeatPenalty = RepeatPenalty;
        clone.AlphaFrequency = AlphaFrequency;
        clone.AlphaPresence = AlphaPresence;
        clone.Temperature = Temperature;
        clone.TopK = TopK;
        clone.TailFreeZ = TailFreeZ;
        clone.TypicalP = TypicalP;
        clone.TopP = TopP;
        clone.MinP = MinP;
        clone.PenalizeNewline = PenalizeNewline;

        return clone;
    }
}