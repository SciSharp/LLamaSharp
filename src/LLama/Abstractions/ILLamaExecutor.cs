using System.Collections.Generic;
using System.Threading;

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
        
        // LLava Section
        //
        /// <summary>
        /// Identify if it's a multi-modal model and there is a image to process.
        /// </summary>
        public bool IsMultiModal { get; }
        /// <summary>
        /// Multi-Modal Projections / Clip Model weights
        /// </summary>
        public LLavaWeights? ClipModel { get;  }

        /// <summary>
        /// List of images: List of images in byte array format.
        /// </summary>
        public List<byte[]> Images { get; }

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
