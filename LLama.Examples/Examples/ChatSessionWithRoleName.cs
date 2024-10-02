using LLama.Common;
using LLama.Sampling;

namespace LLama.Examples.Examples;

public class ChatSessionWithRoleName
{
    public static async Task Run()
    {
        string modelPath = UserSettings.GetModelPath();

        var parameters = new ModelParams(modelPath)
        {
            GpuLayerCount = 5
        };
        using var model = await LLamaWeights.LoadFromFileAsync(parameters);
        using var context = model.CreateContext(parameters);
        var executor = new InteractiveExecutor(context);

        var chatHistoryJson = await File.ReadAllTextAsync("Assets/chat-with-bob.json");
        ChatHistory chatHistory = ChatHistory.FromJson(chatHistoryJson) ?? new ChatHistory();

        ChatSession session = new(executor, chatHistory);

        var inferenceParams = new InferenceParams
        {
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.9f
            },
            AntiPrompts = new List<string> { "User:" }
        };

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("The chat session has started.");

        // show the prompt
        Console.ForegroundColor = ConsoleColor.Green;
        string userInput = Console.ReadLine() ?? "";

        while (userInput != "exit")
        {
            await foreach (
                var text
                in session.ChatAsync(
                    new ChatHistory.Message(AuthorRole.User, userInput),
                    inferenceParams))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(text);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            userInput = Console.ReadLine() ?? "";

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
