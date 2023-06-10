using LLama.Abstractions.Params;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama
{
    public interface ILLamaExecutor
    {
        IEnumerable<string> Infer(string text, SessionParams? sessionParams = null, IEnumerable<string>? antiprompts = null);
    }
}
