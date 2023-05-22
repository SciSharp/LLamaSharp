using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLama.Examples
{
    public class ChatSession
    {
        ChatSession<LLamaModel> _session;
        public ChatSession(string modelPath, string promptFilePath, string[] antiprompt)
        {
            LLamaModel model = new(new LLamaParams(model: modelPath, n_ctx: 512, interactive: true, repeat_penalty: 1.0f, verbose_prompt: false));
            _session = new ChatSession<LLamaModel>(model)
                .WithPromptFile(promptFilePath)
                .WithAntiprompt(antiprompt);
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
                var outputs = _session.Chat(question, encoding: "UTF-8");
                foreach (var output in outputs)
                {
                    Console.Write(output);
                }
            }
        }
    }
}
