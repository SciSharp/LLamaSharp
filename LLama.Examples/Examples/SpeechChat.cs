using LLama.Common;
using NAudio.Wave;
using Spectre.Console;
using Whisper.net;

namespace LLama.Examples.Examples
{
    public static class SpeechChat
    {
        public static async Task Run()
        {
            AnsiConsole.MarkupLine("""
                                  [yellow on black]
                                  This example demonstrates the basics of audio transcriptions, speech recognition, and speech commands,
                                      as well as how to recognize a user's voice in real time and then get a response from LLM.
                                  It uses whisper.net and models could be found in: https://huggingface.co/ggerganov/whisper.cpp/tree/main.
                                  To use it, you need a working microphone and enough RAM to host both audio + language models.
                                  Once you've selected the models, just speak to your microphone and watch the LLM continue your text.
                                  While it's going, you can say something like 'Okay, stop', or 'Stop now', to interrupt the LLM's inference.
                                  
                                  NOTE: You may need to poke around with the voice detection threshold, based on your mic's sensitivity.
                                  [/]
                                  """);

            AnsiConsole.MarkupLine("[white on black]You can find the official ggml models in whisper.cpp's huggingface repository: https://huggingface.co/ggerganov/whisper.cpp/tree/main [/]");

            var whisperModel = UserSettings.GetWhisperPath();
            var languageModel = UserSettings.GetModelPath();

            using var speechRecognitionServer = new SpeechRecognitionServer(whisperModel);

            using var _ = new LlamaSessionSpeechListener(speechRecognitionServer, languageModel);

            AnsiConsole.MarkupLine("[green]Voice active. Begin talking to transcribe. Press any key at any time to exit.[/]");
            await Task.Delay(1000);
            Console.ReadKey();
        }


        private class LlamaSessionSpeechListener
            : ISpeechListener, IDisposable
        {
            private bool _isModelResponding;
            private readonly SpeechRecognitionServer _audioServer;

            private readonly LLamaWeights _model;
            private readonly LLamaContext _context;
            private readonly InteractiveExecutor _executor;

            private string _fullPrompt = "";
            private bool _canceled;

            public LlamaSessionSpeechListener(SpeechRecognitionServer server, string languageModelPath)
            {
                var parameters = new ModelParams(languageModelPath)
                {
                    GpuLayerCount = 99
                };
                _model = LLamaWeights.LoadFromFile(parameters);
                _context = _model.CreateContext(parameters);
                _executor = new InteractiveExecutor(_context);

                _audioServer = server;
                _audioServer.ServiceUsers.Add(this);
            }

            // Whisper is struggling with single words and very short phrases without context, so it's actually better to say something like "Ok, Stop!" to have it work better.
            bool ISpeechListener.IsInterested(string audioTranscription) => !_isModelResponding || audioTranscription.Contains("stop", StringComparison.CurrentCultureIgnoreCase);
            void ISpeechListener.HandleSpeech(string audioTranscription)
            {
                if (_isModelResponding && audioTranscription.Contains("stop", StringComparison.CurrentCultureIgnoreCase))
                    _canceled = true;
                else if (!_isModelResponding)
                    _ = SendMessage(audioTranscription);
            }

            private async Task SendMessage(string newMessage)
            {
                // While a response is queried, we want to detect short phrases/commands like 'stop',
                _audioServer.DetectionSettings = (1, 2); // ..so we lower the min Speech Detection time.

                _isModelResponding = true;
                AddToPrompt($"\n{newMessage}\n", ConsoleColor.Blue);
                await foreach (var token in _executor.InferAsync(_fullPrompt))
                {
                    AddToPrompt(token);
                    if (_canceled)
                    {
                        AddToPrompt("[...stopped]", ConsoleColor.Red);
                        break;
                    }
                }
                _audioServer.DetectionSettings = (2, 3);         // Reset back to default detection settings to avoid false positives.
                (_isModelResponding, _canceled) = (false, false); // Reset the state variables to their default.
            }

            private void AddToPrompt(string msg, ConsoleColor color = ConsoleColor.Yellow)
            {
                _fullPrompt += msg;

                AnsiConsole.Markup($"[{color}]{Markup.Escape(msg)}[/]");
            }

            void IDisposable.Dispose()
            {
                _model.Dispose();
                _context.Dispose();
            }
        }

        public interface ISpeechListener
        {
            bool IsInterested(string audioTranscription);
            void HandleSpeech(string audioTranscription);
        }

