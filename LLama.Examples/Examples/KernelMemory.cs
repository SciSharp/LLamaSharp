using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;

namespace LLama.Examples.Examples
{
    public class KernelMemory
    {
        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("This example is from : \n" +
                "https://github.com/microsoft/kernel-memory/blob/main/examples/101-using-core-nuget/Program.cs");

            var searchClientConfig = new SearchClientConfig
            {
                MaxMatchesCount = 1,
                AnswerTokens = 100,
            };

            var memory = new KernelMemoryBuilder()
                    .WithLLamaSharpDefaults(new LLamaSharpConfig(modelPath)
                    {
                        DefaultInferenceParams = new Common.InferenceParams
                        {
                            AntiPrompts = new List<string> { "\n\n" }
                        }
                    })
                    .WithSearchClientConfig(searchClientConfig)
                    .With(new TextPartitioningOptions
                    {
                        MaxTokensPerParagraph = 300,
                        MaxTokensPerLine = 100,
                        OverlappingTokens = 30
                    })
                .Build();

            await memory.ImportDocumentAsync(@"./Assets/sample-SK-Readme.pdf", steps: Constants.PipelineWithoutSummary);

            var question = "What's Semantic Kernel?";

            Console.WriteLine($"\n\nQuestion: {question}");

            var answer = await memory.AskAsync(question);

            Console.WriteLine($"\nAnswer: {answer.Result}");

            Console.WriteLine("\n\n  Sources:\n");

            foreach (var x in answer.RelevantSources)
            {
                Console.WriteLine($"  - {x.SourceName}  - {x.Link} [{x.Partitions.First().LastUpdate:D}]");
            }
        }
    }
}
