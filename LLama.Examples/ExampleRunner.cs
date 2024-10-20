using Spectre.Console;
using LLama.Examples.Examples;

public class ExampleRunner
{
    private static readonly Dictionary<string, Func<Task>> Examples = new()
    {
        { "Chat Session: LLama3", LLama3ChatSession.Run },
        { "Chat Session: LLama2", LLama2ChatSession.Run },
        { "Chat Session: History", ChatSessionWithHistory.Run },
        { "Chat Session: Role names", ChatSessionWithRoleName.Run },
        { "Chat Session: Role names stripped", ChatSessionStripRoleName.Run },
        { "Chat Session: Pre-processing and reset", ChatSessionWithRestart.Run },
        { "Chat Session: Coding Assistant", CodingAssistant.Run },
        { "Chat Session: Automatic conversation", TalkToYourself.Run },
        { "Chat Session: Chinese characters", ChatChineseGB2312.Run },
        { "Executor: Interactive mode chat", InteractiveModeExecute.Run },
        { "Executor: Llava Interactive mode chat", LlavaInteractiveModeExecute.Run },
        { "Executor: Instruct mode chat", InstructModeExecute.Run },
        { "Executor: Stateless mode chat", StatelessModeExecute.Run },
        { "Save and Load: chat session", SaveAndLoadSession.Run },
        { "Save and Load: state of model and executor", LoadAndSaveState.Run },
        { "LLama Model: Get embeddings", GetEmbeddings.Run },
        { "LLama Model: Quantize", QuantizeModel.Run },
        { "Grammar: Constrain response to json format", GrammarJsonResponse.Run },
        { "Kernel Memory: Document Q&A", KernelMemory.Run },
        { "Kernel Memory: Save and Load", KernelMemorySaveAndLoad.Run },
		{ "Semantic Kernel: HomeAutomation", SemanticKernelHomeAutomation.Run },
        { "Semantic Kernel: Prompt", SemanticKernelPrompt.Run },
        { "Semantic Kernel: Chat", SemanticKernelChat.Run },
        { "Semantic Kernel: Store", SemanticKernelMemory.Run },
        { "Batched Executor: Save/Load", BatchedExecutorSaveAndLoad.Run },
        { "Batched Executor: Fork", BatchedExecutorFork.Run },
        { "Batched Executor: Rewind", BatchedExecutorRewind.Run },
        { "Batched Executor: LLava", BatchedExecutorLLava.Run },
        { "Batched Executor: BoolQ Benchmark", BatchedExecutorBoolQ.Run },
        { "Batched Executor: Beam Search", BatchedExecutorBeamSearch.Run },
        { "Custom Sampling Pipeline", CustomSampler.Run },
        { "Speech Chat: Integration with Whisper.net", SpeechChat.Run },
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

            AnsiConsole.Reset();
            AnsiConsole.MarkupLine("Press ENTER to go to the main menu...");
            Console.ReadLine();

            AnsiConsole.Clear();
        }
    }
}
