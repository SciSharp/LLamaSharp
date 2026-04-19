using System.Collections.Generic;
using LLama.Sampling;

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
	}
}