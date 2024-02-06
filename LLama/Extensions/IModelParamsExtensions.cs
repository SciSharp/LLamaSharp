using System.IO;
using System;
using System.Text;
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
        if (@params.UseMemoryLock && !NativeApi.llama_supports_mlock())
            throw new NotSupportedException("'UseMemoryLock' is not supported (llama_supports_mlock() == false)");
        if (@params.UseMemorymap && !NativeApi.llama_supports_mmap())
            throw new NotSupportedException("'UseMemorymap' is not supported (llama_supports_mmap() == false)");

        var disposer = new GroupDisposable();

        result = NativeApi.llama_model_default_params();
        result.main_gpu = @params.MainGpu;
        result.split_mode = @params.SplitMode;
        result.n_gpu_layers = @params.GpuLayerCount < 0 ? int.MaxValue : @params.GpuLayerCount;
        result.use_mlock = @params.UseMemoryLock;
        result.use_mmap = @params.UseMemorymap;
        result.vocab_only = @params.VocabOnly;

        unsafe
        {
            result.tensor_split = (float*)disposer.Add(@params.TensorSplits.Pin()).Pointer;
        }

        if (@params.MetadataOverrides.Count == 0)
        {
            unsafe
            {
                result.kv_overrides = (LLamaModelMetadataOverride*)IntPtr.Zero;
            }
        }
        else
        {
            // Allocate enough space for all the override items. Pin it in place so we can safely pass it to llama.cpp
            // This is one larger than necessary. The last item indicates the end of the overrides.
            var overrides = new LLamaModelMetadataOverride[@params.MetadataOverrides.Count + 1];
            unsafe
            {
                result.kv_overrides = (LLamaModelMetadataOverride*)disposer.Add(overrides.AsMemory().Pin()).Pointer;
            }

            // Convert each item
            for (var i = 0; i < @params.MetadataOverrides.Count; i++)
            {
                unsafe
                {
                    // Get the item to convert
                    var item = @params.MetadataOverrides[i];

                    // Create the "native" representation to fill in
                    var native = new LLamaModelMetadataOverride
                    {
                        Tag = item.Type
                    };

                    // Write the value into the native struct
                    item.WriteValue(ref native);

                    // Convert key chars to bytes
                    var srcSpan = item.Key.AsSpan();
                    var dstSpan = new Span<byte>(native.key, 128);
                    Encoding.UTF8.GetBytes(srcSpan, dstSpan);

                    // Store it in the array
                    overrides[i] = native;
                }
            }
        }

        return disposer;
    }
}