using System.Collections.Concurrent;

namespace LLama.Web.Common
{
    public static class LLamaModelCache
    {
        private static readonly ConcurrentDictionary<string, LLamaModel> _modelInstances = new ConcurrentDictionary<string, LLamaModel>();

        public static LLamaModel GetOrCreate(ModelOptions modelOptions)
        {
            if (_modelInstances.TryGetValue(modelOptions.Name, out LLamaModel model))
                return model;

            var modelInstance = new LLamaModel(modelOptions);
            if (!_modelInstances.TryAdd(modelOptions.Name, modelInstance))
                throw new Exception($"Failed to cache model {modelOptions.Name}.");

            return modelInstance;
        }


        public static bool Remove(string modelName)
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
