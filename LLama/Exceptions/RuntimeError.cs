using System;

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
