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
    /// Repetition penalty, as described in https://arxiv.org/abs/1909.05858
    /// </summary>
    public float RepeatPenalty { get; set; }

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
    private float _alphaFreq;

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
    private float _alphaPresence;

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

    private float[]? _logits;

    /// <inheritdoc />
    protected override ReadOnlySpan<float> ProcessLogits(SafeLLamaContextHandle ctx, ReadOnlySpan<float> logits, ReadOnlySpan<LLamaToken> lastTokens)
    {
        // Skip work if possible
        if (LogitBias.Count == 0)
            return logits;

        // Create a temporary array to hold logits
        if (_logits == null || _logits.Length < logits.Length)
            _logits = new float[logits.Length];

        // Copy logits
        logits.CopyTo(_logits);
        var mutable = _logits.AsSpan(0, logits.Length);

        // Apply logit bias
        foreach (var (key, value) in LogitBias)
            mutable[key] += value;

        return mutable;
    }

    /// <inheritdoc />
    protected override LLamaToken ProcessTokenDataArray(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, ReadOnlySpan<LLamaToken> lastTokens)
    {
        // Only apply repetition penalty if we really must. Otherwise avoid all this work
        if (lastTokens.Length > 0 && (RepeatPenalty != 0 || AlphaFrequency != 0 || AlphaPresence != 0))
        {
            // Save the logit value for the newline token
            var (nlIndex, nlLogit) = PenalizeNewline ? GetNewlineLogit(ctx, candidates) : (-1, 0);

            // Apply penalties to candidates
            candidates.RepetitionPenalty(ctx, lastTokens, RepeatPenalty, AlphaFrequency, AlphaPresence);

            // Restore newline token
            if (!PenalizeNewline)
                SetNewlineLogit(ctx, candidates, nlIndex, nlLogit);
        }

        // Apply the normal llama.cpp pipeline
        candidates.ApplyGrammar(ctx, Grammar);
        candidates.TopK(ctx, TopK);
        candidates.TailFree(ctx, TailFreeZ);
        candidates.LocallyTypical(ctx, TypicalP);
        candidates.TopP(ctx, TopP);
        candidates.MinP(ctx, MinP);
        candidates.Temperature(ctx, Temperature);
        return candidates.SampleToken(ctx);
    }

    private static (int, float) GetNewlineLogit(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates)
    {
        var nlToken = NativeApi.llama_token_nl(ctx.ModelHandle);

        // Try using the ID as an index
        if (candidates.data.Span[(int)nlToken].id == nlToken)
            return ((int)nlToken, candidates.data.Span[(int)nlToken].logit);
        
        // Exhaustive search
        var span = candidates.data.Span;
        for (var i = 0; i < span.Length; i++)
        {
            if (span[i].id == nlToken)
                return (i, span[i].logit);
        }

        return (-1, 0);
    }

    private static void SetNewlineLogit(SafeLLamaContextHandle ctx, LLamaTokenDataArray candidates, int indexHint, float logit)
    {
        var nlToken = NativeApi.llama_token_nl(ctx.ModelHandle);

        // Try checking the index where we found it last time. It might not be there if `RepetitionPenalty` changed order
        if (indexHint >= 0 && candidates.data.Span[indexHint].id == nlToken)
        {
            candidates.data.Span[indexHint].logit = logit;
            return;
        }

        // Didn't find it, do an exhaustive search for it
        var span = candidates.data.Span;
        for (var i = 0; i < candidates.data.Length; i++)
        {
            if (span[i].id == nlToken)
            {
                span[i].logit = logit;
                return;
            }
        }
    }

    /// <inheritdoc />
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