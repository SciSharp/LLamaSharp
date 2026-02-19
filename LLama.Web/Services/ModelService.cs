using LLama.Web.Async;
using LLama.Web.Common;
using LLama.Web.Models;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace LLama.Web.Services;

/// <summary>
/// Service for handling models, weights, and contexts.
/// </summary>
public class ModelService : IModelService
{
    private readonly AsyncLock _modelLock;
    private readonly AsyncLock _contextLock;
    private readonly LLamaOptions _configuration;
    private readonly ILogger<ModelService> _llamaLogger;
    private readonly IModelDownloadService _modelDownloadService;
    private readonly ConcurrentDictionary<string, LLamaModel> _modelInstances;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelService"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="llamaLogger">The logger.</param>
    /// <param name="modelDownloadService">The model download service.</param>
    public ModelService(IOptions<LLamaOptions> configuration, ILogger<ModelService> llamaLogger, IModelDownloadService modelDownloadService)
    {
        _llamaLogger = llamaLogger;
        _modelLock = new AsyncLock();
        _contextLock = new AsyncLock();
        _configuration = configuration.Value;
        _modelDownloadService = modelDownloadService;
        _modelInstances = new ConcurrentDictionary<string, LLamaModel>();
    }

    /// <summary>
    /// Loads a model with the provided configuration.
    /// </summary>
    /// <param name="modelOptions">The model configuration.</param>
    /// <returns></returns>
    public async Task<LLamaModel> LoadModel(ModelOptions modelOptions)
    {
        if (_modelInstances.TryGetValue(modelOptions.Name, out var existingModel))
            return existingModel;

        using (await _modelLock.LockAsync())
        {
            if (_modelInstances.TryGetValue(modelOptions.Name, out var model))
                return model;

            if (!_modelDownloadService.IsModelReady(modelOptions.Name))
                throw new Exception($"Model '{modelOptions.Name}' is still downloading.");

            // If in single mode, unload any other models.
            if (_configuration.ModelLoadType == ModelLoadType.Single
             || _configuration.ModelLoadType == ModelLoadType.PreloadSingle)
                await UnloadModels();

            if (modelOptions.UBatchSize < modelOptions.BatchSize)
                modelOptions.UBatchSize = modelOptions.BatchSize;

            model = await LLamaModel.CreateAsync(modelOptions, _llamaLogger);
            _modelInstances.TryAdd(modelOptions.Name, model);
            return model;
        }
    }

    /// <summary>
    /// Loads the models.
    /// </summary>
    public async Task LoadModels()
    {
        if (_configuration.ModelLoadType == ModelLoadType.Single
         || _configuration.ModelLoadType == ModelLoadType.Multiple)
            return;

        foreach (var modelConfig in _configuration.Models)
        {
            await LoadModel(modelConfig);

            // Only preload the first model in PreloadSingle mode.
            if (_configuration.ModelLoadType == ModelLoadType.PreloadSingle)
                break;
        }
    }

    /// <summary>
    /// Unloads the model.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <returns></returns>
    public Task UnloadModel(string modelName)
    {
        if (_modelInstances.TryRemove(modelName, out var model))
        {
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
    /// Gets a model by name.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <returns></returns>
    public Task<LLamaModel> GetModel(string modelName)
    {
        _modelInstances.TryGetValue(modelName, out var model);
        return Task.FromResult(model);
    }

    /// <summary>
    /// Gets a context from the specified model.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="contextName">The contextName.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">Model not found</exception>
    public async Task<LLamaContext> GetContext(string modelName, string contextName)
    {
        if (!_modelInstances.TryGetValue(modelName, out var model))
            throw new Exception("Model not found");

        return await model.GetContext(contextName);
    }

    /// <summary>
    /// Creates a context on the specified model.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="contextName">The contextName.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">Model not found</exception>
    public async Task<LLamaContext> CreateContext(string modelName, string contextName)
    {
        if (!_modelInstances.TryGetValue(modelName, out var model))
            throw new Exception("Model not found");

        using (await _contextLock.LockAsync())
        {
            return await model.CreateContext(contextName);
        }
    }

    /// <summary>
    /// Removes a context from the specified model.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="contextName">The contextName.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">Model not found</exception>
    public async Task<bool> RemoveContext(string modelName, string contextName)
    {
        if (!_modelInstances.TryGetValue(modelName, out var model))
            throw new Exception("Model not found");

        using (await _contextLock.LockAsync())
        {
            return await model.RemoveContext(contextName);
        }
    }

    /// <summary>
    /// Loads, gets, or creates a model and a context.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="contextName">The contextName.</param>
    /// <returns></returns>
    /// <exception cref="System.Exception">Model option '{modelName}' not found</exception>
    public async Task<(LLamaModel, LLamaContext)> GetOrCreateModelAndContext(string modelName, string contextName)
    {
        if (_modelInstances.TryGetValue(modelName, out var model))
            return (model, await model.GetContext(contextName) ?? await model.CreateContext(contextName));

        // Get model configuration.
        var modelConfig = _configuration.Models.FirstOrDefault(x => x.Name == modelName);
        if (modelConfig is null)
            throw new Exception($"Model option '{modelName}' not found");

        // Load model.
        model = await LoadModel(modelConfig);

        // Get or create context.
        return (model, await model.GetContext(contextName) ?? await model.CreateContext(contextName));
    }
}
