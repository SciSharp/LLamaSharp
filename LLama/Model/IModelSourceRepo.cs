using System.Collections.Generic;

namespace LLama.Model;

/// <summary>
/// A source for models
/// </summary>
public interface IModelSourceRepo
{
    #region Source
    /// <summary>
    /// Configured set of sources that are scanned to find models
    /// </summary>
    /// <value></value>
    public IEnumerable<string> ListSources();

    /// <summary>
    /// Add a source containing one or more files
    /// </summary>
    /// <param name="source"></param>
    public void AddSource(string source);

    /// <summary>
    /// Remove a source from being scanned and having model files made available
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public bool RemoveSource(string source);

    /// <summary>
    /// Remove all model directories
    /// </summary>
    public void RemoveAllSources();
    #endregion

    #region AvailableModels
    /// <summary>
    /// Get all of the model files that are available to be loaded
    /// </summary>
    /// <value></value>
    public IEnumerable<ModelFileMetadata> GetAvailableModels();

    /// <summary>
    /// Only get the models associated with a specific source
    /// </summary>
    /// <param name="source"></param>
    /// <returns>The files, if any associated with a given source</returns>
    public IEnumerable<ModelFileMetadata> GetAvailableModelsFromSource(string source);

    /// <summary>
    /// Get the file data for given model
    /// </summary>
    /// <param name="modelFileName"></param>
    /// <param name="modelMeta"></param>
    /// <returns>If a model with the given file name is present</returns>
    public bool TryGetModelFileMetadata(string modelFileName, out ModelFileMetadata modelMeta);
    #endregion
}
