/*
=============================================================================
# Microsoft Agent Framework & LLamaSharp (with Deterministic RAG)

This example demonstrates how to integrate LLamaSharp with the Microsoft Agent 
Framework (Microsoft.Agents.AI) and Microsoft's official Vector Data 
abstractions (Microsoft.Extensions.VectorData).

It implements a Deterministic RAG (Retrieval-Augmented Generation) pipeline. 
Instead of relying on the LLM to autonomously use tools (which often causes 
small local models to hallucinate), the C# code explicitly handles the vector 
search and forces the verified facts into the AI's prompt using a strict 
persona constraint.

## 🧠 Key Concepts Demonstrated
* The `IChatClient` Bridge: Wrapping LLamaSharp's `StatelessExecutor` into 
  Microsoft's standard `ChatClientBuilder`.
* Embedding Generation: Utilizing LLamaSharp's `LLamaEmbedder` to generate 
  vectors from text in real-time.
* Vector Storage Integration: Using `CommunityToolkit.VectorData.SqliteVec` 
  to power an in-memory SQLite vector database.
* Strict Agent Constraints: Configuring `ChatClientAgentRunOptions` to lock 
  the model's Temperature to 0.0f to prevent creative hallucinations.

## 🚀 How to Run the Example
To run this example successfully, you will need two separate local models:
1. A Chat Model: Used for the Agent persona (e.g., Llama-3.2-3B-Instruct).
2. An Embedding Model: Used for Vector Search (e.g., Qwen-Embedding-0.6B).

⚠️ Important Dimension Note:
By default, this example's `DocumentRecord` class is hardcoded to expect 
1024 dimensions, which matches the Qwen3 embedding model. If you use a 
different embedding model (like all-MiniLM-L6-v2), you must change the 
dimension attribute in the code to match (e.g., 
`[VectorStoreVector(Dimensions: 384)]`) before compiling, or the SQLite 
database will throw an error.

## 🔍 How It Works Under the Hood
1. Ingestion: Embeds dummy strings and upserts them into the SQLite database.
2. Retrieval: Automatically embeds your query, runs a vector similarity search 
   against SQLite, and extracts the top 2 matching facts.
3. Generation: Spins up a Microsoft Agent with strict instructions, feeds it 
   the facts, and streams a formatted, hallucination-free response.
4. Cleanup: Because the SQLite database is entirely in-memory, no residual 
   .db files are left on your hard drive after closing the application.
=============================================================================
*/

using CommunityToolkit.VectorData.SqliteVec;
using LLama.Abstractions;
using LLama.Common;
using Microsoft.Agents.AI;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Spectre.Console;

namespace LLama.Examples.Examples
{
    public class MicrosoftAgentFrameworkExample
    {
        public class DocumentRecord
        {
            [VectorStoreKey]
            public long Id { get; set; }

            [VectorStoreData]
            public string Text { get; set; } = string.Empty;

            // Note: Adjust dimensions to match your embedding model (e.g., 1024 for Qwen3-0.6B)
            [VectorStoreVector(dimensions: 1024)]
            public ReadOnlyMemory<float> Vector { get; set; }
        }

