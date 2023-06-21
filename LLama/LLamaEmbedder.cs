using LLama.Native;
using System;
using System.Collections.Generic;
using System.Text;
using LLama.Exceptions;
using System.Linq;
using LLama.Common;

namespace LLama
{
    /// <summary>
    /// The embedder for LLama, which supports getting embeddings from text.
    /// </summary>
    public class LLamaEmbedder : IDisposable
    {
        SafeLLamaContextHandle _ctx;

        /// <summary>
        /// Warning: must ensure the original model has params.embedding = true;
        /// </summary>
        /// <param name="ctx"></param>
        internal LLamaEmbedder(SafeLLamaContextHandle ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="params"></param>
        public LLamaEmbedder(ModelParams @params)
        {
            @params.EmbeddingMode = true;
            _ctx = Utils.InitLLamaContextFromModelParams(@params);
        }

        /// <summary>
        /// Get the embeddings of the text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="threads">Threads used for inference.</param>
        /// <param name="addBos">Add bos to the text.</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public unsafe float[] GetEmbeddings(string text, int threads = -1, bool addBos = true, string encoding = "UTF-8")
        {
            if (threads == -1)
            {
                threads = Math.Max(Environment.ProcessorCount / 2, 1);
            }
            int n_past = 0;
            if (addBos)
            {
                text = text.Insert(0, " ");
            }
            var embed_inp = Utils.Tokenize(_ctx, text, addBos, Encoding.GetEncoding(encoding));

            // TODO(Rinne): deal with log of prompt

            if (embed_inp.Count() > 0)
            {
                var embed_inp_array = embed_inp.ToArray();
                if (NativeApi.llama_eval(_ctx, embed_inp_array, embed_inp_array.Length, n_past, threads) != 0)
                {
                    throw new RuntimeError("Failed to eval.");
                }
            }

            int n_embed = NativeApi.llama_n_embd(_ctx);
            var embeddings = NativeApi.llama_get_embeddings(_ctx);
            if (embeddings == null)
            {
                return new float[0];
            }
            var span = new Span<float>(embeddings, n_embed);
            float[] res = new float[n_embed];
            span.CopyTo(res.AsSpan());
            return res;
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
