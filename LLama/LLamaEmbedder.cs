using LLama.Native;
using System;
using LLama.Exceptions;
using LLama.Abstractions;

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
        /// 
        /// </summary>
        /// <param name="params"></param>
        public LLamaEmbedder(IModelParams @params)
        {
            @params.EmbeddingMode = true;
            using var weights = LLamaWeights.LoadFromFile(@params);
            _ctx = weights.CreateContext(@params);
        }

        public LLamaEmbedder(LLamaWeights weights, IModelParams @params)
        {
            _ctx = weights.CreateContext(@params);
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
