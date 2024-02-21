using Spectre.Console;
using LLama.Examples.Examples;

public class ExampleRunner
{
    private static readonly Dictionary<string, Func<Task>> Examples = new()
    {
        { "Run a chat session with history.", ChatSessionWithHistory.Run },
        { "Run a chat session without stripping the role names.", ChatSessionWithRoleName.Run },
        { "Run a chat session with the role names stripped.", ChatSessionStripRoleName.Run },
        { "Run a chat session in Chinese GB2312 encoding", ChatChineseGB2312.Run },
        { "Interactive mode chat by using executor.", InteractiveModeExecute.Run },
        { "Instruct mode chat by using executor.", InstructModeExecute.Run },
        { "Stateless mode chat by using executor.", StatelessModeExecute.Run },
        { "Load and save chat session.", SaveAndLoadSession.Run },
        { "Load and save state of model and executor.", LoadAndSaveState.Run },
        { "Get embeddings from LLama model.", () => Task.Run(GetEmbeddings.Run) },
        { "Quantize the model.", () => Task.Run(QuantizeModel.Run) },
        { "Automatic conversation.", TalkToYourself.Run },
        { "Constrain response to json format using grammar.", GrammarJsonResponse.Run },
        { "Semantic Kernel Prompt.", SemanticKernelPrompt.Run },
        { "Semantic Kernel Chat.", SemanticKernelChat.Run },
        { "Semantic Kernel Memory.", SemanticKernelMemory.Run },
        { "Coding Assistant.", CodingAssistant.Run },
        { "Batched Executor (Fork)", BatchedExecutorFork.Run },
        { "Batched Executor (Rewind)", BatchedExecutorRewind.Run },
        { "SK Kernel Memory.", KernelMemory.Run },
        { "Exit", () => { Environment.Exit(0); return Task.CompletedTask; } }
    };

    public static async Task Run()
    {
        AnsiConsole.Write(new Rule("LLamaSharp Examples"));

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please choose[green] an example[/] to run: ")
                    .AddChoices(Examples.Keys));

            if (Examples.TryGetValue(choice, out var example))
            {
                AnsiConsole.Write(new Rule(choice));
                await example();
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            AnsiConsole.Clear();
        }
    }
}
