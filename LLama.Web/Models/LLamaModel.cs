using LLama.Abstractions;
using LLama.Web.Common;
using System.Collections.Concurrent;

namespace LLama.Web.Models;

/// <summary>
/// Wrapper class for LLamaSharp LLamaWeights
/// </summary>
/// <seealso cref="IDisposable" />
public class LLamaModel : IDisposable
{
    private readonly ILogger _llamaLogger;
    private readonly ModelOptions _config;
    private readonly LLamaWeights _weights;
    private readonly ConcurrentDictionary<string, LLamaContext> _contexts;

    /// <summary>
    /// Use the Create method to instantiate this class.
    /// </summary>
    /// <param name="modelParams">The model parameters.</param>
    /// <param name="llamaLogger">A logger class.</param>
    /// <param name="weights">Model weights.</param>
    private LLamaModel(ModelOptions modelParams, ILogger llamaLogger, LLamaWeights weights)
    {
        _config = modelParams;
        _llamaLogger = llamaLogger;
        _weights = weights;
        _contexts = new ConcurrentDictionary<string, LLamaContext>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LLamaModel"/> class.
    /// </summary>
    /// <param name="modelParams">The model parameters.</param>
    /// <param name="llamaLogger">A logger class.</param>
    public static async Task<LLamaModel> CreateAsync(ModelOptions modelParams, ILogger llamaLogger)
    {
        var weights = await LLamaWeights.LoadFromFileAsync(modelParams);
        return new LLamaModel(modelParams, llamaLogger, weights);
    }

    /// <summary>
    /// Gets the model configuration.
    /// </summary>
    public IModelParams ModelParams => _config;

    /// <summary>
    /// Gets the LLamaWeights
    /// </summary>
    public LLamaWeights LLamaWeights => _weights;

    /// <summary>
    /// Gets the context count.
    /// </summary>
    public int ContextCount => _contexts.Count;

    /// <summary>
    /// Creates a new context session on this model
    /// </summary>
    /// <param name="contextName">The unique context identifier</param>
    /// <returns>LLamaModelContext for this LLamaModel</returns>
    /// <exception cref="Exception">Context exists</exception>
    public Task<LLamaContext> CreateContext(string contextName)
    {
        if (_contexts.TryGetValue(contextName, out _))
        {
            throw new Exception($"Context with id {contextName} already exists.");
        }

        if (_config.MaxInstances > 0 && ContextCount >= _config.MaxInstances)
        {
            throw new Exception($"Maximum model instances reached");
        }

        var context = _weights.CreateContext(_config, _llamaLogger);
        if (_contexts.TryAdd(contextName, context))
        {
            return Task.FromResult(context);
        }

        return Task.FromResult<LLamaContext>(null);
    }

    /// <summary>
    /// Get a contexts belonging to this model
    /// </summary>
    /// <param name="contextName">The unique context identifier</param>
    /// <returns>LLamaModelContext for this LLamaModel with the specified contextName</returns>
    public Task<LLamaContext> GetContext(string contextName)
    {
        if (_contexts.TryGetValue(contextName, out var context))
            return Task.FromResult(context);

        return Task.FromResult<LLamaContext>(null);
    }

    /// <summary>
    /// Remove a context from this model
    /// </summary>
    /// <param name="contextName">The unique context identifier</param>
    /// <returns>true if removed, otherwise false</returns>
    public Task<bool> RemoveContext(string contextName)
    {
        if (!_contexts.TryRemove(contextName, out var context))
            return Task.FromResult(false);

        context?.Dispose();
        return Task.FromResult(true);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        foreach (var context in _contexts.Values)
        {
            context?.Dispose();
        }
        _weights.Dispose();
    }
}
