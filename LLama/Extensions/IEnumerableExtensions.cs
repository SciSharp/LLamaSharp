using System.Collections.Generic;
using System.Linq;

namespace LLama.Extensions
{
    internal static class IEnumerableExtensions
    {
#if NETSTANDARD2_0
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
        {
            return TakeLastImpl(source, count);
        }
#endif

        internal static IEnumerable<T> TakeLastImpl<T>(IEnumerable<T> source, int count)
        {
            var list = source.ToList();

            if (count >= list.Count)
                return list;

            list.RemoveRange(0, list.Count - count);
            return list;
        }
    }
}
