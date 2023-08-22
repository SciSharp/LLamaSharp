namespace LLama.Web.Services
{
    public class LoaderService : IHostedService
    {
        private readonly IModelService _modelService;

        public LoaderService(IModelService modelService)
        {
            _modelService = modelService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _modelService.LoadModels();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _modelService.UnloadModels();
        }
    }
}
