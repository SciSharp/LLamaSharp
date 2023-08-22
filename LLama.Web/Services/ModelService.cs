using LLama.Web.Common;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;

namespace LLama.Web.Services
{

    public class ModelService : IModelService
    {
        private readonly LLamaOptions _configuration;
        private readonly ILogger<ModelService> _logger;
        private readonly SemaphoreSlim _modelLock = new SemaphoreSlim(1, 1);
        private readonly ConcurrentDictionary<string, LLamaWeights> _modelInstances;
        private readonly ConcurrentDictionary<string, LLamaContext> _contextInstances;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        public ModelService(ILogger<ModelService> logger, IOptions<LLamaOptions> options)
        {
            _configuration = options.Value;
            _modelInstances = new ConcurrentDictionary<string, LLamaWeights>();
            _contextInstances = new ConcurrentDictionary<string, LLamaContext>();
        }


        /// <summary>
        /// Loads a model with the provided configuration.
        /// </summary>
        /// <param name="modelConfig">The model configuration.</param>
        /// <returns></returns>
        public async Task<LLamaWeights> LoadModel(ModelOptions modelConfig)
        {
            if (_modelInstances.TryGetValue(modelConfig.Name, out LLamaWeights existingModel))
                return existingModel;

            // Model oading can take some toke so take a lock here
            await _modelLock.WaitAsync();

            try
            {
                // Catch anyone waiting behind the lock
                if (_modelInstances.TryGetValue(modelConfig.Name, out LLamaWeights model))
                    return existingModel;

                // If in single mode unload any other models
                if (_configuration.ModelCacheType == ModelCacheType.Single || _configuration.ModelCacheType == ModelCacheType.PreloadSingle)
                    await UnloadModels();

                model = LLamaWeights.LoadFromFile(modelConfig);
                if (!_modelInstances.TryAdd(modelConfig.Name, model))
                    throw new Exception("Failed to add model");

                return model;
            }
            finally
            {
                _modelLock.Release();
            }
        }


        /// <summary>
        /// Loads the models.
        /// </summary>
        public async Task LoadModels()
        {
            if (_configuration.ModelCacheType == ModelCacheType.Single 
             || _configuration.ModelCacheType == ModelCacheType.Multiple)
                return;

            foreach (var modelConfig in _configuration.Models)
            {
                await LoadModel(modelConfig);

                //Only preload first model if in SinglePreload mode
                if (_configuration.ModelCacheType == ModelCacheType.PreloadSingle)
                    break;
            }
        }


        /// <summary>
        /// Unloads the model.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <returns></returns>
        public Task<bool> UnloadModel(string modelName)
        {
            if (_modelInstances.TryRemove(modelName, out LLamaWeights model))
            {
                foreach (var contextKey in ContextKeys(modelName))
                {
                    if (!_contextInstances.TryRemove(contextKey, out var context))
                        continue;

                    context?.Dispose();
                }
                model?.Dispose();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }


        /// <summary>
        /// Unloads all models.
        /// </summary>
        public async Task UnloadModels()
        {
            foreach (var modelName in _modelInstances.Keys)
            {
                await UnloadModel(modelName);
            }
        }


        /// <summary>
        /// Gets a model ny name.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <returns></returns>
        public Task<LLamaWeights> GetModel(string modelName)
        {
            _modelInstances.TryGetValue(modelName, out LLamaWeights model);
            return Task.FromResult(model);
        }


        /// <summary>
        /// Gets a context from the specified model.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Model not found</exception>
        public Task<LLamaContext> GetContext(string modelName, string key)
        {
            if (!_modelInstances.TryGetValue(modelName, out LLamaWeights model))
                return Task.FromException<LLamaContext>(new Exception("Model not found"));

            _contextInstances.TryGetValue(ContextKey(modelName, key), out LLamaContext context);
            return Task.FromResult(context);
        }


        /// <summary>
        /// Creates a context on the specified model.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Model not found</exception>
        public Task<LLamaContext> CreateContext(string modelName, string key)
        {
            if (!_modelInstances.TryGetValue(modelName, out LLamaWeights model))
                return Task.FromException<LLamaContext>(new Exception("Model not found"));

            var modelConfig = _configuration.Models.FirstOrDefault(x => x.Name == modelName);
            var context = model.CreateContext(modelConfig, Encoding.UTF8);
            if (!_contextInstances.TryAdd(ContextKey(modelName, key), context))
                return Task.FromException<LLamaContext>(new Exception("Failed to add context"));

            return Task.FromResult(context);
        }


        /// <summary>
        /// Removes a context from the specified model.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Model not found</exception>
        public Task<bool> RemoveContext(string modelName, string key)
        {
            if (!_modelInstances.TryGetValue(modelName, out LLamaWeights model))
                return Task.FromResult(false);

            if (_contextInstances.TryRemove(ContextKey(modelName, key), out LLamaContext context))
            {
                context.Dispose();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }


        /// <summary>
        /// Loads, Gets,Creates a Model and a Context
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Model option '{modelName}' not found</exception>
        public async Task<LLamaContext> GetOrCreateModelAndContext(string modelName, string key)
        {
            if (_modelInstances.TryGetValue(modelName, out LLamaWeights model))
            {
                // Get or Create Context
                return await GetContext(modelName, key)
                    ?? await CreateContext(modelName, key);
            }

            // Get model configuration
            var modelConfig = _configuration.Models.FirstOrDefault(x => x.Name == modelName);
            if (modelConfig is null)
                throw new Exception($"Model option '{modelName}' not found");

            // Load Model
            model = await LoadModel(modelConfig);

            // Get or Create Context
            return await GetContext(modelName, key)
                ?? await CreateContext(modelName, key);
        }


        /// <summary>
        /// Gets a list of context keys for the model name provided.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <returns></returns>
        private IEnumerable<string> ContextKeys(string modelName)
        {
            return _contextInstances.Keys.Where(x => x.StartsWith($"{modelName}:"));
        }

        /// <summary>
        /// Create a key for the context collection using the model and key provided.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="contextKey">The context key.</param>
        private string ContextKey(string modelName, string contextKey) => $"{modelName}:{contextKey}";
    }
}
