using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Transform
{
    /// <summary>
    /// The default tokenizer of LLamaSharp. This class should not be inherited.
    /// <b>Note that this class has state. The previous outputs feeded to it will affect its control.</b>
    /// If you use it in a session, please don't reuse it for another session unless you intend to do so.
    /// </summary>
    public sealed class DefaultTokenizer: ITokenizer
    {
        private Encoding _encoding;
        private StreamingTokenDecoder _tokenDecoder;

        /// <summary>
        /// Initialize a new tokenizer with the specified encoding.
        /// </summary>
        /// <param name="encoding"></param>
        public DefaultTokenizer(Encoding encoding)
        {
            _encoding = encoding;
            _tokenDecoder = new StreamingTokenDecoder(encoding);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEnumerable<int> Tokenize(LLamaContext context, string text, bool addBos = true, bool special = false)
        {
            return context.Tokenize(text, addBos, special);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Detokenize(LLamaContext context, int token)
        {
            _tokenDecoder.Add(token, context.NativeHandle.ModelHandle);
            return _tokenDecoder.Read();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Detokenize(LLamaContext context, IEnumerable<int> tokens)
        {
            _tokenDecoder.AddRange(tokens, context.NativeHandle.ModelHandle);
            return _tokenDecoder.Read();
        }
    }
}
