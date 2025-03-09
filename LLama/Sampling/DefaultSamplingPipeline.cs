using System;
using System.Collections.Generic;
using CommunityToolkit.HighPerformance.Buffers;
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
    /// How many tokens should be considered for penalties
    /// </summary>
    public int PenaltyCount { get; init; } = 64;

    /// <summary>
    /// Whether the newline token should be protected from being modified by penalty
    /// </summary>
    public bool PenalizeNewline { get; init; } = false;

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
    
    /// <summary>
    /// Selected grammar optimization mode
    /// </summary>
    public GrammarOptimizationMode GrammarOptimization { get; init; } = GrammarOptimizationMode.Extended;

    /// <summary>
    /// A chain with just the grammar
    /// </summary>
    private SafeLLamaSamplerChainHandle? _grammarChain;


    private static readonly Random RandomSeedGenerator = new();
    private static uint GetRandomSeed()
    {
        lock (RandomSeedGenerator)
            return (uint) RandomSeedGenerator.Next(0, int.MaxValue) + (uint) RandomSeedGenerator.Next(0, int.MaxValue);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        base.Dispose();

        _grammarChain?.Dispose();
        _grammarChain = null;
    }

    /// <inheritdoc />
    public override void Reset()
    {
        base.Reset();

        _grammarChain?.Reset();
    }

    /// <inheritdoc />
    public override void Accept(LLamaToken token)
    {
        base.Accept(token);

        _grammarChain?.Accept(token);
    }

    private SafeLLamaSamplerChainHandle CreateGrammarChain(SafeLLamaContextHandle context)
    {
        if (Grammar == null)
            throw new InvalidOperationException(nameof(Grammar) + " is null");

        var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());
        chain.AddGrammar(context.ModelHandle, Grammar.Gbnf, Grammar.Root);
        return chain;
    }

    /// <inheritdoc />
    protected override SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
    {
        var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

        if (LogitBias.Count > 0)
        {
            using var biases = SpanOwner<LLamaLogitBias>.Allocate(LogitBias.Count);

            // copy the biases into it
            var index = 0;
            foreach (var bias in LogitBias)
            {
                biases.Span[index++] = new LLamaLogitBias
                {
                    Token = bias.Key,
                    Bias = bias.Value
                };
            }

            // Add the biases to the sampler
            chain.AddLogitBias(context.Vocab.Count, biases.Span);

        }

        chain.AddPenalties(PenaltyCount, RepeatPenalty, FrequencyPenalty, PresencePenalty);

        chain.AddTopK(TopK);
        chain.AddTypical(TypicalP, MinKeep);
        chain.AddTopP(TopP, MinKeep);
        chain.AddMinP(MinP, MinKeep);
        chain.AddTemperature(Temperature);

        chain.AddDistributionSampler(Seed);

        return chain;
    }

    /// <inheritdoc />
    public override LLamaToken Sample(SafeLLamaContextHandle ctx, int index)
    {
        if (Grammar == null)
            return base.Sample(ctx, index);

        // Create a chain with the grammar
        _grammarChain ??= CreateGrammarChain(ctx);

        // Rent some buffers to use later
        using var bufferVocabSize = MemoryOwner<LLamaTokenData>.Allocate(ctx.ModelHandle.Vocab.Count);
        using var bufferSingleItem = MemoryOwner<LLamaTokenData>.Allocate(1);

        // Handle grammar optimization modes
        if (GrammarOptimization != GrammarOptimizationMode.None)
        {
            // Basic optimization : Apply the grammar to the selected token and check if it's valid
            using (LLamaTokenDataArrayNative.Create(LLamaTokenDataArray.Create(ctx.GetLogitsIth(index), bufferVocabSize.Memory), out var nativeAll))
            {
                // Apply the chain without the grammar to select one token which may or may not be valid
                Apply(ctx, ref nativeAll);
                    
                // Select the candidate token
                var candidateToken = nativeAll.Data[checked((int)nativeAll.Selected)].ID;

                // Now create another token data array with just that one token
                bufferSingleItem.Span[0] = new LLamaTokenData(candidateToken, 1, 0);
                using (LLamaTokenDataArrayNative.Create(new LLamaTokenDataArray(bufferSingleItem.Memory, true), out var nativeSingleCandidate))
                {
                    // Apply the grammar chain to the single candidate
                    _grammarChain.Apply(ref nativeSingleCandidate);

                    // Check if the token passes the grammar
                    if (!float.IsNegativeInfinity(nativeSingleCandidate.Data[0].Logit))
                    {
                        Accept(candidateToken);
                        return candidateToken;
                    }
                }
                    
                // Extended optimization : Apply the grammar to the TopK tokens and check if the selected token is valid
                if (GrammarOptimization == GrammarOptimizationMode.Extended)
                {
                    // Calculate a safe TopK value
                    var safeTopK = Math.Min(TopK, nativeAll.Data.Length);

                    // Rent a buffer for the TopK candidates
                    using var bufferTopK = MemoryOwner<LLamaTokenData>.Allocate(safeTopK);

                    // Copy only the TopK tokens from the existing candidate pool to the new buffer
                    nativeAll.Data.Slice(0, safeTopK).CopyTo(bufferTopK.Span);

                    // Create a native array with the TopK tokens
                    using (LLamaTokenDataArrayNative.Create(new LLamaTokenDataArray(bufferTopK.Memory, true), out var nativeTopK))
                    {
                        // Apply the grammar chain to the TopK candidates
                        _grammarChain.Apply(ref nativeTopK);
                                
                        // Select the candidate token
                        var candidateTokenTopK = nativeTopK.Data[checked((int)nativeTopK.Selected)];
                                
                        // Check if the token passes the grammar
                        if (!float.IsNegativeInfinity(candidateTokenTopK.Logit))
                        {
                            // Accept and return the token
                            Accept(candidateTokenTopK.ID);
                            return candidateTokenTopK.ID;
                        }
                    }
                }
            }
        }
            
        // If we get here the grammar rejected the token
        using (LLamaTokenDataArrayNative.Create(LLamaTokenDataArray.Create(ctx.GetLogitsIth(index), bufferVocabSize.Memory), out var nativeAll))
        {
            // Apply the grammar _first_. This is slower (since it has to work on the entire vocab), but guaranteed to work
            _grammarChain.Apply(ref nativeAll);

            // Now apply the rest of the pipeline
            Apply(ctx, ref nativeAll);

            // Take the selected token
            var token = nativeAll.Data[checked((int)nativeAll.Selected)].ID;
            Accept(token);
            return token;
        }
    }
    
    /// <summary>
    /// Grammar Optimization Mode
    /// </summary>
    public enum GrammarOptimizationMode
    {
        /// <summary>
        /// No grammar optimization, slow because it has to apply the grammar to the entire vocab.
        /// </summary>
        None,

        /// <summary>
        /// Attempts to return early by only applying the grammar to the selected token and checking if it's valid.
        /// </summary>
        Basic,

        /// <summary>
        /// Attempts to return early by applying the grammar to the top K tokens and checking if the selected token is valid.
        /// </summary>
        Extended
    }
}