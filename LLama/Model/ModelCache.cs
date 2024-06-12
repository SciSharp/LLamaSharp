using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LLama.Common;

namespace LLama.Model;

/// <inheritdoc />
public class ModelCache : IModelCache
{
    private bool _disposed = false;

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

    #region Dispose
    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Unload all models when called explicity via dispose
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
