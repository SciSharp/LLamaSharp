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
    // Model Directories
    /// <summary>
    /// Configured set of directories that are scanned to find local models
    /// </summary>
    /// <value></value>
    public IEnumerable<string> ModelDirectories { get; }

    /// <summary>
    /// Add a directory containing model files
    /// </summary>
    /// <param name="directory"></param>
    public void AddDirectory(string directory);

    /// <summary>
    /// Remove a directory from being scanned and having model files made available
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public bool RemoveDirectory(string directory);

    /// <summary>
    /// Remove all model directories
    /// </summary>
    public void RemoveAllDirectories();

    // Model Files
    /// <summary>
    /// Get all of the model files that are available to be loaded
    /// </summary>
    /// <value></value>
    public IEnumerable<ModelFileMetadata> ModelFileList { get; }

    /// <summary>
    /// Only get the models associated with a specific directory
    /// </summary>
    /// <param name="directory"></param>
    /// <returns>The files, if any associated with a given directory</returns>
    public IEnumerable<ModelFileMetadata> GetAvailableModelsFromDirectory(string directory);

    /// <summary>
    /// Get the file data for given model
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="modelMeta"></param>
    /// <returns>If a model with the given file name is present</returns>
    public bool TryGetModelFileMetadata(string fileName, out ModelFileMetadata modelMeta);

    // Model Load and Unload
    /// <summary>
    /// Load a model file to be used for infernce
    /// </summary>
    /// <param name="modelPath"></param>
    /// <param name="modelConfigurator"></param>
    /// <param name="modelId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The loaded model on success</returns>
    public Task<LLamaWeights> LoadModel(string modelPath,
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

    /// <summary>
    /// Attempt to get a model that's expected to be loaded
    /// </summary>
    /// <param name="modeId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool TryGetLoadedModel(string modeId, out LLamaWeights model);

    /// <summary>
    /// Currently loaded models
    /// </summary>
    /// <returns></returns>
    public IEnumerable<LLamaWeights> GetLoadedModels();
}
