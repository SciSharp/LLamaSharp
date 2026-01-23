using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LLama;
using LLama.Common;
using LLama.Sampling;
using Windows.Storage.Pickers;

namespace LLama.Examples.WinUI
{
    public class ChatMessage : INotifyPropertyChanged
    {
        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
        public HorizontalAlignment Alignment { get; set; }
        public Brush? Background { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public sealed partial class MainWindow : Window
    {
        private LLamaWeights? _weights;
        private LLamaContext? _context;
        private InteractiveExecutor? _executor;
        private ChatSession? _session;
        private ObservableCollection<ChatMessage> _messages = new();

        public MainWindow()
        {
            InitializeComponent();
            ChatHistoryList.ItemsSource = _messages;
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();

            // Get the window handle (HWND) for the WinUI 3 window
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);

            picker.FileTypeFilter.Add(".gguf");
            picker.SuggestedStartLocation = PickerLocationId.Downloads;

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                ModelPathInput.Text = file.Path;
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string path = ModelPathInput.Text;
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            {
                AddMessage("Error: Please provide a valid model path.", false);
                return;
            }

            LoadingRing.IsActive = true;
            LoadButton.IsEnabled = false;

            try
            {
                await Task.Run(() =>
                {
                    var parameters = new ModelParams(path)
                    {
                        ContextSize = 2048,
                        GpuLayerCount = 20
                    };
                    _weights = LLamaWeights.LoadFromFile(parameters);
                    _context = _weights.CreateContext(parameters);
                    _executor = new InteractiveExecutor(_context);
                    _session = new ChatSession(_executor);

                    // Add a default system message to guide the model
                    _session.History.AddMessage(AuthorRole.System, "You are a helpful, brief, and friendly assistant.");
                });

                AddMessage("Model loaded successfully!", false);
            }
            catch (Exception ex)
            {
                AddMessage($"Error loading model: {ex.Message}", false);
            }
            finally
            {
                LoadingRing.IsActive = false;
                LoadButton.IsEnabled = true;
            }
        }

        private void AddMessage(string text, bool isUser)
        {
            _messages.Add(new ChatMessage
            {
                Text = text,
                Alignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Background = new SolidColorBrush(isUser ? Colors.LightSkyBlue : Colors.LightGray)
            });
            ChatScrollViewer.ChangeView(null, ChatScrollViewer.ScrollableHeight, null);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private async void UserInput_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await SendMessage();
            }
        }

        private async Task SendMessage()
        {
            string input = UserInput.Text;
            if (string.IsNullOrWhiteSpace(input) || _session == null) return;

            UserInput.Text = "";
            AddMessage(input, true);

            var botMessage = new ChatMessage
            {
                Text = "",
                Alignment = HorizontalAlignment.Left,
                Background = new SolidColorBrush(Colors.LightGray)
            };
            _messages.Add(botMessage);

            var inferenceParams = new InferenceParams()
            {
                MaxTokens = 512,
                AntiPrompts = new[] { "User:", "###", "Instruction:", "\n\n" },
                SamplingPipeline = new DefaultSamplingPipeline(),
            };

            await foreach (var text in _session.ChatAsync(new ChatHistory.Message(AuthorRole.User, input), inferenceParams))
            {
                // Filter out common prompt prefixes that legacy models might hallucinate
                if (botMessage.Text.Length == 0 && (text.Trim().StartsWith("User:") || text.Trim().StartsWith("Assistant:")))
                {
                    continue;
                }

                botMessage.Text += text;
                ChatScrollViewer.ChangeView(null, ChatScrollViewer.ScrollableHeight, null);
            }

            // If the message is still empty, the model might have been stopped too early or failed
            if (string.IsNullOrWhiteSpace(botMessage.Text))
            {
                botMessage.Text = "(No response generated)";
            }
        }
    }
}
