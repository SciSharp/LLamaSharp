using System;
using System.Text;

namespace LLama.Extensions;

internal static class EncodingExtensions
{
#if NETSTANDARD2_0
    public static int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> output)
    {
        return GetBytesImpl(encoding, chars, output);
    }

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

    internal static int GetBytesImpl(Encoding encoding, ReadOnlySpan<char> chars, Span<byte> output)
    {
        if (chars.Length == 0)
            return 0;

        unsafe
        {
            fixed (char* charPtr = chars)
            fixed (byte* bytePtr = output)
            {
                return encoding.GetBytes(charPtr, chars.Length, bytePtr, output.Length);
            }
        }
    }

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

    internal static string GetStringFromSpan(this Encoding encoding, ReadOnlySpan<byte> bytes)
    {
        unsafe
        {
            fixed (byte* bytesPtr = bytes)
            {
                return encoding.GetString(bytesPtr, bytes.Length);
            }
        }
    }
}