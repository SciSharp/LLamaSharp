using LLama.Grammars;
using LLama.Native;
using System;

namespace LLama.Common
{
    public class LLamaSamplingContext
    {
        public LLamaSamplingParams parameters;

        // mirostat sampler state
        public float mirostat_mu;

        public IntPtr grammar;
        // internal
        public IntPtr parsed_grammar;

        // TODO: replace with ring-buffer
        public LLamaToken[] prev;
        public LLamaTokenData[] cur;
    }
}
