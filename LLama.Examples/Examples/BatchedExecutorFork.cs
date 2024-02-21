using LLama.Batched;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;

namespace LLama.Examples.Examples;

/// <summary>
/// This demonstrates generating multiple replies to the same prompt, with a shared cache
/// </summary>
public class BatchedExecutorFork
{
    private const int n_split = 16;
    private const int n_len = 64;

    public static async Task Run()
    {
        string modelPath = UserSettings.GetModelPath();

        var parameters = new ModelParams(modelPath);
        using var model = LLamaWeights.LoadFromFile(parameters);

        Console.WriteLine("Prompt (leave blank to select automatically):");
        var prompt = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(prompt))
            prompt = "Not many people know that";

        // Create an executor that can evaluate a batch of conversations together
        var executor = new BatchedExecutor(model, parameters);

        // Print some info
        var name = executor.Model.Metadata.GetValueOrDefault("general.name", "unknown model name");
        Console.WriteLine($"Created executor with model: {name}");

        // Evaluate the initial prompt to create one conversation
        var start = executor.Prompt(prompt);
        await executor.Infer();

        // Create the root node of the tree
        var root = new Node(start);

        // Run inference loop
        for (var i = 0; i < n_len; i++)
        {
            if (i != 0)
                await executor.Infer();

            // Occasionally fork all the active conversations
            if (i != 0 && i % n_split == 0)
                root.Split();

            // Sample all active conversations
            root.Sample();
        }

        Console.WriteLine($"{prompt}...");
        root.Print(1);

        Console.WriteLine("Press any key to exit demo");
        Console.ReadKey(true);
    }

    class Node
    {
        private readonly StreamingTokenDecoder _decoder;

        private readonly DefaultSamplingPipeline _sampler;
        private Conversation? _conversation;

        private Node? _left;
        private Node? _right;

        public int ActiveConversationCount => _conversation != null ? 1 : _left!.ActiveConversationCount + _right!.ActiveConversationCount;

        public Node(Conversation conversation)
        {
            _sampler = new DefaultSamplingPipeline();
            _conversation = conversation;
            _decoder = new StreamingTokenDecoder(conversation.Executor.Context);
        }

        public void Sample()
        {
            if (_conversation == null)
            {
                _left?.Sample();
                _right?.Sample();
                return;
            }

            if (_conversation.RequiresInference)
                return;

            // Sample one token
            var ctx = _conversation.Executor.Context.NativeHandle;
            var logitsCopy = _conversation.Sample().ToArray();
            var token = _sampler.Sample(ctx, logitsCopy, Array.Empty<LLamaToken>());
            _sampler.Accept(ctx, token);
            _decoder.Add(token);

            // Prompt the conversation with this token, to continue generating from there
            _conversation.Prompt(token);
        }

        public void Split()
        {
            if (_conversation != null)
            {
                _left = new Node(_conversation.Fork());
                _right = new Node(_conversation.Fork());

                _conversation.Dispose();
                _conversation = null;
            }
            else
            {
                _left?.Split();
                _right?.Split();
            }
        }

        public void Print(int indendation)
        {
            var colors = new[] { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.White };
            Console.ForegroundColor = colors[indendation % colors.Length];

            var message = _decoder.Read().ReplaceLineEndings("");

            var prefix = new string(' ', indendation * 3);
            var suffix = _conversation == null ? "..." : "";
            Console.WriteLine($"{prefix}...{message}{suffix}");

            _left?.Print(indendation + 2);
            _right?.Print(indendation + 2);
        }
    }
}