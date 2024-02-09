# Kernel memory

```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.Handlers;

public class KernelMemory
{
    public static async Task Run()
    {
        Console.WriteLine("Example from: https://github.com/microsoft/kernel-memory/blob/main/examples/101-using-core-nuget/Program.cs");
        Console.Write("Please input your model path: ");
        var modelPath = Console.ReadLine();
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

```