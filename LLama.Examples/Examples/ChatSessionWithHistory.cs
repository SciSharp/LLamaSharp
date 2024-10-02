using LLama.Common;
using LLama.Sampling;

namespace LLama.Examples.Examples;

public class ChatSessionWithHistory
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

        ChatSession session;
        if (Directory.Exists("Assets/chat-with-bob"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Loading session from disk.");
            Console.ForegroundColor = ConsoleColor.White;

            session = new ChatSession(executor);
            session.LoadSession("Assets/chat-with-bob");
        }
        else
        {
            var chatHistoryJson = File.ReadAllText("Assets/chat-with-bob.json");
            ChatHistory chatHistory = ChatHistory.FromJson(chatHistoryJson) ?? new ChatHistory();

            session = new ChatSession(executor, chatHistory);
        }

        session.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(
            new string[] { "User:", "Assistant:" },
            redundancyLength: 8));

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
        Console.WriteLine("Type 'exit' to end the chat session.");
        Console.WriteLine("Type 'save' to save the chat session to disk.");
        Console.WriteLine("Type 'load' to load the chat session from disk.");
        Console.WriteLine("Type 'regenerate' to regenerate the last response.");

        // show the prompt
        Console.ForegroundColor = ConsoleColor.Green;
        string userInput = Console.ReadLine() ?? "";

        while (userInput != "exit")
        {
            // Save the chat state to disk
            if (userInput == "save")
            {
                session.SaveSession("Assets/chat-with-bob");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Session saved.");
            }
            // Load the chat state from disk
            else if (userInput == "load")
            {
                session.LoadSession("Assets/chat-with-bob");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Session loaded.");
            }
            else if (userInput == "regenerate")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Regenerating last response ...");

                await foreach (
                    var text
                    in session.RegenerateAssistantMessageAsync(
                        inferenceParams))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(text);
                }
            }
            else
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
            }

            Console.ForegroundColor = ConsoleColor.Green;
            userInput = Console.ReadLine() ?? "";

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
