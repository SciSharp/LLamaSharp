# Grammer json response

```cs
using LLama.Common;
using LLama.Grammars;

public class GrammarJsonResponse
{
    public static async Task Run()
    {
        var gbnf = (await File.ReadAllTextAsync("Assets/json.gbnf")).Trim();
        var grammar = Grammar.Parse(gbnf, "root");

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
        Console.WriteLine("The executor has been enabled. In this example, the LLM will follow your instructions and always respond in a JSON format. For example, you can input \"Tell me the attributes of a good dish\"");
        Console.ForegroundColor = ConsoleColor.White;

        using var grammarInstance = grammar.CreateInstance();
        var inferenceParams = new InferenceParams() 
        { 
            Temperature = 0.6f, 
            AntiPrompts = new List<string> { "Question:", "#", "Question: ", ".\n" }, 
            MaxTokens = 50,
            Grammar = grammarInstance
        };

        while (true)
        {
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

```