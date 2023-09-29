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
public readonly unsafe struct LLamaNativeBatch
{
    /// <summary>
    /// The number of items pointed at by pos, seq_id and logits.
    /// </summary>
    public readonly int n_tokens;

    /// <summary>
    /// Either `n_tokens` of `llama_token`, or `NULL`, depending on how this batch was created
    /// </summary>
    public readonly llama_token* token;

    /// <summary>
    /// Either `n_tokens * embd * sizeof(float)` or `NULL`, depending on how this batch was created
    /// </summary>
    public readonly float* embd;

    /// <summary>
    /// the positions of the respective token in the sequence
    /// </summary>
    public readonly LLamaPos* pos;

    /// <summary>
    /// the sequence to which the respective token belongs
    /// </summary>
    public readonly LLamaSeqId* seq_id;

    /// <summary>
    /// if zero, the logits for the respective token will not be output
    /// </summary>
    public readonly byte* logits;
}