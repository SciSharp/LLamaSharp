using LLama.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Control
{
    /// <summary>
    /// The default generation control in LLamaSharp, using antiprompts. This class should not be inherited. 
    /// <b>Note that this class has state. The previous outputs feeded to it will affect its control.</b>
    /// If you use it in a session, please don't reuse it for another session unless you intend to do so.
    /// </summary>
    public sealed class DefaultGenerationControl: IGenerationControl
    {
        private AntipromptProcessor _antipromptProcessor;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DefaultGenerationControl()
        {
            _antipromptProcessor = new AntipromptProcessor();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool ShouldStopGeneration(LLamaContext context, IInferenceParams inferenceParams, string lastOutputText)
        {
            _antipromptProcessor.SetAntiprompts(inferenceParams.AntiPrompts);
            return _antipromptProcessor.Add(lastOutputText);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool ShouldStopGeneration(LLamaContext context, IInferenceParams inferenceParams, IEnumerable<int> lastOutputIds)
        {
            return false;
        }
    }
}
