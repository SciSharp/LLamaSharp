using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Abstractions.Params
{
    using llama_token = Int32;
    public class SessionParams
    {
        /// <summary>
        /// number of tokens to keep from initial prompt
        /// </summary>
        public int TokensToKeep { get; set; } = 0;
        /// <summary>
        /// how many new tokens to predict (n_predict), set to -1 to inifinitely generate response
        /// until it complete.
        /// </summary>
        public int ResponseTokensCount { get; set; } = -1;
        /// <summary>
        /// logit bias for specific tokens
        /// </summary>
        public Dictionary<llama_token, float>? LogitBias { get; set; } = null;

        /// <summary>
        /// Sequences where the model will stop generating further tokens.
        /// </summary>
        public IList<string> AntiPrompts { get; set; } = Array.Empty<string>();
        /// <summary>
        /// path to file for saving/loading model eval state
        /// </summary>
        public string PathSession { get; set; } = string.Empty;
        /// <summary>
        /// string to suffix user inputs with
        /// </summary>
        public string InputSuffix { get; set; } = string.Empty;
        /// <summary>
        /// string to prefix user inputs with
        /// </summary>
        public string InputPrefix { get; set; } = string.Empty;
        /// <summary>
        ///  0 or lower to use vocab size
        /// </summary>
        public int TopK { get; set; } = 40;
        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        public float TopP { get; set; } = 0.95f;
        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        public float TfsZ { get; set; } = 1.0f;
        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        public float TypicalP { get; set; } = 1.0f;
        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        public float Temperature { get; set; } = 0.8f;
        /// <summary>
        /// 1.0 = disabled
        /// </summary>
        public float RepeatPenalty { get; set; } = 1.1f;
        /// <summary>
        /// last n tokens to penalize (0 = disable penalty, -1 = context size) (repeat_last_n)
        /// </summary>
        public int RepeatLastTokensCount { get; set; } = 64;
        /// <summary>
        /// frequency penalty coefficient
        /// 0.0 = disabled
        /// </summary>
        public float FrequencyPenalty { get; set; } = .0f;
        /// <summary>
        /// presence penalty coefficient
        /// 0.0 = disabled
        /// </summary>
        public float PresencePenalty { get; set; } = .0f;
        /// <summary>
        /// Mirostat uses tokens instead of words.
        /// algorithm described in the paper https://arxiv.org/abs/2007.14966.
        /// 0 = disabled, 1 = mirostat, 2 = mirostat 2.0
        /// </summary>
        public MiroStateType Mirostat { get; set; } = MiroStateType.Disable;
        /// <summary>
        /// target entropy
        /// </summary>
        public float MirostatTau { get; set; } = 5.0f;
        /// <summary>
        /// learning rate
        /// </summary>
        public float MirostatEta { get; set; } = 0.1f;
        /// <summary>
        /// consider newlines as a repeatable token (penalize_nl)
        /// </summary>
        public bool PenalizeNL { get; set; } = true;
    }

    public enum MiroStateType
    {
        Disable = 0, 
        MiroState = 1, 
        MiroState2 = 2
    }
}
