using LLama.Common;
using System.Text;

namespace LLama.Examples.NewVersion
{
    public class ChatSessionStripRoleName
    {
        public static void Run()
        {
            Console.Write("Please input your model path: ");
            var modelPath = Console.ReadLine();
            var prompt = File.ReadAllText("Assets/chat-with-bob.txt").Trim();

            var parameters = new ModelParams(modelPath, contextSize: 1024, seed: 1337, gpuLayerCount: 5);
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters, Encoding.UTF8);
            var executor = new InteractiveExecutor(context);

            var session = new ChatSession(executor).WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(new string[] { "User:", "Bob:" }, redundancyLength: 8));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The chat session has started. The role names won't be printed.");
            Console.ForegroundColor = ConsoleColor.White;

            // show the prompt
            Console.Write(prompt);
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
}
