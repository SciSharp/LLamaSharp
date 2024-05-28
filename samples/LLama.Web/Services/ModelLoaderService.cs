namespace LLama.Web.Services;

/// <summary>
/// Service for managing loading/preloading of models at app startup
/// </summary>
/// <typeparam name="T">Type used to identify contexts</typeparam>
/// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
public class ModelLoaderService : IHostedService 
{
    private readonly IModelService _modelService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelLoaderService"/> class.
    /// </summary>
    /// <param name="modelService">The model service.</param>
    public ModelLoaderService(IModelService modelService)
    {
        _modelService = modelService;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _modelService.LoadModels();
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
