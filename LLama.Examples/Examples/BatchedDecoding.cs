﻿using System.Diagnostics;
using System.Text;
using LLama.Common;
using LLama.Native;

namespace LLama.Examples.Examples;

/// <summary>
/// This demonstrates generating multiple replies to the same prompt, with a shared cache
/// </summary>
/// <remarks>Note that this is currently using the low level API directly, future work will provide a safer C# wrapper over this!</remarks>
public class BatchedDecoding
{
    private const int n_parallel = 8;
    private const int n_len = 32;

    private const int top_k = 80;
    private const float top_p = 0.8f;
    private const float temp = 0.75f;

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
        var prompt_tokens = model.NativeHandle.Tokenize(prompt, true, false, Encoding.UTF8);
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

        using var batch = LLamaBatchSafeHandle.Create(Math.Max(prompt_tokens.Length, n_parallel), 0, 1);

        // evaluate the initial prompt
        for (var i = 0; i < prompt_tokens.Length; i++)
            batch.LLamaBatchAdd(prompt_tokens[i], i, new[] { (LLamaSeqId)0 }, false);
        Debug.Assert(batch.NativeBatch.n_tokens == prompt_tokens.Length);

        // llama_decode will output logits only for the last token of the prompt
        unsafe
        {
            batch.NativeBatch.logits[batch.NativeBatch.n_tokens - 1] = 1;
        }

        if (context.NativeHandle.Decode(batch) != 0)
        {
            await Console.Error.WriteLineAsync("llama_decode failed");
            return;
        }

        // assign the system KV cache to all parallel sequences
        // this way, the parallel sequences will "reuse" the prompt tokens without having to copy them
        for (var i = 1; i < n_parallel; ++i)
        {
            NativeApi.llama_kv_cache_seq_cp(context.NativeHandle, (LLamaSeqId)0, (LLamaSeqId)i, 0, batch.NativeBatch.n_tokens);
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
            i_batch.Add(batch.NativeBatch.n_tokens - 1);

        var n_cur = batch.NativeBatch.n_tokens;
        var n_decode = 0;

        var streams = new List<int>[n_parallel];
        for (var i = 0; i < n_parallel; i++)
            streams[i] = new();

        var eos = model.EndOfSentenceToken;
        var nl = model.NewlineToken;

        var timer = new Stopwatch();
        timer.Start();
        while (n_cur <= n_len)
        {
            batch.LLamaBatchClear();

            for (var i = 0; i < n_parallel; i++)
            {
                // Skip completed streams
                if (i_batch[i] < 0)
                    continue;

                var n_vocab = model.VocabCount;
                LLamaTokenDataArray candidates;
                unsafe
                {
                    candidates = LLamaTokenDataArray.Create(new Span<float>(NativeApi.llama_get_logits_ith(context.NativeHandle, i_batch[i]), n_vocab));
                }

                candidates.TopK(context.NativeHandle, top_k);
                candidates.TopP(context.NativeHandle, top_p);
                candidates.Temperature(context.NativeHandle, temp);
                var new_token_id = candidates.SampleToken(context.NativeHandle);

                if (new_token_id == eos || new_token_id == nl)
                {
                    i_batch[i] = -1;
                    Console.WriteLine($"Completed Stream {i} early");
                    continue;
                }

                streams[i].Add(new_token_id);

                i_batch[i] = batch.NativeBatch.n_tokens;

                // push this new token for next evaluation
                batch.LLamaBatchAdd(new_token_id, n_cur, new[] { (LLamaSeqId)i }, true);

                n_decode++;
            }

            // all streams are finished
            if (batch.NativeBatch.n_tokens == 0)
            {
                break;
            }

            n_cur++;

            // evaluate the current batch with the transformer model
            if (context.NativeHandle.Decode(batch) != 0)
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
        foreach (var stream in streams)
        {
            var text = context.DeTokenize(stream);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{index++}. {prompt}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
        }

        Console.WriteLine("Press any key to exit demo");
        Console.ReadKey(true);
    }
}