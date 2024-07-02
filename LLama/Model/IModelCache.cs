using System;
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
    /// The caller assumes responsibility for disposing this model and <b>MUST</b> call UnloadModel
    /// </summary>
    /// <param name="metadata">The metadata about the model file to be loaded</param>
    /// <param name="modelId">A required alias to uniquely identify this model'</param>
    /// <param name="modelConfigurator">An optional function to further configure the model parameters beyond default</param>
    /// <param name="cancellationToken"></param>
    /// <returns>An instance of the newly loaded model. This MUST be disposed or Unload</returns>
    public Task<LLamaWeights> LoadModelAsync(ModelFileMetadata metadata,
        string modelId,
        Action<ModelParams>? modelConfigurator = null!,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempt to get a reference to a model that's already loaded
    /// </summary>
    /// <param name="modelId">Identifier of the loaded model</param>
    /// <param name="cachedModel">Will be populated with the reference if the model is cached</param>
    /// <returns>A <b>SHARED</b> instance to a model that's already loaded. Disposing or Unloading this model will affect all references</returns>
    public bool TryGetLoadedModel(string modelId, out LLamaWeights cachedModel);

    /// <summary>
    /// Attempt to clone and cache a new unique model instance
    /// </summary>
    /// <param name="loadedModelId">Model that's expected to be loaded and cloned</param>
    /// <param name="cloneId">Identifier for the newly cloned model</param>
    /// <param name="model">If cloning is successful, this model will be available for use</param>
    /// <returns>True if cloning is successful</returns>
    public bool TryCloneLoadedModel(string loadedModelId, string cloneId, out LLamaWeights model);

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
