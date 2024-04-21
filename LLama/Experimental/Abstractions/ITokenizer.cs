using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Abstractions
{
    /// <summary>
    /// The interface for tokenizer in LLamaSharp. It's responsible for converting text to token ids, or vice versa.
    /// </summary>
    public interface ITokenizer
    {
        // TODO: `ApplyChatTemplate` API

        // TODO: Batched Encode?

        /// <summary>
        /// Get the token ids from the text
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IList<int> Tokenize(string input);

        /// <summary>
        /// Convert the token ids to text.
        /// </summary>
        /// <param name="tokenIds"></param>
        /// <param name="result"></param>
        /// <param name="skipSpecialTokens"></param>
        /// <returns>The consumed tokens for decoding.</returns>
        int ConvertIdsToText(IEnumerable<int> tokenIds, out string result, bool skipSpecialTokens = false);

        // TODO: decode from Logprobs
    }
}
