using Microsoft.KernelMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama;
using LLama.Common;
using Microsoft.KernelMemory.AI;
using Microsoft.SemanticKernel.AI.Embeddings;
using LLama.Native;

namespace LLamaSharp.KernelMemory
{
    /// <summary>
    /// Provides extension methods for the KernelMemoryBuilder class.
    /// </summary>
    public static class BuilderExtensions
    {

        /// <summary>
        /// Adds LLamaSharpTextEmbeddingGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="config">The LLamaSharpConfig instance.</param>
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextEmbeddingGeneration added.</returns>
        public static IKernelMemoryBuilder WithLLamaSharpTextEmbeddingGeneration(this IKernelMemoryBuilder builder, LLamaSharpConfig config)
        {
            var generator = new LLamaSharpTextEmbeddingGenerator(config);
            builder.AddSingleton<ITextEmbeddingGenerator>(generator);
            builder.AddIngestionEmbeddingGenerator(generator);
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextEmbeddingGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="textEmbeddingGenerator">The LLamaSharpTextEmbeddingGeneration instance.</param>
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextEmbeddingGeneration added.</returns>
        public static IKernelMemoryBuilder WithLLamaSharpTextEmbeddingGeneration(this IKernelMemoryBuilder builder, LLamaSharpTextEmbeddingGenerator textEmbeddingGenerator)
        {
            builder.AddSingleton<ITextEmbeddingGenerator>(textEmbeddingGenerator);
            builder.AddIngestionEmbeddingGenerator(textEmbeddingGenerator);
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="config">The LLamaSharpConfig instance.</param>
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextGeneration added.</returns>
        public static IKernelMemoryBuilder WithLLamaSharpTextGeneration(this IKernelMemoryBuilder builder, LLamaSharpConfig config)
        {
            builder.AddSingleton<ITextGenerator>(new LlamaSharpTextGenerator(config));
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="textGenerator">The LlamaSharpTextGeneration instance.</param>
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextGeneration added.</returns>
        public static IKernelMemoryBuilder WithLLamaSharpTextGeneration(this IKernelMemoryBuilder builder, LlamaSharpTextGenerator textGenerator)
        {
            builder.AddSingleton<ITextGenerator>(textGenerator);
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextEmbeddingGeneration and LLamaSharpTextGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="config">The LLamaSharpConfig instance.</param>
        /// <param name="weights"></param>
        /// <param name="context"></param>		        
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextEmbeddingGeneration and LLamaSharpTextGeneration added.</returns>		
        public static IKernelMemoryBuilder WithLLamaSharpDefaults(this IKernelMemoryBuilder builder, LLamaSharpConfig config, LLamaWeights? weights=null, LLamaContext? context=null)
        {
            var parameters = new ModelParams(config.ModelPath)
            {
                ContextSize = config.ContextSize ?? 2048,
                Seed = config.Seed ?? 0,
                GpuLayerCount = config.GpuLayerCount ?? 20,
                Embeddings = true,
                MainGpu = config.MainGpu,
                SplitMode = config.SplitMode
            };

            if (weights == null || context == null)
            {
                weights = LLamaWeights.LoadFromFile(parameters);
                context = weights.CreateContext(parameters);
            }

            var executor = new StatelessExecutor(weights, parameters);
            builder.WithLLamaSharpTextEmbeddingGeneration(new LLamaSharpTextEmbeddingGenerator(config, weights));
            builder.WithLLamaSharpTextGeneration(new LlamaSharpTextGenerator(weights, context, executor, config?.DefaultInferenceParams));
            return builder;
        }		
    }
}
