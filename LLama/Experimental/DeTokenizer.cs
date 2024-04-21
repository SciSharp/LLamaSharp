using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using LLama.Experimental.Abstractions;
using LLama.Experimental.Common;
using LLama.Extensions;

namespace LLama.Experimental
{
    /// <summary>
    /// Defines the process of converting sequence output to text.
    /// 
    /// We should not expose this class to users. Implementing <see cref="ITokenizer"/> 
    /// should be the only thing the user need to concern to customize the tokenizing and detokenizing.
    /// </summary>
    internal static class DeTokenizer
    {
        private static int INITIAL_INCREMENTAL_DETOKENIZATION_OFFSET = 5;

        /// <summary>
        /// Decodes the new token for a sequence. In-place operation.
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="tokenizer"></param>
        /// <param name="samplingMethod"></param>
        public static void DecodeSequenceInplace(Sequence seq, ITokenizer tokenizer, ISamplingMethod samplingMethod)
        {
            var allInputIds = seq.TokenIds;
            var (offset, text) = DetokenizeIncrementally(tokenizer, allInputIds, seq.IncrementalDecodingOffset, skipSpecialTokens: true);

            // TODO: deal with logprobs.

            seq.IncrementalDecodingOffset = offset;
            seq.OutputText += text;
        }

        private static (int, string) DetokenizeIncrementally(ITokenizer tokenizer, IEnumerable<int> allInputIds, int offset, bool skipSpecialTokens = false)
        {
            var consumedTokens = tokenizer.ConvertIdsToText(allInputIds.Skip(offset), out var text, skipSpecialTokens);
            offset += consumedTokens;
            return (offset, text);
        }
    }
}
