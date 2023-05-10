using System;
using System.Collections.Generic;
using System.Text;

namespace LLama
{
    public interface IChatModel
    {
        string Name { get; }
        IEnumerable<string> Chat(string text, string? prompt = null);
        /// <summary>
        /// Init a prompt for chat and automatically produce the next prompt during the chat.
        /// </summary>
        /// <param name="prompt"></param>
        void InitChatPrompt(string prompt);
        void InitChatAntiprompt(string[] antiprompt);
    }
}
