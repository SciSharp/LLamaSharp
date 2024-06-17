using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LLama.Common;

namespace LLama.Model;

/// <summary>
/// A class that helps organize and load local models
/// </summary>
public interface IModelCache : IDisposable
{
    /// <summary>
    /// The current number of file handles in cache.
    /// </summary>
    /// <returns>Number of cached models</returns>
    public int ModelsCached();

    /// <summary>
    /// Load a model file to be used for inference.
    /// The caller assumes responsibility for disposing this model and <b>MUST</b> call Unload
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="modelConfigurator"></param>
    /// <param name="modelId">An alias to uniquely identify this model's underlying handle. If none is supplied, the model's name is used.'</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The loaded model on success</returns>
    public Task<LLamaWeights> LoadModelAsync(ModelFileMetadata metadata,
        Action<ModelParams>? modelConfigurator = null!,
        string modelId = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unload and dispose of a model with the given id
    /// </summary>
    /// <param name="modelId"></param>
    /// <returns></returns>
    public bool UnloadModel(string modelId);

    /// <summary>
    /// Unload all currently loaded models
    /// </summary>
    public void UnloadAllModels();
}
