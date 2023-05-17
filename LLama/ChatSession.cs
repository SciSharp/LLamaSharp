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

        public IEnumerable<string> Chat(string text, string? prompt = null, string encoding = "UTF-8")
        {
            History.Add(new ChatMessageRecord(new ChatCompletionMessage(ChatRole.Human, text), DateTime.Now));
            string totalResponse = "";
            foreach(var response in _model.Chat(text, prompt, encoding))
            {
                totalResponse += response;
                yield return response;
            }
            History.Add(new ChatMessageRecord(new ChatCompletionMessage(ChatRole.Assistant, totalResponse), DateTime.Now));
        }

        public ChatSession<T> WithPrompt(string prompt, string encoding = "UTF-8")
        {
            _model.InitChatPrompt(prompt, encoding);
            return this;
        }

        public ChatSession<T> WithPromptFile(string promptFilename, string encoding = "UTF-8")
        {
            return WithPrompt(File.ReadAllText(promptFilename), encoding);
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
