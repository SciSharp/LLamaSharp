using LLama.Batched;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using Spectre.Console;

namespace LLama.Examples.Examples;

/// <summary>
/// This demonstrates generating tokens and then rewinding to an earlier state
/// </summary>
public class BatchedExecutorRewind
{
    private const int n_generate = 24;
    private const int n_rewind = 12;
    private const int n_repeats = 6;

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
        using var conversation = executor.Prompt(prompt);
        
        // Create the start node wrapping the conversation
        var node = new Node(executor.Context);

        // Print the prompt
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(prompt);

        for (var i = 0; i < n_repeats; i++)
        {
            for (var j = 0; j < n_generate; j++)
            {
                // Run inference
                await executor.Infer();

                // Sample a token
                var token = node.Sample(conversation);

                // Continue conversation with this token
                if (j != n_generate - 1)
                    conversation.Prompt(token);
            }

            // Write out what we generated
            node.Write(n_rewind, i + 1);

            // Rewind back a few tokens
            conversation.Rewind(n_rewind + 1);

            // Prompt with a token
            conversation.Prompt(node.GetToken(n_generate - n_rewind - 1));

            // Create a new node around the rewound conversation
            node = new Node(executor.Context);
        }

        Console.WriteLine("Press any key to exit demo");
        Console.ReadKey(true);
    }

    private class Node
    {
        private readonly LLamaContext _context;

        private readonly List<LLamaToken> _tokens = new List<LLamaToken>();
        private readonly DefaultSamplingPipeline Sampler;

        public Node(LLamaContext context)
        {
            _context = context;
            Sampler = new DefaultSamplingPipeline();
        }

        public LLamaToken Sample(Conversation conversation)
        {
            var token = Sampler.Sample(_context.NativeHandle, conversation.Sample(), Array.Empty<LLamaToken>());
            _tokens.Add(token);
            return token;
        }

        public void Write(int n_rewind, int depth)
        {
            var decoder = new StreamingTokenDecoder(_context);

            for (var i = 0; i < _tokens.Count - n_rewind; i++)
                decoder.Add(_tokens[i]);

            AnsiConsole.MarkupLine($"[green]{new string(' ', depth * 3) + decoder.Read().ReplaceLineEndings(" ")}[/]");

            for (var i = _tokens.Count - n_rewind; i < _tokens.Count; i++)
                decoder.Add(_tokens[i]);

            AnsiConsole.MarkupLine($"[maroon]{decoder.Read().ReplaceLineEndings(" ")}[/]");
        }

        public LLamaToken GetToken(int index)
        {
            return _tokens[index];
        }
    }
}