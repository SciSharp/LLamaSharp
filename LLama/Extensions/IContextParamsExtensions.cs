﻿using System;
using System.IO;
using LLama.Abstractions;
using LLama.Native;

namespace LLama.Extensions
{
    /// <summary>
    /// Extention methods to the IContextParams interface
    /// </summary>
    public static class IContextParamsExtensions
    {
        /// <summary>
        /// Convert the given `IModelParams` into a `LLamaContextParams`
        /// </summary>
        /// <param name="params"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void ToLlamaContextParams(this IContextParams @params, out LLamaContextParams result)
        {
            result = LLamaContextParams.Default();

            result.n_ctx = @params.ContextSize ?? 0;
            result.n_batch = @params.BatchSize;
            result.n_ubatch = @params.UBatchSize;
            result.n_seq_max = @params.SeqMax;
            result.seed = @params.Seed;
            result.embeddings = @params.Embeddings;
            result.rope_freq_base = @params.RopeFrequencyBase ?? 0;
            result.rope_freq_scale = @params.RopeFrequencyScale ?? 0;

            // Default YaRN values copied from here: https://github.com/ggerganov/llama.cpp/blob/381efbf480959bb6d1e247a8b0c2328f22e350f8/common/common.h#L67
            result.yarn_ext_factor = @params.YarnExtrapolationFactor ?? -1f;
            result.yarn_attn_factor = @params.YarnAttentionFactor ?? 1f;
            result.yarn_beta_fast = @params.YarnBetaFast ?? 32f;
            result.yarn_beta_slow = @params.YarnBetaSlow ?? 1f;
            result.yarn_orig_ctx = @params.YarnOriginalContext ?? 0;
            result.rope_scaling_type = @params.YarnScalingType ?? RopeScalingType.Unspecified;

            result.defrag_threshold = @params.DefragThreshold;

            result.cb_eval = IntPtr.Zero;
            result.cb_eval_user_data = IntPtr.Zero;

            result.abort_callback = IntPtr.Zero;
            result.abort_callback_user_data = IntPtr.Zero;

            result.type_k = @params.TypeK ?? GGMLType.GGML_TYPE_F16;
            result.type_k = @params.TypeV ?? GGMLType.GGML_TYPE_F16;
            result.offload_kqv = !@params.NoKqvOffload;
            result.llama_pooling_type = @params.PoolingType;

            result.n_threads = Threads(@params.Threads);
            result.n_threads_batch = Threads(@params.BatchThreads);
        }

        private static uint Threads(uint? value)
        {
            if (value is > 0)
                return (uint)value;

            return (uint)Math.Max(Environment.ProcessorCount / 2, 1);
        }
    }
}
