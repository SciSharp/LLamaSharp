using Microsoft.SemanticMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLamaSharp.KernelMemory
{
    /// <summary>
    /// Provides extension methods for the MemoryClientBuilder class.
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Adds LLamaSharpTextEmbeddingGeneration to the MemoryClientBuilder.
        /// </summary>
        /// <param name="builder">The MemoryClientBuilder instance.</param>
        /// <param name="config">The LLamaSharpConfig instance.</param>
        /// <returns>The MemoryClientBuilder instance with LLamaSharpTextEmbeddingGeneration added.</returns>
        public static MemoryClientBuilder WithLLamaSharpTextEmbeddingGeneration(this MemoryClientBuilder builder, LLamaSharpConfig config)
        {
            builder.WithCustomEmbeddingGeneration(new LLamaSharpTextEmbeddingGeneration(config));
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextGeneration to the MemoryClientBuilder.
        /// </summary>
        /// <param name="builder">The MemoryClientBuilder instance.</param>
        /// <param name="config">The LLamaSharpConfig instance.</param>
        /// <returns>The MemoryClientBuilder instance with LLamaSharpTextGeneration added.</returns>
        public static MemoryClientBuilder WithLLamaSharpTextGeneration(this MemoryClientBuilder builder, LLamaSharpConfig config)
        {
            builder.WithCustomTextGeneration(new LlamaSharpTextGeneration(config));
            return builder;
        }

        /// <summary>
        /// Adds LLamaSharpTextEmbeddingGeneration and LLamaSharpTextGeneration to the MemoryClientBuilder.
        /// </summary>
        /// <param name="builder">The MemoryClientBuilder instance.</param>
        /// <param name="config">The LLamaSharpConfig instance.</param>
        /// <returns>The MemoryClientBuilder instance with LLamaSharpTextEmbeddingGeneration and LLamaSharpTextGeneration added.</returns>
        public static MemoryClientBuilder WithLLamaSharpDefaults(this MemoryClientBuilder builder, LLamaSharpConfig config)
        {
            builder.WithLLamaSharpTextEmbeddingGeneration(config);
            builder.WithLLamaSharpTextGeneration(config);
            return builder;
        }
    }
}
