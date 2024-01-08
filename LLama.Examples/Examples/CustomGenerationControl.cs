using LLama.Abstractions;
using LLama.Common;
using LLama.Control;
using LLama.Examples.Extensions;

namespace LLama.Examples.Examples
{
    public class CustomGenerationControl
    {
        public class NumberGenerationControl: IGenerationControl
        {
            public bool ShouldStopGeneration(LLamaContext context, IInferenceParams inferenceParams, string lastOutputText)
            {
                return lastOutputText.Any(x => char.IsDigit(x) && (x == '4' || x == '5'));
            }

            public bool ShouldStopGeneration(LLamaContext context, IInferenceParams inferenceParams, int lastOutputId)
            {
                return false;
            }
        }
        public static async Task Run()
        {
            Console.Write("Please input your model path: ");
            var modelPath = Console.ReadLine();

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024,
                Seed = 1337,
                GpuLayerCount = 5
            };
            using var model = LLamaWeights.LoadFromFile(parameters);
            var ex = new StatelessExecutor(model, parameters);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("This is an example to show how to customize the generation control of the executors. Here we implement a control mode in which" +
                " the generation will stop once there's a number 4 or 5 is generated. Please try different questions to lead the model to generate answers with and without numbers." +
                " No anti-prompt is used in this example.");
            Console.ForegroundColor = ConsoleColor.White;

            var inferenceParams = new InferenceParams() { Temperature = 0.6f, MaxTokens = 60, GenerationControl = new NumberGenerationControl() };

            while (true)
            {
                Console.Write("\nQuestion: ");
                Console.ForegroundColor = ConsoleColor.Green;
                var prompt = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Answer: ");
                prompt = $"Question: {prompt?.Trim()} Answer: ";
                await foreach (var text in ex.InferAsync(prompt, inferenceParams).Spinner())
                {
                    Console.Write(text);
                }
            }
        }
    }
}
