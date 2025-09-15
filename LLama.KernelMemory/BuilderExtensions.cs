using Microsoft.KernelMemory;
using LLama;
using LLama.Common;
using Microsoft.KernelMemory.AI;

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
            return builder.WithLLamaSharpTextEmbeddingGeneration(generator);
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
            return builder.WithLLamaSharpTextGeneration(new LlamaSharpTextGenerator(config));
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
        public static IKernelMemoryBuilder WithLLamaSharpDefaults(this IKernelMemoryBuilder builder, LLamaSharpConfig config, LLamaWeights? weights=null)
        {
            var parameters = new ModelParams(config.ModelPath)
            {
                ContextSize = config.ContextSize ?? 2048,
                GpuLayerCount = config.GpuLayerCount ?? 20,
                MainGpu = config.MainGpu,
                SplitMode = config.SplitMode,
                BatchSize = 512,
                UBatchSize = 512,
                UseMemorymap = true
            };

            if (weights == null)
            {
                weights = LLamaWeights.LoadFromFile(parameters);
            }

            var executor = new StatelessExecutor(weights, parameters);
            builder.WithLLamaSharpTextEmbeddingGeneration(new LLamaSharpTextEmbeddingGenerator(config, weights));
            builder.WithLLamaSharpTextGeneration(new LlamaSharpTextGenerator(weights, config, executor));
            return builder;
        }		
    }
}
