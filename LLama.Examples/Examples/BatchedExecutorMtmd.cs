using System;
using System.Collections.Generic;
using System.IO;
using LLama.Batched;
using LLama.Common;
using LLama.Exceptions;
using LLama.Native;
using LLama.Sampling;
using Spectre.Console;

namespace LLama.Examples.Examples;

/// <summary>
/// Demonstrates how to evaluate an image with MTMD helpers and continue generation by
/// manually scheduling batches, similar to what the batched executor does internally.
/// </summary>
public class BatchedExecutorMtmd
{
    /// <summary>
    /// Number of completion tokens to generate after sending the image prompt.
    /// </summary>
    public const int TokenCount = 10000;

    public static async Task Run()
    {
        // Load the base LLM and its clip/mtmd sidecar weights so the executor has everything it needs.
        var parameters = new ModelParams(UserSettings.GetModelPath());
        using var model = await LLamaWeights.LoadFromFileAsync(parameters);
        var mtmdParams = MtmdContextParams.Default(); // reuse llama.cpp defaults for helper settings
        mtmdParams.UseGpu = false;
        var marker = mtmdParams.MediaMarker ?? NativeApi.MtmdDefaultMarker() ?? "<media>";

        using var mtmd = await MtmdWeights.LoadFromFileAsync(UserSettings.GetMMProjPath(), model, mtmdParams); // multimodal helper weights

        using var executor = new BatchedExecutor(model, parameters, mtmd); // drives batched token + chunk evaluation

        // Prepend the media marker so the helper knows where to inject the encoded image tokens.
        var defaultPrompt = "\nUSER: Provide a full description of the image.\nASSISTANT: ";
        var promptSuffix = AnsiConsole.Ask("Prompt (or ENTER for default):", defaultPrompt);
        var promptText = string.Concat(marker, promptSuffix);

        var imagePath = UserSettings.GetImagePath();
        AnsiConsole.Write(new CanvasImage(imagePath));

        var vocab = executor.Context.NativeHandle.ModelHandle.Vocab;

        // Simple low-temperature sampler keeps the demo deterministic-ish.
        var sampler = new DefaultSamplingPipeline
        {
            Temperature = 0.1f
        };

        // Stream decoded text to the console as soon as tokens arrive.
        var decoder = new StreamingTokenDecoder(executor.Context)
        {
            DecodeSpecialTokens = false
        };

        try
        {
            // Each conversation tracks its own KV cache sequence IDs.
            var conversation = executor.Create();
            // enqueue the image so MtmdHelper sees it
            conversation.QueueMedia(imagePath); 
            // schedule multimodal prompt
            conversation.Prompt(promptText, addBos: true, special: true); 

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Prompt queued with multimodal chunks. Generating response...\n");
            Console.ResetColor();

            var remaining = TokenCount;

            // Run one decode/sampling/prompt cycle – mirrors the batched executor inner loop.
            async Task<bool> ProcessNextAsync()
            {
                var decodeResult = await executor.Infer();
                if (decodeResult == DecodeResult.NoKvSlot) // KV cache exhausted – surface to the user
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Insufficient KV cache space for multimodal evaluation.");
                    Console.ResetColor();
                    return false;
                }

                if (decodeResult != DecodeResult.Ok)
                    throw new RuntimeError($"Failed to evaluate batch: {decodeResult}.");

                if (!conversation.RequiresSampling) // another conversation may still be queued
                    return true;

                var token = conversation.Sample(sampler); // pull logits (or -1 for mtmd chunk) and sample
                if (token.IsEndOfGeneration(vocab))
                    return false;

                decoder.Add(token);
                var delta = decoder.Read();
                if (!string.IsNullOrEmpty(delta))
                    Console.Write(delta);

                sampler.Accept(token); // keep sampler state in sync
                conversation.Prompt(token); // feed the accepted token back into the batch
                remaining--;
                return remaining > 0;
            }

            while (remaining > 0 && await ProcessNextAsync()) // continue until EOS or budget is reached
            {
            }

            Console.WriteLine();
        }
        catch (IOException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Could not load media '{imagePath}': {ex.Message}");
            Console.ResetColor();
        }
        catch (RuntimeError ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"MTMD processing failed: {ex.Message}");
            Console.ResetColor();
        }
    }
}
