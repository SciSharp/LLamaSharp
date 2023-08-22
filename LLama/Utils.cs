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

        [Obsolete("Use SafeLLamaContextHandle Eval method instead")]
        public static int Eval(SafeLLamaContextHandle ctx, llama_token[] tokens, int startIndex, int n_tokens, int n_past, int n_threads)
        {
            var slice = tokens.AsSpan().Slice(startIndex, n_tokens);
            return ctx.Eval(slice, n_past, n_threads) ? 0 : 1;
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
            
        /// <summary>
        /// Converts a bool "value" to a signed byte of "1" for true and "0" for false to be compatible with a 1 byte C-style bool.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static sbyte BoolToSignedByte(bool value)
        {
            return value ? (sbyte)1 : (sbyte)0;
        }

        /// <summary>
        /// Converts a sbyte "value" to a C# bool.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SignedByteToBool(sbyte value)
        {
            return value > 0;
        }

    }
}
