using System.Text;
using LLama.Batched;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using Spectre.Console;

namespace LLama.Examples.Examples;

/// <summary>
/// Demonstrates using LLava (image embeddings) with the batched executor.
/// </summary>
public class BatchedExecutorLLava
{
    /// <summary>
    /// How many tokens of response to generate
    /// </summary>
    public const int TokenCount = 64;

    public static async Task Run()
    {
        // Load model weights
        var parameters = new ModelParams(UserSettings.GetModelPath());
        using var model = await LLamaWeights.LoadFromFileAsync(parameters);
        using var llava = await LLavaWeights.LoadFromFileAsync(UserSettings.GetMMProjPath());
        
        // Decide on the prompt
        var prompt = model.Tokenize(AnsiConsole.Ask("Prompt (or ENTER for default):", "\nUSER: Provide a full description of the image.\nASSISTANT: "), true, false, Encoding.UTF8);
        
        // Get image and show it
        var image = UserSettings.GetImagePath();
        AnsiConsole.Write(new CanvasImage(image));
        
        // Create an executor with one conversation
        using var executor = new BatchedExecutor(model, parameters);
        using var conversation = executor.Create();

        // Embed the image
        SafeLlavaImageEmbedHandle embedding = null!;
        await AnsiConsole
             .Status()
             .StartAsync("[yellow]Embedding image with CLIP[/]", async _ =>
              {
                  // ReSharper disable once AccessToDisposedClosure
                  embedding = llava.CreateImageEmbeddings(await File.ReadAllBytesAsync(image));
              });
        
        // Pass in the image and run inference until the entire image has been processed
        await AnsiConsole
             .Status()
             .StartAsync("[yellow]Processing image embedding with language model[/]", async _ =>
              {
                  conversation.Prompt(embedding);
                  while (executor.BatchedTokenCount > 0)
                      await executor.Infer();
              });

        // Prompt with the text prompt
        conversation.Prompt(prompt);
        
        // Run inference loop
        var decoder = new StreamingTokenDecoder(executor.Context);
        var sampler = new DefaultSamplingPipeline();
        await AnsiConsole
             .Progress()
             .StartAsync(async ctx =>
              {
                  var task = ctx.AddTask("Generating Response");
                  task.MaxValue = TokenCount;

                  // Run a normal inference loop
                  for (var i = 0; i < TokenCount; i++)
                  {
                      task.Increment(1);

                      await executor.Infer();
                      
                      var token = sampler.Sample(executor.Context.NativeHandle, conversation.GetSampleIndex());
                      if (executor.Context.NativeHandle.ModelHandle.Tokens.IsEndOfGeneration(token))
                          break;
                      
                      decoder.Add(token);
                      conversation.Prompt(token);
                  }
              });

        // Print final result
        var str = decoder.Read();
        AnsiConsole.MarkupInterpolated($"[green]{str}[/]");
    }
}