using LLama.Native;
using System;
using LLama.Exceptions;
using LLama.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LLama
{
    /// <summary>
    /// The embedder for LLama, which supports getting embeddings from text.
    /// </summary>
    public sealed class LLamaEmbedder
        : IDisposable
    {
        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => Context.EmbeddingSize;

        /// <summary>
        /// LLama Context
        /// </summary>
        public LLamaContext Context { get; }

        /// <summary>
        /// Create a new embedder, using the given LLamaWeights
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="params"></param>
        /// <param name="logger"></param>
        public LLamaEmbedder(LLamaWeights weights, IContextParams @params, ILogger? logger = null)
        {
            if (!@params.EmbeddingMode)
                throw new ArgumentException("EmbeddingMode must be true", nameof(@params));

            Context = weights.CreateContext(@params, logger);
        }

        /// <summary>
        /// Get the embeddings of the text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public Task<float[]> GetEmbeddings(string text, CancellationToken cancellationToken = default)
        {
            return GetEmbeddings(text, true, cancellationToken);
        }

        /// <summary>
        /// Get the embeddings of the text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="addBos">Add bos to the text.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public async Task<float[]> GetEmbeddings(string text, bool addBos, CancellationToken cancellationToken = default)
        {
            var tokens = Context.Tokenize(text, addBos);
            if (tokens.Length > Context.ContextSize)
                throw new ArgumentException($"Embedding prompt is longer than the context window ({tokens.Length} > {Context.ContextSize})", nameof(text));

            // Evaluate prompt in batch-size chunks
            var n_past = 0;
            var batch = new LLamaBatch();
            var batchSize = (int)Context.Params.BatchSize;
            for (var i = 0; i < tokens.Length; i += batchSize)
            {
                var n_eval = tokens.Length - i;
                if (n_eval > batchSize)
                    n_eval = batchSize;

                batch.Clear();
                batch.AddRange(tokens.AsSpan(i, n_eval), n_past, LLamaSeqId.Zero, false);
                n_past += n_eval;

                var returnCode = await Context.DecodeAsync(batch, cancellationToken);
                if (returnCode != 0)
                    throw new LLamaDecodeError(returnCode);
            }

            var embeddings = GetEmbeddingsArray();

            // Remove everything we just evaluated from the context cache
            Context.NativeHandle.KvCacheClear();

            // Normalize the embeddings vector
            // https://github.com/ggerganov/llama.cpp/blob/2891c8aa9af17f4ff636ff3868bc34ff72b56e25/examples/embedding/embedding.cpp#L92
            Normalize(embeddings);

            return embeddings;
        }

        private float[] GetEmbeddingsArray()
        {
            var embeddings = NativeApi.llama_get_embeddings(Context.NativeHandle);
            if (embeddings == null)
                return Array.Empty<float>();
            return embeddings.ToArray();
        }

        private static void Normalize(Span<float> embeddings)
        {
            // Calculate length
            var lengthSqr = 0.0;
            foreach (var value in embeddings)
                lengthSqr += value * value;
            var length = (float)Math.Sqrt(lengthSqr);

            // Normalize
            for (var i = 0; i < embeddings.Length; i++)
                embeddings[i] /= length;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Context.Dispose();
        }

    }
}
