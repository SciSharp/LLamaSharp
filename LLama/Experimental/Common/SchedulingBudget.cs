using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// The available slots for scheduling.
    /// </summary>
    internal class SchedulingBudget
    {
        private HashSet<string> _requestIdsNumBatchedTokens;

        private HashSet<string> _requestIdsNumCurrentSeqs;

        public int TokenBudget { get; set; }

        public int MaxNumSeqs { get; set; }

        public int RemainingTokenBudget => TokenBudget - NumBatchedTokens;

        internal int NumCurrentSeqs { get; set; }

        internal int NumBatchedTokens { get; set; }

        public SchedulingBudget(int tokenBudget, int maxNumSeqs)
        {
            TokenBudget = tokenBudget;
            MaxNumSeqs = maxNumSeqs;
            _requestIdsNumBatchedTokens = new HashSet<string>();
            _requestIdsNumCurrentSeqs = new HashSet<string>();
            NumCurrentSeqs = 0;
            NumBatchedTokens = 0;
        }

        public bool CanSchedule(int numNewTokens, int numNewSeqs)
        {
            Debug.Assert(numNewTokens >= 0);
            Debug.Assert(numNewSeqs >= 0);
            return NumBatchedTokens + numNewTokens <= TokenBudget
                && NumCurrentSeqs + numNewSeqs <= MaxNumSeqs;
        }

        public void AddNumBatchedTokens(string requestId, int numBatchedTokens)
        {
            if (_requestIdsNumBatchedTokens.Contains(requestId))
            {
                return;
            }

            _requestIdsNumBatchedTokens.Add(requestId);
            NumBatchedTokens += numBatchedTokens;
        }

        public void SubtractNumBatchedTokens(string requestId, int numBatchedTokens)
        {
            if (_requestIdsNumBatchedTokens.Contains(requestId))
            {
                _requestIdsNumBatchedTokens.Remove(requestId);
                NumBatchedTokens -= numBatchedTokens;
            }
        }

        public void AddNumSeqs(string requestId, int numCurrentSeqs)
        {
            if (_requestIdsNumCurrentSeqs.Contains(requestId))
            {
                return;
            }

            _requestIdsNumCurrentSeqs.Add(requestId);
            NumCurrentSeqs += numCurrentSeqs;
        }

        public void SubtractNumSeqs(string requestId, int numCurrentSeqs)
        {
            if (_requestIdsNumCurrentSeqs.Contains(requestId))
            {
                _requestIdsNumCurrentSeqs.Remove(requestId);
                NumCurrentSeqs -= numCurrentSeqs;
            }
        }
    }
}
