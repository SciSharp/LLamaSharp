using LLama.Abstractions;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using LLama.Extensions;

namespace LLama
{
    using llama_token = Int32;

    public static class Utils
    {
        public static SafeLLamaContextHandle InitLLamaContextFromModelParams(IModelParams @params)
        {
            using var weights = LLamaWeights.LoadFromFile(@params);

            using (@params.ToLlamaContextParams(out var lparams))
                return SafeLLamaContextHandle.Create(weights.NativeHandle, lparams);
        }

        [Obsolete("Use SafeLLamaContextHandle Tokenize method instead")]
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static IEnumerable<llama_token> Tokenize(SafeLLamaContextHandle ctx, string text, bool add_bos, Encoding encoding)
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return ctx.Tokenize(text, add_bos, encoding);
        }

        [Obsolete("Use SafeLLamaContextHandle GetLogits method instead")]
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static Span<float> GetLogits(SafeLLamaContextHandle ctx, int length)
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if (length != ctx.VocabCount)
                throw new ArgumentException("length must be the VocabSize");

            return ctx.GetLogits();
        }

        [Obsolete("Use SafeLLamaContextHandle Eval method instead")]
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static int Eval(SafeLLamaContextHandle ctx, llama_token[] tokens, int startIndex, int n_tokens, int n_past, int n_threads)
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            var slice = tokens.AsSpan().Slice(startIndex, n_tokens);
            return ctx.Eval(slice, n_past, n_threads) ? 0 : 1;
        }

        [Obsolete("Use SafeLLamaContextHandle TokenToString method instead")]
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string TokenToString(llama_token token, SafeLLamaContextHandle ctx, Encoding encoding)
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return ctx.TokenToString(token, encoding);
        }

        [Obsolete("No longer used internally by LlamaSharp")]
        #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string PtrToString(IntPtr ptr, Encoding encoding)
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
#if NET6_0_OR_GREATER
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if(encoding == Encoding.UTF8)
            {
                return Marshal.PtrToStringUTF8(ptr)!;
            }
            // ReSharper disable once PossibleUnintendedReferenceComparison
            else if(encoding == Encoding.Unicode)
            {
                return Marshal.PtrToStringUni(ptr)!;
            }
            else
            {
                return Marshal.PtrToStringAuto(ptr)!;
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
