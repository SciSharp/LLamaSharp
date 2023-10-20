using System.IO;
using System;
using System.Buffers;
using System.Diagnostics;
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
            var maxDevices = NativeApi.llama_max_devices();
            var splits = @params.TensorSplits;
            if (splits != null)
            {
                Debug.Assert(@params.TensorSplits != null);

                // If the splits array is too large just throw
                if (splits.Length > maxDevices)
                    throw new ArgumentException($"TensorSplits size must be <= NativeApi.llama_max_devices() ({maxDevices})");

                // If the splits array is too small pad it up to the necessary size
                if (splits.Length < maxDevices)
                {
                    splits = new float[maxDevices];
                    for (var i = 0; i < @params.TensorSplits.Length; i++)
                        splits[i] = @params.TensorSplits[i];
                }
            }

            result = NativeApi.llama_model_default_params();

            result.main_gpu = @params.MainGpu;
            result.n_gpu_layers = @params.GpuLayerCount;
            result.use_mlock = @params.UseMemoryLock;
            result.use_mmap = @params.UseMemorymap;
            result.vocab_only = @params.VocabOnly;

            var pin = splits.AsMemory().Pin();
            unsafe
            {
                result.tensor_split = (float*)pin.Pointer;
            }

            return pin;
        }
    }
}
