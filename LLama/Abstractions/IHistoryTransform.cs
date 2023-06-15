using LLama.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Abstractions
{
    public interface IHistoryTransform
    {
        string HistoryToText(ChatHistory history);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="history">The existing history.</param>
        /// <param name="role"></param>
        /// <param name="text"></param>
        /// <returns>The updated history.</returns>
        ChatHistory TextToHistory(AuthorRole role, string text);
    }
}
