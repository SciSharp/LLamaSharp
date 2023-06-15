using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Abstractions
{
    public interface ITextTransform
    {
        string Transform(string text);
    }
}
