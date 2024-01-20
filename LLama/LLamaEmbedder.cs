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
            @params.EmbeddingMode = true;
            Context = weights.CreateContext(@params, logger);
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
            var embed_inp_array = Context.Tokenize(text, addBos);

            // TODO(Rinne): deal with log of prompt

            if (embed_inp_array.Length > 0)
                Context.Eval(embed_inp_array.AsSpan(), 0);

            var embeddings = NativeApi.llama_get_embeddings(Context.NativeHandle);
            if (embeddings == null)
                return Array.Empty<float>();

            return embeddings.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Context.Dispose();
        }

    }
}
