using System.Text;
using LLama.Abstractions;
using LLama.Common;
using LLama.Sampling;

namespace LLama.Examples.Examples
{
    public class TalkToYourself
    {
        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            // Load weights into memory
            var @params = new ModelParams(modelPath);
            using var weights = await LLamaWeights.LoadFromFileAsync(@params);

            // Create 2 contexts sharing the same weights
            using var aliceCtx = weights.CreateContext(@params);
            var alice = new InteractiveExecutor(aliceCtx);
            using var bobCtx = weights.CreateContext(@params);
            var bob = new InteractiveExecutor(bobCtx);

            // Initial alice prompt
            var alicePrompt = "Transcript of a dialog, where the Alice interacts with a person named Bob. Alice is friendly, kind, honest and good at writing.\nAlice: Hello";
            var aliceResponse = await Prompt(alice, ConsoleColor.Green, alicePrompt, false, false);

            // Initial bob prompt
            var bobPrompt = $"Transcript of a dialog, where the Bob interacts a person named Alice. Bob is smart, intellectual and good at writing.\nAlice: Hello{aliceResponse}";
            var bobResponse = await Prompt(bob, ConsoleColor.Red, bobPrompt, true, true);

            // swap back and forth from Alice to Bob
            while (true)
            {
                aliceResponse = await Prompt(alice, ConsoleColor.Green, bobResponse, false, true);
                bobResponse = await Prompt(bob, ConsoleColor.Red, aliceResponse, false, true);

                if (Console.KeyAvailable)
                    break;
            }
        }

        private static async Task<string> Prompt(ILLamaExecutor executor, ConsoleColor color, string prompt, bool showPrompt, bool showResponse)
        {
            var inferenceParams = new InferenceParams
            {
                SamplingPipeline = new DefaultSamplingPipeline(),
                AntiPrompts = [ "Alice:", "Bob:", "User:" ],
                MaxTokens = 128,
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
