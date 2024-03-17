using LLama.Common;

namespace LLama.Examples.Examples;

public class ChatSessionWithRestart
{
    public static async Task Run()
    {
        string modelPath = UserSettings.GetModelPath();

        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 1024,
            Seed = 1337,
            GpuLayerCount = 5
        };
        using var model = LLamaWeights.LoadFromFile(parameters);
        using var context = model.CreateContext(parameters);
        var executor = new InteractiveExecutor(context);

        var chatHistoryJson = File.ReadAllText("Assets/chat-with-bob.json");
        ChatHistory chatHistory = ChatHistory.FromJson(chatHistoryJson) ?? new ChatHistory();
        ChatSession prototypeSession = 
            await ChatSession.InitializeSessionFromHistoryAsync(executor, chatHistory);
        prototypeSession.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(
            new string[] { "User:", "Assistant:" },
            redundancyLength: 8));
        var resetState = prototypeSession.GetSessionState();

        ChatSession session = new ChatSession(executor);
        session.LoadSession(resetState);

        InferenceParams inferenceParams = new InferenceParams()
        {
            Temperature = 0.9f,
            AntiPrompts = new List<string> { "User:" }
        };

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("The chat session has started. Write `save` to save session in memory."
            + " Write `reset` to start from the last saved checkpoint");

        // show the prompt
        Console.ForegroundColor = ConsoleColor.Green;
        string userInput = Console.ReadLine() ?? "";

        while (userInput != "exit")
        {
            if(userInput == "reset")
            {
                session.LoadSession(resetState);
                Console.WriteLine($"Reset to history:\n{session.HistoryTransform.HistoryToText(session.History)}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Session reset.");
            }
            else if (userInput == "save")
            {
                resetState = session.GetSessionState();
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
