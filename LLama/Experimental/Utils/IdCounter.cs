using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Utils
{
    internal class IdCounter
    {
        private int _number;

        public IdCounter(int start = 0)
        {
            _number = start;
        }

        public int Next()
        {
            return _number++;
        }

        public void Reset()
        {
            _number = 0;
        }
    }
}
