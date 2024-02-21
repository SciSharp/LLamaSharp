﻿using LLama.Common;

namespace LLama.Examples.Examples
{
    public class InstructModeExecute
    {
        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            var prompt = File.ReadAllText("Assets/dan.txt").Trim();

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024,
                Seed = 1337,
                GpuLayerCount = 5
            };
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var executor = new InstructExecutor(context);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the LLM will follow your instructions. For example, you can input \"Write a story about a fox who want to " +
                "make friend with human, no less than 200 words.\"");
            Console.ForegroundColor = ConsoleColor.White;

            var inferenceParams = new InferenceParams() { Temperature = 0.8f, MaxTokens = 600 };

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
