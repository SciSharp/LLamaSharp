using LLama;
using LLama.Common;
using Microsoft.SemanticKernel.AI.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLamaSharp.KernelMemory
{
    /// <summary>
    /// Provides text embedding generation for LLamaSharp.
    /// </summary>
    public class LLamaSharpTextEmbeddingGeneration : ITextEmbeddingGeneration, IDisposable
    {
        private readonly LLamaSharpConfig _config;
        private readonly LLamaEmbedder _embedder;
        private readonly LLamaWeights _weights;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLamaSharpTextEmbeddingGeneration"/> class.
        /// </summary>
        /// <param name="config">The configuration for LLamaSharp.</param>
        public LLamaSharpTextEmbeddingGeneration(LLamaSharpConfig config)
        {
            this._config = config;
            var @params = new ModelParams(_config.ModelPath);
            _weights = LLamaWeights.LoadFromFile(@params);
            _embedder = new LLamaEmbedder(_weights, @params);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _embedder.Dispose();
            _weights.Dispose();
        }

        /// <inheritdoc/>
        public Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
        {
            IList<ReadOnlyMemory<float>> results = new List<ReadOnlyMemory<float>>();

            foreach (var d in data)
            {
                var embeddings = _embedder.GetEmbeddings(d);
                results.Add(new ReadOnlyMemory<float>(embeddings));
            }

            return Task.FromResult(results);
        }
    }
}
