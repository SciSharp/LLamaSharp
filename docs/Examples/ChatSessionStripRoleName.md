# Use chat session and strip role names

```cs
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ChatSessionStripRoleName
{
    public static void Run()
    {
        Console.Write("Please input your model path: ");
        string modelPath = Console.ReadLine();
        var prompt = File.ReadAllText("Assets/chat-with-bob.txt").Trim();
        InteractiveExecutor ex = new(new LLamaModel(new ModelParams(modelPath, contextSize: 1024, seed: 1337, gpuLayerCount: 5)));
        ChatSession session = new ChatSession(ex).WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(new string[] { "User:", "Bob:" }, redundancyLength: 8));

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("The chat session has started. The role names won't be printed.");
        Console.ForegroundColor = ConsoleColor.White;

        while (true)
        {
            foreach (var text in session.Chat(prompt, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } }))
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