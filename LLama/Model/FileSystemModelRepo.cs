using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LLama.Model;

/// <summary>
/// A model repository that uses a file system to search for available models
/// </summary>
public class FileSystemModelRepo : IModelSourceRepo
{
    /// <summary>
    /// Support model type files
    /// </summary>
    public static readonly string[] ExpectedModelFileTypes = [
        ".gguf"
    ];

    // keys are directories, values are applicable models
    private readonly Dictionary<string, IEnumerable<ModelFileMetadata>> _availableModels = [];

    /// <summary>
    /// Create a model repo that scans the filesystem to find models
    /// </summary>
    /// <param name="directories"></param>
    public FileSystemModelRepo(string[] directories)
    {
        GetModelsFromDirectories(directories);
    }

    #region Sources
    /// <inheritdoc />
    public IEnumerable<string> ListSources() => _availableModels.Keys;

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
                // TODO: handle symbolic links
                var fi = new FileInfo(file);
                directoryModelFiles.Add(new ModelFileMetadata
                {
                    ModelFileName = fi.Name,
                    ModelFileUri = fi.FullName,
                    ModelType = ModelFileType.GGUF,
                    ModelFileSizeInBytes = fi.Length,
                });
            }

            _availableModels.Add(fullDirectoryPath, directoryModelFiles);
        }
    }

    /// <inheritdoc />
    public void AddSource(string directory)
    {
        GetModelsFromDirectories(directory);
    }

    /// <inheritdoc />
    public bool RemoveSource(string directory)
    {
        return _availableModels.Remove(Path.GetFullPath(directory));
    }

    /// <inheritdoc />
    public void RemoveAllSources()
    {
        _availableModels.Clear();
    }
    #endregion Sources

    /// <inheritdoc />
    public IEnumerable<ModelFileMetadata> GetAvailableModels()
        => _availableModels.SelectMany(x => x.Value);

    /// <inheritdoc />
    public IEnumerable<ModelFileMetadata> GetAvailableModelsFromSource(string directory)
    {
        var dirPath = Path.GetFullPath(directory);
        return _availableModels.TryGetValue(dirPath, out var dirModels)
            ? dirModels
            : [];
    }

    /// <inheritdoc />
    public bool TryGetModelFileMetadata(string modelFileName, out ModelFileMetadata modelMeta)
    {
        var filePath = Path.GetFullPath(modelFileName);
        modelMeta = GetAvailableModels().FirstOrDefault(f => f.ModelFileUri == filePath)!;
        return modelMeta != null;
    }
}
