namespace LLama.Examples.Examples
{
    using LLama.Common;
    using System;

    internal class CodingAssistant
    {
        // Source paper with example prompts:
        // https://doi.org/10.48550/arXiv.2308.12950
        const string InstructionPrefix = "[INST]";
        const string InstructionSuffix = "[/INST]";
        const string SystemInstruction = "You're an intelligent, concise coding assistant. " +
            "Wrap code in ``` for readability. Don't repeat yourself. " +
            "Use best practice and good coding standards.";

        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();
            if (!modelPath.Contains("codellama", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: the model you selected is not a Code LLama model!");
                Console.WriteLine("For this example we specifically recommend 'codellama-7b-instruct.Q4_K_S.gguf'");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
            }

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 4096
            };
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var executor = new InstructExecutor(context, InstructionPrefix, InstructionSuffix, null);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the LLM will follow your instructions." +
                "\nIt's a 7B Code Llama, so it's trained for programming tasks like \"Write a C# function reading " +
                "a file name from a given URI\" or \"Write some programming interview questions\"." +
                "\nWrite 'exit' to exit");
            Console.ForegroundColor = ConsoleColor.White;

            var inferenceParams = new InferenceParams()
            {
                Temperature = 0.8f,
                MaxTokens = -1,
            };

            string instruction = $"{SystemInstruction}\n\n";
            await Console.Out.WriteAsync("Instruction: ");
            instruction += Console.ReadLine() ?? "Ask me for instructions.";
            while (instruction != "exit")
            {

                Console.ForegroundColor = ConsoleColor.Green;
                await foreach (var text in executor.InferAsync(instruction + Environment.NewLine, inferenceParams))
                {
                    Console.Write(text);
                }
                Console.ForegroundColor = ConsoleColor.White;

                await Console.Out.WriteAsync("Instruction: ");
                instruction = Console.ReadLine() ?? "Ask me for instructions.";
            }
        }
    }
}
