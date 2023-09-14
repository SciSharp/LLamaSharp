using System.Collections.Generic;

namespace LLama.Abstractions
{
    /// <summary>
    /// Takes a stream of tokens and transforms them.
    /// </summary>
    public interface ITextStreamTransform
    {
        /// <summary>
        /// Takes a stream of tokens and transforms them, returning a new stream of tokens asynchronously.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        IAsyncEnumerable<string> TransformAsync(IAsyncEnumerable<string> tokens);
    }
}
