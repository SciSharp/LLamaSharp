using LLama.Common;

namespace LLama.Examples.Examples
{
    public class InteractiveModeExecute
    {
        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            var prompt = (await File.ReadAllTextAsync("Assets/chat-with-bob.txt")).Trim();

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024,
                Seed = 1337,
                GpuLayerCount = 5
            };
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var ex = new InteractiveExecutor(context);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the prompt is printed, the maximum tokens is set to 128 and the context size is 256. (an example for small scale usage)");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write(prompt);

            var inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" }, MaxTokens = 128 };

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
}
