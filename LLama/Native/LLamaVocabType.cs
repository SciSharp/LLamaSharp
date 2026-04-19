namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_vocab_type</remarks>
public enum LLamaVocabType
{
    /// <summary>
    /// For models without vocab
    /// </summary>
    None = 0,

    /// <summary>
    /// LLaMA tokenizer based on byte-level BPE with byte fallback
    /// </summary>
    SentencePiece = 1,

    /// <summary>
    /// GPT-2 tokenizer based on byte-level BPE
    /// </summary>
    BytePairEncoding = 2,

    /// <summary>
    /// BERT tokenizer based on WordPiece
    /// </summary>
    WordPiece = 3,

    /// <summary>
    /// T5 tokenizer based on Unigram
    /// </summary>
    Unigram = 4,

    /// <summary>
    /// RWKV tokenizer based on greedy tokenization
    /// </summary>
    RWKV = 5,

    /// <summary>
    /// PLaMo-2 tokenizer based on Aho-Corasick with dynamic programming
    /// </summary>
    PLAMO2 = 6
}