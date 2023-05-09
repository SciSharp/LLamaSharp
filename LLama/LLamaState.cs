using System;
using System.Collections.Generic;
using System.Text;

namespace LLama
{
    using llama_token = Int32;
    public record LLamaState(Queue<llama_token> EvalTokens, Queue<float[]> EvalLogits,
        byte[] State, int Size);
}
