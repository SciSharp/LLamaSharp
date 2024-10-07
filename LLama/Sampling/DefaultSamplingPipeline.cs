using System;
using System.Collections.Generic;
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
    public IReadOnlyDictionary<LLamaToken, float> LogitBias { get; init; } = new Dictionary<LLamaToken, float>();

    /// <summary>
    /// Repetition penalty, as described in https://arxiv.org/abs/1909.05858
    /// </summary>
    public float RepeatPenalty { get; init; } = 1;

    /// <summary>
    /// Frequency penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br />
    /// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text
    /// so far, decreasing the model's likelihood to repeat the same line verbatim.
    /// </summary>
    public float AlphaFrequency
    {
        get => _alphaFreq;
        init
        {
            if (value < -2)
                throw new ArgumentOutOfRangeException(nameof(value), "AlphaFrequency must be greater than -2");
            if (value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), "AlphaFrequency must be less than 2");
            _alphaFreq = value;
        }
    }
    private readonly float _alphaFreq;

    /// <summary>
    /// Presence penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br />
    /// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the
    /// text so far, increasing the model's likelihood to talk about new topics.
    /// </summary>
    public float AlphaPresence
    {
        get => _alphaPresence;
        init
        {
            if (value < -2)
                throw new ArgumentOutOfRangeException(nameof(value), "AlphaFrequency must be greater than -2");
            if (value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), "AlphaFrequency must be less than 2");
            _alphaPresence = value;
        }
    }
    private readonly float _alphaPresence;

    /// <summary>
    /// How many tokens should be considered for penalizing repetition
    /// </summary>
    public int RepeatPenaltyCount { get; init; } = 64;

    /// <summary>
    /// Whether the newline token should be protected from being modified by penalty
    /// </summary>
    public bool PenalizeNewline { get; init; } = false;

    /// <summary>
    /// Whether the EOS token should be protected from being modified by penalty
    /// </summary>
    public bool PenalizeEOS { get; init; } = false;

    /// <summary>
    /// Temperature to apply (higher temperature is more "creative")
    /// </summary>
    public float Temperature { get; init; } = 0.75f;

    /// <summary>
    /// Number of tokens to keep in TopK sampling
    /// </summary>
    public int TopK { get; init; } = 40;

    /// <summary>
    /// Z value for tail free sampling
    /// </summary>
    public float TailFreeZ { get; init; } = 1;

    /// <summary>
    /// P value for locally typical sampling
    /// </summary>
    public float TypicalP { get; init; } = 1;

    /// <summary>
    /// P value for TopP sampling
    /// </summary>
    public float TopP { get; init; } = 0.9f;

    /// <summary>
    /// P value for MinP sampling
    /// </summary>
    public float MinP { get; init; } = 0.1f;

    /// <summary>
    /// Grammar to apply to constrain possible tokens
    /// </summary>
    public Grammar? Grammar { get; init; }

    /// <summary>
    /// The minimum number of tokens to keep for samplers which remove tokens
    /// </summary>
    public int MinKeep { get; set; } = 1;

    /// <summary>
    /// Seed to use for random sampling
    /// </summary>
    public uint Seed { get; set; } = 42;

    /// <inheritdoc />
    protected override SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
    {
        var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

        if (Grammar != null)
            chain.AddGrammar(context.ModelHandle, Grammar.Gbnf, Grammar.Root);

        chain.AddPenalties(
            context.VocabCount,
            context.ModelHandle.Tokens.EOS, context.ModelHandle.Tokens.Newline ?? 0,
            RepeatPenaltyCount, RepeatPenalty,
            AlphaFrequency, AlphaPresence,
            PenalizeNewline, PenalizeEOS
        );

        chain.AddTopK(TopK);
        chain.AddTailFree(TailFreeZ, MinKeep);
        chain.AddTypical(TypicalP, MinKeep);
        chain.AddTopP(TopP, MinKeep);
        chain.AddMinP(MinP, MinKeep);
        chain.AddTemperature(Temperature);

        chain.AddSoftmax();
        chain.AddDistributionSampler(Seed);

        return chain;
    }
}