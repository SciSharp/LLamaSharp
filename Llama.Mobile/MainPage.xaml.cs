namespace Llama.Mobile;

using Android.Icu.Text;
using Java.Lang;
using Javax.Annotation;
using Llama.Mobile.Src;
using LLama;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;
using Xamarin.Google.Crypto.Tink.Subtle;
using Xamarin.KotlinX.Coroutines;
using static System.Net.Mime.MediaTypeNames;
using StringBuilder = System.Text.StringBuilder;

public partial class MainPage : ContentPage
{

    public ObservableCollection<Message> Messages { get; } = new();

    //put the gguf model in rasources/raw
    private const string modelName = "Llama-3.2-1B-Instruct-Q4_0.gguf";

    private ChatSession? _session;
    ChatSession Session
    {
        get
        {
            if (_session is null) throw new NullReferenceException("_session can't be null");
            return _session;
        }

        set
        {
            _session = value;
        }
    }

    private static InferenceParams InferenceParams = new InferenceParams()
    {
        MaxTokens = 256, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
        AntiPrompts = new List<string> { "User:" }, // Stop generation once antiprompts appear.

        SamplingPipeline = new DefaultSamplingPipeline(),

    };

public MainPage()
    {
        InitializeComponent();
        chat.BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        string modelPath = Path.Combine(FileSystem.Current.AppDataDirectory, modelName);

        if (!File.Exists(modelPath))
        {
            //get a stream of the model stored in the apk
            using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(modelName);

            //copy the data from the inputStream in to a file, with te same name, in the the app data directory
            using FileStream outputStream = File.Create(modelPath);
            await inputStream.CopyToAsync(outputStream);
            outputStream.Close();
            inputStream.Close();
        }


        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 1024, // The longest length of chat as memory.
            GpuLayerCount = 5 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
        };
        var model = LLamaWeights.LoadFromFile(parameters);
        var context = model.CreateContext(parameters);
        var executor = new InteractiveExecutor(context);

        // Add chat histories as prompt to tell AI how to act.
        var chatHistory = new ChatHistory();
        chatHistory.AddMessage(AuthorRole.System, "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.");
        chatHistory.AddMessage(AuthorRole.User, "Hello, Bob.");
        chatHistory.AddMessage(AuthorRole.Assistant, "Hello. How may I help you today?");

        Session = new(executor, chatHistory);

        pn_loading.IsVisible = false;
        btn_ask.IsEnabled = true;

        await Task.Delay(100); //on the emulator without this little delay the popup isn't shown

        await DisplayAlert("Loaded", "model correctly Loaded", "OK");
    }

    private async void OnAskClicked(object sender, EventArgs e)
    {
        btn_ask.IsEnabled = false;
        Messages.Add(new Message { Type = messageType.User, Text = tx_userPrompt.Text, IsPreparing = false });
        Message response = new Message { Type = messageType.other, Text = "", IsPreparing = true };
        Messages.Add(response);
        chat.ScrollTo(Messages.Last(), position: ScrollToPosition.End, animate: false);

        StringBuilder sp = new StringBuilder();
        bool assistantHeaderRemoved = false;
        bool userRemoved = false;
        await foreach (string text in Session.ChatAsync(new ChatHistory.Message(AuthorRole.User, tx_userPrompt.Text), InferenceParams))
        {
            //raw model output cleaning from words like "Assistant:" and "User:"
            //if your model produces a clean output you can just only leave the following two instruction
            //inside this loop:
            //response.IsPreparing = false;
            //response.AppendText(text); 

            if (!assistantHeaderRemoved)
            {
                sp.Append(text);
                if (sp.Length >= 11)
                {
                    sp = sp.Remove(0, 11);
                    assistantHeaderRemoved = true;
                    response.Text = sp.ToString();
                    response.IsPreparing = false;
                    sp.Clear();
                }
                continue;
            }

            if (text.EndsWith("User"))
            {
                response.AppendText(text.Remove(text.Length - 4));
                userRemoved = true;
            }
            else if (userRemoved)
            {
                if (text == ":")
                {
                    break;
                }
                else
                {
                    response.AppendText("User" + text);
                    userRemoved = false;
                }

            }
            else
            {
                response.AppendText(text);
            }

            chat.ScrollTo(Messages.Last(), position: ScrollToPosition.End, animate: false);
        }
        response.Text = response.Text.TrimEnd('\n');
        btn_ask.IsEnabled = true;
    }
}
