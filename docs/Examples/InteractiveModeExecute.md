# Use interactive executor

```cs
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InteractiveModeExecute
{
    public async static Task Run()
    {
        Console.Write("Please input your model path: ");
        string modelPath = Console.ReadLine();
        var prompt = File.ReadAllText("Assets/chat-with-bob.txt").Trim();

        InteractiveExecutor ex = new(new LLamaModel(new ModelParams(modelPath, contextSize: 256)));

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("The executor has been enabled. In this example, the prompt is printed, the maximum tokens is set to 64 and the context size is 256. (an example for small scale usage)");
        Console.ForegroundColor = ConsoleColor.White;

        Console.Write(prompt);

        var inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" }, MaxTokens = 64 };

        while (true)
        {
            await foreach (var text in ex.InferAsync(prompt, inferenceParams))
            {
                Console.Write(text);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            prompt = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
```