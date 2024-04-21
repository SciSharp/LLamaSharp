using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// The sequence stage for <see cref="Sequence"/>.
    /// </summary>
    public enum SequenceStage
    {
        /// <summary>
        /// The prefill stage, in which the model is processing your prompt.
        /// </summary>
        Prefill, 

        /// <summary>
        /// The decode stage, in which the model is generating the output.
        /// </summary>
        Decode
    }
}
