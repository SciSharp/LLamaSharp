using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// The input prepared for model runner.
    /// </summary>
    /// <param name="TokenIds">The tokens to feed to the model.</param>
    /// <param name="Positions">The positions of these tokens.</param>
    /// <param name="SeqIds">The sequence ids of these tokens.</param>
    /// <param name="WithLogits">Whether the logits need to be computed for the token.</param>
    /// <param name="PromptLengths">The lengths of the prompts if the input is at prefill stage, otherwise empty.</param>
    /// <param name="SubqueryLengths">The lengths of the subqueries if the input is at prefill stage, otherwise empty.</param>
    public record class ModelRunnerInput(
        int[] TokenIds, 
        int[] Positions, 
        int[] SeqIds, 
        bool[] WithLogits, 
        int[] PromptLengths, 
        int[] SubqueryLengths
    );
}
