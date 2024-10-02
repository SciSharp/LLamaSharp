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
    public float RepeatPenalty { get; init; }

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
    /// Temperature to apply (higher temperature is more "creative")
    /// </summary>
    public float Temperature { get; init; } = 0.75f;

    /// <summary>
    /// Number of tokens to keep in TopK sampling
    /// </summary>
    public int TopK { get; init; }

    /// <summary>
    /// Z value for tail free sampling
    /// </summary>
    public float TailFreeZ { get; init; }

    /// <summary>
    /// P value for locally typical sampling
    /// </summary>
    public float TypicalP { get; init; }

    /// <summary>
    /// P value for TopP sampling
    /// </summary>
    public float TopP { get; init; } = 1f;

    /// <summary>
    /// P value for MinP sampling
    /// </summary>
    public float MinP { get; init; }

    /// <summary>
    /// Whether the newline value should be protected from being modified by logit bias and repeat penalty
    /// </summary>
    public bool PenalizeNewline { get; init; }

    /// <summary>
    /// Grammar to apply to constrain possible tokens
    /// </summary>
    public Grammar? Grammar { get; init; }

    /// <summary>
    /// The minimum number of tokens to keep for samplers which remove tokens
    /// </summary>
    public int MinKeep { get; set; } = 1;

    /// <inheritdoc />
    protected override SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
    {
        var chain = SafeLLamaSamplerHandle.CreateChain(LLamaSamplerChainParams.Default());

        if (Grammar != null)
            chain.Add(SafeLLamaSamplerHandle.CreateGrammar(context.ModelHandle, Grammar.Gbnf, Grammar.Root));

        chain.Add(SafeLLamaSamplerHandle.CreateTopK(TopK));
        chain.Add(SafeLLamaSamplerHandle.CreateTailFree(TailFreeZ, MinKeep));
        chain.Add(SafeLLamaSamplerHandle.CreateTypical(TypicalP, MinKeep));
        chain.Add(SafeLLamaSamplerHandle.CreateTopP(TopP, MinKeep));
        chain.Add(SafeLLamaSamplerHandle.CreateMinP(MinP, MinKeep));
        chain.Add(SafeLLamaSamplerHandle.CreateTemperature(Temperature));

        chain.Add(SafeLLamaSamplerHandle.CreateDistributionSampler(0));

        return chain;
    }
}