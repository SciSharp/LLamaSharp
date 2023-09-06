using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using LLama.Native;

namespace LLama.Extensions
{
    internal static class IReadOnlyListExtensions
    {
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
        internal static bool TokensEndsWithAnyString<TList>(this TList tokens, IReadOnlyList<string> queries, SafeLlamaModelHandle model, Encoding encoding)
            where TList : IReadOnlyList<int>
        {
            if (queries.Count == 0 || tokens.Count == 0)
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
    }
}
