using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LLama.Common;
using LLama.Native;

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
    private readonly Dictionary<string, CachedModelReference> _loadedModelCache = [];

    /// <inheritdoc />
    public int ModelsCached()
        => _loadedModelCache.Count;

    /// <inheritdoc />
    public bool TryCloneLoadedModel(string modelId, out LLamaWeights model)
    {
        var isCached = _loadedModelCache.TryGetValue(modelId, out var cachedModel);

        model = null!;
        if (isCached)
        {
            model = cachedModel.Model.CloneFromHandleWithMetadata();
            cachedModel.RefCount++;
        }
        return isCached;
    }

    /// <inheritdoc />
    public async Task<LLamaWeights> LoadModelAsync(ModelFileMetadata metadata,
        Action<ModelParams>? modelConfigurator = null!,
        string modelId = "",
        CancellationToken cancellationToken = default)
    {
        // is the model already loaded? alias could be different but it's up to the caller to be consistent
        if (!string.IsNullOrEmpty(modelId)
            && TryCloneLoadedModel(modelId, out var loadedModel))
        {
            return loadedModel;
        }

        // Configure model params
        var modelParams = new ModelParams(metadata.ModelFileUri);
        modelConfigurator?.Invoke(modelParams);

        // load and cache
        var model = await LLamaWeights.LoadFromFileAsync(modelParams, cancellationToken);

        // Check if it's already cached, if so use that and dispose of this
        // TODO: Consider the case where the alias is different but the underlying model file is the same
        if (string.IsNullOrWhiteSpace(modelId))
        {
            modelId = model.ModelName;

            if (TryCloneLoadedModel(modelId, out loadedModel))
            {
                model.Dispose();
                return loadedModel;
            }
        }

        _loadedModelCache.Add(modelId, new CachedModelReference
        {
            Model = model,
            RefCount = 1
        });
        return model;
    }

    #region Unload
    /// <inheritdoc />
    public bool UnloadModel(string modelId)
    {
        if (_loadedModelCache.TryGetValue(modelId, out var cachedModel))
        {
            // Decrement refcount on model
            cachedModel.Model.Dispose(); // this only disposes the original model...
            cachedModel.RefCount--;
            if (cachedModel.RefCount == 0)
            {
                return _loadedModelCache.Remove(modelId);
            }
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public void UnloadAllModels()
    {
        foreach (var handle in _loadedModelCache.Values)
        {
            for (var i = 0; i < handle.RefCount; i++)
            {
                handle.Model.Dispose();
            }
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
