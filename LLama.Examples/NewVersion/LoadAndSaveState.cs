using LLama.Common;

namespace LLama.Examples.NewVersion
{
    public class LoadAndSaveState
    {
        public static void Run()
        {
            Console.Write("Please input your model path: ");
            var modelPath = Console.ReadLine();
            var prompt = File.ReadAllText("Assets/chat-with-bob.txt").Trim();

            var parameters = new ModelParams(modelPath, contextSize: 1024, seed: 1337, gpuLayerCount: 5);
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var ex = new InteractiveExecutor(context);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the prompt is printed, the maximum tokens is set to 64 and the context size is 256. (an example for small scale usage)");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write(prompt);

            var inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } };

            while (true)
            {
                foreach (var text in ex.Infer(prompt, inferenceParams))
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
                    ex.SaveState(executorStatePath);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("All states saved!");
                    Console.ForegroundColor = ConsoleColor.White;

                    var ctx = ex.Context;
                    ctx.LoadState(modelStatePath);
                    ex = new InteractiveExecutor(ctx);
                    ex.LoadState(executorStatePath);
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
