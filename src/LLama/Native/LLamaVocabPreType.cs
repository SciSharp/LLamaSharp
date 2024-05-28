namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_vocab_pre_type</remarks>
internal enum LLamaVocabPreType
{
    LLAMA_VOCAB_PRE_TYPE_DEFAULT = 0,
    LLAMA_VOCAB_PRE_TYPE_LLAMA3 = 1,
    LLAMA_VOCAB_PRE_TYPE_DEEPSEEK_LLM = 2,
    LLAMA_VOCAB_PRE_TYPE_DEEPSEEK_CODER = 3,
    LLAMA_VOCAB_PRE_TYPE_FALCON = 4,
    LLAMA_VOCAB_PRE_TYPE_MPT = 5,
    LLAMA_VOCAB_PRE_TYPE_STARCODER = 6,
    LLAMA_VOCAB_PRE_TYPE_GPT2 = 7,
}