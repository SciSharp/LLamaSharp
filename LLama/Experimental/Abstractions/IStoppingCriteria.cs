using LLama.Experimental.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Abstractions
{
    /// <summary>
    /// Stopping criteria that can be applied during generation.
    /// </summary>
    public interface IStoppingCriteria
    {
        /// <summary>
        /// Check if the sequence should be stopped and return the status.
        /// 
        /// If it's not supposed to be stopped, be sure to return its current status.
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        StoppingCriteriaOutput CheckStop(Sequence seq); // TODO: include other params?
    }

    /// <summary>
    /// The output of <see cref="IStoppingCriteria.CheckStop(Sequence)"/>
    /// </summary>
    /// <param name="Status">The sequence status.</param>
    /// <param name="StoppingString">If the sequence stops because of the appearance of a string, please set it here.</param>
    /// <param name="StoppingTokenId">If the sequence stops because of the appearance of a token, please set it here.</param>
    public record class StoppingCriteriaOutput(SequenceStatus Status, string? StoppingString = null, int? StoppingTokenId = null);
}
