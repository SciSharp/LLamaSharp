using LLama.Experimental.Abstractions;
using LLama.Experimental.Common;
using LLama.Experimental.Config;
using LLama.Experimental.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LLama.Experimental
{
    /// <summary>
    /// The scheduler to schedule the requests for model inference.
    /// </summary>
    public sealed class Scheduler
        // TODO: LORA
    {
        private ILogger? _logger;

        /// <summary>
        /// Whether we schedule a prompt at previous step.
        /// </summary>
        private bool _prevIsPrompt;

        /// <summary>
        /// Latency of the last prompt step
        /// </summary>
        private float _lastPromptLatency;

        /// <summary>
        /// Time at previous scheduling step
        /// </summary>
        private DateTime _prevTime;

        /// <summary>
        /// Scheduler configuration.
        /// </summary>
        public SchedulerConfig SchedulerConfig { get; set; }

        /// <summary>
        /// KV cache configuration.
        /// </summary>
        public KvCacheConfig KvCacheConfig { get; set; }

        /// <summary>
        /// The maximumum prompt length that can be used. 
        /// It's deduced from the scheduler configuration.
        /// </summary>
        public int MaxPromptLength { get; set; }

        /// <summary>
        /// Sequence groups in the WAITING state. It contain new prefill or preempted requests.
        /// </summary>
        public LinkedList<SequenceGroup> Waiting { get; set; }

        /// <summary>
        /// Sequence groups in the RUNNING state. It contains the requests that is being decoded.
        /// </summary>
        public LinkedList<SequenceGroup> Running { get; set; }

        /// <summary>
        /// Sequence groups in the SWAPPED state. It contains decode requests that are swapped out.
        /// </summary>
        public LinkedList<SequenceGroup> Swapped { get; set; }

        public KvCacheManager KvCacheManager { get; }


        /// <summary>
        /// Create a scheduler. Note that this is not a high-level API. If you are an user, please 
        /// read the documentation and ensure you know what it does. 
        /// </summary>
        /// <param name="schedulerConfig"></param>
        /// <param name="kvCacheConfig"></param>
        /// <param name="logger"></param>
        public Scheduler(SchedulerConfig schedulerConfig, KvCacheConfig kvCacheConfig, ILogger? logger = null)
        {
            SchedulerConfig = schedulerConfig;
            KvCacheConfig = kvCacheConfig;

            if (SchedulerConfig.EnableChunkedPrefill)
            {
                MaxPromptLength = SchedulerConfig.MaxSequenceLength;
            }
            else
            {
                MaxPromptLength = Math.Min(SchedulerConfig.MaxSequenceLength, SchedulerConfig.MaxNumBatchedTokens);
            }

            Waiting = new LinkedList<SequenceGroup>();
            Running = new LinkedList<SequenceGroup>();
            Swapped = new LinkedList<SequenceGroup>();

            _logger = logger;

            // TODO: init with config
            KvCacheManager = new();
        }

        /// <summary>
        /// Add sequence groups to the waiting queue.
        /// </summary>
        /// <param name="seqGroup"></param>
        /// <returns></returns>
        public Scheduler AddSeqGroup(SequenceGroup seqGroup)
        {
            _logger?.LogDebug($"Added seq group {seqGroup.RequestId}");
            Waiting.AddBack(seqGroup);
            return this;
        }

        /// <summary>
        /// Aborts a sequence group with the given IDs.
        /// Check if the sequence group with the given ID
        ///    is present in any of the state queue.
        ///If present, remove the sequence group from the state queue.
        ///    Also, if any of the sequences in the sequence group is not finished,
        ///        free the sequence with status `FINISHED_ABORTED`.
        ///Otherwise, do nothing.
        /// </summary>
        /// <param name="requestIds"></param>
        /// <returns></returns>
        public Scheduler AbortSeqGroup(IEnumerable<string> requestIds)
        {
            var requestIdSet = new HashSet<string>(requestIds.Distinct());

            AbortInternal(Waiting, requestIdSet);
            AbortInternal(Running, requestIdSet);
            AbortInternal(Swapped, requestIdSet);
            return this;
        }

        /// <summary>
        /// Whether all sequences has been finished at this moment.
        /// </summary>
        /// <returns></returns>
        public bool HasUnfinishedSeqs()
        {
            return Waiting.Count != 0 || Running.Count != 0 || Swapped.Count != 0;
        }   

        /// <summary>
        /// Get the number of unfinished sequence groups.
        /// </summary>
        /// <returns></returns>
        public int GetNumUnfinishedSeqGroups()
        {
            return Waiting.Count + Running.Count + Swapped.Count;
        }

        /// <summary>
        /// Free the sequence resource that managed by the scheduler.
        /// It's actually an empty method now and may be implemented in the future if needed.
        /// </summary>
        /// <param name="seq"></param>
        public void FreeSeq(Sequence seq)
        {
            // TODO: implement it if needed.
        }

        /// <summary>
        /// Schedule sequence groups.
        /// This function call changes the internal states of the scheduler, 
        /// such as this.Running, this.Wwapped, and this.Waiting.
        /// </summary>
        /// <returns></returns>
        public (List<SequenceGroupMetadata>, SchedulerOutputs) Schedule()
        {
            var schedulerOutputs = ScheduleInternal();
            var now = DateTime.Now;

            // Create input data structures.
            List<SequenceGroupMetadata> seqGroupMetadataList = new();
            int i = 0;
            foreach(var scheduledSeqGroup in schedulerOutputs.ScheduledSeqGroups)
            {
                var seqGroup = scheduledSeqGroup.SeqGroup;
                var tokenChunkSize = scheduledSeqGroup.TokenChunkSize;
                seqGroup.MaybeSetFirstScheduledTime(now);

                Dictionary<int, SequenceData> seqData = new();

                foreach(var seq in seqGroup.GetSeqsWithStatus(SequenceStatus.Running))
                {
                    var seqId = seq.Id;
                    seqData[seqId] = seq.Data;
                }

                // It assumes the scheduled_seq_groups is ordered by prefill < decoding.
                bool isPrompt = i < schedulerOutputs.NumPrefillGroups;
                var seqGroupMetadata = new SequenceGroupMetadata(
                    seqGroup.RequestId, 
                    isPrompt, 
                    seqData, 
                    seqGroup.SamplingMethod, 
                    seqGroup.StoppingCriteria, 
                    tokenChunkSize
                );
                seqGroupMetadataList.Add(seqGroupMetadata);

                i++;
            }

            return (seqGroupMetadataList, schedulerOutputs);
        }

        /// <summary>
        /// Free finished sequence groups.
        /// </summary>
        public void FreeFinishedSeqGroups()
        {
            Running = new LinkedList<SequenceGroup>(Running.Where(x => !x.IsFinished));
        }

        /// <summary>
        /// Schedule queued requests.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private SchedulerOutputs ScheduleInternal()
        {
            if (SchedulerConfig.EnableChunkedPrefill)
            {
                // TODO: allow chunked prefill.
                throw new NotImplementedException();
            }
            else
            {
                return ScheduleDefault();
            }
        }

        /// <summary>
        /// Schedule queued requests.
        /// 
        /// The current policy is designed to opimimize the throughput. First,
        /// it batches as many prefill requests as possible.And it schedules
        /// decodes.If there's a pressure on GPU memory, decode requests can
        /// be swapped or preempted.
        /// </summary>
        /// <returns></returns>
        private SchedulerOutputs ScheduleDefault()
        {
            // Include running requests to the budget.
            var budget = new SchedulingBudget(SchedulerConfig.MaxNumBatchedTokens, SchedulerConfig.MaxNumSequences);
            // Make sure we include num running seqs before scheduling prefill,
            // so that we don't schedule beyond max_num_seqs for prefill.
            foreach(var seqGroup in Running)
            {
                budget.AddNumSeqs(seqGroup.RequestId, seqGroup.GetMaxNumRunningSeqs());
            }

            var remainingWaiting = Waiting;
            var prefills = SchedulerPrefillOutputs.CreateEmpty();
            var remainingRunning = Running;
            var runningScheduled = SchedulerRunningOutputs.CreateEmpty();
            var remainingSwapped = Swapped;
            var swappedIn = SchedulerSwappedInOutputs.CreateEmpty();

            if(Swapped.Count == 0)
            {
                prefills = SchedulePrefills(Waiting, budget, false);
                remainingWaiting = prefills.RemainingWaitingQueue;
            }

            var policy = PolicyFactory.DefaultPolicy;
            // Don't schedule decodes if prefills are scheduled.
            // NOTE: If `SchedulePrefills` doesn't enable chunking, this.Running
            // only contains decode requests, not chunked prefills.
            
            if(prefills.SeqGroups.Count == 0)
            {
                runningScheduled = ScheduleRunning(Running, budget, policy, false);
                remainingRunning = runningScheduled.RemainingRunningQueue;

                // If any sequence group is preempted, do not swap in any sequence group.
                // Because it means there's no slot for new running requests.
                if(runningScheduled.PreemptedSeqGroups.Count + runningScheduled.SwappedOutSeqGroups.Count == 0)
                {
                    // TODO: implement the swapping.
                }
            }

            Debug.Assert(budget.NumBatchedTokens <= SchedulerConfig.MaxNumBatchedTokens);
            Debug.Assert(budget.NumCurrentSeqs <= SchedulerConfig.MaxNumSequences);

            // Update waiting requests.
            Waiting = remainingWaiting;
            Waiting.ExtendFront(runningScheduled.PreemptedSeqGroups);
            // Update new running requests.
            Running = remainingRunning;
            Running.ExtendFront(prefills.SeqGroups.Select(x => x.SeqGroup));
            Running.ExtendBack(runningScheduled.DecodeSeqGroups.Select(x => x.SeqGroup));
            Running.ExtendBack(swappedIn.DecodeSeqGroups.Select(x => x.SeqGroup));
            // Update swapped requests.
            Swapped = remainingSwapped;
            Swapped.ExtendBack(runningScheduled.SwappedOutSeqGroups);

            // There should be no prefill from running queue because this policy
            // doesn't allow chunked prefills.
            Debug.Assert(runningScheduled.PrefillSeqGroups.Count == 0);
            Debug.Assert(swappedIn.PrefillSeqGroups.Count == 0);
            return new SchedulerOutputs(
                ScheduledSeqGroups: prefills.SeqGroups.Concat(runningScheduled.DecodeSeqGroups).Concat(swappedIn.DecodeSeqGroups),
                NumPrefillGroups: prefills.SeqGroups.Count,
                NumBatchedTokens: budget.NumBatchedTokens,
                IgnoredSeqGroups: prefills.IgnoredSeqGroups
            );
        }

        private SchedulerSwappedInOutputs ScheduleSwapped(LinkedList<SequenceGroup> swappedQueue, SchedulingBudget budget, ISchedulingPolicy policy, bool enableChunking)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Schedule sequence groups that are in prefill stage.
        /// 
        /// Note that the current scheduler treats PREEMPTED_FOR_RECOMPUTE
        /// as a new prefill(that starts from beginning -> most recently generated
        /// tokens).
        /// 
        /// It schedules waiting requests as long as it fits `budget` and
        /// curr_loras smaller than or equal with max_lora from the scheduling config.The input arguments
        /// `budget` and `curr_loras` are updated based on scheduled seq_groups.
        /// </summary>
        /// <param name="waiting">The queue that contains prefill requests. The given arguments are NOT in-place modified.</param>
        /// <param name="budget">The scheduling budget. The argument is in-place updated when any requests are scheduled.</param>
        /// <param name="enableChunking">
        /// If True, seq group can be chunked and only a chunked number of tokens are scheduled  if
        ///  <see cref="SchedulingBudget.NumBatchedTokens"/> has not enough capacity to schedule all tokens.
        /// </param>
        /// <returns></returns>
        private SchedulerPrefillOutputs SchedulePrefills(LinkedList<SequenceGroup> waiting, SchedulingBudget budget, bool enableChunking = false)
        {
            List<SequenceGroup> ignoredSeqGroups = new();
            List<ScheduledSequenceGroup> seqGroups = new();
            // We don't sort waiting queue because we assume it is sorted.
            // Copy the queue so that the input queue is not modified.
            var waitingQueue = new LinkedList<SequenceGroup>(waiting);

            LinkedList<SequenceGroup> leftoverWaitingSequences = new();
            while(PassedDelay(DateTime.Now) && waitingQueue.Count > 0)
            {
                var seqGroup = waitingQueue.PeekFront();

                var waitingSeqs = seqGroup.GetSeqsWithStatus(SequenceStatus.Waiting);
                Debug.Assert(waitingSeqs.Count() == 1, "Waiting sequence group should have only one prompt sequence.");
                var numNewTokens = GetNumNewTokens(seqGroup, SequenceStatus.Waiting, enableChunking, budget);
                if (!enableChunking)
                {
                    var numPromptTokens = waitingSeqs.First().Length;
                    Debug.Assert(numNewTokens == numPromptTokens);
                }

                if (numNewTokens > MaxPromptLength)
                {
                    _logger?.LogWarning($"Input prompt ({numNewTokens} tokens) is too long " +
                        $"and exceeds limit of {MaxPromptLength}.");
                    foreach(var seq in waitingSeqs)
                    {
                        seq.Status = SequenceStatus.FinishIgnored;
                    }
                    ignoredSeqGroups.Add(seqGroup);
                    waitingQueue.RemoveFront();
                    continue;
                }

                // If the sequence group cannot be allocated, stop.
                var canAlloc = KvCacheManager.CanAllocate(seqGroup);
                if(canAlloc == AllocStatus.Later)
                {
                    break;
                }
                else if(canAlloc == AllocStatus.Never)
                {
                    _logger?.LogWarning($"Input prompt ({numNewTokens} tokens) is too long" +
                        " and exceeds the capacity of block_manager");
                    foreach(var seq in waitingSeqs)
                    {
                        seq.Status = SequenceStatus.FinishIgnored;
                    }
                    ignoredSeqGroups.Add(seqGroup);
                    waitingQueue.RemoveFront();
                    continue;
                }

                var numNewSeqs = seqGroup.GetMaxNumRunningSeqs();
                if(numNewTokens == 0 || !budget.CanSchedule(numNewTokens, numNewSeqs))
                {
                    break;
                }

                // Can schedule this request.
                waitingQueue.RemoveFront();
                AllocateAndSetRunning(seqGroup, numNewTokens);
                seqGroups.Add(new ScheduledSequenceGroup(seqGroup, numNewTokens));
                budget.AddNumBatchedTokens(seqGroup.RequestId, numNewTokens);
                budget.AddNumSeqs(seqGroup.RequestId, numNewSeqs);
            }

            waitingQueue.ExtendFront(leftoverWaitingSequences);
            if(seqGroups.Count > 0)
            {
                _prevIsPrompt = true;
            }

            return new SchedulerPrefillOutputs(waitingQueue, seqGroups, ignoredSeqGroups);
        }

        /// <summary>
        /// Schedule sequence groups that are running.
        /// 
        /// Running queue should include decode and chunked prefill requests.
        /// </summary>
        /// <param name="runningQueue">
        /// The queue that contains running requests (i.e., decodes). 
        /// The given arguments are NOT in-place modified.
        /// </param>
        /// <param name="budget">
        /// The scheduling budget. The argument is in-place updated 
        /// when any decodes are preempted.
        /// </param>
        /// <param name="policy">The sorting policy to sort running_queue.</param>
        /// <param name="enableChunking">
        /// If True, seq group can be chunked and only a chunked number of tokens are scheduled  if
        /// `budget.num_batched_tokens` has not enough capacity to schedule all tokens.
        /// </param>
        /// <returns></returns>
        private SchedulerRunningOutputs ScheduleRunning(LinkedList<SequenceGroup> runningQueue, SchedulingBudget budget, 
            ISchedulingPolicy policy, bool enableChunking)
        {
            List<ScheduledSequenceGroup> decodeSeqGroups = new();
            List<ScheduledSequenceGroup> prefillSeqGroups = new();
            List<SequenceGroup> preempted = new();
            List<SequenceGroup> swappedOut = new();

            //NOTE: Preemption happens only when there is no available slot
            //to keep all the sequence groups in the RUNNING state.
            //In this case, the policy is responsible for deciding which sequence
            //groups to preempt.
            var now = DateTime.Now;
            runningQueue = policy.SortByPriority(now, runningQueue);

            while(runningQueue.Count > 0)
            {
                var seqGroup = runningQueue.PeekFront();
                var numRunningTokens = GetNumNewTokens(seqGroup, SequenceStatus.Running, enableChunking, budget);

                // We can have up to 1 running prefill at any given time in running
                // queue, which means we can guarantee chunk size is at least 1.
                Debug.Assert(numRunningTokens != 0);
                var numRunningSeqs = seqGroup.GetMaxNumRunningSeqs();

                runningQueue.RemoveFront();
                bool appended = true;
                while (!CanAppendSlots(seqGroup))
                {
                    // TODO: implement the preemption logic
                    Debug.Assert(false);
                }

                if (appended)
                {
                    _logger?.LogDebug($"append slot for {seqGroup}");
                    AppendSlots(seqGroup);
                    if (seqGroup.IsPrefill)
                    {
                        prefillSeqGroups.Add(new ScheduledSequenceGroup(seqGroup, numRunningTokens));
                    }
                    else
                    {
                        decodeSeqGroups.Add(new ScheduledSequenceGroup(seqGroup, 1));
                    }
                    budget.AddNumBatchedTokens(seqGroup.RequestId, numRunningTokens);
                    budget.AddNumSeqs(seqGroup.RequestId, numRunningSeqs);
                }
            }

            Debug.Assert(runningQueue.Count == 0);
            return new SchedulerRunningOutputs(runningQueue, decodeSeqGroups, prefillSeqGroups, preempted, swappedOut);
        }

        private void AllocateAndSetRunning(SequenceGroup seqGroup, int numNewTokens)
        {
            KvCacheManager.Allocate(seqGroup);
            foreach (var seq in seqGroup.GetSeqsWithStatus(SequenceStatus.Waiting))
            {
                seq.Status = SequenceStatus.Running;
            }
        }

        private bool PassedDelay(DateTime now)
        {
            if (_prevIsPrompt)
            {
                _lastPromptLatency = (now - _prevTime).Milliseconds;
            }
            _prevTime = now;
            _prevIsPrompt = false;
            // Delay scheduling prompts to let waiting queue fill up
            if (SchedulerConfig.DelayFactor > 0 && Waiting.Count > 0)
            {
                var earliestArrivalTime = Waiting.Select(x => x.Metrics.ArrivalTime).Min();
                return (now - earliestArrivalTime).Milliseconds > (SchedulerConfig.DelayFactor * _lastPromptLatency) || Running.Count == 0;
            }
            return true;
        }

        private bool CanAppendSlots(SequenceGroup seqGroup)
        {
            return KvCacheManager.CanAppendSlots(seqGroup);
        }

        private void AppendSlots(SequenceGroup seqGroup)
        {
            // TODO: Implement this method
        }

        /// <summary>
        /// Get the next new tokens to compute for a given sequence group that's in a given `status`.
        /// 
        /// The API could chunk the number of tokens to compute based on `budget`
        /// if `enable_chunking` is True.If a sequence group has multiple
        /// sequences(e.g., running beam search), it means it is in decoding
        /// phase, so chunking doesn't happen.
        /// </summary>
        /// <param name="seqGroup"></param>
        /// <param name="status"></param>
        /// <param name="enableChunking"></param>
        /// <param name="budget"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private int GetNumNewTokens(SequenceGroup seqGroup, SequenceStatus status, bool enableChunking, SchedulingBudget budget)
        {
            int numNewTokens = 0;
            var seqs = seqGroup.GetSeqsWithStatus(status);
            foreach(var seq in seqs)
            {
                numNewTokens += seq.NumNewTokens;
            }
            // Chunk if a running request cannot fit in.
            // If number of seq > 1, it means it is doing beam search in a
            // decode phase. Do not chunk in that case.
            if(enableChunking && seqs.Count() == 1)
            {
                numNewTokens = Math.Min(numNewTokens, budget.RemainingTokenBudget);
            }
            return numNewTokens;
        }

        private void AbortInternal(LinkedList<SequenceGroup> queue, HashSet<string> requestIds)
        {
            Queue<SequenceGroup> abortedGroups = new();
            foreach (var seqGroup in queue)
            {
                if (requestIds.Count == 0)
                {
                    break;
                }
                if (requestIds.Contains(seqGroup.RequestId))
                {
                    _logger?.LogDebug($"Aborted seq group {seqGroup.RequestId}");
                    abortedGroups.Enqueue(seqGroup);
                    requestIds.Remove(seqGroup.RequestId);
                }
            }
            foreach(var abortGroup in abortedGroups)
            {
                queue.Remove(abortGroup);
                foreach(var seq in abortGroup.GetAllSeqs())
                {
                    if (seq.IsFinished)
                    {
                        continue;
                    }
                    seq.Status = SequenceStatus.FinishAborted;
                }
            }
        }
    }

    /// <summary>
    /// The mode of preemption.
    /// </summary>
    public enum PreemptionMode
    {
        /// <summary>
        /// Swap out the blocks of the preempted sequences to CPU memory
        /// and swap them back in when the sequences are resumed.
        /// </summary>
        Swap,

        /// <summary>
        /// Discard the blocks of the preempted sequences and recompute them 
        /// when the sequences are resumed, treating the sequences as new prompts.
        /// </summary>
        Recompute
    }
}
