using LLama.Common;
using LLamaSharp.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LLama.Examples.Examples
{
    public class SemanticKernelChat
    {
        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("This example is from: \n" +
                "https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example17_ChatGPT.cs");

            // Load weights into memory
            var parameters = new ModelParams(modelPath);
            using var model = LLamaWeights.LoadFromFile(parameters);
            var ex = new StatelessExecutor(model, parameters);

            var chatGPT = new LLamaSharpChatCompletion(ex);

            var chatHistory = chatGPT.CreateNewChat("This is a conversation between the " +
                "assistant and the user. \n\n You are a librarian, expert about books. ");

            Console.WriteLine("Chat content:");
            Console.WriteLine("------------------------");

            chatHistory.AddUserMessage("Hi, I'm looking for book suggestions");
            await MessageOutputAsync(chatHistory);

            // First bot assistant message
            var reply = await chatGPT.GetChatMessageContentAsync(chatHistory);
            chatHistory.AddAssistantMessage(reply.Content);
            await MessageOutputAsync(chatHistory);

            // Second user message
            chatHistory.AddUserMessage("I love history and philosophy, I'd like to learn " +
                "something new about Greece, any suggestion");
            await MessageOutputAsync(chatHistory);

            // Second bot assistant message
            reply = await chatGPT.GetChatMessageContentAsync(chatHistory);
            chatHistory.AddAssistantMessage(reply.Content);
            await MessageOutputAsync(chatHistory);
        }

        /// <summary>
        /// Outputs the last message of the chat history
        /// </summary>
        private static Task MessageOutputAsync(Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory)
        {
            var message = chatHistory.Last();

            Console.WriteLine($"{message.Role}: {message.Content}");
            Console.WriteLine("------------------------");

            return Task.CompletedTask;
        }
    }
}
