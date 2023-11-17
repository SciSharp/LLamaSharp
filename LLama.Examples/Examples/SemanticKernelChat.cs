using System.Security.Cryptography;
using LLama.Common;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using LLamaSharp.SemanticKernel.ChatCompletion;

namespace LLama.Examples.Examples
{
    public class SemanticKernelChat
    {
        public static async Task Run()
        {
            Console.WriteLine("Example from: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/README.md");
            Console.Write("Please input your model path: ");
            var modelPath = Console.ReadLine();

            // Load weights into memory
            var parameters = new ModelParams(modelPath);
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var ex = new InteractiveExecutor(context);

            var chatGPT = new LLamaSharpChatCompletion(ex);

            var chatHistory = chatGPT.CreateNewChat("You are a librarian, expert about books");

            Console.WriteLine("Chat content:");
            Console.WriteLine("------------------------");

            chatHistory.AddUserMessage("Hi, I'm looking for book suggestions");
            await MessageOutputAsync(chatHistory);

            // First bot assistant message
            string reply = await chatGPT.GenerateMessageAsync(chatHistory);
            chatHistory.AddAssistantMessage(reply);
            await MessageOutputAsync(chatHistory);

            // Second user message
            chatHistory.AddUserMessage("I love history and philosophy, I'd like to learn something new about Greece, any suggestion");
            await MessageOutputAsync(chatHistory);

            // Second bot assistant message
            reply = await chatGPT.GenerateMessageAsync(chatHistory);
            chatHistory.AddAssistantMessage(reply);
            await MessageOutputAsync(chatHistory);
        }

        /// <summary>
        /// Outputs the last message of the chat history
        /// </summary>
        private static Task MessageOutputAsync(Microsoft.SemanticKernel.AI.ChatCompletion.ChatHistory chatHistory)
        {
            var message = chatHistory.Last();

            Console.WriteLine($"{message.Role}: {message.Content}");
            Console.WriteLine("------------------------");

            return Task.CompletedTask;
        }
    }
}
