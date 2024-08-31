using LLama.Common;
using LLama.Native;

namespace LLama.Examples.Examples
{
    public class GetEmbeddings
    {
        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            var @params = new ModelParams(modelPath)
            {
                // Embedding models can return one embedding per token, or all of them can be combined ("pooled") into
                // one single embedding. Setting PoolingType to "Mean" will combine all of the embeddings using mean average.
                PoolingType = LLamaPoolingType.Mean,
            };
            using var weights = LLamaWeights.LoadFromFile(@params);
            var embedder = new LLamaEmbedder(weights, @params);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                """
                This example displays embeddings from a text prompt.
                Embeddings are vectors that represent information like words, images, or concepts.
                These vector capture important relationships between those objects, 
                like how similar words are in meaning or how close images are visually.
                This allows machine learning models to efficiently understand and process complex data.
                Embeddings of a text in LLM is sometimes useful, for example, to train other MLP models.
                """);

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Please input your text: ");
                Console.ForegroundColor = ConsoleColor.Green;
                var text = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;

                // Get embeddings for the text
                var embeddings = await embedder.GetEmbeddings(text);

                // This should have returned one single embedding vector, because PoolingType was set to Mean above.
                var embedding = embeddings.Single();

                Console.WriteLine($"Embeddings contain {embedding.Length:N0} floating point values:");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(string.Join(", ", embeddings.Take(20)) + ", ...");
                Console.WriteLine();
            }
        }
    }
}
