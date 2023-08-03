using System.Collections.Concurrent;
using LLama.Web.Services;

namespace LLama.Web.Common
{
    public class ModelCacheService : IModelCacheService
    {
        private readonly ConcurrentDictionary<string, LLamaModel> _modelInstances = new ConcurrentDictionary<string, LLamaModel>();

        public LLamaModel GetOrCreate(ModelOptions modelOptions)
        {
            if (_modelInstances.TryGetValue(modelOptions.Name, out LLamaModel model))
                return model;

            var modelInstance = new LLamaModel(modelOptions);
            if (!_modelInstances.TryAdd(modelOptions.Name, modelInstance))
                throw new Exception($"Failed to cache model {modelOptions.Name}.");

            return modelInstance;
        }


        public bool Remove(string modelName)
        {
            // This is pretty brutal as there could be multiple contexts, revisit this ASAP
            if (_modelInstances.TryRemove(modelName, out LLamaModel model))
            {
                model?.Dispose();
                return true;
            }
            return false;
        }
    }
}
