using LLama.Web.Common;
using LLama.Web.Models;
using LLama.Web.Services;
using Microsoft.AspNetCore.SignalR;

namespace LLama.Web.Hubs;

public class SessionConnectionHub : Hub<ISessionClient>
{
    private readonly ILogger<SessionConnectionHub> _logger;
    private readonly IModelSessionService _modelSessionService;

    public SessionConnectionHub(ILogger<SessionConnectionHub> logger, IModelSessionService modelSessionService)
    {
        _logger = logger;
        _modelSessionService = modelSessionService;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.Log(LogLevel.Information, "[OnConnectedAsync], Id: {0}", Context.ConnectionId);

        // Notify client of successful connection
        await Clients.Caller.OnStatus(Context.ConnectionId, SessionConnectionStatus.Connected);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        _logger.Log(LogLevel.Information, "[OnDisconnectedAsync], Id: {0}", Context.ConnectionId);

        // Remove connections session on disconnect
        await _modelSessionService.CloseAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    [HubMethodName("LoadModel")]
    public async Task OnLoadModel(SessionConfig sessionConfig, InferenceOptions inferenceConfig)
    {
        _logger.Log(LogLevel.Information, "[OnLoadModel] - Load new model, Connection: {0}", Context.ConnectionId);
        await _modelSessionService.CloseAsync(Context.ConnectionId);

        // Create model session
        var modelSession = await _modelSessionService.CreateAsync(Context.ConnectionId, sessionConfig, inferenceConfig);
        if (modelSession is null)
        {
            await Clients.Caller.OnError("Failed to create model session");
            return;
        }

        // Notify client
        await Clients.Caller.OnStatus(Context.ConnectionId, SessionConnectionStatus.Loaded);
    }

    [HubMethodName("SendPrompt")]
    public IAsyncEnumerable<TokenModel> OnSendPrompt(string prompt, InferenceOptions inferConfig, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, "[OnSendPrompt] - New prompt received, Connection: {0}", Context.ConnectionId);

        var linkedCancelationToken = CancellationTokenSource.CreateLinkedTokenSource(Context.ConnectionAborted, cancellationToken);
        return _modelSessionService.InferAsync(Context.ConnectionId, prompt, inferConfig, linkedCancelationToken.Token);
    }
}
