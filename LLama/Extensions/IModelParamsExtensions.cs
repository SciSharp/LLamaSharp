using System.IO;
using System;
using System.Buffers;
using LLama.Abstractions;
using LLama.Native;

namespace LLama.Extensions;

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
    public static IDisposable ToLlamaModelParams(this IModelParams @params, out LLamaModelParams result)
    {
        var disposer = new GroupDisposable();

        result = NativeApi.llama_model_default_params();

        result.main_gpu = @params.MainGpu;
        result.n_gpu_layers = @params.GpuLayerCount;
        result.use_mlock = @params.UseMemoryLock;
        result.use_mmap = @params.UseMemorymap;
        result.vocab_only = @params.VocabOnly;

        unsafe
        {
            result.tensor_split = (float*)disposer.Add(@params.TensorSplits.Pin()).Pointer;
        }

        //todo: MetadataOverrides
        //if (@params.MetadataOverrides.Count == 0)
        //{
        //    unsafe
        //    {
        //        result.kv_overrides = (LLamaModelMetadataOverride*)IntPtr.Zero;
        //    }
        //}
        //else
        //{
        //    throw new NotImplementedException("MetadataOverrides");
        //}

        return disposer;
    }
}