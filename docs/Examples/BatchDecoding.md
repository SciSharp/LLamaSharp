# Batch decoding

```cs
using System.Diagnostics;
using System.Text;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;

public class BatchedDecoding
{
    private const int n_parallel = 8;
    private const int n_len = 32;

    public static async Task Run()
    {
        Console.Write("Please input your model path: ");
        var modelPath = Console.ReadLine();

        Console.WriteLine("Prompt (leave blank to select automatically):");
        var prompt = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(prompt))
            prompt = "Not many people know that";

        // Load model
        var parameters = new ModelParams(modelPath);

        using var model = LLamaWeights.LoadFromFile(parameters);

        // Tokenize prompt
        var prompt_tokens = model.Tokenize(prompt, true, false, Encoding.UTF8);
        var n_kv_req = prompt_tokens.Length + (n_len - prompt_tokens.Length) * n_parallel;

        // Create a context
        parameters.ContextSize = (uint)model.ContextSize;
        parameters.BatchSize = (uint)Math.Max(n_len, n_parallel);
        using var context = model.CreateContext(parameters);

        var n_ctx = context.ContextSize;

        // make sure the KV cache is big enough to hold all the prompt and generated tokens
        if (n_kv_req > n_ctx)
        {
            await Console.Error.WriteLineAsync($"error: n_kv_req ({n_kv_req}) > n_ctx, the required KV cache size is not big enough\n");
            await Console.Error.WriteLineAsync("        either reduce n_parallel or increase n_ctx\n");
            return;
        }

        var batch = new LLamaBatch();

        // evaluate the initial prompt
        batch.AddRange(prompt_tokens, 0, LLamaSeqId.Zero, true);

        if (await context.DecodeAsync(batch) != DecodeResult.Ok)
        {
            await Console.Error.WriteLineAsync("llama_decode failed");
            return;
        }

        // assign the system KV cache to all parallel sequences
        // this way, the parallel sequences will "reuse" the prompt tokens without having to copy them
        for (var i = 1; i < n_parallel; ++i)
        {
            context.NativeHandle.KvCacheSequenceCopy((LLamaSeqId)0, (LLamaSeqId)i, 0, batch.TokenCount);
        }

        if (n_parallel > 1)
        {
            Console.WriteLine();
            Console.WriteLine($"generating {n_parallel} sequences...");
        }

        // remember the batch index of the last token for each parallel sequence
        // we need this to determine which logits to sample from
        List<int> i_batch = new();
        for (var i = 0; i < n_parallel; i++)
            i_batch.Add(batch.TokenCount - 1);

        // Create per-stream decoder and sampler
        var decoders = new StreamingTokenDecoder[n_parallel];
        var samplers = new ISamplingPipeline[n_parallel];
        for (var i = 0; i < n_parallel; i++)
        {
            decoders[i] = new StreamingTokenDecoder(context);
            samplers[i] = new DefaultSamplingPipeline
            {
                Temperature = 0.1f + (float)i / n_parallel,
                MinP = 0.25f,
            };
        }

        var n_cur = batch.TokenCount;
        var n_decode = 0;

        var timer = new Stopwatch();
        timer.Start();
        while (n_cur <= n_len)
        {
            batch.Clear();

            for (var i = 0; i < n_parallel; i++)
            {
                // Skip completed streams
                if (i_batch[i] < 0)
                    continue;

                // Use the sampling pipeline to select a token
                var new_token_id = samplers[i].Sample(
                    context.NativeHandle,
                    context.NativeHandle.GetLogitsIth(i_batch[i]),
                    Array.Empty<LLamaToken>()
                );

                // Finish this stream early if necessary
                if (new_token_id == model.EndOfSentenceToken || new_token_id == model.NewlineToken)
                {
                    i_batch[i] = -1;
                    Console.WriteLine($"Completed Stream {i} early");
                    continue;
                }

                // Add this token to the decoder, so it will be turned into text
                decoders[i].Add(new_token_id);

                i_batch[i] = batch.TokenCount;

                // push this new token for next evaluation
                batch.Add(new_token_id, n_cur, (LLamaSeqId)i, true);

                n_decode++;
            }

            // Check if all streams are finished
            if (batch.TokenCount == 0)
            {
                break;
            }

            n_cur++;

            // evaluate the current batch with the transformer model
            if (await context.DecodeAsync(batch) != 0)
            {
                await Console.Error.WriteLineAsync("failed to eval");
                return;
            }
        }

        timer.Stop();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        Console.WriteLine($"Decoded {n_decode} tokens in {timer.ElapsedMilliseconds}ms");
        Console.WriteLine($"Rate: {n_decode / timer.Elapsed.TotalSeconds:##.000} tokens/second");

        var index = 0;
        foreach (var stream in decoders)
        {
            var text = stream.Read();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{index++}. {prompt}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
        }

        Console.WriteLine("Press any key to exit demo");
        Console.ReadKey(true);
    }
}
```