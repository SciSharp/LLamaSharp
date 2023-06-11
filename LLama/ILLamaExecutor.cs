using LLama.Abstractions.Params;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LLama
{
    public interface ILLamaExecutor
    {
        public LLamaModel Model { get; }
        IEnumerable<string> Infer(string text, SessionParams? sessionParams = null);

        IAsyncEnumerable<string> InferAsync(string text, SessionParams? sessionParams = null, CancellationToken token = default);
    }
}
