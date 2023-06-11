using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.OldVersion;

namespace LLama.Examples.Old
{
    public class InstructMode
    {
        LLama.OldVersion.LLamaModel _model;
        public InstructMode(string modelPath, string promptFile)
        {
            _model = new LLama.OldVersion.LLamaModel(new LLamaParams(model: modelPath, n_ctx: 2048, n_predict: -1, top_k: 10000, instruct: true,
                repeat_penalty: 1.1f, n_batch: 256, temp: 0.2f)).WithPromptFile(promptFile);
        }

        public void Run()
        {
            Console.WriteLine("\n### Instruction:\n >");
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
