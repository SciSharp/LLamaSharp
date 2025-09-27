using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using LLama.Common;
using Spectre.Console;
using LLama.Native;
using LLama.Sampling;

namespace LLama.Examples.Examples
{
    // This example shows how to chat with Mtmd model with both image and text as input.
    // It uses the interactive executor to inference.
    public class MtmdInteractiveModeExecute
    {
        public static async Task Run()
        {
            string multiModalProj = UserSettings.GetMMProjPath();
            string modelPath = UserSettings.GetModelPath();
            string modelImage = UserSettings.GetImagePath();
            const int maxTokens = 2048;

            var prompt = $"{{{modelImage}}}\nUSER:\nProvide a full description of the image.\nASSISTANT:\n";

            var parameters = new ModelParams(modelPath);

            var mtmdParameters = MtmdContextParams.Default();
            mtmdParameters.UseGpu = false;

            using var model = await LLamaWeights.LoadFromFileAsync(parameters);
            using var context = model.CreateContext(parameters);

            // Mtmd Init
            using var clipModel = await SafeMtmdWeights.LoadFromFileAsync(multiModalProj, model, mtmdParameters );

            var mediaMarker = mtmdParameters.MediaMarker ?? NativeApi.MtmdDefaultMarker() ?? "<media>";

            var ex = new InteractiveExecutor(context, clipModel);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the prompt is printed, the maximum tokens is set to {0} and the context size is {1}.", maxTokens, parameters.ContextSize );
            Console.WriteLine("To send an image, enter its filename in curly braces, like this {c:/image.jpg}.");

            var inferenceParams = new InferenceParams
            {
                SamplingPipeline = new DefaultSamplingPipeline
                {
                    Temperature = 0.1f
                },

                AntiPrompts = new List<string> { "\nASSISTANT:" },
                MaxTokens = maxTokens

            };

            do
            {
                
                // Evaluate if we have media
                //
                var mediaMatches = Regex.Matches(prompt, "{([^}]*)}").Select(m => m.Value);
                var mediaCount = mediaMatches.Count();
                var hasMedia = mediaCount > 0;

                if (hasMedia)
                {
                    var mediaPathsWithCurlyBraces = Regex.Matches(prompt, "{([^}]*)}").Select(m => m.Value);
                    var mediaPaths = Regex.Matches(prompt, "{([^}]*)}").Select(m => m.Groups[1].Value).ToList();

                    var embeds = new List<SafeMtmdEmbed>();
                    var imageList = new List<byte[]>();
                    var imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ".png",
                        ".jpg",
                        ".jpeg",
                        ".bmp",
                        ".gif",
                        ".webp"
                    };
                    
                    try
                    {
                        foreach (var mediaPath in mediaPaths)
                        {
                            var extension = Path.GetExtension(mediaPath);
                            if (!string.IsNullOrEmpty(extension) && imageExtensions.Contains(extension))
                            {
                                // Keep the raw image data so the caller can reuse or inspect the images later.
                                imageList.Add(File.ReadAllBytes(mediaPath));
                            }

                            var embed = clipModel.LoadMedia(mediaPath);
                            embeds.Add(embed);
                        }
                    }
                    catch (IOException exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(
                            $"Could not load your {(mediaCount == 1 ? "media" : "medias")}:");
                        Console.Write($"{exception.Message}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Please try again.");
                        clipModel.ClearMedia();
                        break;
                    }

                    // Each prompt with images we clear cache
                    // When the prompt contains images we clear KV_CACHE to restart conversation
                    // See:
                    // https://github.com/ggerganov/llama.cpp/discussions/3620
                    ex.Context.NativeHandle.MemorySequenceRemove( LLamaSeqId.Zero, -1, -1 );

                    // Replace placeholders with media markers (one marker per image)
                    foreach (var path in mediaPathsWithCurlyBraces)
                    {
                        prompt = prompt.Replace(path, mediaMarker, StringComparison.Ordinal);
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Here are the images, that are sent to the chat model in addition to your message.");
                    Console.WriteLine();

                    foreach (var consoleImage in imageList.Select(image => new CanvasImage(image.ToArray())))
                    {
                        consoleImage.MaxWidth = 50;
                        AnsiConsole.Write(consoleImage);
                    }

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The images were scaled down for the console only, the model gets full versions.");
                    Console.WriteLine($"Write /exit or press Ctrl+c to return to main menu.");
                    Console.WriteLine();


                    // Initialize Images in executor
                    //
                    ex.Embeds.Clear();
                    foreach (var embed in embeds)
                        ex.Embeds.Add(embed);
                }

                Console.ForegroundColor = Color.White;
                await foreach (var text in ex.InferAsync(prompt, inferenceParams))
                {
                    Console.Write(text);
                }
                Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.Green;
                prompt = Console.ReadLine();
                Console.WriteLine();
                
                // let the user finish with exit
                //
                if (prompt != null && prompt.Equals("/exit", StringComparison.OrdinalIgnoreCase))
                    break;

            }
            while(true);
        }
    }
}
