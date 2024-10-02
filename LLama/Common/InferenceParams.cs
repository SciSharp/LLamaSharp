using LLama.Abstractions;
using System.Collections.Generic;
using LLama.Native;
using LLama.Sampling;
using System;

namespace LLama.Common
{
    /// <summary>
    /// The parameters used for inference.
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
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public Dictionary<LLamaToken, float>? LogitBias { get; set; } = null;

        /// <summary>
        /// Sequences where the model will stop generating further tokens.
        /// </summary>
        public IReadOnlyList<string> AntiPrompts { get; set; } = [];

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public int TopK { get; set; } = 40;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float TopP { get; set; } = 0.95f;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float MinP { get; set; } = 0.05f;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float TfsZ { get; set; } = 1.0f;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float TypicalP { get; set; } = 1.0f;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float Temperature { get; set; } = 0.8f;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float RepeatPenalty { get; set; } = 1.1f;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public int RepeatLastTokensCount { get; set; } = 64;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float FrequencyPenalty { get; set; } = .0f;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float PresencePenalty { get; set; } = .0f;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. MirostatSamplingPipeline or Mirostat2SamplingPipeline")]
        public MirostatType Mirostat { get; set; } = MirostatType.Disable;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. MirostatSamplingPipeline or Mirostat2SamplingPipeline")]
        public float MirostatTau { get; set; } = 5.0f;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. MirostatSamplingPipeline or Mirostat2SamplingPipeline")]
        public float MirostatEta { get; set; } = 0.1f;

        /// <inheritdoc />
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public bool PenalizeNL { get; set; } = true;

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
