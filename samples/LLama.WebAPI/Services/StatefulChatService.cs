
using LLama.WebAPI.Models;
using Microsoft;
using System.Runtime.CompilerServices;

namespace LLama.WebAPI.Services;

public class StatefulChatService : IDisposable
{
    private readonly ChatSession _session;
    private readonly LLamaContext _context;
    private readonly ILogger<StatefulChatService> _logger;
    private bool _continue = false;

    private const string SystemPrompt = "Transcript of a dialog, where the User interacts with an Assistant. Assistant is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.";

    public StatefulChatService(IConfiguration configuration, ILogger<StatefulChatService> logger)
    {
        var @params = new Common.ModelParams(configuration["ModelPath"]!)
        {
            ContextSize = 512,
        };

        // todo: share weights from a central service
        using var weights = LLamaWeights.LoadFromFile(@params);

        _logger = logger;
        _context = new LLamaContext(weights, @params);

        _session = new ChatSession(new InteractiveExecutor(_context));
        _session.History.AddMessage(Common.AuthorRole.System, SystemPrompt);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public async Task<string> Send(SendMessageInput input)
    {

        if (!_continue)
        {
            _logger.LogInformation("Prompt: {text}", SystemPrompt);
            _continue = true;
        }
        _logger.LogInformation("Input: {text}", input.Text);
        var outputs = _session.ChatAsync(
            new Common.ChatHistory.Message(Common.AuthorRole.User, input.Text),
            new Common.InferenceParams()
            {
                RepeatPenalty = 1.0f,
                AntiPrompts = new string[] { "User:" },
            });

        var result = "";
        await foreach (var output in outputs)
        {
            _logger.LogInformation("Message: {output}", output);
            result += output;
        }

        return result;
    }

    public async IAsyncEnumerable<string> SendStream(SendMessageInput input)
    {
        if (!_continue)
        {
            _logger.LogInformation(SystemPrompt);
            _continue = true;
        }

        _logger.LogInformation(input.Text);

        var outputs = _session.ChatAsync(
            new Common.ChatHistory.Message(Common.AuthorRole.User, input.Text!)
            , new Common.InferenceParams()
            {
                RepeatPenalty = 1.0f,
                AntiPrompts = new string[] { "User:" },
            });

        await foreach (var output in outputs)
        {
            _logger.LogInformation(output);
            yield return output;
        }
    }
}
