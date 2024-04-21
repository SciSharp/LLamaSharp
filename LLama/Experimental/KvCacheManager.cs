using LLama.Experimental.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental
{
    public class KvCacheManager
    {
        public bool CanAppendSlots(SequenceGroup seqGroup)
        {
            return true;
        }

        public AllocStatus CanAllocate(SequenceGroup seqGroup)
        {
            return AllocStatus.OK;
        }

        public void Allocate(SequenceGroup seqGroup)
        {
        }
    }

    public enum AllocStatus
    {
        OK, 
        Later, 
        Never
    }
}
