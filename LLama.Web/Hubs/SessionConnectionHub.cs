using LLama.Web.Common;
using LLama.Web.Models;
using LLama.Web.Services;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

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

            // Remove existing connections session
            await _modelSessionService.RemoveAsync(Context.ConnectionId);

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

            // Get connections session
            var modelSession = await _modelSessionService.GetAsync(Context.ConnectionId);
            if (modelSession is null)
            {
                await Clients.Caller.OnError("No model has been loaded");
                return;
            }


            // Create unique response id
            var responseId = Guid.NewGuid().ToString();

            // Send begin of response
            await Clients.Caller.OnResponse(new ResponseFragment(responseId, isFirst: true));

            // Send content of response
            var stopwatch = Stopwatch.GetTimestamp();
            await foreach (var fragment in modelSession.InferAsync(prompt, CancellationTokenSource.CreateLinkedTokenSource(Context.ConnectionAborted)))
            {
                await Clients.Caller.OnResponse(new ResponseFragment(responseId, fragment));
            }

            // Send end of response
            var elapsedTime = Stopwatch.GetElapsedTime(stopwatch);
            var signature = modelSession.IsInferCanceled()
                ? $"Inference cancelled after {elapsedTime.TotalSeconds:F0} seconds"
                : $"Inference completed in {elapsedTime.TotalSeconds:F0} seconds";
            await Clients.Caller.OnResponse(new ResponseFragment(responseId, signature, isLast: true));
            _logger.Log(LogLevel.Information, "[OnSendPrompt] - Inference complete, Connection: {0}, Elapsed: {1}, Canceled: {2}", Context.ConnectionId, elapsedTime, modelSession.IsInferCanceled());
        }

    }
}
