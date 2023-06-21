# Get embeddings

```cs
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GetEmbeddings
{
    public static void Run()
    {
        Console.Write("Please input your model path: ");
        string modelPath = Console.ReadLine();
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
```