using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LLama.Common;

namespace LLama.Model;

internal class CachedModelReference
{
    public LLamaWeights Model { get; init; } = null!;
    public int RefCount { get; set; } = 0;
}

/// <inheritdoc />
public class ModelCache : IModelCache
{
    private bool _disposed = false;

    // model id/alias, to loaded model
    private readonly Dictionary<string, LLamaWeights> _loadedModelCache = [];

    /// <inheritdoc />
    public int ModelsCached()
        => _loadedModelCache.Count;

    /// <inheritdoc />
    public bool TryCloneLoadedModel(string loadedModelId,
        string cloneId,
        out LLamaWeights model)
    {
        var isCached = _loadedModelCache.TryGetValue(loadedModelId, out var cachedModel);

        model = null!;
        if (isCached)
        {
            TryAddModel(cloneId, cachedModel.CloneFromHandleWithMetadata);
            model = _loadedModelCache[loadedModelId];
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public bool TryGetLoadedModel(string modelId, out LLamaWeights cachedModel)
    {
        return _loadedModelCache.TryGetValue(modelId, out cachedModel);
    }

    private void TryAddModel(string modelId, Func<LLamaWeights> modelCreator)
    {
        if (IsModelIdInvalid(modelId))
        {
            throw new ArgumentException("Model identifier is not unique");
        }

        _loadedModelCache.Add(modelId, modelCreator());
    }

    private async Task TryAddModelAsync(string modelId, Func<Task<LLamaWeights>> modelCreator)
    {
        if (IsModelIdInvalid(modelId))
        {
            throw new ArgumentException("Model identifier is not unique");
        }

        _loadedModelCache.Add(modelId, await modelCreator());
    }

    private bool IsModelIdInvalid(string modelId) =>
        string.IsNullOrWhiteSpace(modelId) || _loadedModelCache.ContainsKey(modelId);

    /// <inheritdoc />
    public async Task<LLamaWeights> LoadModelAsync(ModelFileMetadata metadata,
        string modelId,
        Action<ModelParams>? modelConfigurator = null!,
        CancellationToken cancellationToken = default)
    {
        await TryAddModelAsync(modelId, async () =>
        {
            return await ModelCreator(metadata.ModelFileUri, modelConfigurator, cancellationToken);
        });
        return _loadedModelCache[modelId];

        // Helper to create the model
        static async Task<LLamaWeights> ModelCreator(string fileUri,
            Action<ModelParams>? modelConfigurator,
            CancellationToken cancellationToken)
        {
            var modelParams = new ModelParams(fileUri);
            modelConfigurator?.Invoke(modelParams);

            return await LLamaWeights.LoadFromFileAsync(modelParams, cancellationToken);
        }
    }

    #region Unload
    /// <inheritdoc />
    public bool UnloadModel(string modelId)
    {
        if (_loadedModelCache.TryGetValue(modelId, out var cachedModel))
        {
            cachedModel.Dispose();
            return _loadedModelCache.Remove(modelId);
        }
        return false;
    }

    /// <inheritdoc />
    public void UnloadAllModels()
    {
        foreach (var model in _loadedModelCache.Values)
        {
            model.Dispose();
        }
        _loadedModelCache.Clear();
    }
    #endregion

    #region Dispose
    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Unload all models when called explicitly via dispose
    /// </summary>
    /// <param name="disposing">Whether or not this call is made explicitly(true) or via GC</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            UnloadAllModels();
        }

        _disposed = true;
    }
    #endregion
}
