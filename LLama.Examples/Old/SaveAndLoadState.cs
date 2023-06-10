using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.Old;

namespace LLama.Examples
{
    public class SaveAndLoadState: IDisposable
    {
        LLama.Old.LLamaModel _model;
        public SaveAndLoadState(string modelPath, string prompt)
        {
            _model = new LLama.Old.LLamaModel(new LLamaParams(model: modelPath, n_ctx: 2048, n_predict: -1, top_k: 10000, instruct: true,
                repeat_penalty: 1.1f, n_batch: 256, temp: 0.2f)).WithPrompt(prompt);
        }

        public void Run(string question)
        {
            // Only run once here.
            Console.Write("\nUser:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(question);
            Console.ForegroundColor = ConsoleColor.White;
            var outputs = _model.Call(question);
            foreach (var output in outputs)
            {
                Console.Write(output);
            }
        }

        public void SaveState(string filename)
        {
            _model.SaveState(filename);
            Console.WriteLine("Saved state!");
        }

        public void LoadState(string filename)
        {
            _model.LoadState(filename);
            Console.WriteLine("Loaded state!");
        }

        public void Dispose()
        {
            _model.Dispose();
        }
    }
}
