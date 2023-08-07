using LLama.Abstractions;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LLama.Exceptions;
using LLama.Extensions;

namespace LLama
{
    using llama_token = Int32;
    public static class Utils
    {
        public static SafeLLamaContextHandle InitLLamaContextFromModelParams(IModelParams @params)
        {
            using (@params.ToLlamaContextParams(out var lparams))
            {
                var model = SafeLlamaModelHandle.LoadFromFile(@params.ModelPath, lparams);
                var ctx = SafeLLamaContextHandle.Create(model, lparams);

                if (!string.IsNullOrEmpty(@params.LoraAdapter))
                    model.ApplyLoraFromFile(@params.LoraAdapter, @params.LoraBase, @params.Threads);

                return ctx;
            }
        }

        [Obsolete("Use SafeLLamaContextHandle Tokenize method instead")]
        public static IEnumerable<llama_token> Tokenize(SafeLLamaContextHandle ctx, string text, bool add_bos, Encoding encoding)
        {
            return ctx.Tokenize(text, add_bos, encoding);
        }

        public static unsafe Span<float> GetLogits(SafeLLamaContextHandle ctx, int length)
        {
            var logits = NativeApi.llama_get_logits(ctx);
            return new Span<float>(logits, length);
        }

        public static unsafe int Eval(SafeLLamaContextHandle ctx, llama_token[] tokens, int startIndex, int n_tokens, int n_past, int n_threads)
        {
            int result;
            fixed(llama_token* p = tokens)
            {
                result = NativeApi.llama_eval_with_pointer(ctx, p + startIndex, n_tokens, n_past, n_threads);
            }
            return result;
        }

        public static string TokenToString(llama_token token, SafeLLamaContextHandle ctx, Encoding encoding)
        {
            return PtrToString(NativeApi.llama_token_to_str(ctx, token), encoding);
        }

        public static unsafe string PtrToString(IntPtr ptr, Encoding encoding)
        {
#if NET6_0_OR_GREATER
            if(encoding == Encoding.UTF8)
            {
                return Marshal.PtrToStringUTF8(ptr);
            }
            else if(encoding == Encoding.Unicode)
            {
                return Marshal.PtrToStringUni(ptr);
            }
            else
            {
                return Marshal.PtrToStringAuto(ptr);
            }
#else
            byte* tp = (byte*)ptr.ToPointer();
            List<byte> bytes = new();
            while (true)
            {
                byte c = *tp++;
                if (c == '\0')
                {
                    break;
                }
                else
                {
                    bytes.Add(c);
                }
            }
            return encoding.GetString(bytes.ToArray());
#endif
        }
    }
}
