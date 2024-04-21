using LLama.Experimental.Abstractions;
using LLama.Experimental.Common;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Core.LLamaCpp
{
    // TODO: This is only the most simple implementation to run the example. It should be replaced in the future.
    public class LLamaGreedySamplingMethod: ISamplingMethod
    {
        private LLamaContext _context;

        public LLamaGreedySamplingMethod(LLamaContext context)
        {
            _context = context;
        }

        public int GetMaxNumRunningSeqs(int defaultValue, int currentNumSeqs)
        {
            return defaultValue;
        }

        /// <summary>
        /// Whether to skip special tokens.
        /// </summary>
        public bool SkipSpecialTokens => false;
        
        public SequenceOutput SampleSequence(Span<float> logits, int seqId, SamplingMetadata samplingMetadata)
        {
            // Process token data array to select a final token
            var candidates = LLamaTokenDataArray.Create(logits);
            return new SequenceOutput()
            {
                OutputTokenId = (int)candidates.SampleTokenGreedy(_context.NativeHandle), 
                ParentSeqId = seqId
            };
        }
    }
}
