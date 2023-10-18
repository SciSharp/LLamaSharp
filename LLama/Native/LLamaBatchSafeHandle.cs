using System;

namespace LLama.Native;

using llama_token = Int32;

public sealed class LLamaBatchSafeHandle
    : SafeLLamaHandleBase
{
    private readonly int _embd;
    public LLamaNativeBatch Batch { get; private set; }

    /// <summary>
    /// the token ids of the input (used when embd is NULL)
    /// </summary>
    public Span<llama_token> Token
    {
        get
        {
            unsafe
            {
                if (_embd != 0)
                    return new Span<int>(null, 0);
                else
                    return new Span<int>(Batch.token, Batch.n_tokens);
            }
        }
    }

    /// <summary>
    /// token embeddings (i.e. float vector of size n_embd) (used when token is NULL)
    /// </summary>
    public Span<llama_token> Embed
    {
        get
        {
            unsafe
            {
                // If embd != 0, llama_batch.embd will be allocated with size of n_tokens *embd * sizeof(float)
                /// Otherwise, llama_batch.token will be allocated to store n_tokens llama_token

                if (_embd != 0)
                    return new Span<llama_token>(Batch.embd, Batch.n_tokens * _embd);
                else
                    return new Span<llama_token>(null, 0);
            }
        }
    }

    /// <summary>
    /// the positions of the respective token in the sequence
    /// </summary>
    public Span<LLamaPos> Pos
    {
        get
        {
            unsafe
            {
                return new Span<LLamaPos>(Batch.pos, Batch.n_tokens);
            }
        }
    }

    /// <summary>
    /// the sequence to which the respective token belongs
    /// </summary>
    public Span<LLamaSeqId> Sequence_ID
    {
        get
        {
            unsafe
            {
                return new Span<LLamaSeqId>(Batch.seq_id, Batch.n_tokens);
            }
        }
    }

    /// <summary>
    /// if zero, the logits for the respective token will not be output
    /// </summary>
    public Span<byte> Logits
    {
        get
        {
            unsafe
            {
                return new Span<byte>(Batch.logits, Batch.n_tokens);
            }
        }
    }

    public LLamaBatchSafeHandle(int n_tokens, int embd)
        : base((nint)1)
    {
        _embd = embd;
        Batch = NativeApi.llama_batch_init(n_tokens, embd);
    }

    protected override bool ReleaseHandle()
    {
        NativeApi.llama_batch_free(Batch);
        Batch = default;
        SetHandle(IntPtr.Zero);
        return true;
    }
}