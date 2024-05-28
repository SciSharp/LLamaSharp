namespace LLama.Extensions;

/// <summary>
/// Extensions to the KeyValuePair struct
/// </summary>
internal static class KeyValuePairExtensions
{
#if NETSTANDARD2_0
    /// <summary>
    /// Deconstruct a KeyValuePair into it's constituent parts.
    /// </summary>
    /// <param name="pair">The KeyValuePair to deconstruct</param>
    /// <param name="first">First element, the Key</param>
    /// <param name="second">Second element, the Value</param>
    /// <typeparam name="TKey">Type of the Key</typeparam>
    /// <typeparam name="TValue">Type of the Value</typeparam>
    public static void Deconstruct<TKey, TValue>(this System.Collections.Generic.KeyValuePair<TKey, TValue> pair, out TKey first, out TValue second)
    {
        first = pair.Key;
        second = pair.Value;
    }
#elif !NET6_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER
    #error Target framework not supported!
#endif
}