        public sealed class SpeechRecognitionServer
            : IDisposable
        {
            private static readonly TimeSpan ClipLength = TimeSpan.FromMilliseconds(250);

            private const float VoiceDetectionThreshold = 0.01f; // Adjust as needed
            private readonly string[] _knownFalsePositives = ["[BLANK_AUDIO]", "Thank you", "[silence]"];

            private readonly WaveInEvent _waveIn;
            private readonly WaveFormat _waveFormat = new(16000, 16, 1); // 16KHz, 16 bits, Mono Channel
            private readonly List<byte> _recordedBytes = [];

            private readonly WhisperProcessor? _processor;

            private readonly string _whisperPrompt =
"""
The short audio comes from a user that is speaking to an AI Language Model in real time.
Pay extra attentions for commands like 'ok stop' or just 'stop'.
In case of inaudible sentences that might be, assume they're saying 'stop'.
""".Trim();

            // Tracked stats for Speech Recognition, Parsing, and Serving.
            private int _currentBlankClips;  // Ideally would work with milliseconds,
            private int _totalNonBlankClips; // ..but for example's sake they work on a

            private int _nonIdleTime;        // ..clip-based quant-length (1 = clipLength).
            // Default detection settings: A speech of 750ms, followed by pause of 500ms. (2x250ms)
            public (int minBlanksPerSeparation, int minNonBlanksForValidMessages) DetectionSettings = (2, 3);

            public readonly HashSet<ISpeechListener> ServiceUsers = [];

            public SpeechRecognitionServer(string modelPath)
            {
                var whisperFactory = WhisperFactory.FromPath(modelPath);

                var builder = whisperFactory
                    .CreateBuilder()
                    .WithThreads(16)
                    .WithPrompt(_whisperPrompt)
                    .WithSingleSegment()
                    .WithLanguage("en");
                ((BeamSearchSamplingStrategyBuilder)builder.WithBeamSearchSamplingStrategy()).WithPatience(0.2f).WithBeamSize(5);
                _processor = builder.Build();

                _waveIn = new WaveInEvent { BufferMilliseconds = (int)ClipLength.TotalMilliseconds, WaveFormat = _waveFormat };
                _waveIn.DataAvailable += OnAudioDataAvailable;
                _waveIn.StartRecording();
            }

            private void OnAudioDataAvailable(object? sender, WaveInEventArgs e)
            {
                // Cache the recorded bytes
                _recordedBytes.AddRange(e.Buffer[..e.BytesRecorded]);
                if (_recordedBytes.Count > 110000000) { _recordedBytes.RemoveRange(0, 50000000); }

                // Get the max volume contained inside the clip. Since the clip is recorded as bytes, we need to translate them to samples before getting their volume.
                var maxVolume = 0f; // This byte->sample algorithm is from: https://github.com/naudio/NAudio/blob/master/Docs/RecordingLevelMeter.md#calculating-peak-values
                for (var i = 0; i < e.BytesRecorded; i += 2)
                {
                    maxVolume = Math.Max(maxVolume, Math.Abs((short)((e.Buffer[i + 1] << 8) | e.Buffer[i + 0]) / 32768f));
                }

                // Compare the volume with the threshold and act accordingly. Once an interesting and 'full' set of clips pops up, serve it.
                if (maxVolume >= VoiceDetectionThreshold)
                {
                    _currentBlankClips = 0;
                    _totalNonBlankClips++;
                    _nonIdleTime++;
                }
                else if (++_currentBlankClips < DetectionSettings.minBlanksPerSeparation)
                {
                    _nonIdleTime++;
                }
                else
                {
                    if (_totalNonBlankClips >= DetectionSettings.minNonBlanksForValidMessages)
                        SendTranscription();
                    else if (_totalNonBlankClips > 0) { } // This might be case of a false-positive -- knock, noise, cough, anything.
                    (_currentBlankClips, _totalNonBlankClips, _nonIdleTime) = (0, 0, 0);
                }


                async void SendTranscription()
                {
                    var bytesPerClip = _waveFormat.BitsPerSample * (int)ClipLength.TotalMilliseconds * 2;
                    var capturedClipBytes = _recordedBytes.TakeLast(bytesPerClip * (_nonIdleTime + 2)).ToArray();

                    // Save to temporary file.
                    var transcribedText = await ProcessAudio(capturedClipBytes, "Assets\\temp.wav"); 

                    // False positive.. yikes!
                    if (_knownFalsePositives.Contains(transcribedText))
                        return;    
                    
                    foreach (var user in ServiceUsers.Where(x => x.IsInterested(transcribedText)))
                        user.HandleSpeech(transcribedText);
                }
            }

            /// <summary> Requests a transcription and responds with the text. </summary>
            private async Task<string> ProcessAudio(byte[] bytes, string tempWavFilePath)
            {
                await using var wavStream = new MemoryStream();

                await using (var writer = new WaveFileWriter(tempWavFilePath, _waveFormat))
                    writer.Write(bytes, 0, bytes.Length);
                await using (var fileStream = File.OpenRead(tempWavFilePath))
                    await fileStream.CopyToAsync(wavStream);

                wavStream.Seek(0, SeekOrigin.Begin);

                Console.Beep();
                return string.Join(' ', await _processor!.ProcessAsync(wavStream).Select(x => x.Text).ToListAsync()).Trim();
            }

            void IDisposable.Dispose()
            {
                _waveIn.Dispose();
                _processor?.Dispose();
            }
        }
    }
}
