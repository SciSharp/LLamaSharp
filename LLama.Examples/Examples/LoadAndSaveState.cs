using LLama.Common;
using LLama.Sampling;

namespace LLama.Examples.Examples
{
    // This example shows how to save/load state of the executor.
    public class LoadAndSaveState
    {
        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            var prompt = (await File.ReadAllTextAsync("Assets/chat-with-bob.txt")).Trim();

            var parameters = new ModelParams(modelPath)
            {
                GpuLayerCount = 5
            };
            using var model = await LLamaWeights.LoadFromFileAsync(parameters);
            using var context = model.CreateContext(parameters);
            var ex = new InteractiveExecutor(context);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the prompt is printed, " +
                "the maximum tokens is set to 64 and the context size is 256. (an example for small scale usage)");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(prompt);

            var inferenceParams = new InferenceParams
            {
                SamplingPipeline = new DefaultSamplingPipeline
                {
                    Temperature = 0.6f
                },

                AntiPrompts = new List<string> { "User:" }
            };

            while (true)
            {
                await foreach (var text in ex.InferAsync(prompt, inferenceParams))
                {
                    Console.Write(text);
                }

                prompt = Console.ReadLine();
                if (prompt == "save")
                {
                    Console.Write("Your path to save model state: ");
                    var modelStatePath = Console.ReadLine();
                    ex.Context.SaveState(modelStatePath);

                    Console.Write("Your path to save executor state: ");
                    var executorStatePath = Console.ReadLine();
                    await ex.SaveState(executorStatePath);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("All states saved!");
                    Console.ForegroundColor = ConsoleColor.White;

                    var ctx = ex.Context;
                    ctx.LoadState(modelStatePath);
                    ex = new InteractiveExecutor(ctx);
                    await ex.LoadState(executorStatePath);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Loaded state!");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write("Now you can continue your session: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    prompt = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
