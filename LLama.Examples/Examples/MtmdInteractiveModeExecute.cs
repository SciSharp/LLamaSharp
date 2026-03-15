using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using LLama;
using LLama.Common;
using LLama.Exceptions;
using Spectre.Console;
using LLama.Native;
using LLama.Sampling;

namespace LLama.Examples.Examples
{
    // This example shows how to chat with Mtmd model with audio, image and text as input.
    // It uses the interactive executor to inference.
    public class MtmdInteractiveModeExecute
    {
        private sealed record TemplateMarkers(string? AssistantEndMarker, string? AssistantToUserMarker);

        public static async Task Run()
        {
            string multiModalProj = UserSettings.GetMMProjPath();
            string modelPath = UserSettings.GetModelPath();
            const int maxTokens = 4096;

            var parameters = new ModelParams(modelPath);

            var mtmdParameters = MtmdContextParams.Default();
            mtmdParameters.UseGpu = false;

            using var model = await LLamaWeights.LoadFromFileAsync(parameters);
            using var context = model.CreateContext(parameters);

            // Mtmd Init
            using var clipModel = await MtmdWeights.LoadFromFileAsync(multiModalProj, model, mtmdParameters );

            var mediaMarker = mtmdParameters.MediaMarker ?? NativeApi.MtmdDefaultMarker() ?? "<media>";
            var supportsVision = clipModel.SupportsVision;
            var supportsAudio = clipModel.SupportsAudio;

            var ex = new InteractiveExecutor(context, clipModel);
            var chatHistory = new ChatHistory();
            var templateMarkers = ResolveTemplateMarkers(model);
            var antiPrompts = GetEffectiveAntiPrompts(templateMarkers);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The executor has been enabled. In this example the maximum tokens is set to {0} and the context size is {1}.", maxTokens, parameters.ContextSize );
            Console.WriteLine("Model: {0}", modelPath);
            Console.WriteLine("MMProj: {0}", multiModalProj);
            Console.WriteLine("Supported modalities: vision={0} | audio={1} | video=no", supportsVision ? "yes" : "no", supportsAudio ? "yes" : "no");
            if (supportsVision)
                Console.WriteLine("To send an image, enter its filename in double braces, like this {{c:/image.jpg}}.");
            if (supportsAudio)
                Console.WriteLine("To send audio, enter its filename in double braces, like this {{c:/audio.wav}}.");
            Console.WriteLine("Video inputs are not supported (format would be {{c:/video.mp4}}).");
            Console.WriteLine("Commands: /exit (return to main menu) | /clear (reset the chat history and KV cache).");
            Console.WriteLine("Press Ctrl+c to return to main menu.");
            Console.Write("User: ");

            void ResetConversation()
            {
                context.NativeHandle.MemoryClear();
                foreach (var embed in ex.Embeds)
                    embed.Dispose();
                ex.Embeds.Clear();
                clipModel.ClearMedia();
                chatHistory.Messages.Clear();
                ex = new InteractiveExecutor(context, clipModel);
                Console.Write("User: ");
            }

            var inferenceParams = new InferenceParams
            {
                SamplingPipeline = new DefaultSamplingPipeline
                {
                    Temperature = 0.1f
                },

                AntiPrompts = antiPrompts,
                MaxTokens = maxTokens,
                DecodeSpecialTokens = ShouldDecodeSpecialTokens(antiPrompts)

            };

            do
            {
                Console.ForegroundColor = ConsoleColor.Green;
                var prompt = Console.ReadLine();
                Console.WriteLine();

                if (prompt == null || prompt.Equals("/exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (prompt.Equals("/clear", StringComparison.OrdinalIgnoreCase))
                {
                    ResetConversation();
                    continue;
                }

                if (string.IsNullOrWhiteSpace(prompt))
                {
                    Console.Write("User: ");
                    continue;
                }

                var userPrompt = prompt;

                // Evaluate if we have media
                //
                var mediaMatches = Regex.Matches(userPrompt, @"\{\{(.*?)\}\}").Select(m => m.Value);
                var mediaCount = mediaMatches.Count();
                var hasMedia = mediaCount > 0;

                if (hasMedia)
                {
                    var mediaPathsWithCurlyBraces = Regex.Matches(userPrompt, @"\{\{(.*?)\}\}").Select(m => m.Value);
                    var mediaPaths = Regex.Matches(userPrompt, @"\{\{(.*?)\}\}").Select(m => m.Groups[1].Value).ToList();

                    var embeds = new List<SafeMtmdEmbed>();
                    var imageList = new List<byte[]>();
                    var audioList = new List<string>();
                    var imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ".png",
                        ".jpg",
                        ".jpeg",
                        ".bmp",
                        ".gif",
                        ".webp"
                    };
                    var audioExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ".wav",
                        ".mp3",
                        ".flac",
                        ".ogg",
                        ".m4a",
                        ".aac",
                        ".opus"
                    };
                    var videoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ".mp4",
                        ".mkv",
                        ".mov",
                        ".avi",
                        ".webm"
                    };
                    
                    try
                    {
                        foreach (var mediaPath in mediaPaths)
                        {
                            var extension = Path.GetExtension(mediaPath);
                            var isImage = !string.IsNullOrEmpty(extension) && imageExtensions.Contains(extension);
                            var isAudio = !string.IsNullOrEmpty(extension) && audioExtensions.Contains(extension);
                            var isVideo = !string.IsNullOrEmpty(extension) && videoExtensions.Contains(extension);

                            if (isVideo)
                                throw new NotSupportedException("Video inputs are not supported by MTMD.");
                            if (isImage && !supportsVision)
                                throw new InvalidOperationException("This model does not support vision inputs.");
                            if (isAudio && !supportsAudio)
                                throw new InvalidOperationException("This model does not support audio inputs.");

                            if (isImage)
                            {
                                // Keep the raw image data so the caller can reuse or inspect the images later.
                                imageList.Add(File.ReadAllBytes(mediaPath));
                            }
                            else if (isAudio)
                            {
                                audioList.Add(mediaPath);
                            }

                            var embed = clipModel.LoadMediaStandalone(mediaPath);
                            embeds.Add(embed);
                        }
                    }
                    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or RuntimeError or NotSupportedException or InvalidOperationException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(
                            $"Could not load your {(mediaCount == 1 ? "media" : "medias")}:");
                        Console.Write($"{exception.Message}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Please try again.");
                        foreach (var embed in embeds)
                            embed.Dispose();
                        break;
                    }


                    // Replace placeholders with media markers (one marker per image)
                    foreach (var path in mediaPathsWithCurlyBraces)
                    {
                        userPrompt = userPrompt.Replace(path, mediaMarker, StringComparison.Ordinal);
                    }

                    if (imageList.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Here are the images, that are sent to the chat model in addition to your message.");
                        Console.WriteLine();

                        foreach (var consoleImage in imageList.Select(image => new CanvasImage(image.ToArray())))
                        {
                            consoleImage.MaxWidth = 50;
                            AnsiConsole.Write(consoleImage);
                        }

                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("The images were scaled down for the console only, the model gets full versions.");
                        Console.WriteLine();
                    }

                    if (audioList.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Loaded audio files: {string.Join(", ", audioList)}");
                        Console.WriteLine();
                    }


                    // Initialize Images in executor
                    //
                    ex.Embeds.Clear();
                    foreach (var embed in embeds)
                        ex.Embeds.Add(embed);
                }

