using System.Security.Cryptography;
using LLama.Common;
using LLamaSharp.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;
using LLamaSharp.SemanticKernel.TextCompletion;

namespace LLama.Examples.Examples
{
    public class SemanticKernelPrompt
    {
        public static async Task Run()
        {
            Console.WriteLine("Example from: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example17_ChatGPT.cs");
            Console.Write("Please input your model path: ");
            var modelPath = Console.ReadLine();

            // Load weights into memory
            var parameters = new ModelParams(modelPath);
            using var model = LLamaWeights.LoadFromFile(parameters);
            var ex = new StatelessExecutor(model, parameters);

            var builder = new KernelBuilder();
            builder.WithAIService<ITextCompletion>("local-llama", new LLamaSharpTextCompletion(ex), true);

            var kernel = builder.Build();

            var prompt = @"{{$input}}

One line TLDR with the fewest words.";

            ChatRequestSettings settings = new() {MaxTokens = 100};
            var summarize = kernel.CreateSemanticFunction(prompt, requestSettings: settings);

            string text1 = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

            string text2 = @"
1. An object at rest remains at rest, and an object in motion remains in motion at constant speed and in a straight line unless acted on by an unbalanced force.
2. The acceleration of an object depends on the mass of the object and the amount of force applied.
3. Whenever one object exerts a force on another object, the second object exerts an equal and opposite on the first.";

            Console.WriteLine((await kernel.RunAsync(text1, summarize)).GetValue<string>());

            Console.WriteLine((await kernel.RunAsync(text2, summarize)).GetValue<string>());
        }
    }
}
 