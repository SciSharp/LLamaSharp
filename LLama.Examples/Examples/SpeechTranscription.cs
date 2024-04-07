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

            using var audioServer = new AudioServer(model);
            audioServer.ServiceUsers.Add(new AudioEchoer());

            loadFinished = true; loading.Wait();
            await ConsoleStyleHelpers.WaitUntilExit();
        }

        class AudioEchoer : IAudioServiceUser
        {
            bool IAudioServiceUser.IsOfInterest(string AudioTranscription)
            {
                if (AudioTranscription.Contains("Artificial Intelligence", StringComparison.CurrentCultureIgnoreCase)) {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Skipped text because it's not of interest {AudioTranscription}");
                    Console.ForegroundColor = ConsoleColor.White;
                    return false;
                }
                else { return true; }
            }
            void IAudioServiceUser.ProcessText(string AudioTranscription)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(AudioTranscription);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public interface IAudioServiceUser
        {
            bool IsOfInterest(string AudioTranscription);
            void ProcessText(string AudioTranscription);
        }

        public class AudioServer : IDisposable
        {
            const int clipLength = 250; // ms
            const float voiceDetectionThreshold = 0.02f;
            readonly string[] knownFalsePositives = ["[BLANK_AUDIO]", "Thank you", "[silence]"];

            WaveInEvent waveIn;
            WaveFormat waveFormat = new(16000, 16, 1); // 16KHz, 16 bits, Mono Channel
            List<byte> recordedBytes = [];

            WhisperFactory? whisperFactory;
            WhisperProcessor? processor;
            string whisperPrompt =
"""
The short audio comes from a non-native-english speaker/user that talks to an LLM in real time.
Transcribe knowing this as a fact, and that multiple phrases or questions might appear together.
If there are pauses, form paragraphs that leaves related parts together, and splits the next in new lines.
""";

            // Tracked stats for Speech Recognition, Parsing, and Serving.
            int currentBlankClips;  // Ideally would work with milliseconds,
            int totalNonBlankClips; // ..but for example's sake they work on a
            int nonIdleTime;        // ..clip-based quant-length (1 = clipLength).
                                    // Default detection settings: A speech of 750ms, followed by pause of 500ms. (2x250ms)
            public (int minBlanksPerSeperation, int minNonBlanksForValidMessages) detectionSettings = (2, 3);

            public HashSet<IAudioServiceUser> ServiceUsers = [];

            public AudioServer(string modelPath)
            {
                whisperFactory = WhisperFactory.FromPath(modelPath);
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
                    if (totalNonBlankClips > detectionSettings.minNonBlanksForValidMessages) { SendTranscription(); }
                    else if (totalNonBlankClips > 0) { } // This might be case of a false-positive -- knock, noise, cough, anything.
                    (currentBlankClips, totalNonBlankClips, nonIdleTime) = (0, 0, 0);
                }


                async void SendTranscription()
                {
                    var bytesPerClip = waveFormat.BitsPerSample * clipLength * 2;
                    var capturedClipBytes = recordedBytes.TakeLast(bytesPerClip * (nonIdleTime + 2)).ToArray();
                    var transcribedText = await ProcessAudio(capturedClipBytes, "Assets\\temp.wav"); // Save to temporary file.
                    if (knownFalsePositives.Contains(transcribedText)) { return; }					 // False positive.. yikes!
                    foreach (var user in ServiceUsers.Where(x => x.IsOfInterest(transcribedText))) { user.ProcessText(transcribedText); }
                }
            }

            /// <summary> Requests a transcription and responds with the text. Whisper.net currently doesn't work well with parallelism. </summary>
            async Task<string> ProcessAudio(byte[] bytes, string tempWavFilePath)
            {
                var wavStream = new MemoryStream();
                using (var writer = new WaveFileWriter(tempWavFilePath, waveFormat)) { writer.Write(bytes, 0, bytes.Length); }
                using (var fileStream = File.OpenRead(tempWavFilePath)) { await fileStream.CopyToAsync(wavStream); }
                wavStream.Seek(0, SeekOrigin.Begin);

                return string.Join(' ', await processor!.ProcessAsync(wavStream).Select(result => result.Text).ToListAsync()).Trim();
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
                        Console.Write($"{i}: ");
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
            public static async Task LoadPrint(string startText, Func<bool> ShouldContinue)
            {
                var startTime = DateTime.Now;
                Console.Write(startText);
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
