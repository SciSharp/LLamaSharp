using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// Data associated with a sequence.
    /// </summary>
    public class SequenceData
    {
        /// <summary>
        /// The token IDs of the prompt.
        /// </summary>
        public IList<int> PromptTokenIds { get; set; }

        /// <summary>
        /// The token IDs of the output. Set to an empty list if None.
        /// </summary>
        public List<int> OutputTokenIds { get; set; }

        /// <summary>
        /// The stage of the sequence data.
        /// </summary>
        public SequenceStage Stage { get; private set; }

        /// <summary>
        /// The number of all the tokens in the sequence, including prompt and output.
        /// </summary>
        public int Length => OutputTokenIds.Count + PromptTokenIds.Count;

        /// <summary>
        /// All the token IDs, including prompt and output.
        /// </summary>
        public IEnumerable<int> TokenIds => PromptTokenIds.Concat(OutputTokenIds);

        /// <summary>
        /// The number of prefill tokens that are already computed.
        /// </summary>
        public int NumComputedTokens { get; private set; }

        /// <summary>
        /// The number of prefil tokens that are not computed.
        /// </summary>
        public int NumUncomputedTokens => Length - NumComputedTokens;

        /// <summary>
        /// The last token ID in the sequence.
        /// </summary>
        public int LastTokenId
        {
            get
            {
                if(OutputTokenIds.Count == 0)
                {
                    return PromptTokenIds[PromptTokenIds.Count - 1];
                }
                return OutputTokenIds[OutputTokenIds.Count - 1];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="promptTokens"></param>
        /// <param name="outputTokens"></param>
        public SequenceData(IList<int> promptTokens, IEnumerable<int>? outputTokens = null)
        {
            OutputTokenIds = outputTokens is not null ? new List<int>(outputTokens) : new List<int>();
            PromptTokenIds = promptTokens;

            // TODO: cumulative_logprob?
            NumComputedTokens = 0;
            Stage = SequenceStage.Prefill;
        }

        /// <summary>
        /// Add a token id to the output token ids.
        /// </summary>
        /// <param name="tokenId"></param>
        public void AppendToken(int tokenId)
        {
            OutputTokenIds.Add(tokenId);
        }

        /// <summary>
        /// Update number of tokens computed so far.
        /// </summary>
        /// <param name="numNewComputedTokens"></param>
        public void UpdateNumComputedTokens(int numNewComputedTokens)
        {
            NumComputedTokens += numNewComputedTokens;
            Debug.Assert(NumComputedTokens <= Length);
            if(NumUncomputedTokens == 0)
            {
                Stage = SequenceStage.Decode;
            }
        }

        /// <summary>
        /// Reset the number of computed tokens from this sequence. It is
        /// supposed to be called when a sequence needs to be started from
        /// the beginning again(e.g., sequence is preempted).
        /// </summary>
        public void ResetStageForRecompute()
        {
            NumComputedTokens = 0;
            Stage = SequenceStage.Prefill;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"SequenceData(\n    PromptTokens: {string.Join(", ", PromptTokenIds)}, \n    " +
                $"OutputTokens: {string.Join(", ", OutputTokenIds)}, Stage: {Stage}\n)";
        }
    }
}
