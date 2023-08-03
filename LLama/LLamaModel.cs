using LLama.Common;
using LLama.Native;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LLama
{
    /// <summary>
    /// The abstraction of a LLama model, which holds the context in the native library.
    /// </summary>
    public class LLamaModel : IDisposable
    {
        // TODO: expose more properties.
        ILLamaLogger? _logger;
        SafeLlamaModelHandle _model;
        ConcurrentDictionary<string, LLamaModelContext> _contexts;
        /// <summary>
        /// The model params set for this model.
        /// </summary>
        public ModelParams Params { get; set; }
        /// <summary>
        /// The native handle, which is used to be passed to the native APIs. Please avoid using it 
        /// unless you know what is the usage of the Native API.
        /// </summary>
        public SafeLlamaModelHandle NativeHandle => _model;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelParams">Model params.</param>
        /// <param name="logger">The logger.</param>
        public LLamaModel(ModelParams modelParams, ILLamaLogger? logger = null)
        {
            _logger = logger;
            _contexts = new ConcurrentDictionary<string, LLamaModelContext>();
            _logger?.Log(nameof(LLamaModelContext), $"Initializing LLama model with params: {modelParams}", ILLamaLogger.LogLevel.Info);

            Params = modelParams;
            var contextParams = Utils.CreateContextParams(modelParams);
            _model = SafeLlamaModelHandle.LoadFromFile(modelParams.ModelPath, contextParams);
            if (!string.IsNullOrEmpty(modelParams.LoraAdapter))
                _model.ApplyLoraFromFile(modelParams.LoraAdapter, modelParams.LoraBase, modelParams.Threads);
        }

        /// <summary>
        /// Creates a new context session on this model
        /// </summary>
        /// <param name="contextId">The unique context identifier</param>
        /// <param name="encoding">The contexts text encoding</param>
        /// <returns>LLamaModelContext for this LLamaModel</returns>
        /// <exception cref="Exception">Context exists</exception>
        public Task<LLamaModelContext> CreateContext(string contextId, string encoding = "UTF-8")
        {
            if (_contexts.TryGetValue(contextId, out var context))
                throw new Exception($"Context with id {contextId} already exists.");

            context = new LLamaModelContext(this, encoding, _logger);
            if (_contexts.TryAdd(contextId, context))
                return Task.FromResult(context);

            return Task.FromResult<LLamaModelContext>(null);
        }

        /// <summary>
        /// Get a contexts belonging to this model
        /// </summary>
        /// <param name="contextId">The unique context identifier</param>
        /// <returns>LLamaModelContext for this LLamaModel with the specified contextId</returns>
        public Task<LLamaModelContext> GetContext(string contextId)
        {
            if (_contexts.TryGetValue(contextId, out var context))
                return Task.FromResult(context);

            return Task.FromResult<LLamaModelContext>(null);
        }

        /// <summary>
        /// Remove a context from this model
        /// </summary>
        /// <param name="contextId">The unique context identifier</param>
        /// <returns>true if removed, otherwise false</returns>
        public Task<bool> RemoveContext(string contextId)
        {
            if (!_contexts.TryRemove(contextId, out var context))
                return Task.FromResult(false);

            context?.Dispose();
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            foreach (var context in _contexts.Values)
            {
                context?.Dispose();
            }
            _model.Dispose();
        }
    }
}
