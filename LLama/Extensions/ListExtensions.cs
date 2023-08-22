using System;
using System.Collections.Generic;

namespace LLama.Extensions
{
    internal static class ListExtensions
    {
        public static void AddRangeSpan<T>(this List<T> list, ReadOnlySpan<T> span)
        {
            for (var i = 0; i < span.Length; i++)
                list.Add(span[i]);
        }
    }
}
