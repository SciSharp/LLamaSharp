using LLama.Experimental.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama.Experimental.Core.LLamaCpp
{
    /// <summary>
    /// llama.cpp tokenizer.
    /// </summary>
    public sealed class LLamaTokenizer: ITokenizer
    {
        private LLamaContext _context;

        public LLamaTokenizer(LLamaContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public IList<int> Tokenize(string input)
        {
            // TODO: refactor this!!
            return _context.Tokenize(input).Select(x => ((int)x)).ToArray();
        }

        /// <inheritdoc/>
        public int ConvertIdsToText(IEnumerable<int> tokenIds, out string result, bool skipSpecialTokens = false)
        {
            // TODO: integrate `StreamingDecoder` here. Currently only English has been supported.
            // We should add a byte array to `sequence`.
            result = _context.DeTokenize(tokenIds.Select(x => (Native.LLamaToken)x).ToArray());
            return tokenIds.Count();
        }
    }
}
