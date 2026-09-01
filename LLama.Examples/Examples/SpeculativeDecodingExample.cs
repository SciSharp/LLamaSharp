using LLama.Common;
using LLama.Native;
using Spectre.Console;

namespace LLama.Examples.Examples
{
    /// <summary>
    /// Demonstrates how to accelerate inference using Speculative Decoding (Draft-Simple) 
    /// and Multi-Token Prediction (MTP) using the StatelessExecutor.
    /// </summary>
    public class SpeculativeDecodingExample
    {
        public static async Task Run()
        {
            Console.WriteLine("=== Speculative Decoding & Multi-Token Prediction (MTP) ===");

            // Ask the user which mode they want to run
            var mode = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Which speculative decoding mode do you want to test?")
                    .AddChoices("Self-Speculation (MTP or Standard)", "Dual-Model Speculation (Target + Draft)"));

            string targetModelPath = UserSettings.GetModelPath();

            if (mode == "Self-Speculation (MTP or Standard)")
            {
                bool useMtp = AnsiConsole.Confirm("Does this model support MTP (e.g. DeepSeek-R1, Qwen MTP)?");
                int draftTokens = AnsiConsole.Ask<int>("Enter draft tokens budget per burst (e.g., 3 for MTP, 16 for standard):", useMtp ? 3 : 16);

                // 1. Configure Target Parameters
                var modelParams = new ModelParams(targetModelPath)
                {
                    ContextSize = 1024,
                    LoadMTP = useMtp, // Explicitly load MTP tensors if requested
                    GpuLayerCount = -1,
                    MainGpu = 0
                };

                // 2. Configure Draft Parameters (Crucial for MTP)
                // In MTP mode, the target weights are re-used, but we MUST create a secondary 
                // context specifically flagged as 'Mtp' to evaluate the projection heads.
                var draftParams = new ModelParams(targetModelPath)
                {
                    ContextSize = 1024,
                    LoadMTP = useMtp,
                    GpuLayerCount = -1,
                    MainGpu = 0,
                    ContextType = useMtp ? LLamaContextType.Mtp : LLamaContextType.Default
                };

                Console.WriteLine("\nLoading Model...");
                using var weights = LLamaWeights.LoadFromFile(modelParams);

                var executor = new StatelessExecutor(
                    weights: weights,
                    @params: modelParams,
                    draftParams: draftParams, // Pass the explicit draft params here to trigger MTP context creation!
                    draftTokens: draftTokens,
                    useMtp: useMtp
                )
                {
                    ApplyTemplate = true // Formats the prompt correctly for Instruct models
                };

                await RunChatLoop(executor);
            }
            else
            {
                Console.WriteLine("\n[System] Please provide the path to the smaller DRAFT model:");
                Console.WriteLine("[System] IMPORTANT: Target and Draft models MUST have the exact same vocabulary size!");

                // Use AnsiConsole instead of UserSettings to prevent overwriting the global Target default
                string draftModelPath = AnsiConsole.Ask<string>("Draft model.gguf path:", targetModelPath);

                int draftTokens = AnsiConsole.Ask<int>("Enter draft tokens budget per burst:", 16);

                var targetParams = new ModelParams(targetModelPath)
                {
                    ContextSize = 1024,
                    GpuLayerCount = -1,
                    MainGpu = 0
                };
                var draftParams = new ModelParams(draftModelPath)
                {
                    ContextSize = 1024,
                    GpuLayerCount = -1,
                    MainGpu = 0
                };

                Console.WriteLine("\nLoading Target Model...");
                using var targetWeights = LLamaWeights.LoadFromFile(targetParams);

                Console.WriteLine("Loading Draft Model...");
                using var draftWeights = LLamaWeights.LoadFromFile(draftParams);

                var executor = new StatelessExecutor(
                    weights: targetWeights,
                    @params: targetParams,
                    draftWeights: draftWeights,
                    draftParams: draftParams,
                    draftTokens: draftTokens,
                    useMtp: false
                )
                {
                    ApplyTemplate = true // Required for Instruct models
                };

                await RunChatLoop(executor);
            }
        }

        private static async Task RunChatLoop(StatelessExecutor executor)
        {
            // No AntiPrompts array, the template handles stopping automatically 
            var inferenceParams = new InferenceParams { MaxTokens = 256 };

            Console.WriteLine("\nReady! Type 'exit' to quit.");
            while (true)
            {
                string prompt = AnsiConsole.Ask<string>("\n[green]User:[/]");
                if (prompt.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                Console.Write("[yellow]Bot:[/]");

                // Speculative decoding works transparently with the standard streaming API
                await foreach (var token in executor.InferAsync(prompt, inferenceParams))
                {
                    Console.Write(token);
                }
                Console.WriteLine();
            }
        }
    }
}