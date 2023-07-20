# Use stateless executor

```cs
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StatelessModeExecute
{
    public static void Run()
    {
        Console.Write("Please input your model path: ");
        string modelPath = Console.ReadLine();

        StatelessExecutor ex = new(new LLamaModel(new ModelParams(modelPath, contextSize: 256)));

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("The executor has been enabled. In this example, the inference is an one-time job. That says, the previous input and response has " +
            "no impact on the current response. Now you can ask it questions. Note that in this example, no prompt was set for LLM and the maximum response tokens is 50. " +
            "It may not perform well because of lack of prompt. This is also an example that could indicate the improtance of prompt in LLM. To improve it, you can add " +
            "a prompt for it yourself!");
        Console.ForegroundColor = ConsoleColor.White;

        var inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "Question:", "#", "Question: ", ".\n" }, MaxTokens = 50 };

        while (true)
        {
            Console.Write("\nQuestion: ");
            Console.ForegroundColor = ConsoleColor.Green;
            string prompt = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White; 
            Console.Write("Answer: ");
            prompt = $"Question: {prompt.Trim()} Answer: ";
            foreach (var text in ex.Infer(prompt, inferenceParams))
            {
                Console.Write(text);
            }
        }
    }
}
```