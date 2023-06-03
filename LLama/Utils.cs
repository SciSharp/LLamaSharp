using LLama.Native;
using System;
using System.Collections.Generic;
using System.Text;
using LLama.Exceptions;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;

namespace LLama
{
    using llama_token = Int32;
    internal static class Utils
    {
        public static SafeLLamaContextHandle llama_init_from_gpt_params(ref LLamaParams @params)
        {
            var lparams = NativeApi.llama_context_default_params();

            lparams.n_ctx = @params.n_ctx;
            lparams.n_gpu_layers = @params.n_gpu_layers;
            lparams.seed = @params.seed;
            lparams.f16_kv = @params.memory_f16;
            lparams.use_mmap = @params.use_mmap;
            lparams.use_mlock = @params.use_mlock;
            lparams.logits_all = @params.perplexity;
            lparams.embedding = @params.embedding;

            if (!File.Exists(@params.model))
            {
                throw new FileNotFoundException($"The model file does not exist: {@params.model}");
            }

            var ctx_ptr = NativeApi.llama_init_from_file(@params.model, lparams);

            if(ctx_ptr == IntPtr.Zero )
            {
                throw new RuntimeError($"Failed to load model {@params.model}.");
            }

            SafeLLamaContextHandle ctx = new(ctx_ptr);

            if (!string.IsNullOrEmpty(@params.lora_adapter))
            {
                int err = NativeApi.llama_apply_lora_from_file(ctx, @params.lora_adapter,
                    string.IsNullOrEmpty(@params.lora_base) ? null : @params.lora_base, @params.n_threads);
                if(err != 0)
                {
                    throw new RuntimeError("Failed to apply lora adapter.");
                }
            }
            return ctx;
        }

        public static List<llama_token> llama_tokenize(SafeLLamaContextHandle ctx, string text, bool add_bos, string encodingName)
        {
            var encoding = Encoding.GetEncoding(encodingName);
            var cnt = encoding.GetByteCount(text);
            llama_token[] res = new llama_token[cnt + (add_bos ? 1 : 0)];
            int n = NativeApi.llama_tokenize(ctx, text, encoding, res, res.Length, add_bos);
            if(n < 0)
            {
                throw new RuntimeError("Error happened during tokenization. It's possibly caused by wrong encoding. Please try to " +
                    "specify the encoding.");
            }
            return res.Take(n).ToList();
        }

        public unsafe static Span<float> llama_get_logits(SafeLLamaContextHandle ctx, int length)
        {
            var logits = NativeApi.llama_get_logits(ctx);
            return new Span<float>(logits, length);
        }

        public static unsafe string PtrToStringUTF8(IntPtr ptr)
        {
#if NET6_0_OR_GREATER
            return Marshal.PtrToStringUTF8(ptr);
#else
            byte* tp = (byte*)ptr.ToPointer();
            List<byte> bytes = new();
            while (true)
            {
                byte c = *tp++;
                if(c == '\0')
                {
                    break;
                }
                else
                {
                    bytes.Add(c);
                }
            }
            return Encoding.UTF8.GetString(bytes.ToArray());
#endif
        }
    }
}
