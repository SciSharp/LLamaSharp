using LLama.Experimental.Abstractions;
using LLama.Experimental.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Core
{
    // TODO: This is only the most simple implementation to run the test now. We should replace it in the future.
    public class SequenceLengthStoopingCriteria: IStoppingCriteria
    {
        private int _maxSequenceLength;

        public SequenceLengthStoopingCriteria(int maxSequenceLength)
        {
            _maxSequenceLength = maxSequenceLength;
        }

        public StoppingCriteriaOutput CheckStop(Sequence seq)
        {
            if(seq.Length >= _maxSequenceLength)
            {
                return new StoppingCriteriaOutput(SequenceStatus.FinishLengthCapped, null, null);
            }
            return new StoppingCriteriaOutput(seq.Status, null, null);
        }
    }
}
