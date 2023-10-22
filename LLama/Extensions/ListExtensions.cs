using System;
using System.Collections.Generic;

namespace LLama.Extensions
{
    internal static class ListExtensions
    {
#if NETSTANDARD2_0
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
    }
}
