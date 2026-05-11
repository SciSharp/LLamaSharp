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

        /// <summary>
        /// Defines the strategy the executor should use when the context window is full 
        /// and the model architecture does not support native memory shifting. 
        /// Defaults to <see cref="ContextOverflowStrategy.ThrowException"/> to prevent 
        /// unintended data loss and latency spikes.
        /// </summary>
        public ContextOverflowStrategy OverflowStrategy { get; set; } = ContextOverflowStrategy.ThrowException;

        /// <summary>
        /// The percentage of past tokens to discard when <see cref="OverflowStrategy"/> 
        /// is set to <see cref="ContextOverflowStrategy.TruncateAndReprefill"/>. 
        /// Defaults to 0.1f (10%). Valid range is typically between 0.01f and 0.99f.
        /// </summary>
        public float ContextTruncationPercentage { get; set; } = 0.1f;
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
