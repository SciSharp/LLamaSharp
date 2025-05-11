namespace Llama.Mobile;

using LLama.Native;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        //Load the native library
        NativeApi.llama_empty_call();

        label1.Text = "llama.cpp loaded successfully";
    }
}
