using NAudio.Wave;

using Whisper.net;

namespace LLama.Examples.Examples
{
    public class SpeechTranscription
    {
        public static async Task Run()
        {
            if (ConsoleStyleHelpers.SelectAudioModel() is not string model) { return; }

            bool loadFinished = false;
            var loading = ConsoleStyleHelpers.LoadPrint("Loading model...", () => loadFinished);

            using var speechRecognitionServer = new SpeechRecognitionServer(model);
            speechRecognitionServer.ServiceUsers.Add(new AudioEchoer());

            loadFinished = true; loading.Wait();
            await ConsoleStyleHelpers.WaitUntilExit();
        }

        class AudioEchoer : ISpeechRecognitionServiceUser
        {
            bool ISpeechRecognitionServiceUser.IsOfInterest(string audioTranscription)
            {
                if (audioTranscription.Contains("Artificial Intelligence", StringComparison.CurrentCultureIgnoreCase)) {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Skipped text because it's not of interest: {audioTranscription}");
                    Console.ForegroundColor = ConsoleColor.White;
                    return false;
                }
                else { return true; }
            }
            void ISpeechRecognitionServiceUser.ProcessText(string audioTranscription)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(audioTranscription);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public interface ISpeechRecognitionServiceUser
        {
            bool IsOfInterest(string AudioTranscription);
            void ProcessText(string AudioTranscription);
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
            public (int minBlanksPerSeperation, int minNonBlanksForValidMessages) detectionSettings = (2, 3);
            // Default detection settings: A speech of 750ms, followed by pause of 500ms. (2x250ms)

            public HashSet<ISpeechRecognitionServiceUser> ServiceUsers = [];

            public SpeechRecognitionServer(string modelPath)
            {
                // Adjust the path based on your GPU's type. On your build you ideally want just the correct runtime build for your project, but here we're having all references, so it's getting confused.
                var libPath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\.nuget\packages\whisper.net.runtime.cublas\1.5.0\build\win-x64\whisper.dll"; // Defaulting to cuBlas.
                if (!File.Exists(libPath)) { Console.Error.WriteLine($"Could not find dll file at {libPath}.\nWhisper will load with the default runtime (possibly CPU)."); libPath = null; }
                whisperFactory = WhisperFactory.FromPath(modelPath, libraryPath: libPath);

                var builder = whisperFactory.CreateBuilder().WithThreads(16).WithPrompt(whisperPrompt).WithSingleSegment().WithLanguage("en");
                (builder.WithBeamSearchSamplingStrategy() as BeamSearchSamplingStrategyBuilder)!.WithPatience(0.2f).WithBeamSize(5);
                processor = builder.Build();

                waveIn = new WaveInEvent() { BufferMilliseconds = clipLength, WaveFormat = waveFormat };
                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.StartRecording();
            }

            void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
            {
                // Cache the recorded bytes
                recordedBytes.AddRange(e.Buffer[..e.BytesRecorded]);
                if (recordedBytes.Count > 110000000) { recordedBytes.RemoveRange(0, 50000000); }

                // Get the max volume contained inside the clip
                var maxVolume = 0f; // This byte->sample algorithm is from: https://github.com/naudio/NAudio/blob/master/Docs/RecordingLevelMeter.md#calculating-peak-values
                for (int i = 0; i < e.BytesRecorded; i += 2) { maxVolume = Math.Max(maxVolume, Math.Abs((short) ((e.Buffer[i + 1] << 8) | e.Buffer[i + 0]) / 32768f)); }

                // Compare the volume with the threshold and act accordingly.
                // Once an interesting and 'full' set of clips pops up, serve it.
                if (maxVolume > voiceDetectionThreshold) {
                    currentBlankClips = 0;
                    totalNonBlankClips++;
                    nonIdleTime++;
                }
                else if (++currentBlankClips < detectionSettings.minBlanksPerSeperation) { nonIdleTime++; }
                else {
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
                    foreach (var user in ServiceUsers.Where(x => x.IsOfInterest(transcribedText))) { user.ProcessText(transcribedText); }
                }
            }

            /// <summary> Requests a transcription and responds with the text. </summary>
            async Task<string> ProcessAudio(byte[] bytes, string tempWavFilePath)
            {
                var wavStream = new MemoryStream();
                using (var writer = new WaveFileWriter(tempWavFilePath, waveFormat)) { writer.Write(bytes, 0, bytes.Length); }
                using (var fileStream = File.OpenRead(tempWavFilePath)) { await fileStream.CopyToAsync(wavStream); }
                wavStream.Seek(0, SeekOrigin.Begin);

                Console.Beep();
                return string.Join(' ', await processor!.ProcessAsync(wavStream).Select(x => x.Text).ToListAsync());
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
                else if (models.Length != 0) {
                    for (int i = 0; i < models.Length; i++) {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write($"{i + 1}: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(models[i]["Assets\\".Length..]);
                    }
                    while (true) {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write($"Please choose a model (1-{models.Length}): ");
                        if (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out var i) || i > models.Length) { Console.WriteLine(); continue; }
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.White;
                        return models[i - 1];
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Download a non-quantized model and place it in the executing directory:");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\t{Environment.CurrentDirectory}\\Assets");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You can find the official ggml models in whisper.cpp's huggingface repository: ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("\thttps://huggingface.co/ggerganov/whisper.cpp/tree/main");
                    Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Voice active. Begin talking to transcribe. Press any key at any time to exit.");
                Console.ForegroundColor = ConsoleColor.White;
                await Task.Delay(1000);
                Console.ReadKey();
            }
        }
    }
}
