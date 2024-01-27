using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Handlers;
using LLava;
using LLama.Common;

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
            
            byte[] imageArray = System.IO.File.ReadAllBytes(exampleImage);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            string imageText = "<img src=\"data:image/jpeg;base64," + base64ImageRepresentation + "\">";
            
            // LLaVa chat format is "<system_prompt>\nUSER:<image_embeddings>\n<textual_prompt>\nASSISTANT:"
            var prompt = "A chat between a curious human and an artificial intelligence assistant. The assistant gives helpful, detailed, and polite answers to the human's questions.\nUSER:"+ imageText + "  \nDescribe the image\nASSISTANT:";

            
            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 2048,
                Seed = 1337,
                GpuLayerCount = 5
            };
            
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var clipModel = LLavaWeights.LoadFromFile(mmProject);
            using var context = model.CreateContext(parameters);

            var executor = new LLavaInteractExecutor(context, clipModel );

            var session = new ChatSession(executor);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The chat session has started. In this example, the prompt is printed for better visual result.");
            Console.ForegroundColor = ConsoleColor.White;

            prompt = "";
            
            // show the prompt
            Console.Write(prompt);
            
            var inferenceParams = new InferenceParams() { Temperature = 0.1f, AntiPrompts = new List<string> { "USER:" }, MaxTokens = 128 };

            while (true)
            {
                await foreach (var text in executor.InferAsync(prompt, inferenceParams))
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
