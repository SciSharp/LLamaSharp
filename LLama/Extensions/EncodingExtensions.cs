using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Extensions;

internal static class EncodingExtensions
{
#if NETSTANDARD2_0
    public static int GetChars(this Encoding encoding, ReadOnlySpan<byte> bytes, Span<char> output)
    {
        unsafe
        {
            fixed (byte* bytePtr = bytes)
            fixed (char* charPtr = output)
            {
                return encoding.GetChars(bytePtr, bytes.Length, charPtr, output.Length);
            }
        }
    }

    public static int GetCharCount(this Encoding encoding, ReadOnlySpan<byte> bytes)
    {
        unsafe
        {
            fixed (byte* bytePtr = bytes)
            {
                return encoding.GetCharCount(bytePtr, bytes.Length);
            }
        }
    }
#endif
}