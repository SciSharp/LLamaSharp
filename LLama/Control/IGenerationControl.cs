using LLama.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Control
{
    /// <summary>
    /// Control the text generation of LLama Executors.
    /// </summary>
    public interface IGenerationControl
    {
        /// <summary>
        /// Use the last output text to determine if the generation should stop.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inferenceParams"></param>
        /// <param name="lastOutputText"></param>
        /// <returns></returns>
        bool ShouldStopGeneration(LLamaContext context, IInferenceParams inferenceParams, string lastOutputText);

        /// <summary>
        /// Use the last output ids to determine if the generation should stop.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inferenceParams"></param>
        /// <param name="lastOutputIds"></param>
        /// <returns></returns>
        bool ShouldStopGeneration(LLamaContext context, IInferenceParams inferenceParams, IEnumerable<int> lastOutputIds);
    }
}
