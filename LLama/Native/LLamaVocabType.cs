using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Native
{
    public enum LLamaVocabType
    {
        LLAMA_VOCAB_TYPE_SPM = 0, // SentencePiece
        LLAMA_VOCAB_TYPE_BPE = 1, // Byte Pair Encoding
    };
}
