using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Exceptions
{
    public class GrammarFormatException
        : Exception
    {
        public GrammarFormatException()
        {

        }

        public GrammarFormatException(string message)
            : base(message)
        {

        }
    }
}
