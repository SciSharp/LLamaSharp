using LLama.Common;
using NAudio.Wave;
using Whisper.net;

namespace LLama.Examples.Examples
{
    public class SpeechChat
    {
        public static async Task Run()
        {
            ConsoleStyleHelpers.WriteLine(
"""
This example demonstrates the basics of audio transcriptions, speech recognition, and speech commands,
    as well as how to recognize a user's voice in real time and then get a response from LLM.
It uses whisper.net and models could be found in: https://huggingface.co/ggerganov/whisper.cpp/tree/main.
To use it, you need a working microphone and enough RAM to host both audio + language models.
Once you've selected the models, just speak to your microphone and watch the LLM continue your text.
While it's going, you can say something like 'Okay, stop', or 'Stop now', to interrupt the LLM's inference.

NOTE: You may need to poke around with the voice detection threshold, based on your mic's sensitivity.
-----------------------------------------------------------------------------------------------------------
""", ConsoleColor.Yellow);

            if (ConsoleStyleHelpers.SelectAudioModel() is not string model) { return; }

            bool loadFinished = false;
            var loading = ConsoleStyleHelpers.LoadPrint("Loading transcription model...", () => loadFinished);

            using var speechRecognitionServer = new SpeechRecognitionServer(model);
            loadFinished = true; loading.Wait();

            Console.WriteLine("Audio model loaded. Insert path for language model.");
            using var _ = new LlamaSession_SpeechListener(speechRecognitionServer);

            await ConsoleStyleHelpers.WaitUntilExit();
        }


        class LlamaSession_SpeechListener : ISpeechListener, IDisposable
        {
            bool isModelResponding;
            SpeechRecognitionServer audioServer;

            LLamaWeights model;
            LLamaContext context;
            InteractiveExecutor executor;

            string fullPrompt = "";
            bool canceled;

            public LlamaSession_SpeechListener(SpeechRecognitionServer server)
            {
                var parameters = new ModelParams(UserSettings.GetModelPath())
                {
                    GpuLayerCount = 99
                };
                model = LLamaWeights.LoadFromFile(parameters);
                context = model.CreateContext(parameters);
                executor = new InteractiveExecutor(context);
                (audioServer = server).ServiceUsers.Add(this);
            }

            // Whisper is struggling with single words and very short phrases without context, so it's actually better to say something like "Ok, Stop!" to have it work better.
            bool ISpeechListener.IsInterested(string audioTranscription) => !isModelResponding || audioTranscription.Contains("stop", StringComparison.CurrentCultureIgnoreCase);
            void ISpeechListener.HandleSpeech(string audioTranscription)
            {
                if (isModelResponding && audioTranscription.Contains("stop", StringComparison.CurrentCultureIgnoreCase)) { canceled = true; }
                else if (!isModelResponding) { _ = SendMessage(audioTranscription); }
            }

            async Task SendMessage(string newMessage)
            {
                // While a response is queried, we want to detect short phrases/commands like 'stop',
                audioServer.detectionSettings = (1, 2); // ..so we lower the min Speech Detection time.

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
                ConsoleStyleHelpers.Write(msg, color);
            }

            void IDisposable.Dispose()
            {
                model.Dispose();
                context.Dispose();
            }
        }

        public interface ISpeechListener
        {
            bool IsInterested(string audioTranscription);
            void HandleSpeech(string audioTranscription);
        }

        public class SpeechRecognitionServer : IDisposable
        {
            const int clipLength = 250; // ms
            const float voiceDetectionThreshold = 0.01f; // Adjust as needed
            readonly string[] knownFalsePositives = ["[BLANK_AUDIO]", "Thank you", "[silence]"];

            WaveInEvent waveIn;
            WaveFormat waveFormat = new(16000, 16, 1); // 16KHz, 16 bits, Mono Channel
            List<byte> recordedBytes = [];

            WhisperFactory? whisperFactory;
            WhisperProcessor? processor;
            string whisperPrompt =
"""
The short audio comes from a user that is speaking to an AI Language Model in real time.
Pay extra attentions for commands like 'ok stop' or just 'stop'.
In case of inaudible sentences that might be, assume they're saying 'stop'.
""".Trim();

            // Tracked stats for Speech Recognition, Parsing, and Serving.
            int currentBlankClips;  // Ideally would work with milliseconds,
            int totalNonBlankClips; // ..but for example's sake they work on a
            int nonIdleTime;        // ..clip-based quant-length (1 = clipLength).
            // Default detection settings: A speech of 750ms, followed by pause of 500ms. (2x250ms)
            public (int minBlanksPerSeparation, int minNonBlanksForValidMessages) detectionSettings = (2, 3);

            public HashSet<ISpeechListener> ServiceUsers = [];

            public SpeechRecognitionServer(string modelPath)
            {
                // Adjust the path based on your GPU's type. On your build you ideally want just the correct runtime build for your project, but here we're having all references, so it's getting confused.
                var libPath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\.nuget\packages\whisper.net.runtime.cublas\1.5.0\build\win-x64\whisper.dll"; // Defaulting to cuBlas.
                if (!File.Exists(libPath)) { ConsoleStyleHelpers.WriteLine($"Could not find dll file at {libPath}.\nWhisper will load with the default runtime (possibly CPU).\nIf you own a non-Nvidia GPU, you need to adjust the library path based on your GPU's type.", ConsoleColor.Red); libPath = null; }
                whisperFactory = WhisperFactory.FromPath(modelPath, libraryPath: libPath);

                var builder = whisperFactory.CreateBuilder().WithThreads(16).WithPrompt(whisperPrompt).WithSingleSegment().WithLanguage("en");
                (builder.WithBeamSearchSamplingStrategy() as BeamSearchSamplingStrategyBuilder)!.WithPatience(0.2f).WithBeamSize(5);
                processor = builder.Build();

                waveIn = new WaveInEvent() { BufferMilliseconds = clipLength, WaveFormat = waveFormat };
                waveIn.DataAvailable += OnAudioDataAvailable;
                waveIn.StartRecording();
            }

