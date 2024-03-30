using System.Text.Json.Serialization;
using LLama.Common;

namespace LLama.Abstractions
{
    /// <summary>
    /// An interface for text transformations.
    /// These can be used to compose a pipeline of text transformations, such as:
    /// - Tokenization
    /// - Lowercasing
    /// - Punctuation removal
    /// - Trimming
    /// - etc.
    /// </summary>
    [JsonConverter(typeof(PolymorphicJSONConverter<ITextTransform>))]
    public interface ITextTransform
    {
        /// <summary>
        /// Takes a string and transforms it.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string Transform(string text);

        /// <summary>
        /// Copy the transform.
        /// </summary>
        /// <returns></returns>
        ITextTransform Clone();
    }
}
