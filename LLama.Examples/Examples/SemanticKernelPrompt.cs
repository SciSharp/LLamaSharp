using LLama.Common;
using LLamaSharp.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using LLamaSharp.SemanticKernel.TextCompletion;
using Microsoft.SemanticKernel.TextGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace LLama.Examples.Examples
{
    public class SemanticKernelPrompt
    {
        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("This example is from: " +
                "https://github.com/microsoft/semantic-kernel/blob/main/dotnet/README.md");

            // Load weights into memory
            var parameters = new ModelParams(modelPath);
            using var model = LLamaWeights.LoadFromFile(parameters);
            var ex = new StatelessExecutor(model, parameters);

            var builder = Kernel.CreateBuilder();
            builder.Services.AddKeyedSingleton<ITextGenerationService>("local-llama", new LLamaSharpTextCompletion(ex));

            var kernel = builder.Build();

            var prompt = @"{{$input}}

One line TLDR with the fewest words.";

            ChatRequestSettings settings = new() { MaxTokens = 100 };
            var summarize = kernel.CreateFunctionFromPrompt(prompt, settings);

            string text1 = @"
1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

            string text2 = @"
1. An object at rest remains at rest, and an object in motion remains in motion at constant speed and in a straight line unless acted on by an unbalanced force.
2. The acceleration of an object depends on the mass of the object and the amount of force applied.
3. Whenever one object exerts a force on another object, the second object exerts an equal and opposite on the first.";

            Console.WriteLine((await kernel.InvokeAsync(summarize, new() { ["input"] = text1 })).GetValue<string>());

            Console.WriteLine((await kernel.InvokeAsync(summarize, new() { ["input"] = text2 })).GetValue<string>());
        }
    }
}
