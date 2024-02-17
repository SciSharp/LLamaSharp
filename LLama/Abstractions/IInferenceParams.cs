using System.Collections.Generic;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;

namespace LLama.Abstractions
{  
	 /// <summary>
	 /// The paramters used for inference.
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
		/// logit bias for specific tokens
		/// </summary>
		public Dictionary<LLamaToken, float>? LogitBias { get; set; }

		/// <summary>
		/// Sequences where the model will stop generating further tokens.
		/// </summary>
		public IReadOnlyList<string> AntiPrompts { get; set; }

		/// <summary>
		///  0 or lower to use vocab size
		/// </summary>
		public int TopK { get; set; }

		/// <summary>
		/// 1.0 = disabled
		/// </summary>
		public float TopP { get; set; }

        /// <summary>
        /// 0.0 = disabled
        /// </summary>
        public float MinP { get; set; }

        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        public float TfsZ { get; set; }

		/// <summary>
		/// 1.0 = disabled
		/// </summary>
		public float TypicalP { get; set; }

		/// <summary>
		/// 1.0 = disabled
		/// </summary>
		public float Temperature { get; set; }

		/// <summary>
		/// 1.0 = disabled
		/// </summary>
		public float RepeatPenalty { get; set; }

		/// <summary>
		/// last n tokens to penalize (0 = disable penalty, -1 = context size) (repeat_last_n)
		/// </summary>
		public int RepeatLastTokensCount { get; set; }

		/// <summary>
		/// frequency penalty coefficient
		/// 0.0 = disabled
		/// </summary>
		public float FrequencyPenalty { get; set; }

		/// <summary>
		/// presence penalty coefficient
		/// 0.0 = disabled
		/// </summary>
		public float PresencePenalty { get; set; }

		/// <summary>
		/// Mirostat uses tokens instead of words.
		/// algorithm described in the paper https://arxiv.org/abs/2007.14966.
		/// 0 = disabled, 1 = mirostat, 2 = mirostat 2.0
		/// </summary>
		public MirostatType Mirostat { get; set; }

		/// <summary>
		/// target entropy
		/// </summary>
		public float MirostatTau { get; set; }

		/// <summary>
		/// learning rate
		/// </summary>
		public float MirostatEta { get; set; }

		/// <summary>
		/// consider newlines as a repeatable token (penalize_nl)
		/// </summary>
		public bool PenalizeNL { get; set; }

		/// <summary>
		/// Grammar to constrain possible tokens
		/// </summary>
		SafeLLamaGrammarHandle? Grammar { get; set; }

		/// <summary>
		/// Set a custom sampling pipeline to use. <b>If this is set All other sampling parameters are ignored!</b>
		/// </summary>
		ISamplingPipeline? SamplingPipeline { get; set; }
	}
}