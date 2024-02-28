using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Json.More;
using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Handlers;
using LLava;
using LLama.Common;
using LLama.Examples.Extensions;

namespace LLama.Examples.Examples
{
    public class LLavaExample
    {
        public static async Task Run()
        {
            Console.WriteLine("Multimodal example");
            
            // Get Model
            //
            Console.WriteLine("Please input your model path: ");
            var modelPath = Console.ReadLine();
            
            // Get mmProject
            //
            Console.WriteLine("Please input your mmProject model path: ");
            var mmProject = Console.ReadLine();

            Console.WriteLine("Please input a image file (path+filename): ");
            var exampleImage = Console.ReadLine();
            
            
            // Mistral llava 1.6
            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 4096,
                Seed = 1337,
                GpuLayerCount = 5
            };
            
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var clipModel = LLavaWeights.LoadFromFile(mmProject);
            using var context = model.CreateContext(parameters);

            var executor = new LLavaInteractExecutor(context, clipModel );

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The chat session has started. In this example, the prompt is printed for better visual result.");
            Console.ForegroundColor = ConsoleColor.White;
            
            var inferenceParams = new InferenceParams() { Temperature = 0.2f, AntiPrompts = new List<string> { "USER:" }, MaxTokens = 5000 };

            Console.Write("USER: ");
            Console.ForegroundColor = ConsoleColor.Green;
            string userPrompt = Console.ReadLine();
            Console.WriteLine("");      
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            var prompt = new Prompt( userPrompt, System.IO.File.ReadAllBytes(exampleImage)); 
            await foreach (var text in executor.InferAsync(JsonSerializer.Serialize(prompt), inferenceParams))
            {
                Console.Write(text);
            }
            Console.ForegroundColor = ConsoleColor.White;
                    
        }            
    }
}
