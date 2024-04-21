using LLama.Experimental.Abstractions;
using LLama.Experimental.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama.Experimental.Extensions
{
    /// <inheritdoc/>
    public static class SchedulingPolicyExtensions
    {
        /// <summary>
        /// Sorts the sequence groups by priority.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="now"></param>
        /// <param name="seqGroups"></param>
        /// <returns></returns>
        public static LinkedList<SequenceGroup> SortByPriority(this ISchedulingPolicy policy, DateTime now, LinkedList<SequenceGroup> seqGroups)
        {
            return new LinkedList<SequenceGroup>(seqGroups.OrderByDescending(seqGroups => policy.GetPriority(now, seqGroups)));
        }
    }
}
