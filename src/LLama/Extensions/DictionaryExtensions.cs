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
#elif !NET6_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER
    #error Target framework not supported!
#endif

        internal static TValue GetValueOrDefaultImpl<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            // ReSharper disable once CanSimplifyDictionaryTryGetValueWithGetValueOrDefault (this is a shim for  that method!)
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
