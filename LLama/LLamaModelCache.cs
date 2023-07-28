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
        /// <param name="modelPath">Path to the model bin</param>
        /// <param name="contextParams">The models context parameters</param>
        /// <returns>Exiting model instance if it exists, otherwise a new model is created and returned</returns>
        /// <exception cref="RuntimeError"></exception>
        public static SafeLlamaModelHandle GetOrCreate(string modelPath, LLamaContextParams contextParams)
        {
            if (_modelInstances.TryGetValue(modelPath, out SafeLlamaModelHandle model))
                return model;

            var modelInstance = SafeLlamaModelHandle.LoadFromFile(modelPath, contextParams);
            if (!_modelInstances.TryAdd(modelPath, modelInstance))
                throw new RuntimeError($"Failed to cache model {modelPath}.");

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
