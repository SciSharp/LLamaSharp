using LLama.Common;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LLama
{
    /// <summary>
    /// Placeholder for now
    /// TODO: Handle model, context collections
    /// </summary>
    public static class LLamaContextFactory
    {
        public static LLamaContextParams CreateContextParams(ModelParams modelParams)
        {
            var lparams = NativeApi.llama_context_default_params();

            lparams.n_ctx = modelParams.ContextSize;
            lparams.n_batch = modelParams.BatchSize;
            lparams.main_gpu = modelParams.MainGpu;
            lparams.n_gpu_layers = modelParams.GpuLayerCount;
            lparams.seed = modelParams.Seed;
            lparams.f16_kv = modelParams.UseFp16Memory;
            lparams.use_mmap = modelParams.UseMemoryLock;
            lparams.use_mlock = modelParams.UseMemoryLock;
            lparams.logits_all = modelParams.Perplexity;
            lparams.embedding = modelParams.EmbeddingMode;
            lparams.low_vram = modelParams.LowVram;

            if (modelParams.TensorSplits.Length != 1)
            {
                throw new ArgumentException("Currently multi-gpu support is not supported by both llama.cpp and LLamaSharp.");
            }
            lparams.tensor_split = modelParams.TensorSplits;

            if (!File.Exists(modelParams.ModelPath))
            {
                throw new FileNotFoundException($"The model file does not exist: {modelParams.ModelPath}");
            }

            return lparams;
        }

        public static SafeLlamaModelHandle CreateModel(string modelPath, LLamaContextParams contextParams)
        {
            return SafeLlamaModelHandle.LoadFromFile(modelPath, contextParams);
        }


        public static SafeLLamaContextHandle CreateContext(SafeLlamaModelHandle model, LLamaContextParams contextParams)
        {
            return SafeLLamaContextHandle.Create(model, contextParams);
        }
    }
}