            void OnAudioDataAvailable(object? sender, WaveInEventArgs e)
            {
                // Cache the recorded bytes
                recordedBytes.AddRange(e.Buffer[..e.BytesRecorded]);
                if (recordedBytes.Count > 110000000) { recordedBytes.RemoveRange(0, 50000000); }

                // Get the max volume contained inside the clip. Since the clip is recorded as bytes, we need to translate them to samples before getting their volume.
                var maxVolume = 0f; // This byte->sample algorithm is from: https://github.com/naudio/NAudio/blob/master/Docs/RecordingLevelMeter.md#calculating-peak-values
                for (int i = 0; i < e.BytesRecorded; i += 2) { maxVolume = Math.Max(maxVolume, Math.Abs((short) ((e.Buffer[i + 1] << 8) | e.Buffer[i + 0]) / 32768f)); }

                // Compare the volume with the threshold and act accordingly. Once an interesting and 'full' set of clips pops up, serve it.
                if (maxVolume >= voiceDetectionThreshold) { currentBlankClips = 0; totalNonBlankClips++; nonIdleTime++; }
                else if (++currentBlankClips < detectionSettings.minBlanksPerSeparation) { nonIdleTime++; }
                else
                {
                    if (totalNonBlankClips >= detectionSettings.minNonBlanksForValidMessages) { SendTranscription(); }
                    else if (totalNonBlankClips > 0) { } // This might be case of a false-positive -- knock, noise, cough, anything.
                    (currentBlankClips, totalNonBlankClips, nonIdleTime) = (0, 0, 0);
                }


                async void SendTranscription()
                {
                    var bytesPerClip = waveFormat.BitsPerSample * clipLength * 2;
                    var capturedClipBytes = recordedBytes.TakeLast(bytesPerClip * (nonIdleTime + 2)).ToArray();
                    var transcribedText = await ProcessAudio(capturedClipBytes, "Assets\\temp.wav"); // Save to temporary file.
                    if (knownFalsePositives.Contains(transcribedText)) { return; }                   // False positive.. yikes!
                    foreach (var user in ServiceUsers.Where(x => x.IsInterested(transcribedText))) { user.HandleSpeech(transcribedText); }
                }
            }

            /// <summary> Requests a transcription and responds with the text. </summary>
            async Task<string> ProcessAudio(byte[] bytes, string tempWavFilePath)
            {
                await using var wavStream = new MemoryStream();
                using (var writer = new WaveFileWriter(tempWavFilePath, waveFormat)) { writer.Write(bytes, 0, bytes.Length); }
                using (var fileStream = File.OpenRead(tempWavFilePath)) { await fileStream.CopyToAsync(wavStream); }
                wavStream.Seek(0, SeekOrigin.Begin);

                Console.Beep();
                return string.Join(' ', await processor!.ProcessAsync(wavStream).Select(x => x.Text).ToListAsync()).Trim();
            }

            void IDisposable.Dispose()
            {
                waveIn.Dispose();
                processor?.Dispose();
            }
        }

        public static class ConsoleStyleHelpers
        {
            public static string? SelectAudioModel()
            {
                var models = Directory.GetFiles("Assets", "*bin");
                if (models.Length == 1) { return models[0]; }
                else if (models.Length != 0)
                {
                    WriteLine("Available Models:", ConsoleColor.Green);
                    for (int i = 0; i < models.Length; i++)
                    {
                        Write($"{i + 1}. ", ConsoleColor.Blue);
                        WriteLine(models[i]["Assets\\".Length..], ConsoleColor.Yellow);
                    }
                    while (true)
                    {
                        Write($"Please choose a model (1-{models.Length}): ", ConsoleColor.DarkCyan);
                        if (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out var i) || i > models.Length || i <= 0) { Console.WriteLine(); continue; }
                        Console.WriteLine();
                        return models[i - 1];
                    }
                }
                else
                {
                    WriteLine($"Download a non-quantized model and place it in the executing directory:", ConsoleColor.Red);
                    WriteLine($"\t{Environment.CurrentDirectory}\\Assets", ConsoleColor.Yellow);
                    WriteLine("You can find the official ggml models in whisper.cpp's huggingface repository: ", ConsoleColor.Red);
                    WriteLine("\thttps://huggingface.co/ggerganov/whisper.cpp/tree/main", ConsoleColor.Blue);
                    return null;
                }
            }
            public static async Task LoadPrint(string initialText, Func<bool> ShouldContinue)
            {
                var startTime = DateTime.Now;
                Console.WriteLine(initialText);
                while (!ShouldContinue()) { Console.Write("."); await Task.Delay(100); }
                Console.WriteLine($" Completed in {(DateTime.Now - startTime).TotalSeconds:f2}s.");
            }

            public async static Task WaitUntilExit()
            {
                WriteLine("Voice active. Begin talking to transcribe. Press any key at any time to exit.", ConsoleColor.Green);
                await Task.Delay(1000);
                Console.ReadKey();
            }

            public static void Write(string text, ConsoleColor consoleColor) => ColorAction(consoleColor, () => Console.Write(text));
            public static void WriteLine(string text, ConsoleColor consoleColor) => ColorAction(consoleColor, () => Console.WriteLine(text));
            public static void ColorAction(ConsoleColor consoleColor, Action action)
            {
                Console.ForegroundColor = consoleColor;
                action?.Invoke();
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
