using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

using llama_token = Int32;

/// <summary>
/// Input data for llama_decode
/// A llama_batch object can contain input about one or many sequences
/// The provided arrays (i.e. token, embd, pos, etc.) must have size of n_tokens
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct LLamaNativeBatch
{
    private int n_tokens;
    private llama_token* token;
    private float* embd;
    private int* pos;
    private int* seq_id;
    private byte* logits;

    /// <summary>
    /// Size of all other properties
    /// </summary>
    public int TokenCount => n_tokens;

    ///// <summary>
    ///// the token ids of the input (used when embd is NULL)
    ///// </summary>
    //public Span<llama_token> Token => new Span<llama_token>(token, n_tokens);

    ///// <summary>
    ///// token embeddings (i.e. float vector of size n_embd) (used when token is NULL)
    ///// </summary>
    //public Span<float> Embd => new Span<float>(embd, n_tokens);

    /// <summary>
    /// the positions of the respective token in the sequence
    /// </summary>
    public Span<int> Pos => new Span<llama_token>(pos, n_tokens);

    /// <summary>
    /// the sequence to which the respective token belongs
    /// </summary>
    public Span<int> SeqId => new Span<llama_token>(seq_id, n_tokens);

    /// <summary>
    /// if zero, the logits for the respective token will not be output
    /// </summary>
    public Span<byte> Logits => new Span<byte>(logits, n_tokens);
}