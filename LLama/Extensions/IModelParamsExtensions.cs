using System.IO;
using System;
using System.Buffers;
using LLama.Abstractions;
using LLama.Native;

namespace LLama.Extensions
{
    /// <summary>
    /// Extention methods to the IModelParams interface
    /// </summary>
    public static class IModelParamsExtensions
    {
        /// <summary>
        /// Convert the given `IModelParams` into a `LLamaModelParams`
        /// </summary>
        /// <param name="params"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static MemoryHandle ToLlamaModelParams(this IModelParams @params, out LLamaModelParams result)
        {
            if (@params.TensorSplits != null && @params.TensorSplits.Length != 1)
                throw new ArgumentException("Currently multi-gpu support is not supported by both llama.cpp and LLamaSharp.");

            result = NativeApi.llama_model_default_params();

            result.main_gpu = @params.MainGpu;
            result.n_gpu_layers = @params.GpuLayerCount;
            result.use_mlock = @params.UseMemoryLock;
            result.use_mmap = @params.UseMemorymap;
            result.vocab_only = @params.VocabOnly;

            var pin = @params.TensorSplits.AsMemory().Pin();
            unsafe
            {
                result.tensor_split = (float*)pin.Pointer;
            }

            return pin;
        }
    }
}
