using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.Native;
using LLama.Common;
using LLama;
using LLama.Abstractions;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace NetStandardTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"AppContext.BaseDirectory: {AppContext.BaseDirectory}");
            var showLLamaCppLogs = true;
            NativeLibraryConfig
              .All
              .WithLogCallback((level, message) =>
              {
                  if (showLLamaCppLogs)
                      Console.WriteLine($"[llama {level}]: {message.TrimEnd('\n')}");
              });

            // Configure native library to use. This must be done before any other llama.cpp methods are called!
            NativeLibraryConfig
              .All
              .WithCuda();
              //.WithAutoDownload() // An experimental feature
              //.DryRun(out var loadedllamaLibrary, out var loadedLLavaLibrary);

            // Calling this method forces loading to occur now.
            NativeApi.llama_empty_call();

            //string modelPath = @"D:\development\llama\weights\Wizard-Vicuna-7B-Uncensored.Q4_K_M.gguf";

            //var prompt = File.ReadAllText(@"D:\development\llama\native\LLamaSharp\LLama.Examples\Assets\chat-with-bob.txt").Trim();

            //var parameters = new ModelParams(modelPath)
            //{
            //    Seed = 1337,
            //    GpuLayerCount = 5
            //};
            //var model = LLamaWeights.LoadFromFile(parameters);
            //var context = model.CreateContext(parameters);
            //var ex = new InteractiveExecutor(context);

            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine("The executor has been enabled. In this example, the prompt is printed, the maximum tokens is set to 128 and the context size is 256. (an example for small scale usage)");
            //Console.ForegroundColor = ConsoleColor.White;

            //Console.Write(prompt);

            //var inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" }, MaxTokens = 128 };

            Run().Wait();
        }

        private static async Task Run()
        {
            string multiModalProj = @"/home/rinne/models/llava-v1.5-7b-mmproj-Q4_0.gguf";
            string modelPath = @"/home/rinne/models/llava-v1.5-7b-Q4_K.gguf";
            string modelImage = @"/home/rinne/code/forks/LLamaSharp/Assets/LLamaSharp-Integrations.png";
            const int maxTokens = 1024;

            var prompt = $"{{{modelImage}}}\nUSER:\nProvide a full description of the image.\nASSISTANT:\n";

            var parameters = new ModelParams(modelPath);

            using var model = await LLamaWeights.LoadFromFileAsync(parameters);
            using var context = model.CreateContext(parameters);

            // Llava Init
            using var clipModel = await LLavaWeights.LoadFromFileAsync(multiModalProj);

            var ex = new InteractiveExecutor(context, clipModel);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example, the prompt is printed, the maximum tokens is set to {0} and the context size is {1}.", maxTokens, parameters.ContextSize);
            Console.WriteLine("To send an image, enter its filename in curly braces, like this {c:/image.jpg}.");

            var inferenceParams = new InferenceParams() { Temperature = 0.1f, AntiPrompts = new List<string> { "\nUSER:" }, MaxTokens = maxTokens };

            do
            {
                // Evaluate if we have images
                //
                //var imageMatches = Regex.Matches(prompt, "{([^}]*)}").Select(m => m.Value);
                var imageCount = 1;
                var hasImages = imageCount > 0;

                if (hasImages)
                {
                    List<string> imagePathsWithCurlyBraces = new();
                    foreach(var match in Regex.Matches(prompt, "{([^}]*)}"))
                    {
                        imagePathsWithCurlyBraces.Add(((Match)match).Value);
                    }
                    List<string> imagePaths = new();
                    foreach (var match in Regex.Matches(prompt, "{([^}]*)}"))
                    {
                        imagePaths.Add(((Match)match).Groups[1].Value);
                    }

                    List<byte[]> imageBytes;
                    try
                    {
                        imageBytes = imagePaths.Select(File.ReadAllBytes).ToList();
                    }
                    catch (IOException exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(
                            $"Could not load your {(imageCount == 1 ? "image" : "images")}:");
                        Console.Write($"{exception.Message}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Please try again.");
                        break;
                    }

                    // Each prompt with images we clear cache
                    // When the prompt contains images we clear KV_CACHE to restart conversation
                    // See:
                    // https://github.com/ggerganov/llama.cpp/discussions/3620
                    ex.Context.NativeHandle.KvCacheRemove(LLamaSeqId.Zero, -1, -1);

                    int index = 0;
                    foreach (var path in imagePathsWithCurlyBraces)
                    {
                        // First image replace to tag <image, the rest of the images delete the tag
                        prompt = prompt.Replace(path, index++ == 0 ? "<image>" : "");
                    }


                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Here are the images, that are sent to the chat model in addition to your message.");
                    Console.WriteLine();

                    //foreach (var consoleImage in imageBytes?.Select(bytes => new CanvasImage(bytes)))
                    //{
                    //    consoleImage.MaxWidth = 50;
                    //    AnsiConsole.Write(consoleImage);
                    //}

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The images were scaled down for the console only, the model gets full versions.");
                    Console.WriteLine($"Write /exit or press Ctrl+c to return to main menu.");
                    Console.WriteLine();


                    // Initialize Images in executor
                    //
                    foreach (var image in imagePaths)
                    {
                        ex.Images.Add(File.ReadAllBytes(image));
                    }
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
            while (true);
        }
    }
}
