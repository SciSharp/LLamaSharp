using LLama.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public class LLamaSamplingParams
    {
        public int n_prev = 64;       // number of previous tokens to remember
        public int n_probs = 0;        // if greater than 0, output the probabilities of top n_probs tokens.
        public int top_k = 40;       // <= 0 to use vocab size
        public float top_p = 0.95f;    // 1.0 = disabled
        public float min_p = 0.05f;    // 0.0 = disabled
        public float tfs_z = 1.00f;    // 1.0 = disabled
        public float typical_p = 1.00f;    // 1.0 = disabled
        public float temp = 0.70f;    // <= 0.0 to sample greedily, 0.0 to not output probabilities
        public float dynatemp_range = 0.0f;  //0.0 = disabled
        public float dynatemp_exponent = 1.0f;  // controls how entropy maps to temperature in dynamic temperature sampler
        public int penalty_last_n = 64;       // last n tokens to penalize (0 = disable penalty, -1 = context size)
        public float penalty_repeat = 1.10f;    // 1.0 = disabled
        public float penalty_freq = 0.00f;    // 0.0 = disabled
        public float penalty_present = 0.00f;    // 0.0 = disabled
        public int mirostat = 0;        // 0 = disabled, 1 = mirostat, 2 = mirostat 2.0
        public float mirostat_tau = 5.00f;    // target entropy
        public float mirostat_eta = 0.10f;    // learning rate
        public bool penalize_nl = true;     // consider newlines as a repeatable token

        public string samplers_sequence = "kfypmt"; // top_k, tail_free, typical_p, top_p, min_p, temp

        public string grammar = string.Empty;  // optional BNF-like grammar to constrain sampling

        // Classifier-Free Guidance
        // https://arxiv.org/abs/2306.17806

        public string cfg_negative_prompt = string.Empty; // string to help guidance
        public float cfg_scale = 1.0f; // how strong is guidance

        //std::unordered_map<llama_token, float> logit_bias; // logit bias for specific tokens
        public IntPtr logit_bias;
        public LLamaToken[] penalty_prompt_tokens;
        public bool use_penalty_prompt_tokens = false;
    }
    public struct logit_bias_struct
    {
        public LLamaToken token;
        public float bias;
    }


}
