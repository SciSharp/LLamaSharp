using LLama.Abstractions;
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace LLama
{
    /// <summary>
    /// A class to execute text completion task.
    /// </summary>
    public class TextCompletion
    {
        public string Execute(string prompt, IInferenceParams? inferenceParams = null)
        {
            throw new NotImplementedException();
        }

        public ChatHistory Execute(ChatHistory prompt, IInferenceParams? inferenceParams = null)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<string> StreamingExecute(string prompt, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
