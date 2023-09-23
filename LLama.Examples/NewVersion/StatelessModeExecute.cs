using LLama.Common;

namespace LLama.Examples.NewVersion
{
    public class StatelessModeExecute
    {
        public static async Task Run()
        {
            Console.Write("Please input your model path: ");
            var modelPath = Console.ReadLine();

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024,
                Seed = 1337,
                GpuLayerCount = 5
            };
            using var model = LLamaWeights.LoadFromFile(parameters);
            var ex = new StatelessExecutor(model, parameters);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the inference is an one-time job. That says, the previous input and response has " +
                "no impact on the current response. Now you can ask it questions. Note that in this example, no prompt was set for LLM and the maximum response tokens is 50. " +
                "It may not perform well because of lack of prompt. This is also an example that could indicate the improtance of prompt in LLM. To improve it, you can add " +
                "a prompt for it yourself!");
            Console.ForegroundColor = ConsoleColor.White;

            var inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "Question:", "#", "Question: ", ".\n" }, MaxTokens = 50 };

            while (true)
            {
                Console.Write("\nQuestion: ");
                Console.ForegroundColor = ConsoleColor.Green;
                var prompt = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Answer: ");
                prompt = $"Question: {prompt?.Trim()} Answer: ";
                await foreach (var text in Spinner(ex.InferAsync(prompt, inferenceParams)))
                {
                    Console.Write(text);
                }
            }
        }

        /// <summary>
        /// Show a spinner while waiting for the next result
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static async IAsyncEnumerable<string> Spinner(IAsyncEnumerable<string> source)
        {
            var enumerator = source.GetAsyncEnumerator();

            var characters = new[] { '|', '/', '-', '\\' };

            while (true)
            {
                var next = enumerator.MoveNextAsync();

                var (Left, Top) = Console.GetCursorPosition();

                // Keep showing the next spinner character while waiting for "MoveNextAsync" to finish
                var count = 0;
                while (!next.IsCompleted)
                {
                    count = (count + 1) % characters.Length;
                    Console.SetCursorPosition(Left, Top);
                    Console.Write(characters[count]);
                    await Task.Delay(75);
                }

                // Clear the spinner character
                Console.SetCursorPosition(Left, Top);
                Console.Write(" ");
                Console.SetCursorPosition(Left, Top);

                if (!next.Result)
                    break;
                yield return enumerator.Current;
            }
        }
    }
}
