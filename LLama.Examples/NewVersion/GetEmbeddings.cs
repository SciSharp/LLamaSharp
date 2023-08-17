using LLama.Common;

namespace LLama.Examples.NewVersion
{
    public class GetEmbeddings
    {
        public static void Run()
        {
            Console.Write("Please input your model path: ");
            var modelPath = Console.ReadLine();
            var embedder = new LLamaEmbedder(new ModelParams(modelPath));

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
