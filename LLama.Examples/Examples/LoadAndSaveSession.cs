using LLama.Common;
using LLama.Sampling;

namespace LLama.Examples.Examples
{
    public class SaveAndLoadSession
    {
        public static async Task Run()
        {
            string modelPath = UserSettings.GetModelPath();

            var prompt = (await File.ReadAllTextAsync("Assets/chat-with-bob.txt")).Trim();

            var parameters = new ModelParams(modelPath)
            {
                GpuLayerCount = 5
            };
            using var model = await LLamaWeights.LoadFromFileAsync(parameters);
            using var context = model.CreateContext(parameters);
            var ex = new InteractiveExecutor(context);

            var session = new ChatSession(ex);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The chat session has started. In this example, the prompt is printed for better visual result. Input \"save\" to save and reload the session.");
            Console.ForegroundColor = ConsoleColor.White;

            // show the prompt
            Console.Write(prompt);
            while (true)
            {
                await foreach (
                    var text
                    in session.ChatAsync(
                        new ChatHistory.Message(AuthorRole.User, prompt),
                        new InferenceParams()
                        {
                            SamplingPipeline = new DefaultSamplingPipeline
                            {
                                Temperature = 0.6f
                            },
                            AntiPrompts = new List<string> { "User:" }
                        }))
                {
                    Console.Write(text);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                prompt = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                if (prompt == "save")
                {
                    Console.Write("Preparing to save the state, please input the path you want to save it: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    var statePath = Console.ReadLine();
                    session.SaveSession(statePath);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Saved session!");
                    Console.ForegroundColor = ConsoleColor.White;

                    ex.Context.Dispose();
                    ex = new(new LLamaContext(model, parameters));
                    session = new ChatSession(ex);
                    session.LoadSession(statePath);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Loaded session!");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write("Now you can continue your session: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    prompt = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
