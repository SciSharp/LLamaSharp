using LLama.Batched;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using Spectre.Console;

namespace LLama.Examples.Examples;

/// <summary>
/// This demonstrates generating multiple replies to the same prompt, with a shared cache
/// </summary>
public class BatchedExecutorFork
{
    private const int n_split = 16;
    private const int n_len = 72;

    public static async Task Run()
    {
        string modelPath = UserSettings.GetModelPath();

        var parameters = new ModelParams(modelPath);
        using var model = LLamaWeights.LoadFromFile(parameters);

        var prompt = AnsiConsole.Ask("Prompt (or ENTER for default):", "Not many people know that");

        // Create an executor that can evaluate a batch of conversations together
        using var executor = new BatchedExecutor(model, parameters);

        // Print some info
        var name = executor.Model.Metadata.GetValueOrDefault("general.name", "unknown model name");
        Console.WriteLine($"Created executor with model: {name}");

        // Evaluate the initial prompt to create one conversation
        using var start = executor.Prompt(prompt);
        await executor.Infer();

        // Create the root node of the tree
        var root = new Node(start);

        await AnsiConsole
            .Progress()
            .StartAsync(async progress =>
            {
                var reporter = progress.AddTask("Running Inference (1)", maxValue: n_len);

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

                    // Update progress bar
                    reporter.Increment(1);
                    reporter.Description($"Running Inference ({root.ActiveConversationCount})");
                }

                // Display results
                var display = new Tree(prompt);
                root.Display(display);
                AnsiConsole.Write(display);
            });
    }

    private class Node
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
            var token = _sampler.Sample(ctx, _conversation.Sample(), Array.Empty<LLamaToken>());
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

        public void Display<T>(T tree, int depth = 0)
            where T : IHasTreeNodes
        {
            var colors = new[] { "red", "green", "blue", "yellow", "white" };
            var color = colors[depth % colors.Length];

            var message = _decoder.Read().ReplaceLineEndings("");

            var n = tree.AddNode($"[{color}]{message}[/]");

            _left?.Display(n, depth + 1);
            _right?.Display(n, depth + 1);
        }
    }
}