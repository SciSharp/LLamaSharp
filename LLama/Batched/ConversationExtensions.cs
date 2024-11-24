using System;
using LLama.Native;

namespace LLama.Batched;

/// <summary>
/// Extension method for <see cref="Conversation"/>
/// </summary>
public static class ConversationExtensions
{
    /// <summary>
    /// Sample a token from this conversation using the given sampler chain
    /// </summary>
    /// <param name="conversation"><see cref="Conversation"/> to sample from</param>
    /// <param name="sampler"></param>
    /// <param name="offset">Offset from the end of the conversation to the logits to sample, see <see cref="Conversation.GetSampleIndex"/> for more details</param>
    /// <returns></returns>
    public static LLamaToken Sample(this Conversation conversation, SafeLLamaSamplerChainHandle sampler, int offset = 0)
    {
        return sampler.Sample(conversation.Executor.Context.NativeHandle, conversation.GetSampleIndex(offset));
    }

    /// <summary>
    /// Rewind a <see cref="Conversation"/> back to an earlier state by removing tokens from the end
    /// </summary>
    /// <param name="conversation">The conversation to rewind</param>
    /// <param name="tokens">The number of tokens to rewind</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if `tokens` parameter is larger than TokenCount</exception>
    public static void Rewind(this Conversation conversation, int tokens)
    {
        if (tokens > conversation.TokenCount)
            throw new ArgumentOutOfRangeException(nameof(tokens), "Cannot rewind more than the total number of tokens");

        conversation.Modify((end, kv) =>
        {
            // Remove those tokens from KV
            kv.Remove(end.Value - tokens, tokens);

            // Return adjusted end position
            return end.Value - tokens;
        });
    }

    /// <summary>
    /// Shift all tokens over to the left, removing "count" tokens from the start and shifting everything over.
    /// Leaves "keep" tokens at the start completely untouched. This can be used to free up space when the context
    /// gets full, keeping the prompt at the start intact.
    /// </summary>
    /// <param name="conversation">The conversation to rewind</param>
    /// <param name="count">How much to shift tokens over by</param>
    /// <param name="keep">The number of tokens at the start which should <b>not</b> be shifted</param>
    public static void ShiftLeft(this Conversation conversation, int count, int keep)
    {
        // Given a setup like this (shift=5, keep=3):
        //
        // AAABBBBBCCCCCCCCC...
        // 
        // We want to remove all the B's, shift all the C's and leave all the A's untouched

        conversation.Modify((end, kv) =>
        {
            // Remove the B's
            kv.Remove(keep, count);

            // Shift the C's
            kv.Add(keep + count, end, -count);

            // Update total count
            return end.Value - count;
        });
    }
}