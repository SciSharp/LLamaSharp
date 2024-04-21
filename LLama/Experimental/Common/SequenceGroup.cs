using LLama.Experimental.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// A group of sequences that are generated from the same prompt.
    /// </summary>
    public sealed class SequenceGroup
        // TODO: Multi-modal data
    {
        /// <summary>
        /// The ID of the request.
        /// </summary>
        public string RequestId { get; }

        /// <summary>
        /// Mapping from seq ID to the sequence.
        /// </summary>
        public IDictionary<int, Sequence> SeqDict { get; }

        /// <summary>
        /// The sampling method to do the sampling.
        /// </summary>
        public ISamplingMethod SamplingMethod { get; set; }

        /// <summary>
        /// The stopping criteria to decide whether the generation of the sequence should be stopped.
        /// </summary>
        public IStoppingCriteria StoppingCriteria { get; set; }

        /// <summary>
        /// The metrics for the scheduling and inference of this sequence group.
        /// </summary>
        public RequestMetrics Metrics { get; }

        /// <summary>
        /// The common prompt of the sequences in this sequence group.
        /// </summary>
        public string? Prompt
        {
            get
            {
                // All sequences in the group should have the same prompt.
                // We use the prompt of an arbitrary sequence.
                return SeqDict.First().Value.Prompt;
            }
        }

        /// <summary>
        /// The prompt tokens of the sequences in this sequence group.
        /// </summary>
        public IList<int> PromptTokenIds
        {
            get
            {
                return SeqDict.First().Value.Data.PromptTokenIds;
            }
        }

        /// <summary>
        /// Whether the request of this sequence group has been finished.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return SeqDict.Values.All(seq => seq.IsFinished);
            }
        }

        /// <summary>
        /// Whether this sequence group is at prefill stage.
        /// </summary>
        public bool IsPrefill
        {
            get
            {
                return SeqDict.Values.First().IsPrefill;
            }
        }

        /// <summary>
        /// The number of sequences in this sequence group.
        /// </summary>
        public int NumSeqs => SeqDict.Count;

        /// <summary>
        /// The number of unfinished sequences in this sequence group.
        /// </summary>
        public int NumUnfinishedSeqs => GetUnfinishedSeqs().Count();

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceGroup"/> class.
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="sequences"></param>
        /// <param name="samplingMethod"></param>
        /// <param name="stoppingCriteria"></param>
        /// <param name="arrivalTime"></param>
        /// <exception cref="ArgumentException"></exception>
        public SequenceGroup(string requestId, Sequence[] sequences, ISamplingMethod samplingMethod, IStoppingCriteria stoppingCriteria, DateTime arrivalTime)
        {
            if(sequences.Length == 0)
            {
                throw new ArgumentException($"The sequences bypassed to SequenceGroup cannot be empty.");
            }

            RequestId = requestId;
            SeqDict = sequences.ToDictionary(Sequence => Sequence.Id, Sequence => Sequence); 
            SamplingMethod = samplingMethod;
            StoppingCriteria = stoppingCriteria;
            Metrics = new RequestMetrics()
            {
                ArrivalTime = arrivalTime
            };
        }

        /// <summary>
        /// Sets the first token time for Request level timings.
        /// </summary>
        /// <param name="time"></param>
        public void MaybeSetFirstTokenTime(DateTime time)
        {
            if (Metrics.FirstTokenTime is null)
            {
                Metrics.FirstTokenTime = time;
            }
        }

        /// <summary>
        /// Sets the first scheduled time and time in queue for Request level timings
        /// </summary>
        /// <param name="time"></param>
        public void MaybeSetFirstScheduledTime(DateTime time)
        {
            if (Metrics.FirstScheduledTime is null)
            {
                Metrics.FirstScheduledTime = time;
                Metrics.TimeInQueue = time - Metrics.ArrivalTime;
            }
        }

        /// <summary>
        /// Sets the finished time for Request level timings.
        /// </summary>
        /// <param name="time"></param>
        public void SetFinishedTime(DateTime time)
        {
            Metrics.FinishedTime = time;
        }

        /// <summary>
        /// Get all sequences with the given status.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IEnumerable<Sequence> GetSeqsWithStatus(SequenceStatus status)
        {
            return SeqDict.Values.Where(seq => seq.Status == status);
        }

        /// <summary>
        /// Get all sequences in this sequence group.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Sequence> GetAllSeqs()
        {
            return SeqDict.Values;
        }

        /// <summary>
        /// Get all unfinished sequences in this sequence group.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Sequence> GetUnfinishedSeqs()
        {
            return SeqDict.Values.Where(seq => !seq.IsFinished);
        }

        /// <summary>
        /// Get all finished sequences in this sequence group.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Sequence> GetFinishedSeqs()
        {
            return SeqDict.Values.Where(seq => seq.IsFinished);
        }

        /// <summary>
        /// The maximum number of sequences running in parallel in the remaining
        /// lifetime of the request.
        /// </summary>
        /// <returns></returns>
        public int GetMaxNumRunningSeqs()
        {
            int defaultValue = NumUnfinishedSeqs;
            return SamplingMethod.GetMaxNumRunningSeqs(defaultValue, NumSeqs);
        }

        /// <summary>
        /// Add a new sequence.
        /// </summary>
        /// <param name="seq"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(Sequence seq)
        {
            if (SeqDict.ContainsKey(seq.Id))
            {
                throw new ArgumentException($"Sequence {seq.Id} already exists.");
            }
            SeqDict[seq.Id] = seq;
        }

        /// <summary>
        /// Remove the sequence of seq id.
        /// </summary>
        /// <param name="seqId"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Remove(int seqId)
        {
            if (!SeqDict.ContainsKey(seqId))
            {
                throw new ArgumentException($"Sequence {seqId} not found.");
            }
            SeqDict.Remove(seqId);
        }
        
        /// <summary>
        /// Get the number of tokens to be computed in this sequence group.
        /// </summary>
        public int GetNumComputedTokens()
        {
            int numUncomputedTokens = 0;
            foreach(var seq in GetAllSeqs())
            {
                numUncomputedTokens += seq.Data.NumUncomputedTokens;
            }
            return numUncomputedTokens;
        }

        /// <summary>
        /// Update number of tokens computed so far.
        /// </summary>
        /// <param name="numNewComputedTokens"></param>
        public void UpdateNumComputedTokens(int numNewComputedTokens)
        {
            foreach(var seq in SeqDict.Values)
            {
                if (!seq.IsFinished)
                {
                    seq.Data.UpdateNumComputedTokens(numNewComputedTokens);
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"SequenceGroup(RequestId = {RequestId}, \n    " +
                $"SamplingMethod = ({SamplingMethod.GetType().Name}), \n    " +
                $"StoppingCriteria = ({StoppingCriteria.GetType().Name}), \n    " +
                $"NumSeqs = {SeqDict.Count}\n)";
        }
    }
}
