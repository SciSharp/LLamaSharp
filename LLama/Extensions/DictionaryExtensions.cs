using System.Collections.Generic;

namespace LLama.Extensions
{
    internal static class DictionaryExtensions
    {
#if NETSTANDARD2_0
        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            return GetValueOrDefaultImpl(dictionary, key, defaultValue);
        }
#endif

        internal static TValue GetValueOrDefaultImpl<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
