using LLama.Common;
using LLama.Grammar;
using LLama.Native;

namespace LLama.Examples.NewVersion
{
    public class GrammarJsonResponse
    {
        public static void Run()
        {
            var grammarBytes = File.ReadAllText("Assets/json.gbnf").Trim();
            var parsedGrammar = new GrammarParser();

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
            ParseState state = parsedGrammar.Parse(grammarBytes);
            using var grammar = SafeLLamaGrammarHandle.Create(state.Rules, 0);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the LLM will follow your instructions and always respond in a JSON format. For example, you can input \"Tell me the attributes of a good dish\"");
            Console.ForegroundColor = ConsoleColor.White;

            var inferenceParams = new InferenceParams() 
            { 
                Temperature = 0.6f, 
                AntiPrompts = new List<string> { "Question:", "#", "Question: ", ".\n" }, 
                MaxTokens = 50,
                Grammar = grammar
            };

            while (true)
            {
                Console.Write("\nQuestion: ");
                Console.ForegroundColor = ConsoleColor.Green;
                var prompt = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Answer: ");
                prompt = $"Question: {prompt?.Trim()} Answer: ";
                foreach (var text in ex.Infer(prompt, inferenceParams))
                {
                    Console.Write(text);
                }
            }
        }
    }
}
