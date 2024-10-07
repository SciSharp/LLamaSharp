using System;
using System.Collections.Generic;

namespace LLama.Extensions;

internal static class SpanExtensions
{
#if NETSTANDARD2_0
    public static void Sort<T, TComparer>(this Span<T> span, TComparer comparer)
        where TComparer : IComparer<T>
    {
        // Get a temp array
        var temp = ArrayPool<T>.Shared.Rent(span.Length);
        var tempSpan = temp.AsSpan(0, span.Length);

        // Copy to temporary
        span.CopyTo(tempSpan);

        // Do the sorting in the array
        Array.Sort(temp, 0, tempSpan.Length, comparer);

        // Copy back to span
        tempSpan.CopyTo(span);
    }
#elif !NET6_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER
#error Target framework not supported!
#endif
}