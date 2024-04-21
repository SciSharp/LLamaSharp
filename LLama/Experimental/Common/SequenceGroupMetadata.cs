using LLama.Experimental.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// Metadata for a sequence group.
    /// </summary>
    public class SequenceGroupMetadata
    {
        /// <summary>
        /// The ID of the request.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Whether the request is at prompt stage.
        /// </summary>
        public bool IsPrompt { get; set; }

        /// <summary>
        /// The sequence data. (Seq id -> sequence data)
        /// </summary>
        public Dictionary<int, SequenceData> SeqData { get; set; }

        /// <summary>
        /// The sampling method used to generate the outputs.
        /// </summary>
        public ISamplingMethod SamplingMethod { get; set; }

        /// <summary>
        /// The stopping criteria to decide whether the generation of the sequence should be stopped.
        /// </summary>
        public IStoppingCriteria StoppingCriteria { get; set; }

        /// <summary>
        /// The number of tokens to be processed (per sequence). 
        /// </summary>
        public int TokenChunkSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="isPrompt"></param>
        /// <param name="seqData"></param>
        /// <param name="samplingMethod"></param>
        /// <param name="stoppingCriteria"></param>
        /// <param name="tokenChunkSize"></param>
        public SequenceGroupMetadata(string requestId, bool isPrompt, Dictionary<int, SequenceData> seqData, 
            ISamplingMethod samplingMethod, IStoppingCriteria stoppingCriteria, int? tokenChunkSize)
        {
            RequestId = requestId;
            IsPrompt = isPrompt;
            SeqData = seqData;
            SamplingMethod = samplingMethod;
            StoppingCriteria = stoppingCriteria;

            if(tokenChunkSize is null)
            {
                if (isPrompt)
                {
                    TokenChunkSize = seqData.Values.First().Length;
                }
                else
                {
                    TokenChunkSize = 1;
                }
            }
            else
            {
                TokenChunkSize = tokenChunkSize.Value;
            }
        }
    }
}
