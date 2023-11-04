using Microsoft.KernelMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static KernelMemoryBuilder WithLLamaSharpTextEmbeddingGeneration(this KernelMemoryBuilder builder, LLamaSharpConfig config)
        {
            builder.WithCustomEmbeddingGeneration(new LLamaSharpTextEmbeddingGeneration(config));
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="config">The LLamaSharpConfig instance.</param>
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextGeneration added.</returns>
        public static KernelMemoryBuilder WithLLamaSharpTextGeneration(this KernelMemoryBuilder builder, LLamaSharpConfig config)
        {
            builder.WithCustomTextGeneration(new LlamaSharpTextGeneration(config));
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextEmbeddingGeneration and LLamaSharpTextGeneration to the KernelMemoryBuilder.
        /// </summary>
        /// <param name="builder">The KernelMemoryBuilder instance.</param>
        /// <param name="config">The LLamaSharpConfig instance.</param>
        /// <returns>The KernelMemoryBuilder instance with LLamaSharpTextEmbeddingGeneration and LLamaSharpTextGeneration added.</returns>
        public static KernelMemoryBuilder WithLLamaSharpDefaults(this KernelMemoryBuilder builder, LLamaSharpConfig config)
        {
            builder.WithLLamaSharpTextEmbeddingGeneration(config);
            builder.WithLLamaSharpTextGeneration(config);
            return builder;
        }
    }
}
