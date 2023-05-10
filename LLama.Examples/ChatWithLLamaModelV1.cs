using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.Types;

namespace LLama.Examples
{
    public class ChatWithLLamaModelV1
    {
        LLamaModelV1 _model;
        public ChatWithLLamaModelV1(string modelPath)
        {
            _model = new(modelPath, logits_all: false, verbose: false, n_ctx: 512);
        }

        public void Run()
        {
            List<ChatCompletionMessage> chats = new List<ChatCompletionMessage>();
            chats.Add(new ChatCompletionMessage(ChatRole.Human, "Hi, Alice, I'm Rinne."));
            chats.Add(new ChatCompletionMessage(ChatRole.Assistant, "Hi, Rinne, I'm Alice, an assistant that answer any question. What can I do for you?"));
            while (true)
            {
                Console.Write("\nYou: ");
                Console.ForegroundColor = ConsoleColor.Green;
                var question = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                chats.Add(new ChatCompletionMessage(ChatRole.Human, question));
                var outputs = _model.CreateChatCompletion(chats, max_tokens: 256);
                Console.Write($"LLama AI: ");
                foreach (var output in outputs)
                {
                    Console.Write($"{output.Choices[0].Delta.Content}");
                }
            }
        }
    }
}
