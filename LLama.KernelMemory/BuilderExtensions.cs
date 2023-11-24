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

namespace LLamaSharp.KernelMemory
{
    /// <summary>
    /// Provides extension methods for the KernelMemoryBuilder class.
    /// </summary>
    public static class BuilderExtensions
    {

        private static IKernelMemoryBuilder WithCustomEmbeddingGeneration(this IKernelMemoryBuilder builder, ITextEmbeddingGeneration embeddingGeneration)
        {
            builder.AddSingleton<ITextEmbeddingGeneration>(embeddingGeneration);
            builder.AddIngestionEmbeddingGenerator(embeddingGeneration);
            return builder;
        }

        private static IKernelMemoryBuilder WithCustomTextGeneration(this IKernelMemoryBuilder builder, ITextGeneration textGeneration)
        {
            builder.AddSingleton<ITextGeneration>(textGeneration);
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextEmbeddingGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="config">The LLamaSharpConfig instance.</param>
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextEmbeddingGeneration added.</returns>
        public static IKernelMemoryBuilder WithLLamaSharpTextEmbeddingGeneration(this IKernelMemoryBuilder builder, LLamaSharpConfig config)
        {
            builder.WithCustomEmbeddingGeneration(new LLamaSharpTextEmbeddingGeneration(config));
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextEmbeddingGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="textEmbeddingGeneration">The LLamaSharpTextEmbeddingGeneration instance.</param>
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextEmbeddingGeneration added.</returns>
        public static IKernelMemoryBuilder WithLLamaSharpTextEmbeddingGeneration(this IKernelMemoryBuilder builder, LLamaSharpTextEmbeddingGeneration textEmbeddingGeneration)
        {
            builder.WithCustomEmbeddingGeneration(textEmbeddingGeneration);
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
            builder.WithCustomTextGeneration(new LlamaSharpTextGeneration(config));
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="textGeneration">The LlamaSharpTextGeneration instance.</param>
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextGeneration added.</returns>
        public static IKernelMemoryBuilder WithLLamaSharpTextGeneration(this IKernelMemoryBuilder builder, LlamaSharpTextGeneration textGeneration)
        {
            builder.WithCustomTextGeneration(textGeneration);
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextEmbeddingGeneration and LLamaSharpTextGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="config">The LLamaSharpConfig instance.</param>
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextEmbeddingGeneration and LLamaSharpTextGeneration added.</returns>
        public static IKernelMemoryBuilder WithLLamaSharpDefaults(this IKernelMemoryBuilder builder, LLamaSharpConfig config)
        {
            var parameters = new ModelParams(config.ModelPath)
            {
                ContextSize = config?.ContextSize ?? 2048,
                Seed = config?.Seed ?? 0,
                GpuLayerCount = config?.GpuLayerCount ?? 20
            };
            var weights = LLamaWeights.LoadFromFile(parameters);
            var context = weights.CreateContext(parameters);
            var executor = new StatelessExecutor(weights, parameters);
            var embedder = new LLamaEmbedder(weights, parameters);
            builder.WithLLamaSharpTextEmbeddingGeneration(new LLamaSharpTextEmbeddingGeneration(embedder));
            builder.WithLLamaSharpTextGeneration(new LlamaSharpTextGeneration(weights, context, executor, config?.DefaultInferenceParams));
            return builder;
        }
    }
}
