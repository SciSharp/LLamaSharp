using LLama.Common;

namespace LLama.Examples.Examples
{
    public class GetEmbeddings
    {
        public static void Run()
        {
            string modelPath = UserSettings.GetModelPath();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            var @params = new ModelParams(modelPath) { EmbeddingMode = true };
            using var weights = LLamaWeights.LoadFromFile(@params);
            var embedder = new LLamaEmbedder(weights, @params);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                """
                This example displays embeddings from a text prompt.
                Embeddings are numerical codes that represent information like words, images, or concepts.
                These codes capture important relationships between those objects, 
                like how similar words are in meaning or how close images are visually.
                This allows machine learning models to efficiently understand and process complex data.
                Embeddings of a text in LLM is sometimes useful, for example, to train other MLP models.
                """); // NOTE: this description was AI generated

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Please input your text: ");
                Console.ForegroundColor = ConsoleColor.Green;
                var text = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;

                float[] embeddings = embedder.GetEmbeddings(text).Result;
                Console.WriteLine($"Embeddings contain {embeddings.Length:N0} floating point values:");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(string.Join(", ", embeddings.Take(20)) + ", ...");
                Console.WriteLine();
            }
        }
    }
}
