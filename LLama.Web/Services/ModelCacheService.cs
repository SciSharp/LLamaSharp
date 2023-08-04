using System.Collections.Concurrent;
using LLama.Web.Services;

namespace LLama.Web.Common
{
    public class ModelCacheService : IModelCacheService
    {
        private readonly ConcurrentDictionary<string, LLamaModel> _modelInstances = new ConcurrentDictionary<string, LLamaModel>();

        public Task<LLamaModel> Create(ModelOptions modelOptions)
        {
            if (_modelInstances.TryGetValue(modelOptions.Name, out LLamaModel model))
                return Task.FromResult(model);

            model = new LLamaModel(modelOptions);
            if (!_modelInstances.TryAdd(modelOptions.Name, model))
                throw new Exception($"Failed to cache model {modelOptions.Name}.");

            return Task.FromResult(model);
        }


        public Task<LLamaModel> Get(string modelName)
        {
            _modelInstances.TryGetValue(modelName, out LLamaModel model);
            return Task.FromResult(model);
        }


        public Task<bool> Remove(string modelName)
        {
            if (_modelInstances.TryRemove(modelName, out LLamaModel model))
            {
                model?.Dispose();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
