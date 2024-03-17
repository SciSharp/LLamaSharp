using LLama.Common;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LLama.Abstractions
{
    /// <summary>
    /// Takes a stream of tokens and transforms them.
    /// </summary>
    [JsonConverter(typeof(PolymorphicJSONConverter<ITextStreamTransform>))]
    public interface ITextStreamTransform
    {
        /// <summary>
        /// Takes a stream of tokens and transforms them, returning a new stream of tokens asynchronously.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        IAsyncEnumerable<string> TransformAsync(IAsyncEnumerable<string> tokens);

        /// <summary>
        /// Copy the transform.
        /// </summary>
        /// <returns></returns>
        ITextStreamTransform Clone();
    }
}
