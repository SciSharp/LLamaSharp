using LLama.Common;

namespace LLama.Examples.Examples
{
    public class LlavaInteractiveModeExecute
    {
        public static async Task Run()
        {
            string multiModalProj = UserSettings.GetMMProjPath();
            string modelPath = UserSettings.GetModelPath();
            string imagePath = UserSettings.GetImagePath();

            var prompt = (await File.ReadAllTextAsync("Assets/vicuna-llava-v16.txt")).Trim();

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 4096,
                Seed = 1337,
                GpuLayerCount = 5
            };
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            
            // Llava Init
            using var clipModel = LLavaWeights.LoadFromFile(multiModalProj);
            
            var ex = new InteractiveExecutor(context, clipModel );

            ex.ImagePath = imagePath;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the prompt is printed, the maximum tokens is set to 1024 and the context size is 4096. ");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write(prompt);

            var inferenceParams = new InferenceParams() { Temperature = 0.1f, AntiPrompts = new List<string> { "USER:" }, MaxTokens = 1024 };

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
