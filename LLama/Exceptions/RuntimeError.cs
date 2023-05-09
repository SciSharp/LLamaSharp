using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Exceptions
{
    public class RuntimeError: Exception
    {
        public RuntimeError()
        {

        }

        public RuntimeError(string message): base(message)
        {

        }
    }
}
