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
		/// how many new tokens to predict (n_predict), set to -1 to inifinitely generate response
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
	}
}