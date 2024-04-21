using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// The model output associated with a sequence group.
    /// </summary>
    /// <param name="Samples"></param>
    public record class SequenceGroupOutput(List<SequenceOutput> Samples)
    {

    }
}
