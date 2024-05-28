using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LLama.Native;

namespace LLama.Extensions
{
    internal static class IReadOnlyListExtensions
    {
        /// <summary>
        /// Find the index of `item` in `list`
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">list to search</param>
        /// <param name="item">item to search for</param>
        /// <returns></returns>
        public static int? IndexOf<T>(this IReadOnlyList<T> list, T item)
            where T : IEquatable<T>
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(item))
                    return i;
            }

            return null;
        }

        /// <summary>
        /// Check if the given set of tokens ends with any of the given strings
        /// </summary>
        /// <param name="tokens">Tokens to check</param>
        /// <param name="queries">Strings to search for</param>
        /// <param name="model">Model to use to convert tokens into bytes</param>
        /// <param name="encoding">Encoding to use to convert bytes into characters</param>
        /// <returns></returns>
        [Obsolete("Use an Antiprompt processor instead")]
        internal static bool TokensEndsWithAnyString<TTokens, TQueries>(this TTokens tokens, TQueries? queries, SafeLlamaModelHandle model, Encoding encoding)
            where TTokens : IReadOnlyList<LLamaToken>
            where TQueries : IReadOnlyList<string>
        {
            if (queries == null || queries.Count == 0 || tokens.Count == 0)
                return false;

            // Find the length of the longest query
            var longest = 0;
            foreach (var candidate in queries)
                longest = Math.Max(longest, candidate.Length);

            // Rent an array to detokenize into
            var builderArray = ArrayPool<char>.Shared.Rent(longest);
            try
            {
                // Convert as many tokens as possible into the builderArray
                var characters = model.TokensToSpan(tokens, builderArray.AsSpan(0, longest), encoding);

                // Check every query to see if it's present
                foreach (var query in queries)
                    if (characters.EndsWith(query.AsSpan()))
                        return true;

                return false;
            }
            finally
            {
                ArrayPool<char>.Shared.Return(builderArray);
            }
        }

        /// <summary>
        /// Check if the given set of tokens ends with any of the given strings
        /// </summary>
        /// <param name="tokens">Tokens to check</param>
        /// <param name="queries">Strings to search for</param>
        /// <param name="model">Model to use to convert tokens into bytes</param>
        /// <param name="encoding">Encoding to use to convert bytes into characters</param>
        /// <returns></returns>
        [Obsolete("Use an Antiprompt processor instead")]
        internal static bool TokensEndsWithAnyString<TTokens>(this TTokens tokens, IList<string>? queries, SafeLlamaModelHandle model, Encoding encoding)
            where TTokens : IReadOnlyList<LLamaToken>
        {
            if (queries == null || queries.Count == 0 || tokens.Count == 0)
                return false;

            return tokens.TokensEndsWithAnyString(new ReadonlyWrapper<string>(queries), model, encoding);
        }

        private readonly struct ReadonlyWrapper<T>
            : IReadOnlyList<T>
        {
            private readonly IList<T> _list;

            public int Count => _list.Count;

            public T this[int index] => _list[index];

            public ReadonlyWrapper(IList<T> list)
            {
                _list = list;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_list).GetEnumerator();
            }
        }
    }
}
