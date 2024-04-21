using LLama.Experimental.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// The output data of a request to the LLM.
    /// </summary>
    /// <param name="RequestId">The unique ID of the request.</param>
    /// <param name="Prompt">The prompt string of the request.</param>
    /// <param name="PromptTokenIds">The token IDs of the prompt.</param>
    /// <param name="Outputs">The output sequences of the request.</param>
    /// <param name="Finished">Whether the whole request is finished.</param>
    /// <param name="Metrics">Metrics associated with the request.</param>
    public record class RequestOutput(
        string RequestId, 
        string? Prompt, 
        IList<int> PromptTokenIds, 
        IList<CompletionOutput> Outputs, 
        bool Finished, 
        RequestMetrics Metrics
    )
    {
        /// <summary>
        /// Create an instance from <see cref="SequenceGroup"/>.
        /// </summary>
        /// <param name="seqGroup"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static RequestOutput FromSeqGroup(SequenceGroup seqGroup)
        {
            var seqs = seqGroup.GetAllSeqs();
            if(seqs.Count() != 1)
            {
                // TODO: deal with beam search here.
                throw new NotImplementedException();
            }

            List<CompletionOutput> outputs = new();
            int index = 0;
            foreach(var seq in seqs)
            {
                outputs.Add(new CompletionOutput(index, seq.OutputText, seq.OutputTokens, 
                    seq.Status.GetFinishedReason(), seq.StoppingString, seq.StoppingTokenId));
                index++;
            }

            if (seqGroup.IsFinished)
            {
                seqGroup.SetFinishedTime(DateTime.Now);
            }
            return new RequestOutput(seqGroup.RequestId, seqGroup.Prompt, seqGroup.PromptTokenIds, 
                outputs, seqGroup.IsFinished, seqGroup.Metrics);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ClassStringFormatter.Format(this);
        }
    }

    /// <summary>
    /// The output data of one completion output of a request.
    /// </summary>
    /// <param name="Index">The index of the output in the request.</param>
    /// <param name="Text">The generated output text.</param>
    /// <param name="TokenIds">The token IDs of the generated output text.</param>
    /// <param name="FinishReason">The reason why the sequence is finished.</param>
    /// <param name="StoppingString">
    /// The stop string that caused the completion to stop, 
    /// Null if the completion finished for some other reason.
    /// </param>
    /// <param name="StoppingToken">
    /// The stop string that caused the completion to stop, 
    /// Null if the completion finished for some other reason.
    /// </param>
    public record class CompletionOutput(
        int Index, 
        string Text, 
        IList<int> TokenIds, 
        string FinishReason, 
        string? StoppingString, 
        int? StoppingToken
    )
    {
        /// <summary>
        /// Whether the completion has finished.
        /// </summary>
        public bool IsFinished => !string.IsNullOrEmpty(FinishReason);

        /// <inheritdoc/>
        public override string ToString()
        {
            return ClassStringFormatter.Format(this);
        }
    }
}
