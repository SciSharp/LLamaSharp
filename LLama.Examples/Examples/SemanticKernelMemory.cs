using LLama.Common;
using Microsoft.SemanticKernel.Memory;
using LLamaSharp.SemanticKernel.TextEmbedding;

namespace LLama.Examples.Examples
{
    public class SemanticKernelMemory
    {
        private const string MemoryCollectionName = "SKGitHub";

        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            Console.WriteLine("This example is from: \n" +
                "https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example14_SemanticMemory.cs");

            var seed = 1337u;
            // Load weights into memory
            var parameters = new ModelParams(modelPath)
            {
                Embeddings = true
            };

            using var model = await LLamaWeights.LoadFromFileAsync(parameters);
            var embedding = new LLamaEmbedder(model, parameters);

            Console.WriteLine("====================================================");
            Console.WriteLine("======== Semantic Memory (volatile, in RAM) ========");
            Console.WriteLine("====================================================");

            /* You can build your own semantic memory combining an Embedding Generator
             * with a Memory storage that supports search by similarity (ie semantic search).
             *
             * In this example we use a volatile memory, a local simulation of a vector DB.
             *
             * You can replace VolatileMemoryStore with Qdrant (see QdrantMemoryStore connector)
             * or implement your connectors for Pinecone, Vespa, Postgres + pgvector, SQLite VSS, etc.
             */

            var memory = new MemoryBuilder()
                .WithTextEmbeddingGeneration(new LLamaSharpEmbeddingGeneration(embedding))
                .WithMemoryStore(new VolatileMemoryStore())
                .Build();

            await RunExampleAsync(memory);
        }

        private static async Task RunExampleAsync(ISemanticTextMemory memory)
        {
            await StoreMemoryAsync(memory);

            await SearchMemoryAsync(memory, "How do I get started?");

            /*
            Output:

            Query: How do I get started?

            Result 1:
              URL:     : https://github.com/microsoft/semantic-kernel/blob/main/README.md
              Title    : README: Installation, getting started, and how to contribute

            Result 2:
              URL:     : https://github.com/microsoft/semantic-kernel/blob/main/samples/dotnet-jupyter-notebooks/00-getting-started.ipynb
              Title    : Jupyter notebook describing how to get started with the Semantic Kernel

            */

            await SearchMemoryAsync(memory, "Can I build a chat with SK?");

            /*
            Output:

            Query: Can I build a chat with SK?

            Result 1:
              URL:     : https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/ChatSkill/ChatGPT
              Title    : Sample demonstrating how to create a chat skill interfacing with ChatGPT

            Result 2:
              URL:     : https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/README.md
              Title    : README: README associated with a sample chat summary react-based webapp

            */

            await SearchMemoryAsync(memory, "Jupyter notebook");

            await SearchMemoryAsync(memory, "README: README associated with a sample chat summary react-based webapp");

            await SearchMemoryAsync(memory, "Jupyter notebook describing how to pass prompts from a file to a semantic skill or function");
        }

        private static async Task SearchMemoryAsync(ISemanticTextMemory memory, string query)
        {
            Console.WriteLine("\nQuery: " + query + "\n");

            var memories = memory.SearchAsync(MemoryCollectionName, query, limit: 10, minRelevanceScore: 0.5);

            int i = 0;
            await foreach (MemoryQueryResult result in memories)
            {
                Console.WriteLine($"Result {++i}:");
                Console.WriteLine("  URL:     : " + result.Metadata.Id);
                Console.WriteLine("  Title    : " + result.Metadata.Description);
                Console.WriteLine("  Relevance: " + result.Relevance);
                Console.WriteLine();
            }

            Console.WriteLine("----------------------");
        }

        private static async Task StoreMemoryAsync(ISemanticTextMemory memory)
        {
            /* Store some data in the semantic memory.
             *
             * When using Azure Cognitive Search the data is automatically indexed on write.
             *
             * When using the combination of VolatileStore and Embedding generation, SK takes
             * care of creating and storing the index
             */

            Console.WriteLine("\nAdding some GitHub file URLs and their descriptions to the semantic memory.");
            var githubFiles = SampleData();
            var i = 0;
            foreach (var entry in githubFiles)
            {
                var result = await memory.SaveReferenceAsync(
                    collection: MemoryCollectionName,
                    externalSourceName: "GitHub",
                    externalId: entry.Key,
                    description: entry.Value,
                    text: entry.Value);

                Console.WriteLine($"#{++i} saved.");
                Console.WriteLine(result);
            }

            Console.WriteLine("\n----------------------");
        }

        private static Dictionary<string, string> SampleData()
        {
            return new Dictionary<string, string>
            {
                ["https://github.com/microsoft/semantic-kernel/blob/main/README.md"]
                    = "README: Installation, getting started, and how to contribute",
                ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/02-running-prompts-from-file.ipynb"]
                    = "Jupyter notebook describing how to pass prompts from a file to a semantic skill or function",
                ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks//00-getting-started.ipynb"]
                    = "Jupyter notebook describing how to get started with the Semantic Kernel",
                ["https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/ChatSkill/ChatGPT"]
                    = "Sample demonstrating how to create a chat skill interfacing with ChatGPT",
                ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/SemanticKernel/Memory/VolatileMemoryStore.cs"]
                    = "C# class that defines a volatile embedding store",
                ["https://github.com/microsoft/semantic-kernel/blob/main/samples/dotnet/KernelHttpServer/README.md"]
                    = "README: How to set up a Semantic Kernel Service API using Azure Function Runtime v4",
                ["https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/README.md"]
                    = "README: README associated with a sample chat summary react-based webapp",
            };
        }
    }
}
