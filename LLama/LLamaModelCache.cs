using LLama.Common;
using LLama.Exceptions;
using LLama.Native;
using System.Collections.Concurrent;

namespace LLama
{
    /// <summary>
    /// Placeholder for now
    /// TODO: Handle model collections
    /// </summary>
    public static class LLamaModelCache
    {
        private static readonly ConcurrentDictionary<string, SafeLlamaModelHandle> _modelInstances = new ConcurrentDictionary<string, SafeLlamaModelHandle>();

        /// <summary>
        /// Basic cache by filename to allow multile contexts to use the same model/resources
        /// </summary>
        /// <param name="modelParams">The models parameters</param>
        /// <returns>Exiting model instance if it exists, otherwise a new model is created and returned</returns>
        /// <exception cref="RuntimeError"></exception>
        public static SafeLlamaModelHandle GetOrCreate(ModelParams modelParams)
        {
            if (_modelInstances.TryGetValue(modelParams.ModelPath, out SafeLlamaModelHandle model))
                return model;

            var contextParams = Utils.CreateContextParams(modelParams);
            var modelInstance = SafeLlamaModelHandle.LoadFromFile(modelParams.ModelPath, contextParams);
            if (!string.IsNullOrEmpty(modelParams.LoraAdapter))
                modelInstance.ApplyLoraFromFile(modelParams.LoraAdapter, modelParams.LoraBase, modelParams.Threads);

            if (!_modelInstances.TryAdd(modelParams.ModelPath, modelInstance))
                throw new RuntimeError($"Failed to cache model {modelParams.ModelPath}.");

            return modelInstance;
        }


        public static bool Remove(string modelPath)
        {
            // This is pretty brutal as there could be multiple contexts, revisit this ASAP
            if (_modelInstances.TryRemove(modelPath, out SafeLlamaModelHandle model))
            {
                model?.Dispose();
                return true;
            }
            return false;
        }
    }
}
