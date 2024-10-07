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
    /// <summary>
    /// Set how many tokens to generate before rewinding
    /// </summary>
    private const int TokensGenerate = 24;

    /// <summary>
    /// Set how many tokens to rewind
    /// </summary>
    private const int TokensRewind = 12;

    /// <summary>
    /// Set how many times to generate and rewind
    /// </summary>
    private const int RepeatCount = 6;

    public static async Task Run()
    {
        // Load model weights
        var parameters = new ModelParams(UserSettings.GetModelPath());
        using var model = await LLamaWeights.LoadFromFileAsync(parameters);

        var prompt = AnsiConsole.Ask("Prompt (or ENTER for default):", "Not many people know that");

        // Create an executor that can evaluate a batch of conversations together
        using var executor = new BatchedExecutor(model, parameters);

        // Print some info
        var name = model.Metadata.GetValueOrDefault("general.name", "unknown model name");
        Console.WriteLine($"Created executor with model: {name}");

        // Evaluate the initial prompt to create one conversation
        using var conversation = executor.Create();
        conversation.Prompt(executor.Context.Tokenize(prompt));
        
        // Create the start node wrapping the conversation
        var node = new Node();

        // Print the prompt
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(prompt);

        for (var i = 0; i < RepeatCount; i++)
        {
            for (var j = 0; j < TokensGenerate; j++)
            {
                // Run inference
                await executor.Infer();

                // Sample a token
                var token = node.Sample(conversation);

                // Continue conversation with this token
                if (j != TokensGenerate - 1)
                    conversation.Prompt(token);
            }

            // Write out what we generated
            node.Write(executor.Context, TokensRewind, i + 1);

            // Rewind back a few tokens
            conversation.Rewind(TokensRewind + 1);

            // Prompt with a token
            conversation.Prompt(node.GetToken(TokensGenerate - TokensRewind - 1));

            // Create a new node around the rewound conversation
            node = new Node();
        }

        Console.WriteLine("Press any key to exit demo");
        Console.ReadKey(true);
    }

    private class Node
    {
        private readonly List<LLamaToken> _tokens = [ ];
        private readonly DefaultSamplingPipeline _sampler = new();

        public LLamaToken Sample(Conversation conversation)
        {
            var token = _sampler.Sample(conversation.Executor.Context.NativeHandle, conversation.GetSampleIndex());
            _tokens.Add(token);
            return token;
        }

        public void Write(LLamaContext context, int rewind, int depth)
        {
            var decoder = new StreamingTokenDecoder(context);

            for (var i = 0; i < _tokens.Count - rewind; i++)
                decoder.Add(_tokens[i]);

            AnsiConsole.MarkupLine($"[green]{new string(' ', depth * 3) + decoder.Read().ReplaceLineEndings(" ")}[/]");

            for (var i = _tokens.Count - rewind; i < _tokens.Count; i++)
                decoder.Add(_tokens[i]);

            AnsiConsole.MarkupLine($"[maroon]{decoder.Read().ReplaceLineEndings(" ")}[/]");
        }

        public LLamaToken GetToken(int index)
        {
            return _tokens[index];
        }
    }
}