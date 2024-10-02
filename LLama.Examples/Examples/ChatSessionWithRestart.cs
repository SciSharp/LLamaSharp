using LLama.Common;
using LLama.Sampling;

namespace LLama.Examples.Examples;

public class ChatSessionWithRestart
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
        ChatSession prototypeSession = 
            await ChatSession.InitializeSessionFromHistoryAsync(executor, chatHistory);
        prototypeSession.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(
            new string[] { "User:", "Assistant:" },
            redundancyLength: 8));
        var resetState = prototypeSession.GetSessionState();

        ChatSession session = new ChatSession(executor);
        session.LoadSession(resetState);

        var inferenceParams = new InferenceParams
        {
            SamplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.9f
            },
            AntiPrompts = new List<string> { "User:" }
        };

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("The chat session has started. Starting point saved.");
        Console.WriteLine("Type 'exit' to end the chat session.");
        Console.WriteLine("Type 'save' to save chat session state in memory.");
        Console.WriteLine("Type 'reset' to reset the chat session to its saved state.");
        Console.WriteLine("Type 'answer for assistant' to add and process provided user and assistant messages.");

        // show the prompt
        Console.ForegroundColor = ConsoleColor.Green;
        string userInput = Console.ReadLine() ?? "";

        while (userInput != "exit")
        {
            // Load the session state from the reset state
            if(userInput == "reset")
            {
                session.LoadSession(resetState);
                Console.WriteLine($"Reset to history:\n{session.HistoryTransform.HistoryToText(session.History)}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Session reset.");
            }
            // Assign new reset state.
            else if (userInput == "save")
            {
                resetState = session.GetSessionState();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Session saved.");
            }
            // Provide user and override assistant answer with your own.
            else if (userInput == "answer for assistant")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Provide user input: ");

                Console.ForegroundColor = ConsoleColor.Green;
                string userInputOverride = Console.ReadLine() ?? "";

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Provide assistant input: ");
                
                Console.ForegroundColor = ConsoleColor.Green;
                string assistantInputOverride = Console.ReadLine() ?? "";
                
                await session.AddAndProcessUserMessage(userInputOverride);
                await session.AddAndProcessAssistantMessage(assistantInputOverride);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("User and assistant messages processed. Provide next user message:");
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
