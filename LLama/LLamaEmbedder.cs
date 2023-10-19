using LLama.Native;
using System;
using LLama.Exceptions;
using LLama.Abstractions;
using Microsoft.Extensions.Logging;

namespace LLama
{
    /// <summary>
    /// The embedder for LLama, which supports getting embeddings from text.
    /// </summary>
    public sealed class LLamaEmbedder
        : IDisposable
    {
        private readonly LLamaContext _ctx;

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => _ctx.EmbeddingSize;

        /// <summary>
        /// Create a new embedder (loading temporary weights)
        /// </summary>
        /// <param name="allParams"></param>
        /// <param name="logger"></param>
        [Obsolete("Preload LLamaWeights and use the constructor which accepts them")]
        public LLamaEmbedder(ILLamaParams allParams, ILogger? logger = null)
            : this(allParams, allParams, logger)
        {
        }

        /// <summary>
        /// Create a new embedder (loading temporary weights)
        /// </summary>
        /// <param name="modelParams"></param>
        /// <param name="contextParams"></param>
        /// <param name="logger"></param>
        [Obsolete("Preload LLamaWeights and use the constructor which accepts them")]
        public LLamaEmbedder(IModelParams modelParams, IContextParams contextParams, ILogger? logger = null)
        {
            using var weights = LLamaWeights.LoadFromFile(modelParams);

            contextParams.EmbeddingMode = true;
            _ctx = weights.CreateContext(contextParams, logger);
        }

        /// <summary>
        /// Create a new embedder, using the given LLamaWeights
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="params"></param>
        /// <param name="logger"></param>
        public LLamaEmbedder(LLamaWeights weights, IContextParams @params, ILogger? logger = null)
        {
            @params.EmbeddingMode = true;
            _ctx = weights.CreateContext(@params, logger);
        }

        /// <summary>
        /// Get the embeddings of the text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="threads">unused</param>
        /// <param name="addBos">Add bos to the text.</param>
        /// <param name="encoding">unused</param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        [Obsolete("'threads' and 'encoding' parameters are no longer used")]
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public float[] GetEmbeddings(string text, int threads = -1, bool addBos = true, string encoding = "UTF-8")
        {
            return GetEmbeddings(text, addBos);
        }

        /// <summary>
        /// Get the embeddings of the text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public float[] GetEmbeddings(string text)
        {
            return GetEmbeddings(text, true);
        }

        /// <summary>
        /// Get the embeddings of the text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="addBos">Add bos to the text.</param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public float[] GetEmbeddings(string text, bool addBos)
        {
            var embed_inp_array = _ctx.Tokenize(text, addBos);

            // TODO(Rinne): deal with log of prompt

            if (embed_inp_array.Length > 0)
                _ctx.Eval(embed_inp_array, 0);

            unsafe
            {
                var embeddings = NativeApi.llama_get_embeddings(_ctx.NativeHandle);
                if (embeddings == null)
                    return Array.Empty<float>();

                return new Span<float>(embeddings, EmbeddingSize).ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _ctx.Dispose();
        }
    }
}
