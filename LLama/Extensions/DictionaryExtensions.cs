using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Extensions
{
    public static class DictionaryExtension
    {
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> pair, out T1 first, out T2 second)
        {
            first = pair.Key;
            second = pair.Value;
        }
        public static void Update<T1, T2>(this Dictionary<T1, T2> dic, IDictionary<T1, T2> other)
        {
            foreach (var (key, value) in other)
            {
                dic[key] = value;
            }
        }
        public static T2 GetOrDefault<T1, T2>(this Dictionary<T1, T2> dic, T1 key, T2 defaultValue)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            return defaultValue;
        }
    }
}
