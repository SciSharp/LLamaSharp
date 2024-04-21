using System;
using System.Collections.Generic;
using System.Text;
using LLama.Experimental.Abstractions;
using LLama.Experimental.Common;

namespace LLama.Experimental
{
    /// <summary>
    /// First in first out policy.
    /// </summary>
    public class FCFS: ISchedulingPolicy
    {
        /// <inheritdoc/>
        public int GetPriority(DateTime now, SequenceGroup seqGroup)
        {
            return (now - seqGroup.Metrics.ArrivalTime).Milliseconds;
        }
    }

    public class PolicyFactory
    {
        public static ISchedulingPolicy DefaultPolicy { get; set; } = new FCFS();
    }
}
