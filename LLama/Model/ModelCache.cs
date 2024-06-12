using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LLama.Common;

namespace LLama.Model;

/// <summary>
/// Types of supported model files
/// </summary>
public enum ModelFileType
{
    GGUF
}

/// <summary>
/// Metadata about available models
/// </summary>
public class ModelFileMetadata
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public string FileName { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
    public ModelFileType ModelType { get; init; }
    public long SizeInBytes { get; init; } = 0;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

/// <summary>
/// A class that helps organize and load local models
/// </summary>
public interface IModelCache
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

/// <inheritdoc />
public class ModelCache : IModelCache
{
    /// <summary>
    /// Support model type files
    /// </summary>
    public static readonly string[] ExpectedModelFileTypes = [
        ".gguf"
    ];

    // keys are directories, values are applicable models
    private readonly Dictionary<string, IEnumerable<ModelFileMetadata>> _availableModels = [];

    // model id/alias, to loaded model
    private readonly Dictionary<string, LLamaWeights> _loadedModelCache = [];

    /// <summary>
    /// Create a new model manager that seeds available models from the given directory list
    /// </summary>
    /// <param name="directories"></param>
    public ModelCache(string[] directories)
    {
        GetModelsFromDirectories(directories);
    }

    private void GetModelsFromDirectories(params string[] dirs)
    {
        foreach (var dir in dirs)
        {
            var fullDirectoryPath = Path.GetFullPath(dir);

            if (!Directory.Exists(fullDirectoryPath))
            {
                Trace.TraceError($"Model directory '{fullDirectoryPath}' does not exist");
                continue;
            }

            if (_availableModels.ContainsKey(fullDirectoryPath))
            {
                Trace.TraceWarning($"Model directory '{fullDirectoryPath}' already probed");
                continue;
            }

            // find models in current dir that are of expected type
            List<ModelFileMetadata> directoryModelFiles = [];
            foreach (var file in Directory.EnumerateFiles(fullDirectoryPath))
            {
                if (!ExpectedModelFileTypes.Contains(Path.GetExtension(file)))
                {
                    continue;
                }

                // expected model file
                var fi = new FileInfo(file);
                directoryModelFiles.Add(new ModelFileMetadata
                {
                    FileName = fi.Name,
                    FilePath = fi.FullName,
                    ModelType = ModelFileType.GGUF,
                    SizeInBytes = fi.Length,
                });
            }

            _availableModels.Add(fullDirectoryPath, directoryModelFiles);
        }
    }

    /// <inheritdoc />
    public IEnumerable<ModelFileMetadata> ModelFileList
        => _availableModels.SelectMany(x => x.Value);
    /// <inheritdoc />
    public IEnumerable<string> ModelDirectories
        => _availableModels.Keys;

    /// <inheritdoc />
    public void AddDirectory(string directory)
    {
        GetModelsFromDirectories(directory);
    }

    /// <inheritdoc />
    public bool RemoveDirectory(string directory)
    {
        return _availableModels.Remove(Path.GetFullPath(directory));
    }

    /// <inheritdoc />
    public void RemoveAllDirectories()
    {
        _availableModels.Clear();
    }

    /// <inheritdoc />
    public IEnumerable<ModelFileMetadata> GetAvailableModelsFromDirectory(string directory)
    {
        var dirPath = Path.GetFullPath(directory);
        return _availableModels.TryGetValue(dirPath, out var dirModels)
            ? dirModels
            : [];
    }

    /// <inheritdoc />
    public bool TryGetModelFileMetadata(string fileName, out ModelFileMetadata modelMeta)
    {
        var filePath = Path.GetFullPath(fileName);
        modelMeta = ModelFileList.FirstOrDefault(f => f.FilePath == filePath)!;
        return modelMeta != null;
    }

    /// <inheritdoc />
    public IEnumerable<LLamaWeights> GetLoadedModels()
    {
        return _loadedModelCache.Values;
    }

    /// <inheritdoc />
    public bool TryGetLoadedModel(string modelId, out LLamaWeights model)
    {
        var isCached = _loadedModelCache.TryGetValue(modelId, out model!);

        // Externall disposed, act like it's not in here
        if (isCached && model.NativeHandle.IsClosed)
        {
            _ = _loadedModelCache.Remove(modelId);
            isCached = false;
            model = null!;
        }

        return isCached;
    }

    /// <inheritdoc />
    public async Task<LLamaWeights> LoadModel(string modelPath,
        Action<ModelParams>? modelConfigurator = null!,
        string modelId = "",
        CancellationToken cancellationToken = default)
    {
        // is the model already loaded? alias could be different but it's up to the caller to be consistent
        if (!string.IsNullOrEmpty(modelId)
            && TryGetLoadedModel(modelId, out var loadedModel))
        {
            Trace.TraceWarning($"Model {modelId} already loaded");
            return loadedModel;
        }

        // Configure model params
        var modelParams = new ModelParams(modelPath);
        modelConfigurator?.Invoke(modelParams);

        // load and cache
        var model = await LLamaWeights.LoadFromFileAsync(modelParams, cancellationToken);
        if (string.IsNullOrWhiteSpace(modelId))
        {
            modelId = model.ModelName;
            
            // Check if cached again with alias
            // TODO: Consider the case where the alias is different but the underlying model file is the same
            if (TryGetLoadedModel(modelId, out loadedModel))
            {
                model.Dispose();
                return loadedModel;
            }
        }
        _loadedModelCache.Add(modelId, model);
        return model;
    }

    /// <inheritdoc />
    public bool UnloadModel(string modelId)
    {
        if (TryGetLoadedModel(modelId, out var model))
        {
            model.Dispose();
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
}