        public static async Task Run()
        {
            Console.WriteLine("=== Microsoft Agent Framework & Deterministic RAG ===");

            // 1. Get Model Paths using standard LLama.Examples prompts
            string chatModelPath = UserSettings.GetModelPath(); //https://huggingface.co/bartowski/Llama-3.2-3B-Instruct-GGUF/resolve/main/Llama-3.2-3B-Instruct-Q4_K_M.gguf?download=true
            string embedModelPath = UserSettings.GetEmbedModelPath(); //https://huggingface.co/Qwen/Qwen3-Embedding-0.6B-GGUF/blob/main/Qwen3-Embedding-0.6B-Q8_0.gguf

            Console.WriteLine("\nLoading Models into Memory...");

            var chatParams = new ModelParams(chatModelPath) { ContextSize = 4096, GpuLayerCount = -1 };
            using var chatWeights = LLamaWeights.LoadFromFile(chatParams);
            using var chatContext = chatWeights.CreateContext(chatParams);
            var chatExecutor = new StatelessExecutor(chatWeights, chatParams);

            var embedParams = new ModelParams(embedModelPath) { Embeddings = true, GpuLayerCount = -1 };
            using var embedWeights = LLamaWeights.LoadFromFile(embedParams);
            using var embedder = new LLamaEmbedder(embedWeights, embedParams);

            // 2. Setup SQLite Vector Database
            Console.WriteLine("Initializing In-Memory SQLite Vector Database...");
            string connectionString = "Data Source=SharedVectorDb;Mode=Memory;Cache=Shared";

            // 2a. OPEN A MASTER CONNECTION TO KEEP THE DB ALIVE
            // This prevents SQLite from destroying the DB between vector store operations
            using var keepAliveConnection = new SqliteConnection(connectionString);
            await keepAliveConnection.OpenAsync();

            // 2b. Initialize the vector store using the same connection string
            var vectorStore = new SqliteVectorStore(connectionString);

            VectorStoreCollection<long, DocumentRecord> documentCollection =
                vectorStore.GetCollection<long, DocumentRecord>("company_docs");

            await documentCollection.EnsureCollectionExistsAsync();

            // 3. Seed the Database
            string[] rawDocuments = [
                "Project Zephyr launches in Q4. The marketing budget is $50,000.",
                "The CEO announced that Fridays are now remote-work days.",
                "Project Zephyr's lead engineer is Sarah Connor."
            ];

            long idCounter = 1;
            foreach (var doc in rawDocuments)
            {
                var embeddingBatch = await embedder.GetEmbeddings(doc);
                float[] vectorArray = embeddingBatch.First();

                await documentCollection.UpsertAsync(new DocumentRecord
                {
                    Id = idCounter++,
                    Text = doc,
                    Vector = new ReadOnlyMemory<float>(vectorArray)
                });
            }

            // 4. Build the Microsoft Agent ChatClient
            IChatClient chatClient = new ChatClientBuilder(chatExecutor.AsChatClient()).Build();

            // 5. Run the Interactive Loop
            Console.WriteLine("\nSetup Complete. Type 'exit' to quit.");
            while (true)
            {
                string task = AnsiConsole.Ask("Enter your research task (or ENTER for default): ", "Draft an email about Project Zephyr");
                if (task.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                Console.WriteLine("\n[System] Searching database...");
                var embeddingBatch = await embedder.GetEmbeddings(task);
                float[] queryVector = embeddingBatch.First();

                var searchResults = documentCollection.SearchAsync(
                    new ReadOnlyMemory<float>(queryVector),
                    top: 2
                );

                var retrievedFacts = new List<string>();
                await foreach (var result in searchResults)
                {
                    retrievedFacts.Add($"- {result.Record.Text}");
                }
                string contextString = string.Join("\n", retrievedFacts);

                Console.WriteLine($"[System] Facts Retrieved:\n{contextString}\n");
                Console.WriteLine("[System] Agent is drafting response...");

                var writer = chatClient.AsAIAgent(
                    name: "Writer",
                    instructions: "You are a strict corporate communications AI. Your ONLY job is to write a short email using EXACTLY the facts provided by the user. You must NOT add, infer, or invent any outside information."
                );

                var runOptions = new ChatClientAgentRunOptions
                {
                    ChatOptions = new ChatOptions { MaxOutputTokens = 1000, Temperature = 0.0f }
                };

                string strictPrompt = $"Draft an email to the team using ONLY these exact facts:\n{contextString}\nDo not add any other information.";

                var writerResponse = await writer.RunAsync(strictPrompt, options: runOptions);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n{writerResponse.Text}\n");
                Console.ResetColor();
            }
        }
    }
}