using LLama.Common;
using LLama.Grammars;
using LLama.Sampling;

namespace LLama.Examples.Examples
{
    public class GrammarJsonResponse
    {
        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            var gbnf = (await File.ReadAllTextAsync("Assets/json.gbnf")).Trim();
            var grammar = Grammar.Parse(gbnf, "root");

            var parameters = new ModelParams(modelPath)
            {
                Seed = 1337,
                GpuLayerCount = 5
            };
            using var model = await LLamaWeights.LoadFromFileAsync(parameters);
            var ex = new StatelessExecutor(model, parameters);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the LLM will follow your instructions and always respond in a JSON format. For example, you can input \"Tell me the attributes of a good dish\"");
            Console.ForegroundColor = ConsoleColor.White;

            var samplingPipeline = new DefaultSamplingPipeline
            {
                Temperature = 0.6f
            };

            var inferenceParams = new InferenceParams()
            {
                SamplingPipeline = samplingPipeline,
                AntiPrompts = new List<string> { "Question:", "#", "Question: ", ".\n" },
                MaxTokens = 50,
            };

            while (true)
            {
                using var grammarInstance = grammar.CreateInstance();
                samplingPipeline.Grammar = grammarInstance;

                Console.Write("\nQuestion: ");
                Console.ForegroundColor = ConsoleColor.Green;
                var prompt = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Answer: ");
                prompt = $"Question: {prompt?.Trim()} Answer: ";
                await foreach (var text in ex.InferAsync(prompt, inferenceParams))
                {
                    Console.Write(text);
                }
            }
        }
    }
}
