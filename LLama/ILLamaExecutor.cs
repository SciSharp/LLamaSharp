using LLama.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LLama
{
    public interface ILLamaExecutor
    {
        public LLamaModel Model { get; }
        IEnumerable<string> Infer(string text, InferenceParams? inferenceParams = null, CancellationToken token = default);

        IAsyncEnumerable<string> InferAsync(string text, InferenceParams? inferenceParams = null, CancellationToken token = default);
    }
}
