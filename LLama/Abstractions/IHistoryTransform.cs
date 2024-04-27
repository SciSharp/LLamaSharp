using LLama.Common;
using System;
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
        string HistoryToText(IChatHistory history);

        /// <summary>
        /// Converts plain text to a ChatHistory instance.
        /// </summary>
        /// <param name="role">The role for the author.</param>
        /// <param name="text">The chat history as plain text.</param>
        /// <param name="type">The type of the chat history.</param>
        /// <returns>The updated history.</returns>
        IChatHistory TextToHistory(AuthorRole role, string text, Type type);

        /// <summary>
        /// Copy the transform.
        /// </summary>
        /// <returns></returns>
        IHistoryTransform Clone();
    }
}
