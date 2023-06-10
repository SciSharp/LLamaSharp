using LLama.OldVersion;
using LLama.WebAPI.Models;

namespace LLama.WebAPI.Services;

public class ChatService
{
    private readonly ChatSession<LLamaModel> _session;

    public ChatService()
    {
        LLamaModel model = new(new LLamaParams(model: @"ggml-model-q4_0.bin", n_ctx: 512, interactive: true, repeat_penalty: 1.0f, verbose_prompt: false));
        _session = new ChatSession<LLamaModel>(model)
            .WithPromptFile(@"Assets\chat-with-bob.txt")
            .WithAntiprompt(new string[] { "User:" });
    }

    public string Send(SendMessageInput input)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(input.Text);

        Console.ForegroundColor = ConsoleColor.White;
        var outputs = _session.Chat(input.Text);
        var result = "";
        foreach (var output in outputs)
        {
            Console.Write(output);
            result += output;
        }

        return result;
    }
}
