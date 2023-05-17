using LLama.Native;
using System;
using System.Collections.Generic;
using System.Text;
using LLama.Exceptions;

namespace LLama
{
    public class LLamaEmbedder: IDisposable
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

        public LLamaEmbedder(LLamaParams @params)
        {
            @params.embedding = true;
            _ctx = Utils.llama_init_from_gpt_params(ref @params);
        }

        public unsafe float[] GetEmbeddings(string text, int n_thread = -1, bool add_bos = true, string encoding = "UTF-8")
        {
            if(n_thread == -1)
            {
                n_thread = Math.Max(Environment.ProcessorCount / 2, 1);
            }
            int n_past = 0;
            if (add_bos)
            {
                text = text.Insert(0, " ");
            }
            var embed_inp = Utils.llama_tokenize(_ctx, text, add_bos, encoding);

            // TODO(Rinne): deal with log of prompt

            if (embed_inp.Count > 0)
            {
                var embed_inp_array = embed_inp.ToArray();
                if (NativeApi.llama_eval(_ctx, embed_inp_array, embed_inp_array.Length, n_past, n_thread) != 0)
                {
                    throw new RuntimeError("Failed to eval.");
                }
            }

            int n_embed = NativeApi.llama_n_embd(_ctx);
            var embeddings = NativeApi.llama_get_embeddings(_ctx);
            if(embeddings == null)
            {
                return new float[0];
            }
            var span = new Span<float>(embeddings, n_embed);
            float[] res = new float[n_embed];
            span.CopyTo(res.AsSpan());
            return res;
        }

        public void Dispose()
        {
            _ctx.Dispose();
        }
    }
}
