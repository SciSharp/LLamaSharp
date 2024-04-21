using LLama.Experimental.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Abstractions
{
    /// <summary>
    /// Define the scheduling policy, which decides the priority orders of sequences.
    /// </summary>
    public interface ISchedulingPolicy
    {
        /// <summary>
        /// Get the priority of a sequence group.
        /// </summary>
        /// <param name="now"></param>
        /// <param name="seqGroup"></param>
        /// <returns></returns>
        int GetPriority(DateTime now, SequenceGroup seqGroup);
    }
}
