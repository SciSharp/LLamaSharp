using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.ContentStorage.DevTools;
using Microsoft.KernelMemory.FileSystem.DevTools;
using Microsoft.KernelMemory.MemoryStorage.DevTools;
using System.Diagnostics;

namespace LLama.Examples.Examples;

public class KernelMemorySaveAndLoad
{
    static string StorageFolder => Path.GetFullPath($"./storage-{nameof(KernelMemorySaveAndLoad)}");
    static bool StorageExists => Directory.Exists(StorageFolder) && Directory.GetDirectories(StorageFolder).Length > 0;

    public static async Task Run()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(
            """

            This program uses the Microsoft.KernelMemory package to ingest documents
            and store the embeddings as local files so they can be quickly recalled
            when this application is launched again. 

            """);

        string modelPath = UserSettings.GetModelPath();
        IKernelMemory memory = CreateMemoryWithLocalStorage(modelPath);

        Console.ForegroundColor = ConsoleColor.Yellow;
        if (StorageExists)
        {
            Console.WriteLine(
                """
                
                Kernel memory files have been located!
                Information about previously analyzed documents has been loaded.

                """);
        }
        else
        {
            Console.WriteLine(
                $"""

                 Existing kernel memory was not found.
                 Documents will be analyzed (slow) and information saved to disk.
                 Analysis will not be required the next time this program is run.
                 Press ENTER to proceed...
 
                 """);
            Console.ReadLine();
            await IngestDocuments(memory);
        }

        await AskSingleQuestion(memory, "What formats does KM support?");
        await StartUserChatSession(memory);
    }

    private static IKernelMemory CreateMemoryWithLocalStorage(string modelPath)
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

        SimpleFileStorageConfig storageConfig = new()
        {
            Directory = StorageFolder,
            StorageType = FileSystemTypes.Disk,
        };

        SimpleVectorDbConfig vectorDbConfig = new()
        {
            Directory = StorageFolder,
            StorageType = FileSystemTypes.Disk,
        };

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Kernel memory folder: {StorageFolder}");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        return new KernelMemoryBuilder()
            .WithSimpleFileStorage(storageConfig)
            .WithSimpleVectorDb(vectorDbConfig)
            .WithLLamaSharpDefaults(lsConfig)
            .WithSearchClientConfig(searchClientConfig)
            .With(parseOptions)
            .Build();
    }

    private static async Task AskSingleQuestion(IKernelMemory memory, string question)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Question: {question}");
        await ShowAnswer(memory, question);
    }

    private static async Task StartUserChatSession(IKernelMemory memory)
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Question: ");
            string question = Console.ReadLine()!;
            if (string.IsNullOrEmpty(question))
                return;

            await ShowAnswer(memory, question);
        }
    }

    private static async Task IngestDocuments(IKernelMemory memory)
    {
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
    }

    private static async Task ShowAnswer(IKernelMemory memory, string question)
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