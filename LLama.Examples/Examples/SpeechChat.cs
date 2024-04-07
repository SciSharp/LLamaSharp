﻿using LLama.Common;

using static LLama.Examples.Examples.SpeechTranscription;

namespace LLama.Examples.Examples
{
    public class SpeechChat
    {
        public static async Task Run()
        {
            if (ConsoleStyleHelpers.SelectAudioModel() is not string model) { return; }

            bool loadFinished = false;
            var loading = ConsoleStyleHelpers.LoadPrint("Loading transcription model...", () => loadFinished);

            using var audioServer = new AudioServer(model);
            loadFinished = true; loading.Wait();

            Console.WriteLine("Audio model loaded. Insert path for language model.");
            using var llamaServer = new LlamaServer(audioServer);

            await ConsoleStyleHelpers.WaitUntilExit();
        }


        class LlamaServer : IAudioServiceUser, IDisposable
        {
            bool isModelResponding;
            AudioServer audioServer;

            LLamaWeights model;
            LLamaContext context;
            InteractiveExecutor executor;

            string fullPrompt = "";
            bool canceled;

            public LlamaServer(AudioServer server)
            {
                var parameters = new ModelParams(UserSettings.GetModelPath()) { ContextSize = 1024, Seed = 1337, GpuLayerCount = 99 };
                model = LLamaWeights.LoadFromFile(parameters);
                context = model.CreateContext(parameters);
                executor = new InteractiveExecutor(context);
                (audioServer = server).ServiceUsers.Add(this);
            }

            bool IAudioServiceUser.IsOfInterest(string AudioTranscription) => !isModelResponding || AudioTranscription.Contains("stop", StringComparison.CurrentCultureIgnoreCase);
            void IAudioServiceUser.ProcessText(string AudioTranscription)
            {
                if (isModelResponding && AudioTranscription.Contains("stop", StringComparison.CurrentCultureIgnoreCase)) { canceled = true; }
                else if (!isModelResponding) { _ = SendMessage(AudioTranscription); }
            }

            async Task SendMessage(string newMessage)
            {
                // While a response is queried, we want to detect short phrases/commands like 'stop',
                audioServer.detectionSettings = (1, 1); // ..so we lower the min Speech Detection time.

                isModelResponding = true;
                AddToPrompt($"\n{newMessage}\n", ConsoleColor.Blue);
                await foreach (var token in executor.InferAsync(fullPrompt))
                {
                    AddToPrompt(token, ConsoleColor.Yellow);
                    if (canceled) { AddToPrompt("[...stopped]", ConsoleColor.Red); break; }
                }
                audioServer.detectionSettings = (2, 3);         // Reset back to default detection settings to avoid false positives.
                (isModelResponding, canceled) = (false, false); // Reset the state variables to their default.
            }

            void AddToPrompt(string msg, ConsoleColor color = ConsoleColor.Yellow)
            {
                fullPrompt += msg;

                Console.ForegroundColor = color;
                Console.Write(msg);
                Console.ForegroundColor = ConsoleColor.White;
            }

            void IDisposable.Dispose()
            {
                model.Dispose();
                context.Dispose();
            }

        }
    }
}