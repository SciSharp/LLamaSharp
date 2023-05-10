using LLama.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LLama
{
    public class ChatSession<T> where T: IChatModel
    {
        IChatModel _model;
        List<ChatMessageRecord> History { get; } = new List<ChatMessageRecord>();
        
        public ChatSession(T model)
        {
            _model = model;
        }

        public IEnumerable<string> Chat(string text, string? prompt = null)
        {
            return _model.Chat(text, prompt);
        }

        public ChatSession<T> WithPrompt(string prompt)
        {
            _model.InitChatPrompt(prompt);
            return this;
        }

        public ChatSession<T> WithPromptFile(string promptFilename)
        {
            return WithPrompt(File.ReadAllText(promptFilename));
        }

        /// <summary>
        /// Set the keyword to split the return value of chat AI.
        /// </summary>
        /// <param name="humanName"></param>
        /// <returns></returns>
        public ChatSession<T> WithAntiprompt(string[] antiprompt)
        {
            _model.InitChatAntiprompt(antiprompt);
            return this;
        }
    }
}
