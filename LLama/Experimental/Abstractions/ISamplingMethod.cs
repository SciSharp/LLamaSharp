using LLama.Experimental.Common;
using LLama.Experimental.Runner.LLamaCpp;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Abstractions
{
    /// <summary>
    /// Method to sample the model output.
    /// </summary>
    public interface ISamplingMethod
        // TODO: We should reconsider this design. Maybe it's better to use `SamplingParams` to let user set, 
        // and choose the actual sampler internally according to the params.
    {
        /// <summary>
        ///The maximum number of sequences running in parallel.
        ///
        /// If you don't know what to return, you can return the default value.
        /// 
        /// Generally, if you want to select several results from n results, you need 
        /// to set the maximum number of sequences to run.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="currentNumSeqs"></param>
        /// <returns></returns>
        int GetMaxNumRunningSeqs(int defaultValue, int currentNumSeqs);

        /// <summary>
        /// Whether to skip special tokens.
        /// </summary>
        bool SkipSpecialTokens { get; }

        /// <summary>
        /// Sample the sequence logits to get the token.
        /// </summary>
        /// <param name="logits"></param>
        /// <param name="seqId"></param>
        /// <param name="samplingMetadata"></param>
        /// <returns></returns>
        SequenceOutput SampleSequence(Span<float> logits, int seqId, SamplingMetadata samplingMetadata);
        // TODO: maybe we shouldn't expose all the samplingMetadata to users here.
    }
}
