using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using LLama.Abstractions;
using LLama.Common;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Connectors.AI.LLama.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.LLama.TextCompletion;

namespace LLama.Examples.NewVersion
{
    public class SemanticKernelChat
    {
        public static async Task Run()
        {
            Console.WriteLine("Example from: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/README.md");
            Console.Write("Please input your model path: ");
            var modelPath = Console.ReadLine();

            // Load weights into memory
            var parameters = new ModelParams(modelPath)
            {
                Seed = RandomNumberGenerator.GetInt32(int.MaxValue),
            };
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var ex = new InteractiveExecutor(context);
            //var builder = new KernelBuilder();
            //builder.WithAIService<IChatCompletion>("local-llama", new LLamaSharpChatCompletion(ex), true);
            //var kernel = builder.Build();

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
            var message = chatHistory.Messages.Last();

            Console.WriteLine($"{message.Role}: {message.Content}");
            Console.WriteLine("------------------------");

            return Task.CompletedTask;
        }
    }
}
