using System.Collections.Generic;
using System.Threading;
using LLama.Native;

namespace LLama.Abstractions
{
    /// <summary>
    /// A high level interface for LLama models.
    /// </summary>
    public interface ILLamaExecutor
    {
        /// <summary>
        /// The loaded context for this executor.
        /// </summary>
        public LLamaContext Context { get; }
        
        // Multimodal Section

        /// <summary>
        /// Identify if it's a multi-modal model and there is a image to process.
        /// </summary>
        public bool IsMultiModal { get; }
        /// <summary>
        /// Multi-Modal Projections / Clip Model weights
        /// </summary>
        public MtmdWeights? ClipModel { get;  }

        /// <summary>
        /// List of media: List of media for Multi-Modal models.
        /// </summary>
        public List<SafeMtmdEmbed> Embeds { get; }

        /// <summary>
        /// Asynchronously infers a response from the model.
        /// </summary>
        /// <param name="text">Your prompt</param>
        /// <param name="inferenceParams">Any additional parameters</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns></returns>
        IAsyncEnumerable<string> InferAsync(string text, IInferenceParams? inferenceParams = null, CancellationToken token = default);
    }
}
