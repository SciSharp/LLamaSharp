using LLama.Batched;
using LLama.Common;
using LLama.Native;
using Spectre.Console;

namespace LLama.Examples.Examples;

/// <summary>
/// This demonstrates beam search using the batched executor
/// </summary>
public class BatchedExecutorBeamSearch
{
    /// <summary>
    /// Set how many tokens to generate
    /// </summary>
    private const int TokensGenerate = 24;

    /// <summary>
    /// Set how many parallel beams to keep
    /// </summary>
    private const int BeamsCount = 3;

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
        var conversation = executor.Create();
        var startTokens = executor.Context.Tokenize(prompt);
        conversation.Prompt(startTokens);
        
        // Create one beam, containing that conversation
        var beams = new List<Beam>();
        beams.Add(new Beam(conversation, 1.0, startTokens));

        // Print the prompt
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(prompt);

        // Generate loop
        for (var i = 0; i < TokensGenerate; i++)
        {
            await executor.Infer();

            // Create new beams, forked from all original beams
            beams = (from oldBeam in beams
                     from beam in oldBeam.Sample(BeamsCount)
                     select beam).OrderBy(a => a.CumulativeProbability).ToList();

            // Trim down list by removing low probability beams
            while (beams.Count > BeamsCount)
            {
                var beam = beams[0];
                AnsiConsole.MarkupLineInterpolated($"[red]Culling Beam (prob:{beam.CumulativeProbability:P10})[/]: {beam}");

                beam.Dispose();
                beams.RemoveAt(0);
            }
        }

        // Print out all remaining beams
        AnsiConsole.MarkupLineInterpolated($"Final Beams:");
        beams.Reverse();
        foreach (var beam in beams)
            AnsiConsole.MarkupLineInterpolated($"[green]Culling Beam (prob:{beam.CumulativeProbability:P10})[/]: {beam}");

        Console.WriteLine("Press any key to exit demo");
        Console.ReadKey(true);
    }

    private class Beam
        : IDisposable
    {
        public readonly Conversation Conversation;
        public readonly double CumulativeProbability;
        public readonly IReadOnlyList<LLamaToken> Tokens;

        public Beam(Conversation conversation, double prob, IReadOnlyList<LLamaToken> tokens)
        {
            Conversation = conversation;
            CumulativeProbability = prob;
            Tokens = tokens;
        }

        public void Dispose()
        {
            Conversation.Dispose();
        }

        public List<Beam> Sample(int nbeams)
        {
            // Apply softmax, this calculates probabilities and sorts tokens into descending order
            var logitsArr = LLamaTokenDataArray.Create(Conversation.Sample());
            logitsArr.Softmax(Conversation.Executor.Context.NativeHandle);

            // Create new forked conversations, one for each beam
            var results = new List<Beam>();
            for (var i = 0; i < nbeams; i++)
            {
                var item = logitsArr.Data.Span[i];

                var c = Conversation.Fork();
                c.Prompt(item.id);

                var p = CumulativeProbability * item.p;

                var t = Tokens.ToList();
                t.Add(item.id);

                results.Add(new Beam(c, p, t));
            }

            // Dispose self now that child beams have spawned
            Conversation.Dispose();
            return results;
        }

        public override string ToString()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return Conversation.Executor.Context.DeTokenize(Tokens);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}