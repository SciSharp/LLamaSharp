namespace LLama.Native;

/// <summary>
/// Input data for llama_encode/llama_decode
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
    /// (if set to NULL, the token position will be tracked automatically by llama_encode/llama_decode)
    /// </summary>
    public LLamaPos* pos;

    /// <summary>
    /// https://github.com/ggerganov/llama.cpp/blob/master/llama.h#L139 ???
    /// </summary>
    public int* n_seq_id;

    /// <summary>
    /// the sequence to which the respective token belongs
    /// (if set to NULL, the sequence ID will be assumed to be 0)
    /// </summary>
    public LLamaSeqId** seq_id;

    /// <summary>
    /// if zero, the logits for the respective token will not be output.
    /// If set to NULL:
    /// <list type="bullet">
    ///  <item>If embeddings: all tokens are output</item>
    ///  <item>If not: only the last token is output</item>
    /// </list>
    /// </summary>
    public byte* logits;
}