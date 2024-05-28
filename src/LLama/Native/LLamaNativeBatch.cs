using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// Input data for llama_decode
/// A llama_batch object can contain input about one or many sequences
/// The provided arrays (i.e. token, embd, pos, etc.) must have size of n_tokens
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct LLamaNativeBatch
{
    /// <summary>
    /// The number of items pointed at by pos, seq_id and logits.
    /// </summary>
    public int n_tokens;

    /// <summary>
    /// Either `n_tokens` of `llama_token`, or `NULL`, depending on how this batch was created
    /// </summary>
    public LLamaToken* tokens;

    /// <summary>
    /// Either `n_tokens * embd * sizeof(float)` or `NULL`, depending on how this batch was created
    /// </summary>
    public float* embd;

    /// <summary>
    /// the positions of the respective token in the sequence
    /// </summary>
    public LLamaPos* pos;

    /// <summary>
    /// https://github.com/ggerganov/llama.cpp/blob/master/llama.h#L139 ???
    /// </summary>
    public int* n_seq_id;

    /// <summary>
    /// the sequence to which the respective token belongs
    /// </summary>
    public LLamaSeqId** seq_id;

    /// <summary>
    /// if zero, the logits for the respective token will not be output
    /// </summary>
    public byte* logits;

    // Note from llama.cpp:
    // > helpers for smooth API transition - can be deprecated in the future
    // > for future-proof code, use the above fields instead and ignore everything below
    private LLamaPos _all_pos_0;
    private LLamaPos _all_pos_1;
    private LLamaSeqId _all_seq_id;
}