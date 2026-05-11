#nullable enable

using LLama.Common;
using LLama.Abstractions;
using LLama.Native;
using LLama.Sampling;

namespace LLama.Web.Common
{
    public class InferenceOptions
        : IInferenceParams
    {
        /// <inheritdoc />
        public int TokensKeep { get; set; } = 0;

        /// <inheritdoc />
        public int MaxTokens { get; set; } = -1;

        /// <inheritdoc />
        public IReadOnlyList<string> AntiPrompts { get; set; } = Array.Empty<string>();

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
}
