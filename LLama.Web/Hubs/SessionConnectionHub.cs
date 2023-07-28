using LLama.Web.Common;
using LLama.Web.Services;
using Microsoft.AspNetCore.SignalR;

namespace LLama.Web.Hubs
{
    public class SessionConnectionHub : Hub<ISessionClient>
    {
        private readonly ILogger<SessionConnectionHub> _logger;
        private readonly ConnectionSessionService _modelSessionService;

        public SessionConnectionHub(ILogger<SessionConnectionHub> logger, ConnectionSessionService modelSessionService)
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


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.Log(LogLevel.Information, "[OnDisconnectedAsync], Id: {0}", Context.ConnectionId);

            // Remove connections session on dissconnect
            await _modelSessionService.RemoveAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }


        [HubMethodName("LoadModel")]
        public async Task OnLoadModel(LLamaExecutorType executorType, string modelName, string promptName, string parameterName)
        {
            _logger.Log(LogLevel.Information, "[OnLoadModel] - Load new model, Connection: {0}, Model: {1}, Prompt: {2}, Parameter: {3}", Context.ConnectionId, modelName, promptName, parameterName);
          

            // Create model session
            var modelSessionResult = await _modelSessionService.CreateAsync(executorType, Context.ConnectionId, modelName, promptName, parameterName);
            if (modelSessionResult.HasError)
            {
                await Clients.Caller.OnError(modelSessionResult.Error);
                return;
            }

            // Notify client
            await Clients.Caller.OnStatus(Context.ConnectionId, SessionConnectionStatus.Loaded);
        }


        [HubMethodName("SendPrompt")]
        public async Task OnSendPrompt(string prompt)
        {
            _logger.Log(LogLevel.Information, "[OnSendPrompt] - New prompt received, Connection: {0}", Context.ConnectionId);

            // Send Infer response
            await foreach (var responseFragment in _modelSessionService.InferAsync(Context.ConnectionId, prompt, CancellationTokenSource.CreateLinkedTokenSource(Context.ConnectionAborted)))
            {
                await Clients.Caller.OnResponse(responseFragment);
            }
        }

    }
}
