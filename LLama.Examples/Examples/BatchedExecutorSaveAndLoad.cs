using LLama.Batched;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using Spectre.Console;

namespace LLama.Examples.Examples;

/// <summary>
/// This demonstrates generating multiple replies to the same prompt, with a shared cache
/// </summary>
public class BatchedExecutorSaveAndLoad
{
    private const int n_len = 18;

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

        // Create a conversation
        var conversation = executor.Create();
        conversation.Prompt(prompt);

        // Run inference loop
        var decoder = new StreamingTokenDecoder(executor.Context);
        var sampler = new DefaultSamplingPipeline();
        var lastToken = (LLamaToken)0;
        for (var i = 0; i < n_len; i++)
        {
            await executor.Infer();

            var token = sampler.Sample(executor.Context.NativeHandle, conversation.Sample(), ReadOnlySpan<LLamaToken>.Empty);
            lastToken = token;
            decoder.Add(token);
            conversation.Prompt(token);
        }

        // Can't save a conversation while RequiresInference is true
        if (conversation.RequiresInference)
            await executor.Infer();

        // Save this conversation and dispose it
        conversation.Save("demo_conversation.state");
        conversation.Dispose();
        AnsiConsole.WriteLine($"Saved state: {new FileInfo("demo_conversation.state").Length} bytes");

        // Now create a new conversation by loading that state
        conversation = executor.Load("demo_conversation.state");
        AnsiConsole.WriteLine("Loaded state");

        // Prompt it again with the last token, so we can continue generating
        conversation.Rewind(1);
        conversation.Prompt(lastToken);

        // Continue generating text
        for (var i = 0; i < n_len; i++)
        {
            await executor.Infer();

            var token = sampler.Sample(executor.Context.NativeHandle, conversation.Sample(), ReadOnlySpan<LLamaToken>.Empty);
            decoder.Add(token);
            conversation.Prompt(token);
        }

        // Display final ouput
        AnsiConsole.MarkupLine($"[red]{prompt}{decoder.Read()}[/]");
    }
}