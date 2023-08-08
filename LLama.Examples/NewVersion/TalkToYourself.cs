using System.Security.Cryptography;
using System.Text;
using LLama.Abstractions;
using LLama.Common;
using LLama.Extensions;
using LLama.Native;

namespace LLama.Examples.NewVersion
{
    public class TalkToYourself
    {
        public static async Task Run()
        {
            Console.Write("Please input your model path: ");
            string modelPath = "C:\\Users\\Martin\\Documents\\Python\\oobabooga_windows\\text-generation-webui\\models\\llama-2-7b-chat.ggmlv3.q6_K.bin";

            // todo: model path is passed here, but isn't needed
            var @params = new ModelParams(modelPath)
            {
                Seed = RandomNumberGenerator.GetInt32(int.MaxValue)
            };

            // todo: all this pin stuff is ugly and should be hidden in the higher level wrapper
            using var pin = @params.ToLlamaContextParams(out var lparams);

            // todo: we need a higher level wrapper around the model weights (LLamaWeights??)
            var weights = SafeLlamaModelHandle.LoadFromFile(modelPath, lparams);

            // todo: need a method on the LLamaWeights which does this
            var ctx1 = new LLamaContext(weights.CreateContext(lparams), @params, Encoding.UTF8);
            var ctx2 = new LLamaContext(weights.CreateContext(lparams), @params, Encoding.UTF8);

            var alice = new InteractiveExecutor(ctx1);
            var bob = new InteractiveExecutor(ctx2);

            // Initial alice prompt
            var alicePrompt = "Transcript of a dialog, where the Alice interacts a person named Bob. Alice is friendly, kind, honest and good at writing.\nAlice: Hello";
            var aliceResponse = await Prompt(alice, ConsoleColor.Green, alicePrompt, false, false);

            // Initial bob prompt
            var bobPrompt = $"Transcript of a dialog, where the Bob interacts a person named Alice. Bob is smart, intellectual and good at writing.\nAlice: Hello{aliceResponse}";
            var bobResponse = await Prompt(alice, ConsoleColor.Red, bobPrompt, true, true);

            // swap back and forth from Alice to Bob
            while (true)
            {
                aliceResponse = await Prompt(alice, ConsoleColor.Green, bobResponse, false, true);
                bobResponse = await Prompt(alice, ConsoleColor.Red, aliceResponse, false, true);
                Thread.Sleep(1000);
            }
        }

        private static async Task<string> Prompt(ILLamaExecutor executor, ConsoleColor color, string prompt, bool showPrompt, bool showResponse)
        {
            var inferenceParams = new InferenceParams
            {
                Temperature = 0.9f,
                AntiPrompts = new List<string> { "Alice:", "Bob:", "User:" },
                MaxTokens = 128,
                Mirostat = MirostatType.Mirostat2,
                MirostatTau = 10,
            };

            Console.ForegroundColor = ConsoleColor.White;
            if (showPrompt)
                Console.Write(prompt);

            Console.ForegroundColor = color;
            var builder = new StringBuilder();
            await foreach (var text in executor.InferAsync(prompt, inferenceParams))
            {
                builder.Append(text);
                if (showResponse)
                    Console.Write(text);
            }

            return builder.ToString();
        }
    }
}
