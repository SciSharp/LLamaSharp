﻿using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLama.Examples.NewVersion
{
    public class InstructModeExecute
    {
        public static void Run()
        {
            Console.Write("Please input your model path: ");
            string modelPath = Console.ReadLine();
            var prompt = File.ReadAllText("Assets/dan.txt").Trim();

            InstructExecutor ex = new(new LLamaModelContext(new ModelParams(modelPath, contextSize: 1024)));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the LLM will follow your instructions. For example, you can input \"Write a story about a fox who want to " +
                "make friend with human, no less than 200 words.\"");
            Console.ForegroundColor = ConsoleColor.White;

            var inferenceParams = new InferenceParams() { Temperature = 0.8f, MaxTokens = 600 };

            while (true)
            {
                foreach (var text in ex.Infer(prompt, inferenceParams))
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
