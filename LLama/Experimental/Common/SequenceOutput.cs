using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// The model output associated with a sequence.
    /// </summary>
    public class SequenceOutput
        // TODO: Beam search
    {
        /// <summary>
        /// The output token ID.
        /// </summary>
        public int OutputTokenId { get; init; }

        /// <summary>
        /// The ID of the parent sequence (for forking in beam search).
        /// </summary>
        public int ParentSeqId { get; init; }

        /// <summary>
        /// The logprobs of the output token.
        /// (Token id -> logP(x_i+1 | x_0, ..., x_i))
        /// </summary>
        public float[]? Logprobs { get; init; }
    }
}
