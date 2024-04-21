using LLama.Experimental.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Abstractions
{
    /// <summary>
    /// It defines how to execute the model.
    /// </summary>
    public interface IModelRunner: IDisposable
    {
        /// <summary>
        /// Deal with the scheduled sequences to get the output.
        /// </summary>
        /// <param name="seqGroupMetadataList"></param>
        /// <returns></returns>
        SamplerOutput ExecuteModel(IEnumerable<SequenceGroupMetadata> seqGroupMetadataList);
    }
}
