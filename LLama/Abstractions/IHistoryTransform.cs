using LLama.Common;
using System.Text.Json.Serialization;

namespace LLama.Abstractions
{
    /// <summary>
    /// Transform history to plain text and vice versa.
    /// </summary>
    [JsonConverter(typeof(PolymorphicJSONConverter<IHistoryTransform>))]
    public interface IHistoryTransform
    {
        /// <summary>
        /// Convert a ChatHistory instance to plain text.
        /// </summary>
        /// <param name="history">The ChatHistory instance</param>
        /// <returns></returns>
        string HistoryToText(ChatHistory history);
        
        /// <summary>
        /// Converts plain text to a ChatHistory instance.
        /// </summary>
        /// <param name="role">The role for the author.</param>
        /// <param name="text">The chat history as plain text.</param>
        /// <returns>The updated history.</returns>
        ChatHistory TextToHistory(AuthorRole role, string text);

        /// <summary>
        /// Copy the transform.
        /// </summary>
        /// <returns></returns>
        IHistoryTransform Clone();
    }
}
