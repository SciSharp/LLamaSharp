using LLama.Experimental.Abstractions;
using LLama.Experimental.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Core
{
    // TODO: This is only the most simple implementation to run the test now. We should replace it in the future.
    public class AntipromptStoppingCriteria: IStoppingCriteria
    {
        private string[] _antiprompts;

        public AntipromptStoppingCriteria(string[] antiprompts)
        {
            _antiprompts = antiprompts;
        }

        public StoppingCriteriaOutput CheckStop(Sequence seq)
        {
            foreach (var antiprompt in _antiprompts)
            {
                if (seq.OutputText.EndsWith(antiprompt))
                {
                    return new StoppingCriteriaOutput(SequenceStatus.FinishStopped, antiprompt, null);
                }
            }
            return new StoppingCriteriaOutput(seq.Status, null, null);
        }
    }
}
