using System;
using System.Collections.Generic;
using LLama.Common;
using LLama.Native;
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
        /// logit bias for specific tokens
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public Dictionary<LLamaToken, float>? LogitBias { get; set; }

		/// <summary>
		/// Sequences where the model will stop generating further tokens.
		/// </summary>
		public IReadOnlyList<string> AntiPrompts { get; set; }

        /// <summary>
        ///  0 or lower to use vocab size
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public int TopK { get; set; }

        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float TopP { get; set; }

        /// <summary>
        /// 0.0 = disabled
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float MinP { get; set; }

        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float TfsZ { get; set; }

        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float TypicalP { get; set; }

        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float Temperature { get; set; }

        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float RepeatPenalty { get; set; }

        /// <summary>
        /// last n tokens to penalize (0 = disable penalty, -1 = context size) (repeat_last_n)
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public int RepeatLastTokensCount { get; set; }

        /// <summary>
        /// frequency penalty coefficient
        /// 0.0 = disabled
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float FrequencyPenalty { get; set; }

        /// <summary>
        /// presence penalty coefficient
        /// 0.0 = disabled
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        public float PresencePenalty { get; set; }

        /// <summary>
        /// Mirostat uses tokens instead of words.
        /// algorithm described in the paper https://arxiv.org/abs/2007.14966.
        /// 0 = disabled, 1 = mirostat, 2 = mirostat 2.0
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. MirostatSamplingPipeline or Mirostat2SamplingPipeline")]
        public MirostatType Mirostat { get; set; }

        /// <summary>
        /// target entropy
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. MirostatSamplingPipeline or Mirostat2SamplingPipeline")]
        public float MirostatTau { get; set; }

        /// <summary>
        /// learning rate
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. MirostatSamplingPipeline or Mirostat2SamplingPipeline")]
        public float MirostatEta { get; set; }

        /// <summary>
        /// consider newlines as a repeatable token (penalize_nl)
        /// </summary>
        [Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
        // ReSharper disable once InconsistentNaming (obsolete, will be removed anyway)
        public bool PenalizeNL { get; set; }

		/// <summary>
		/// Grammar to constrain possible tokens
		/// </summary>
		[Obsolete("Use the SamplingPipeline property instead with a configured pipeline e.g. DefaultSamplingPipeline")]
		SafeLLamaGrammarHandle? Grammar { get; set; }

		/// <summary>
		/// Set a custom sampling pipeline to use. <b>If this is set All other sampling parameters are ignored!</b>
		/// </summary>
		ISamplingPipeline? SamplingPipeline { get; set; }
	}

    internal static class IInferanceParamsExtensions
    {
        public static ISamplingPipeline Create(this IInferenceParams @params, ref ISamplingPipeline? pipeline)
        {
            // This method exists to adapt the old style of inference params to the newer sampling pipeline system. It's touching a lot
            // of obsolete things which we don't really care about, disable the warning.
            #pragma warning disable CS0618 // Type or member is obsolete

            if (@params.Mirostat == MirostatType.Mirostat)
            {
                if (pipeline is not MirostatSamplingPipeline)
                    pipeline = new MirostatSamplingPipeline();

                var m = (MirostatSamplingPipeline)pipeline;
                m.Eta = @params.MirostatEta;
                m.Tau = @params.MirostatTau;
                return m;
            }

            if (@params.Mirostat == MirostatType.Mirostat2)
            {
                if (pipeline is not Mirostat2SamplingPipeline)
                    pipeline = new Mirostat2SamplingPipeline();

                var m = (Mirostat2SamplingPipeline)pipeline;
                m.Eta = @params.MirostatEta;
                m.Tau = @params.MirostatTau;
                return m;
            }

            if (pipeline is not DefaultSamplingPipeline)
                pipeline = new DefaultSamplingPipeline();

            var d = (DefaultSamplingPipeline)pipeline;
            d.AlphaPresence = @params.PresencePenalty;
            d.MinP = @params.MinP;
            d.PenalizeNewline = @params.PenalizeNL;
            d.RepeatPenalty = @params.RepeatPenalty;
            d.TailFreeZ = @params.TfsZ;
            d.Temperature = @params.Temperature;
            d.TopK = @params.TopK;
            d.TopP = @params.TopP;
            d.AlphaFrequency = @params.FrequencyPenalty;
            d.TypicalP = @params.TypicalP;
            d.Grammar = @params.Grammar;

            d.LogitBias.Clear();
            @params.LogitBias?.CopyTo(d.LogitBias);

            return d;

            #pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}