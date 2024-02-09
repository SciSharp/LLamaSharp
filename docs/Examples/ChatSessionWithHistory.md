# Chat session with history

```cs
using LLama.Common;

namespace LLama.Examples.Examples;

public class ChatSessionWithHistory
{
    public static async Task Run()
    {
        Console.Write("Please input your model path: ");
        var modelPath = Console.ReadLine();

        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 1024,
            Seed = 1337,
            GpuLayerCount = 5
        };
        using var model = LLamaWeights.LoadFromFile(parameters);
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

        InferenceParams inferenceParams = new InferenceParams()
        {
            Temperature = 0.9f,
            AntiPrompts = new List<string> { "User:" }
        };

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("The chat session has started.");

        // show the prompt
        Console.ForegroundColor = ConsoleColor.Green;
        string userInput = Console.ReadLine() ?? "";

        while (userInput != "exit")
        {
            if (userInput == "save")
            {
                session.SaveSession("Assets/chat-with-bob");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Session saved.");
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


```