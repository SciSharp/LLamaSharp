using LLama.Common;

namespace LLama.Examples.Examples
{
    public class GetEmbeddings
    {
        public static void Run()
        {
            Console.Write("Please input your model path: ");
            var modelPath = Console.ReadLine();

            var @params = new ModelParams(modelPath);
            using var weights = LLamaWeights.LoadFromFile(@params);
            var embedder = new LLamaEmbedder(weights, @params);

            while (true)
            {
                Console.Write("Please input your text: ");
                Console.ForegroundColor = ConsoleColor.Green;
                var text = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine(string.Join(", ", embedder.GetEmbeddings(text)));
                Console.WriteLine();
            }
        }
    }
}
