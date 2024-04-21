using LLama.Experimental.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// Metadata for input sequences. Used in sampler.
    /// </summary>
    /// <param name="SeqIds">List of seq ids.</param>
    /// <param name="SeqData">Seq_id -> SequenceData.</param>
    /// <param name="PromptLengths">Lengths of prompts.</param>
    /// <param name="SelectedTokenIndices">Token indices selected for sampling.</param>
    public record class SamplingMetadata(
        IList<int> SeqIds, 
        IDictionary<int, SequenceData> SeqData, 
        IList<int> PromptLengths, 
        IList<int> SelectedTokenIndices
    )
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return ClassStringFormatter.Format(this);
        }
    }
}
