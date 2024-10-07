using LLama.Batched;
using LLama.Common;
using LLama.Native;
using Spectre.Console;

namespace LLama.Examples.Examples;

/// <summary>
/// This demonstrates beam search using the batched executor.
///
/// Beam search is a technique for finding the most likely multi-token completion from a prompt. The search keeps track of a
/// set of "beams", each beam is a possible completion and keeps track of it's cumulative probability. At each step all
/// of the current beams are split into multiple beams by extending the beam with different possible tokens (greedy sampling the
/// top N tokens), the set of _all_ beams is then trimmed down to just the most likely beams. This allows multiple possibilities to
/// be considered, and can find a higher probability result than simply greedy sampling the most likely token at every stage.
/// </summary>
public class BatchedExecutorBeamSearch
{
    public static async Task Run()
    {
        // Load model weights
        var parameters = new ModelParams(UserSettings.GetModelPath());
        using var model = await LLamaWeights.LoadFromFileAsync(parameters);

        var prompt = AnsiConsole.Ask("Prompt (or ENTER for default):", "The cat sat on");
        var tokensGenerate = AnsiConsole.Ask("How many tokens to generate?", 8);
        var beamsCount = AnsiConsole.Ask("How many parallel beams to keep track of?", 8);

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
        var beams = new List<Beam> { new Beam(conversation, 1.0, startTokens, [conversation.ConversationId]) };

        // Generate loop
        for (var i = 0; i < tokensGenerate; i++)
        {
            await executor.Infer();

            // Create new beams, forked from all original beams
            beams = (from oldBeam in beams
                     from beam in oldBeam.Sample(beamsCount)
                     select beam).OrderBy(a => a.CumulativeProbability).ToList();

            // Trim down list by removing low probability beams
            while (beams.Count > beamsCount)
            {
                var beam = beams[0];

                var text = beam.ToString().EscapeMarkup();
                AnsiConsole.MarkupLine($"[red]Culling Beam {beam.Conversation.ConversationId} (prob:{beam.CumulativeProbability:P5})[/]: {text}");

                beam.Dispose();
                beams.RemoveAt(0);
            }

            // Normalize all remaining beam probabilities.
            NormalizeBeams(beams);
        }

        // Print out all remaining beams
        AnsiConsole.MarkupLineInterpolated($"Final Beams:");
        beams.Reverse();
        foreach (var beam in beams)
        {
            AnsiConsole.MarkupLineInterpolated($"[yellow]Probability: {beam.CumulativeProbability:P10}[/]");
            AnsiConsole.MarkupLineInterpolated($"[yellow]Sequence: {string.Join(",", beam.Sequence)}[/]");
            AnsiConsole.MarkupLineInterpolated($"[green]{beam}[/]");
            Console.WriteLine();
        }

        Console.WriteLine("Press any key to exit demo");
        Console.ReadKey(true);
    }

    /// <summary>
    /// As the beam grows the cumulative probability gets very small. Normalizing all the beams prevents the value collapsing to zero.
    /// </summary>
    /// <param name="beams"></param>
    private static void NormalizeBeams(List<Beam> beams)
    {
        // Find max probability
        var max = beams.MaxBy(a => a.CumulativeProbability)!.CumulativeProbability;

        // Divide all beams by max, this makes the max prob = 1.0
        foreach (var beam in beams)
            beam.CumulativeProbability /= max;
    }

    private class Beam
        : IDisposable
    {
        public readonly Conversation Conversation;
        public readonly IReadOnlyList<LLamaToken> Tokens;
        public readonly IReadOnlyList<LLamaSeqId> Sequence;

        public double CumulativeProbability;

        public Beam(Conversation conversation, double prob, IReadOnlyList<LLamaToken> tokens, IReadOnlyList<LLamaSeqId> sequence)
        {
            Conversation = conversation;
            Tokens = tokens;
            Sequence = sequence;

            CumulativeProbability = prob;
        }

        public void Dispose()
        {
            Conversation.Dispose();
        }

        public List<Beam> Sample(int nbeams)
        {
            // Apply softmax, this calculates probabilities and sorts tokens into descending order
            var logitsArr = LLamaTokenDataArray.Create(Conversation.Sample());
            logitsArr.Softmax();

            // Create new forked conversations, one for each beam
            var results = new List<Beam>();
            for (var i = 0; i < nbeams; i++)
            {
                // After softmax the logits array is in descending order of probability. Take the first `nbeams` items to make new beams.
                var item = logitsArr.Data.Span[i];

                // Fork the parent conversation. This shares all of the KV cache with the parent (and other forks)
                // so does not cost any extra memory.
                var c = Conversation.Fork();

                // Extend the conversation with the selected token.
                c.Prompt(item.ID);

                // Keep track of the cumulative probability of this entire sequence.
                var p = CumulativeProbability * item.Probability;

                // Keep track of all tokens in this sequence, for decoding later
                var t = Tokens.ToList();
                t.Add(item.ID);

                // Keep track of which beam this beam was derived from.
                var s = Sequence.ToList();
                s.Add(c.ConversationId);

                results.Add(new Beam(c, p, t, s));
            }

            // Dispose self now that child beams have spawned
            Conversation.Dispose();
            return results;
        }

        public override string ToString()
        {
            var decoder = new StreamingTokenDecoder(Conversation.Executor.Context);
            decoder.AddRange(Tokens);
            return decoder.Read();
        }
    }
}