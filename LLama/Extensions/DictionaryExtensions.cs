using System.Collections.Generic;

namespace LLama.Extensions
{
    internal static class DictionaryExtensions
    {
#if NETSTANDARD2_0_OR_GREATER
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            return GetValueOrDefaultImpl(dictionary, key, defaultValue);
        }
#elif !NET6_0_OR_GREATER
#error Target framework not supported!
#endif

        internal static TValue GetValueOrDefaultImpl<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
