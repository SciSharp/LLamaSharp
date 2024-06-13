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
    private readonly Dictionary<string, SafeLlamaModelHandle> _loadedModelCache = [];

    /// <summary>
    /// Create a new model manager that seeds available models from the given directory list
    /// </summary>
    /// <param name="directories"></param>
    public ModelCache(string[] directories)
    {
        GetModelsFromDirectories(directories);
    }

    /// <inheritdoc />
    public IEnumerable<ModelFileMetadata> ModelFileList
        => _availableModels.SelectMany(x => x.Value);

    /// <inheritdoc />
    public IEnumerable<string> ModelDirectories
        => _availableModels.Keys;

    #region Directories
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
    #endregion Directories

    /// <inheritdoc />
    public bool TryGetModelFileMetadata(string fileName, out ModelFileMetadata modelMeta)
    {
        var filePath = Path.GetFullPath(fileName);
        modelMeta = ModelFileList.FirstOrDefault(f => f.FilePath == filePath)!;
        return modelMeta != null;
    }

    /// <inheritdoc />
    public bool TryGetLoadedModel(string modelId, out LLamaWeights model)
    {
        var isCached = _loadedModelCache.TryGetValue(modelId, out var handle);
        model = isCached
            ? LLamaWeights.FromSafeModelHandle(handle)
            : null!;
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
            return loadedModel;
        }

        // Configure model params
        var modelParams = new ModelParams(modelPath);
        modelConfigurator?.Invoke(modelParams);

        // load and cache
        var model = await LLamaWeights.LoadFromFileAsync(modelParams, cancellationToken);

        // Check if it's already cached, if so use that and dispose of this
        // TODO: Consider the case where the alias is different but the underlying model file is the same
        if (string.IsNullOrWhiteSpace(modelId))
        {
            modelId = model.ModelName;

            if (TryGetLoadedModel(modelId, out loadedModel))
            {
                model.Dispose();
                return loadedModel;
            }
        }

        // Increment the model reference count while this model exists (newly created)
        // DangerousAddRef throws if it fails, so there is no need to check "success"
        // Do this here since we're passing this to the caller to own and it's not done as part of the normal weight creation
        var refSuccess = false;
        model.NativeHandle.DangerousAddRef(ref refSuccess);

        _loadedModelCache.Add(modelId, model.NativeHandle);
        return model;
    }

    #region Unload
    /// <inheritdoc />
    public bool UnloadModel(string modelId)
    {
        if (_loadedModelCache.TryGetValue(modelId, out var handle))
        {
            // Decrement refcount on model
            handle.DangerousRelease();
            handle.Dispose();
            if (handle.IsClosed || handle.IsInvalid)
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
            handle.DangerousRelease();
            handle.Dispose();
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
