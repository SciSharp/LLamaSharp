using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using System.Diagnostics;

namespace LLama.Examples.Examples
{
    // This example is from Microsoft's official kernel memory "custom prompts" example:
    // https://github.com/microsoft/kernel-memory/blob/6d516d70a23d50c6cb982e822e6a3a9b2e899cfa/examples/101-dotnet-custom-Prompts/Program.cs#L1-L86

    // Microsoft.KernelMemory has more features than Microsoft.SemanticKernel.
    // See https://microsoft.github.io/kernel-memory/ for details.

    public class KernelMemory
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                """

                This program uses the Microsoft.KernelMemory package to ingest documents
                and answer questions about them in an interactive chat prompt.

                """);

            // Setup the kernel memory with the LLM model
            string modelPath = UserSettings.GetModelPath();
            IKernelMemory memory = CreateMemory(modelPath);

            // Ingest documents (format is automatically detected from the filename)
            string[] filesToIngest = [
                Path.GetFullPath(@"./Assets/sample-SK-Readme.pdf"),
                Path.GetFullPath(@"./Assets/sample-KM-Readme.pdf"),
            ];

            for (int i = 0; i < filesToIngest.Length; i++)
            {
                string path = filesToIngest[i];
                Stopwatch sw = Stopwatch.StartNew();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Importing {i + 1} of {filesToIngest.Length}: {path}");
                await memory.ImportDocumentAsync(path, steps: Constants.PipelineWithoutSummary);
                Console.WriteLine($"Completed in {sw.Elapsed}\n");
            }

            // Ask a predefined question
            Console.ForegroundColor = ConsoleColor.Green;
            string question1 = "What formats does KM support";
            Console.WriteLine($"Question: {question1}");
            await AnswerQuestion(memory, question1);

            // Let the user ask additional questions
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Question: ");
                string question = Console.ReadLine()!;
                if (string.IsNullOrEmpty(question))
                    return;

                await AnswerQuestion(memory, question);
            }
        }

        private static IKernelMemory CreateMemory(string modelPath)
        {
            Common.InferenceParams infParams = new() { AntiPrompts = ["\n\n"] };

            LLamaSharpConfig lsConfig = new(modelPath) { DefaultInferenceParams = infParams };

            SearchClientConfig searchClientConfig = new()
            {
                MaxMatchesCount = 1,
                AnswerTokens = 100,
            };

            TextPartitioningOptions parseOptions = new()
            {
                MaxTokensPerParagraph = 300,
                MaxTokensPerLine = 100,
                OverlappingTokens = 30
            };

            return new KernelMemoryBuilder()
                .WithLLamaSharpDefaults(lsConfig)
                .WithSearchClientConfig(searchClientConfig)
                .With(parseOptions)
                .Build();
        }

        private static async Task AnswerQuestion(IKernelMemory memory, string question)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Generating answer...");

            MemoryAnswer answer = await memory.AskAsync(question);
            Console.WriteLine($"Answer generated in {sw.Elapsed}");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Answer: {answer.Result}");
            foreach (var source in answer.RelevantSources)
            {
                Console.WriteLine($"Source: {source.SourceName}");
            }
            Console.WriteLine();
        }
    }
}