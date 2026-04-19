using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LLama.Extensions;

public static class IntPtrExtensions
{

    /// <summary>
    /// Converts a native UTF-8 string pointer to a managed string, returning a fallback value when no data is available.
    /// </summary>
    /// <param name="ptr">Pointer to a null-terminated UTF-8 string.</param>
    /// <param name="defaultValue">Value to return when the pointer is <see cref="IntPtr.Zero"/> or when the string is empty.</param>
    /// <returns>Managed string representation of the native data, or <paramref name="defaultValue"/> when unavailable.</returns>
    public static string PtrToStringWithDefault(this IntPtr ptr, string defaultValue="")
    {
        return ptr.PtrToString() ?? defaultValue;
    }

    /// <summary>
    /// Converts a pointer to a null-terminated UTF-8 string into a managed string.
    /// </summary>
    /// <param name="ptr">Pointer to the first byte of a null-terminated UTF-8 string.</param>
    /// <returns>Managed string representation, or <c>null</c> when the pointer is zero or the string is empty.</returns>
    public static string? PtrToString(this IntPtr ptr )
    {
        if (ptr == IntPtr.Zero)
            return null;

#if NETSTANDARD2_0
        unsafe
        {
            var length = 0;
            var current = (byte*)ptr;
            while (current[length] != 0)
                length++;

            if (length == 0)
                return null;

            var buffer = new byte[length];
            Marshal.Copy(ptr, buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }
#else
        return Marshal.PtrToStringUTF8(ptr);
#endif
    }        
    
}
