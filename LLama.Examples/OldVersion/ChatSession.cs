using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.OldVersion;

namespace LLama.Examples.Old
{
    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public class ChatSession
    {
        LLama.OldVersion.ChatSession<LLama.OldVersion.LLamaModel> _session;
        public ChatSession(string modelPath, string promptFilePath, string[] antiprompt)
        {
            LLama.OldVersion.LLamaModel model = new(new LLamaParams(model: modelPath, n_ctx: 512, interactive: true, repeat_penalty: 1.0f, verbose_prompt: false));
            _session = new ChatSession<LLama.OldVersion.LLamaModel>(model)
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
