using System.IO;
using System;
using System.Text;
using LLama.Abstractions;
using LLama.Native;
using System.Collections.Generic;

namespace LLama.Extensions;

/// <summary>
/// Extension methods to the IModelParams interface
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

        result = LLamaModelParams.Default();

        result.main_gpu = @params.MainGpu;
        result.n_gpu_layers = @params.GpuLayerCount < 0 ? int.MaxValue : @params.GpuLayerCount;
        if (@params.SplitMode.HasValue)
            result.split_mode = @params.SplitMode.Value;

        result.use_mlock = @params.UseMemoryLock;
        result.use_mmap = @params.UseMemorymap;
        result.use_direct_io = @params.UseDirectIO;
        result.vocab_only = @params.VocabOnly;
        result.check_tensors = @params.CheckTensors;

        unsafe
        {
            result.tensor_split = (float*)disposer.Add(@params.TensorSplits.Pin()).Pointer;
        }

        // Add tensor buffer overrides
        unsafe
        {
            result.tensor_buft_overrides = ConvertOverrides(@params.TensorBufferOverrides, disposer);
        }

        // Add metadata overrides
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

    /// <summary>
    /// Get a map from name of device (`ggml_backend_buft_name`) to the device type (`ggml_backend_dev_buffer_type`)
    /// </summary>
    /// <returns>Dictionary mapping buffer type names to their handles</returns>
    private static IReadOnlyDictionary<string, IntPtr> GetAvailableBufferTypes()
    {
        var result = new Dictionary<string, IntPtr>();

        var count = NativeApi.ggml_backend_dev_count();
        for (nuint i = 0; i < count; i++)
        {
            var dev = NativeApi.ggml_backend_dev_get(i);
            var buft = NativeApi.ggml_backend_dev_buffer_type(dev);

            var name = NativeApi.ggml_backend_buft_name(buft).PtrToString();
            if (string.IsNullOrEmpty(name))
                continue;

            result[name] = buft;
        }

        return result;
    }

    private static unsafe LLamaModelTensorBufferOverride* ConvertOverrides(List<TensorBufferOverride> overrides, GroupDisposable disposer)
    {
        // Early out if there are no overrides
        if (overrides.Count == 0)
            return null;

        var bufferTypes = GetAvailableBufferTypes();

        var overridesCount = 0;
        var overridesArray = new LLamaModelTensorBufferOverride[overrides.Count + 1];

        foreach (var @override in overrides)
        {
            // Check if we have this buffer type
            if (!bufferTypes.TryGetValue(@override.BufferType, out var bufferType))
                continue;

            // Create null terminated string and pin this memory so it can be passed to native code
            var patternBytes = Encoding.UTF8.GetBytes(@override.Pattern + "\0");
            var patternPin = patternBytes.AsMemory().Pin();
            disposer.Add(patternPin);

            // Add the item to the overridesArray
            overridesArray[overridesCount++] = new()
            {
                Pattern = (byte*)patternPin.Pointer,
                BufferType = bufferType
            };
        }

        // Early out if there were no valid overrides
        if (overridesCount == 0)
            return null;

        // Pin it so it can be safely passed across to native code
        var overrideArrayPin = overridesArray.AsMemory().Pin();
        disposer.Add(overrideArrayPin);

        return (LLamaModelTensorBufferOverride*)overrideArrayPin.Pointer;
    }
}
