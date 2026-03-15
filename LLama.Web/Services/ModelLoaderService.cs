namespace LLama.Web.Services;

/// <summary>
/// Service for managing loading/preloading of models at app startup
/// </summary>
/// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
public class ModelLoaderService : IHostedService 
{
    private readonly IModelService _modelService;
    private readonly IModelDownloadService _modelDownloadService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelLoaderService"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public ModelLoaderService(IModelService modelService, IModelDownloadService modelDownloadService)
    {
        _modelService = modelService;
        _modelDownloadService = modelDownloadService;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _modelDownloadService.StartDownloads(cancellationToken);

        _ = Task.Run(async () =>
        {
            await _modelDownloadService.WaitForDownloadsAsync(cancellationToken);
            await _modelService.LoadModels();
        }, cancellationToken);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _modelService.UnloadModels();
    }
}
