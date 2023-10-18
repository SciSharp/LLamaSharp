
using LLama.WebAPI.Models;
using Microsoft;
using System.Runtime.CompilerServices;

namespace LLama.WebAPI.Services;

public class StatefulChatService : IDisposable
{
    private readonly ChatSession _session;
    private readonly LLamaContext _context;
    private bool _continue = false;

    private const string SystemPrompt = "Transcript of a dialog, where the User interacts with an Assistant. Assistant is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.\n\n"
                                       + "User: ";

    public StatefulChatService(IConfiguration configuration)
    {
        var @params = new Common.ModelParams(configuration["ModelPath"])
        {
            ContextSize = 512,
        };

        // todo: share weights from a central service
        using var weights = LLamaWeights.LoadFromFile(@params);

        _context = new LLamaContext(weights, @params);
        _session = new ChatSession(new InteractiveExecutor(_context));
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public async Task<string> Send(SendMessageInput input)
    {
        var userInput = input.Text;
        if (!_continue)
        {
            userInput = SystemPrompt + userInput;
            Console.Write(SystemPrompt);
            _continue = true;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(input.Text);

        Console.ForegroundColor = ConsoleColor.White;
        var outputs = _session.ChatAsync(userInput, new Common.InferenceParams()
        {
            RepeatPenalty = 1.0f,
            AntiPrompts = new string[] { "User:" },
        });
        var result = "";
        await foreach (var output in outputs)
        {
            Console.Write(output);
            result += output;
        }

        return result;
    }

    public async IAsyncEnumerable<string> SendStream(SendMessageInput input)
    {
        var userInput = input.Text;
        if (!_continue)
        {
            userInput = SystemPrompt + userInput;
            Console.Write(SystemPrompt);
            _continue = true;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(input.Text);

        Console.ForegroundColor = ConsoleColor.White;
        var outputs = _session.ChatAsync(userInput, new Common.InferenceParams()
        {
            RepeatPenalty = 1.0f,
            AntiPrompts = new string[] { "User:" },
        });
        await foreach (var output in outputs)
        {
            Console.Write(output);
            yield return output;
        }
    }
}
