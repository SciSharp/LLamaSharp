using LLama.Web.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace LLama.Web.Hubs
{
    public class InteractiveHub : Hub<IInteractiveClient>
    {
        //TODO: Move all this out to ModelSessionService
        private static readonly ConcurrentDictionary<string, ModelSession> _modelSessions = new ConcurrentDictionary<string, ModelSession>();

        private readonly ILogger<InteractiveHub> _logger;
        private readonly LLamaOptions _options;

        public InteractiveHub(ILogger<InteractiveHub> logger, IOptions<LLamaOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }


        public override async Task OnConnectedAsync()
        {
            _logger.Log(LogLevel.Information, "OnConnectedAsync, Id: {0}", Context.ConnectionId);
            await base.OnConnectedAsync();
            await Clients.Caller.OnStatus("Connected");
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.Log(LogLevel.Information, "[OnDisconnectedAsync], Id: {0}", Context.ConnectionId);

            if (_modelSessions.TryRemove(Context.ConnectionId, out var modelSession))
            {
                _logger.Log(LogLevel.Information, "[OnDisconnectedAsync] - Removed InteractiveExecutor, Id: {0}", Context.ConnectionId);
                modelSession.Dispose();
            }

            await base.OnDisconnectedAsync(exception);
        }


        [HubMethodName("LoadModel")]
        public async Task OnLoadModel(string modelName, string promptName, string parameterName)
        {
            _logger.Log(LogLevel.Information, "[OnLoadModel] - Load new model, Connection: {0}, Model: {1}, Prompt: {2}, Parameter: {3}", Context.ConnectionId, modelName, promptName, parameterName);
            if (_modelSessions.TryRemove(Context.ConnectionId, out var modelSession))
            {
                _logger.Log(LogLevel.Information, "[OnLoadModel] - Removed existing model session, Id: {0}", Context.ConnectionId);
                modelSession.Dispose();
            }

            var modelOption = _options.Models.First(x => x.Name == modelName);
            var promptOption = _options.Prompts.First(x => x.Name == promptName);
            var parameterOption = _options.Parameters.First(x => x.Name == parameterName);
            modelSession = new ModelSession(Context.ConnectionId, modelOption, promptOption, parameterOption);
            if (!_modelSessions.TryAdd(Context.ConnectionId, modelSession))
            {
                _logger.Log(LogLevel.Error, "[OnLoadModel] - Failed to add new model session, Connection: {0}", Context.ConnectionId);
                await Clients.Caller.OnError("No model has been loaded");
                return;
                
            }
            _logger.Log(LogLevel.Information, "[OnLoadModel] - New model session added, Connection: {0}", Context.ConnectionId);
            await Clients.Caller.OnStatus("Loaded");
        }


        [HubMethodName("SendPrompt")]
        public async Task OnSendPrompt(string prompt)
        {
            var stopwatch = Stopwatch.GetTimestamp();
            _logger.Log(LogLevel.Information, "[OnSendPrompt] - New prompt received, Connection: {0}", Context.ConnectionId);
            if (!_modelSessions.TryGetValue(Context.ConnectionId, out var modelSession))
            {
                _logger.Log(LogLevel.Warning, "[OnSendPrompt] -  No model has been loaded for this connection, Connection: {0}", Context.ConnectionId);
                await Clients.Caller.OnError("No model has been loaded");
                return;
            }

            // Create unique response id
            modelSession.ResponseId = Guid.NewGuid().ToString();

            // Send begin of response
            await Clients.Caller.OnResponse(new ResponseFragment(modelSession.ResponseId, isFirst: true));

            // Send content of response
            await foreach (var fragment in modelSession.InferAsync(prompt, CancellationTokenSource.CreateLinkedTokenSource(Context.ConnectionAborted)))
            {
                await Clients.Caller.OnResponse(new ResponseFragment(modelSession.ResponseId, fragment));
            }

            // Send end of response
            var elapsedTime = Stopwatch.GetElapsedTime(stopwatch);
            var signature = modelSession.IsInferCanceled()
                ? $"Inference cancelled after {elapsedTime.TotalSeconds:F0} seconds"
                : $"Inference completed in {elapsedTime.TotalSeconds:F0} seconds";
            await Clients.Caller.OnResponse(new ResponseFragment(modelSession.ResponseId, signature, isLast: true));
            _logger.Log(LogLevel.Information, "[OnSendPrompt] - Inference complete, Connection: {0}, Elapsed: {1}, Canceled: {2}", Context.ConnectionId, elapsedTime, modelSession.IsInferCanceled());
        }
      
    }
}
