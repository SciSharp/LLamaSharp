using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// The scheduling decision made from a scheduler.
    /// </summary>
    /// <param name="ScheduledSeqGroups">Scheduled sequence groups.</param>
    /// <param name="NumPrefillGroups">Number of prefill groups scheduled.</param>
    /// <param name="NumBatchedTokens">Total number of batched tokens.</param>
    /// <param name="IgnoredSeqGroups">Sequence groups that are going to be ignored.</param>
    public record class SchedulerOutputs(
        IEnumerable<ScheduledSequenceGroup> ScheduledSeqGroups, 
        int NumPrefillGroups, 
        int NumBatchedTokens, 
        IEnumerable<SequenceGroup> IgnoredSeqGroups
    )
    {
        /// <summary>
        /// Whether the scheduler output is empty.
        /// </summary>
        public bool IsEmpty => ScheduledSeqGroups.Count() == 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="SeqGroup">A sequence group that's scheduled.</param>
    /// <param name="TokenChunkSize">
    /// The total chunk size (number of tokens) to process for next iteration.
    /// 1 for decoding. Same as prompt tokens for prefill, but if prefill is
    /// chunked, it can be smaller than that.
    /// </param>
    public record class ScheduledSequenceGroup(SequenceGroup SeqGroup, int TokenChunkSize);

    /// <summary>
    /// The requests that are scheduled from a waiting queue.
    /// </summary>
    /// <param name="RemainingWaitingQueue"></param>
    /// <param name="SeqGroups"></param>
    /// <param name="IgnoredSeqGroups"></param>
    public record class SchedulerPrefillOutputs(
        LinkedList<SequenceGroup> RemainingWaitingQueue, 
        List<ScheduledSequenceGroup> SeqGroups, 
        List<SequenceGroup> IgnoredSeqGroups
    )
    {
        /// <inheritdoc/>
        public static SchedulerPrefillOutputs CreateEmpty()
        {
            return new SchedulerPrefillOutputs(
                new LinkedList<SequenceGroup>(), 
                new List<ScheduledSequenceGroup>(), 
                new List<SequenceGroup>()
            );
        }
    }

    /// <summary>
    /// The requests that are scheduled from a running queue.
    /// 
    /// Could contain prefill (prefill that's chunked) or decodes. If there's not
    /// enough memory, it can be preempted (for recompute) or swapped out.
    /// </summary>
    /// <param name="RemainingRunningQueue"></param>
    /// <param name="DecodeSeqGroups"></param>
    /// <param name="PrefillSeqGroups"></param>
    /// <param name="PreemptedSeqGroups"></param>
    /// <param name="SwappedOutSeqGroups"></param>
    public record class SchedulerRunningOutputs(
        LinkedList<SequenceGroup> RemainingRunningQueue, 
        List<ScheduledSequenceGroup> DecodeSeqGroups, 
        List<ScheduledSequenceGroup> PrefillSeqGroups, 
        List<SequenceGroup> PreemptedSeqGroups, 
        List<SequenceGroup> SwappedOutSeqGroups
    )
    {
        /// <inheritdoc/>
        public static SchedulerRunningOutputs CreateEmpty()
        {
            return new SchedulerRunningOutputs(
                new LinkedList<SequenceGroup>(), 
                new List<ScheduledSequenceGroup>(), 
                new List<ScheduledSequenceGroup>(), 
                new List<SequenceGroup>(), 
                new List<SequenceGroup>()
            );
        }
    }

    /// <summary>
    /// The requests that are scheduled from a swap queue.
    /// Could contain prefill (prefill that's chunked) or decodes.
    /// </summary>
    /// <param name="DecodeSeqGroups"></param>
    /// <param name="PrefillSeqGroups"></param>
    public record class SchedulerSwappedInOutputs(
        List<ScheduledSequenceGroup> DecodeSeqGroups, 
        List<ScheduledSequenceGroup> PrefillSeqGroups
    )
    {
        /// <inheritdoc/>
        public static SchedulerSwappedInOutputs CreateEmpty()
        {
            return new SchedulerSwappedInOutputs(new List<ScheduledSequenceGroup>(), new List<ScheduledSequenceGroup>());
        }
    }
}
