using LLama.Common;
using LLama.Sampling;
using System.Collections.Generic;

namespace LLama.Abstractions
{  
    /// <summary>
    /// The parameters used for inference.
    /// </summary>
    public interface IInferenceParams
    {
		/// <summary>
		/// number of tokens to keep from initial prompt
		/// </summary>
		public int TokensKeep { get; set; }

		/// <summary>
		/// how many new tokens to predict (n_predict), set to -1 to infinitely generate response
		/// until it complete.
		/// </summary>
		public int MaxTokens { get; set; }

		/// <summary>
		/// Sequences where the model will stop generating further tokens.
		/// </summary>
		public IReadOnlyList<string> AntiPrompts { get; set; }

		/// <summary>
		/// Set a custom sampling pipeline to use.
		/// </summary>
		ISamplingPipeline SamplingPipeline { get; set; }

		/// <summary>
		/// If true, special characters will be converted to text. If false they will be invisible.
		/// </summary>
		/// <remark>
		/// Controls the behavior of decoders like <see cref="StreamingTokenDecoder" />
		/// </remark>
		public bool DecodeSpecialTokens { get; set; }

        /// <summary>
        /// Defines the strategy the executor should use when the context window is full 
        /// and the model architecture (e.g., models with 2D RoPE embeddings) does not 
        /// support native memory shifting.
        /// </summary>
        ContextOverflowStrategy OverflowStrategy { get; set; }

        /// <summary>
        /// The percentage of past tokens to discard when <see cref="OverflowStrategy"/> 
        /// is set to <see cref="ContextOverflowStrategy.TruncateAndReprefill"/>. 
        /// For example, 0.1f represents dropping the oldest 10% of the conversational context.
        /// </summary>
        float ContextTruncationPercentage { get; set; }
    }
}