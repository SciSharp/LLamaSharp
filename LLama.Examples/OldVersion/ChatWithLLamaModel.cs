﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.OldVersion;

namespace LLama.Examples.Old
{
    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public class ChatWithLLamaModel
    {
        LLama.OldVersion.LLamaModel _model;
        public ChatWithLLamaModel(string modelPath, string promptFilePath, string[] antiprompt)
        {
            _model = new LLama.OldVersion.LLamaModel(new LLamaParams(model: modelPath, n_ctx: 512, interactive: true, antiprompt: antiprompt.ToList(),
                repeat_penalty: 1.0f)).WithPromptFile(promptFilePath);
        }

        public void Run()
        {
            Console.Write("\nUser:");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                var question = Console.ReadLine();
                question += "\n";
                Console.ForegroundColor = ConsoleColor.White;
                var outputs = _model.Call(question);
                foreach (var output in outputs)
                {
                    Console.Write(output);
                }
            }
        }
    }
}
