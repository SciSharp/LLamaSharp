using System;
using System.Text;

namespace LLama.Extensions;

internal static class EncodingExtensions
{
#if NETSTANDARD2_0
    public static int GetChars(this Encoding encoding, ReadOnlySpan<byte> bytes, Span<char> output)
    {
        return GetCharsImpl(encoding, bytes, output);
    }

    public static int GetCharCount(this Encoding encoding, ReadOnlySpan<byte> bytes)
    {
        return GetCharCountImpl(encoding, bytes);
    }
#elif !NET6_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER
    #error Target framework not supported!
#endif

    internal static int GetCharsImpl(Encoding encoding, ReadOnlySpan<byte> bytes, Span<char> output)
    {
        if (bytes.Length == 0)
            return 0;

        unsafe
        {
            fixed (byte* bytePtr = bytes)
            fixed (char* charPtr = output)
            {
                return encoding.GetChars(bytePtr, bytes.Length, charPtr, output.Length);
            }
        }
    }

    internal static int GetCharCountImpl(Encoding encoding, ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
            return 0;

        unsafe
        {
            fixed (byte* bytePtr = bytes)
            {
                return encoding.GetCharCount(bytePtr, bytes.Length);
            }
        }
    }
}