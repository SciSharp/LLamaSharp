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
        /// number of tokens to keep from initial prompt when applying context shifting
        /// </summary>
        public int TokensKeep { get; set; } = 0;

        /// <summary>
        /// how many new tokens to predict (n_predict), set to -1 to infinitely generate response
        /// until it complete.
        /// </summary>
        public int MaxTokens { get; set; } = -1;

        /// <summary>
        /// Sequences where the model will stop generating further tokens.
        /// </summary>
        public IReadOnlyList<string> AntiPrompts { get; set; } = [];

        /// <inheritdoc />
        public ISamplingPipeline SamplingPipeline { get; set; } = new DefaultSamplingPipeline();

        /// <inheritdoc />
        public bool DecodeSpecialTokens { get; set; }
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
