using LLama.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LLama.Abstractions
{
    /// <summary>
    /// A high level interface for LLama models.
    /// </summary>
    public interface ILLamaExecutor
    {
        /// <summary>
        /// The loaded model for this executor.
        /// </summary>
        public LLamaModel Model { get; }
        /// <summary>
        /// Infers a response from the model.
        /// </summary>
        /// <param name="text">Your prompt</param>
        /// <param name="inferenceParams">Any additional parameters</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns></returns>
        IEnumerable<string> Infer(string text, InferenceParams? inferenceParams = null, CancellationToken token = default);

        IAsyncEnumerable<string> InferAsync(string text, InferenceParams? inferenceParams = null, CancellationToken token = default);
    }
}
