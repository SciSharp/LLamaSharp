using LLama.Abstractions;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

        [Obsolete("Use SafeLLamaContextHandle GetLogits method instead")]
        public static Span<float> GetLogits(SafeLLamaContextHandle ctx, int length)
        {
            if (length != ctx.VocabCount)
                throw new ArgumentException("length must be the VocabSize");

            return ctx.GetLogits();
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

        [Obsolete("Use SafeLLamaContextHandle TokenToString method instead")]
        public static string TokenToString(llama_token token, SafeLLamaContextHandle ctx, Encoding encoding)
        {
            return ctx.TokenToString(token, encoding);
        }

        [Obsolete("No longer used internally by LlamaSharp")]
        public static string PtrToString(IntPtr ptr, Encoding encoding)
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
            unsafe
            {
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
            }
#endif
        }
    }
}