                var formattedPrompt = BuildChatDelta(model, chatHistory, userPrompt);

                Console.ForegroundColor = ConsoleColor.White;
                var responseBuilder = new StringBuilder();
                await foreach (var text in ex.InferAsync(formattedPrompt, inferenceParams))
                {
                    Console.Write(text);
                    responseBuilder.Append(text);
                }
                Console.WriteLine();
                chatHistory.AddMessage(AuthorRole.Assistant, responseBuilder.ToString());
                Console.Write("User: ");

            }
            while(true);
        }

        private static string BuildChatDelta(LLamaWeights model, ChatHistory history, string userContent)
        {
            var pastPrompt = FormatChatHistory(model, history, addAssistant: false);
            history.AddMessage(AuthorRole.User, userContent);
            var fullPrompt = FormatChatHistory(model, history, addAssistant: true);

            if (!fullPrompt.StartsWith(pastPrompt, StringComparison.Ordinal))
                return fullPrompt;

            var delta = fullPrompt[pastPrompt.Length..];
            if (pastPrompt.Length > 0 && pastPrompt.EndsWith('\n') && (delta.Length == 0 || delta[0] != '\n'))
                delta = "\n" + delta;

            return delta;
        }

        private static List<string> GetEffectiveAntiPrompts(TemplateMarkers templateMarkers)
        {
            var antiPrompts = new List<string>();
            AddMarker(antiPrompts, templateMarkers.AssistantEndMarker);
            AddMarker(antiPrompts, templateMarkers.AssistantToUserMarker);

            if (antiPrompts.Count == 0)
                antiPrompts.Add("User:");

            return antiPrompts;
        }

        private static void AddMarker(List<string> values, string? marker)
        {
            if (string.IsNullOrWhiteSpace(marker))
                return;

            if (!values.Contains(marker))
                values.Add(marker);

            var trimmedMarker = marker.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedMarker) && !values.Contains(trimmedMarker))
                values.Add(trimmedMarker);
        }

        private static bool ShouldDecodeSpecialTokens(IReadOnlyList<string> antiPrompts)
        {
            foreach (var antiPrompt in antiPrompts)
            {
                if (string.IsNullOrWhiteSpace(antiPrompt))
                    continue;

                if (antiPrompt.Contains('<', StringComparison.Ordinal) || antiPrompt.Contains('>', StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        private static string FormatChatHistory(LLamaWeights model, ChatHistory history, bool addAssistant)
        {
            var template = new LLamaTemplate(model.NativeHandle)
            {
                AddAssistant = addAssistant,
            };

            foreach (var message in history.Messages)
                template.Add(message.AuthorRole.ToString().ToLowerInvariant(), message.Content);

            return LLamaTemplate.Encoding.GetString(template.Apply());
        }

        private static TemplateMarkers ResolveTemplateMarkers(LLamaWeights model)
        {
            const string userMarkerA = "__LLAMA_USER_A__";
            const string assistantMarkerA = "__LLAMA_ASSISTANT_A__";
            const string userMarkerB = "__LLAMA_USER_B__";

            try
            {
                var assistantTemplate = new LLamaTemplate(model.NativeHandle)
                {
                    AddAssistant = false
                };
                assistantTemplate.Add("user", userMarkerA);
                assistantTemplate.Add("assistant", assistantMarkerA);

                var assistantRendered = LLamaTemplate.Encoding.GetString(assistantTemplate.Apply());
                var assistantIndex = assistantRendered.IndexOf(assistantMarkerA, StringComparison.Ordinal);
                if (assistantIndex < 0)
                    return new TemplateMarkers(null, null);

                var assistantEndMarker = assistantRendered[(assistantIndex + assistantMarkerA.Length)..];

                var conversationTemplate = new LLamaTemplate(model.NativeHandle)
                {
                    AddAssistant = false
                };
                conversationTemplate.Add("user", userMarkerA);
                conversationTemplate.Add("assistant", assistantMarkerA);
                conversationTemplate.Add("user", userMarkerB);

                var conversationRendered = LLamaTemplate.Encoding.GetString(conversationTemplate.Apply());
                var assistantConversationIndex = conversationRendered.IndexOf(assistantMarkerA, StringComparison.Ordinal);
                var userIndex = conversationRendered.IndexOf(userMarkerB, StringComparison.Ordinal);
                if (assistantConversationIndex < 0 || userIndex <= assistantConversationIndex)
                    return new TemplateMarkers(NormalizeMarker(assistantEndMarker), null);

                var assistantToUserMarker = conversationRendered.Substring(
                    assistantConversationIndex + assistantMarkerA.Length,
                    userIndex - (assistantConversationIndex + assistantMarkerA.Length));

                return new TemplateMarkers(
                    NormalizeMarker(assistantEndMarker),
                    NormalizeMarker(assistantToUserMarker));
            }
            catch
            {
                return new TemplateMarkers(null, null);
            }
        }

        private static string? NormalizeMarker(string? marker)
        {
            return string.IsNullOrWhiteSpace(marker) ? null : marker;
        }
    }
}
