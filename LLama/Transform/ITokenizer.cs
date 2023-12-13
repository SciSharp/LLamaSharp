using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Transform
{
    public interface ITokenizer
    {
        IEnumerable<int> Tokenize(LLamaContext context, string text, bool addBos = true, bool special = false);

        string Detokenize(LLamaContext context, int token);

        string Detokenize(LLamaContext context, IEnumerable<int> tokens);
    }
}
