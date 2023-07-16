namespace LLama.Web.Services
{
    public class ModelSessionService : IModelSessionService
    {
        private readonly ILogger<ModelSessionService> _logger;

        public ModelSessionService(ILogger<ModelSessionService> logger)
        {
            _logger = logger;
        }

    }


}
