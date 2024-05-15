using LLama.Web.Common;
using LLama.Web.Models;

namespace LLama.Web.Services;

/// <summary>
/// Service for managing language Models
/// </summary>
public interface IModelService
{
    /// <summary>
    /// Gets the model with the specified name.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    Task<LLamaModel> GetModel(string modelName);

    /// <summary>
    /// Loads a model from a ModelConfig object.
    /// </summary>
    /// <param name="modelOptions">The model configuration.</param>
    Task<LLamaModel> LoadModel(ModelOptions modelOptions);

    /// <summary>
    /// Loads all models found in appsettings.json
    /// </summary>
    Task LoadModels();

    /// <summary>
    /// Unloads the model with the specified name.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    Task UnloadModel(string modelName);

    /// <summary>
    /// Unloads all models.
    /// </summary>
    Task UnloadModels();

    /// <summary>
    /// Gets a context with the specified identifier
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="contextName">The context identifier.</param>
    Task<LLamaContext> GetContext(string modelName, string contextName);

    /// <summary>
    /// Removes the context.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="contextName">The context identifier.</param>
    Task<bool> RemoveContext(string modelName, string contextName);

    /// <summary>
    /// Creates a context.
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="contextName">The context identifier.</param>
    Task<LLamaContext> CreateContext(string modelName, string contextName);

    /// <summary>
    /// Gets the or create model and context.
    /// This will load a model from disk if not already loaded, and also create the context
    /// </summary>
    /// <param name="modelName">Name of the model.</param>
    /// <param name="contextName">The context identifier.</param>
    /// <returns>Both loaded Model and Context</returns>
    Task<(LLamaModel, LLamaContext)> GetOrCreateModelAndContext(string modelName, string contextName);
}