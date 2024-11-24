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
    [Obsolete($"Use {nameof(FrequencyPenalty)} instead.")]
    public float AlphaFrequency
    {
        get => _frequencyPenalty;
        init
        {
            if (value < -2)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(AlphaFrequency)} must be greater than -2");
            if (value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(AlphaFrequency)} must be less than 2");
            _frequencyPenalty = value;
        }
    }

    /// <summary>
    /// Presence penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br />
    /// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the
    /// text so far, increasing the model's likelihood to talk about new topics.
    /// </summary>
    [Obsolete($"Use {nameof(PresencePenalty)} instead.")]
    public float AlphaPresence
    {
        get => _presencePenalty;
        init
        {
            if (value < -2)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(AlphaPresence)} must be greater than -2");
            if (value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(AlphaPresence)} must be less than 2");
            _presencePenalty = value;
        }
    }

    /// <summary>
    /// Frequency penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br />
    /// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text
    /// so far, decreasing the model's likelihood to repeat the same line verbatim.
    /// </summary>
    public float FrequencyPenalty
    {
        get => _frequencyPenalty;
        init
        {
            if (value < -2)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(FrequencyPenalty)} must be greater than -2");
            if (value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(FrequencyPenalty)} must be less than 2");
            _frequencyPenalty = value;
        }
    }
    private readonly float _frequencyPenalty;

    /// <summary>
    /// Presence penalty as described by OpenAI: https://platform.openai.com/docs/api-reference/chat/create<br />
    /// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the
    /// text so far, increasing the model's likelihood to talk about new topics.
    /// </summary>
    public float PresencePenalty
    {
        get => _presencePenalty;
        init
        {
            if (value < -2)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(PresencePenalty)} must be greater than -2");
            if (value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(PresencePenalty)} must be less than 2");
            _presencePenalty = value;
        }
    }
    private readonly float _presencePenalty;

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
    [Obsolete($"This doesn't do what the name implies. If you're sure you want to use it, use {nameof(PreventEOS)}.")]
    public bool PenalizeEOS { get; init; } = false;

    /// <summary>
    /// Whether the EOS token should be suppressed. Setting this to 'true' prevents EOS from being sampled
    /// </summary>
    public bool PreventEOS { get; init; } = false;

    /// <summary>
    /// Temperature to apply (higher temperature is more "creative")
    /// </summary>
    public float Temperature { get; init; } = 0.75f;

    /// <summary>
    /// Number of tokens to keep in TopK sampling
    /// </summary>
    public int TopK { get; init; } = 40;

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
    public uint Seed { get; set; } = GetRandomSeed();


    private static Random RandomSeedGenerator = new();
    private static uint GetRandomSeed()
    {
        lock (RandomSeedGenerator)
            return (uint) RandomSeedGenerator.Next(0, int.MaxValue) + (uint) RandomSeedGenerator.Next(0, int.MaxValue);
    }


    /// <inheritdoc />
    protected override SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
    {
        var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

        // Rent a temporary array and copy the biases into it
        var biases = ArrayPool<LLamaLogitBias>.Shared.Rent(LogitBias.Count);
        try
        {
            var index = 0;
            foreach (var bias in LogitBias)
            {
                biases[index++] = new LLamaLogitBias
                {
                    Token = bias.Key,
                    Bias = bias.Value
                };
            }

            // Add the biases to the sampler
            chain.AddLogitBias(context.ModelHandle.VocabCount, biases.AsSpan(0, LogitBias.Count));
        }
        finally
        {
            ArrayPool<LLamaLogitBias>.Shared.Return(biases);
        }

        if (Grammar != null)
            chain.AddGrammar(context.ModelHandle, Grammar.Gbnf, Grammar.Root);

        chain.AddPenalties(
            context.VocabCount,
            context.ModelHandle.Tokens.EOS, context.ModelHandle.Tokens.Newline ?? 0,
            RepeatPenaltyCount, RepeatPenalty,
            FrequencyPenalty, PresencePenalty,
            PenalizeNewline, PreventEOS
        );

        chain.AddTopK(TopK);
        chain.AddTypical(TypicalP, MinKeep);
        chain.AddTopP(TopP, MinKeep);
        chain.AddMinP(MinP, MinKeep);
        chain.AddTemperature(Temperature);

        chain.AddDistributionSampler(Seed);

        return chain;
    }
}