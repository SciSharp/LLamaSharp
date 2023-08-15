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
        /// Convert the given `IModelParams` into a `LLamaContextParams`
        /// </summary>
        /// <param name="params"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static MemoryHandle ToLlamaContextParams(this IModelParams @params, out LLamaContextParams result)
        {
            if (!File.Exists(@params.ModelPath))
                throw new FileNotFoundException($"The model file does not exist: {@params.ModelPath}");

            if (@params.TensorSplits != null && @params.TensorSplits.Length != 1)
                throw new ArgumentException("Currently multi-gpu support is not supported by both llama.cpp and LLamaSharp.");

            result = NativeApi.llama_context_default_params();
            result.n_ctx = @params.ContextSize;
            result.n_batch = @params.BatchSize;
            result.main_gpu = @params.MainGpu;
            result.n_gpu_layers = @params.GpuLayerCount;
            result.seed = @params.Seed;
            result.f16_kv = @params.UseFp16Memory;
            result.use_mmap = @params.UseMemorymap;
            result.use_mlock = @params.UseMemoryLock;
            result.logits_all = @params.Perplexity;
            result.embedding = @params.EmbeddingMode;
            result.low_vram = @params.LowVram;
            result.n_gqa = @params.GroupedQueryAttention;
            result.rms_norm_eps = @params.RmsNormEpsilon;
            result.rope_freq_base = @params.RopeFrequencyBase;
            result.rope_freq_scale = @params.RopeFrequencyScale;
            result.mul_mat_q = @params.MulMatQ;

            var pin = @params.TensorSplits.AsMemory().Pin();
            unsafe
            {
                result.tensor_split = (nint)pin.Pointer;
            }

            return pin;
        }
    }
}
