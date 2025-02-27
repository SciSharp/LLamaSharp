using System;
using System.Collections.Generic;

namespace LLama.Extensions
{
    internal static class ListExtensions
    {
#if !NET6_0_OR_GREATER
        public static void EnsureCapacity<T>(this List<T> list, int capacity)
        {
            if (list.Capacity < capacity)
                list.Capacity = capacity;
        }
#endif

        public static void AddSpan<T>(this List<T> list, ReadOnlySpan<T> items)
        {
            list.EnsureCapacity(list.Count + items.Length);

            for (var i = 0; i < items.Length; i++)
                list.Add(items[i]);
        }

#if !NET6_0_OR_GREATER
        public static void CopyTo<T>(this List<T> list, Span<T> dest)
        {
            if (dest.Length < list.Count)
                throw new ArgumentException($"dest is too small ({dest.Length} < {list.Count})");

            for (var i = 0; i < list.Count; i++)
                dest[i] = list[i];
        }
#endif
    }
}
