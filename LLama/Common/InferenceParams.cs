using LLama.Abstractions;
using System;
using System.Collections.Generic;
using LLama.Native;
using LLama.Sampling;

namespace LLama.Common
{
    /// <summary>
    /// The paramters used for inference.
    /// </summary>
    public record InferenceParams
        : IInferenceParams
    {
        /// <summary>
        /// number of tokens to keep from initial prompt
        /// </summary>
        public int TokensKeep { get; set; } = 0;

        /// <summary>
        /// how many new tokens to predict (n_predict), set to -1 to inifinitely generate response
        /// until it complete.
        /// </summary>
        public int MaxTokens { get; set; } = -1;

        /// <summary>
        /// logit bias for specific tokens
        /// </summary>
        public Dictionary<LLamaToken, float>? LogitBias { get; set; } = null;

        /// <summary>
        /// Sequences where the model will stop generating further tokens.
        /// </summary>
        public IReadOnlyList<string> AntiPrompts { get; set; } = Array.Empty<string>();

        /// <inheritdoc />
        public int TopK { get; set; } = 40;

        /// <inheritdoc />
        public float TopP { get; set; } = 0.95f;

        /// <inheritdoc />
        public float MinP { get; set; } = 0.05f;

        /// <inheritdoc />
        public float TfsZ { get; set; } = 1.0f;

        /// <inheritdoc />
        public float TypicalP { get; set; } = 1.0f;

        /// <inheritdoc />
        public float Temperature { get; set; } = 0.8f;

        /// <inheritdoc />
        public float RepeatPenalty { get; set; } = 1.1f;

        /// <inheritdoc />
        public int RepeatLastTokensCount { get; set; } = 64;

        /// <inheritdoc />
        public float FrequencyPenalty { get; set; } = .0f;

        /// <inheritdoc />
        public float PresencePenalty { get; set; } = .0f;

        /// <inheritdoc />
        public MirostatType Mirostat { get; set; } = MirostatType.Disable;

        /// <inheritdoc />
        public float MirostatTau { get; set; } = 5.0f;

        /// <inheritdoc />
        public float MirostatEta { get; set; } = 0.1f;

        /// <inheritdoc />
        public bool PenalizeNL { get; set; } = true;

        /// <inheritdoc />
        public SafeLLamaGrammarHandle? Grammar { get; set; }

        /// <inheritdoc />
        public ISamplingPipeline? SamplingPipeline { get; set; }
    }

    /// <summary>
    /// Type of "mirostat" sampling to use.
    /// https://github.com/basusourya/mirostat
    /// </summary>
    public enum MirostatType
    {
        /// <summary>
        /// Disable Mirostat sampling
        /// </summary>
        Disable = 0,

        /// <summary>
        /// Original mirostat algorithm
        /// </summary>
        Mirostat = 1,

        /// <summary>
        /// Mirostat 2.0 algorithm
        /// </summary>
        Mirostat2 = 2
    }
}
