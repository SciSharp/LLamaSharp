using System;

namespace LLama.Native;

/// <summary>
/// A batch allows submitting multiple tokens to multiple sequences simultaneously
/// </summary>
public class LLamaBatch
{
    private byte[] _logits;

    private LLamaToken[] _tokens;
    private LLamaPos[] _positions;

    private int[] _sequenceIdCount;
    private LLamaSeqId[][] _sequenceIds;
    private IntPtr[] _sequenceIdsPtrs;

    /// <summary>
    /// The number of tokens in this batch
    /// </summary>
    public int TokenCount { get; private set; }

    /// <summary>
    /// Maximum number of tokens that can be added to this batch
    /// </summary>
    private int TokenCapacity { get; set; }

    /// <summary>
    /// Maximum number of sequences a token can be assigned to
    /// </summary>
    public int MaxSequences { get; private set; }

    /// <summary>
    /// Create a new batch for submitting inputs to llama.cpp
    /// </summary>
    public LLamaBatch()
    {
        // These can both be grown later, start off with reasonable numbers.
        const int n_tokens = 128;
        const int n_seq_max = 1;

        MaxSequences = n_seq_max;
        TokenCapacity = n_tokens;

        _logits = new byte[n_tokens];
        _tokens = new LLamaToken[n_tokens];
        _positions = new LLamaPos[n_tokens];

        _sequenceIdCount = new int[n_tokens];
        _sequenceIdsPtrs = new IntPtr[_sequenceIdCount.Length];

        _sequenceIds = new LLamaSeqId[n_tokens][];
        for (var i = 0; i < _sequenceIds.Length; i++)
            _sequenceIds[i] = new LLamaSeqId[MaxSequences];
    }

    private void GrowTokenCapacity()
    {
        var n_tokens = TokenCount * 2;
        TokenCapacity = n_tokens;

        Array.Resize(ref _logits, n_tokens);
        Array.Resize(ref _tokens, n_tokens);
        Array.Resize(ref _positions, n_tokens);

        Array.Resize(ref _sequenceIdCount, n_tokens);
        Array.Resize(ref _sequenceIdsPtrs, n_tokens);

        Array.Resize(ref _sequenceIds, n_tokens);
        for (int i = 0; i < _sequenceIds.Length; i++)
        {
            // Growing the array filled elements with null, temporarily violating the nullability contract!
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (_sequenceIds[i] == null)
                _sequenceIds[i] = new LLamaSeqId[MaxSequences];
        }
    }

    private void GrowMaxSequences(int atLeast)
    {
        var n_seq = Math.Max(MaxSequences * 2, atLeast);
        MaxSequences = n_seq;

        for (var i = 0; i < _sequenceIds.Length; i++)
            Array.Resize(ref _sequenceIds[i], MaxSequences);
    }

    internal GroupDisposable ToNativeBatch(out LLamaNativeBatch batch)
    {
        // This group holds all of the memory pins
        var group = new GroupDisposable();

        unsafe
        {
            batch = new LLamaNativeBatch
            {
                n_tokens = TokenCount,
                logits = (byte*)group.Add(_logits.AsMemory().Pin()).Pointer,

                n_seq_id = (int*)group.Add(_sequenceIdCount.AsMemory().Pin()).Pointer,
                pos = (LLamaPos*)group.Add(_positions.AsMemory().Pin()).Pointer,
                seq_id = (LLamaSeqId**)group.Add(_sequenceIdsPtrs.AsMemory().Pin()).Pointer,

                // embd is not currently supported, so this is always null!
                embd = null,

                // Note that if embd is **not null** then this will be null!
                tokens = (LLamaToken*)group.Add(_tokens.AsMemory().Pin()).Pointer,
            };

            // Create pointers to each of the arrays in turns
            for (var i = 0; i < _sequenceIdsPtrs.Length; i++)
                _sequenceIdsPtrs[i] = (IntPtr)group.Add(_sequenceIds[i].AsMemory().Pin()).Pointer;
        }

        return group;
    }

    /// <summary>
    /// Add a single token to the batch at the same position in several sequences
    /// </summary>
    /// <remarks>https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2</remarks>
    /// <param name="token">The token to add</param>
    /// <param name="pos">The position to add it att</param>
    /// <param name="sequences">The set of sequences to add this token to</param>
    /// <param name="logits"></param>
    public void Add(LLamaToken token, LLamaPos pos, ReadOnlySpan<LLamaSeqId> sequences, bool logits)
    {
        if (TokenCount == TokenCapacity)
            GrowTokenCapacity();
        if (sequences.Length > MaxSequences)
            GrowMaxSequences(sequences.Length);

        _tokens[TokenCount] = token;
        _positions[TokenCount] = pos;

        _sequenceIdCount[TokenCount] = sequences.Length;
        for (var i = 0; i < sequences.Length; i++)
            _sequenceIds[TokenCount][i] = sequences[i];

        _logits[TokenCount] = Convert.ToByte(logits);

        TokenCount++;
    }

    /// <summary>
    /// Add a single token to the batch at a certain position for a single sequences
    /// </summary>
    /// <remarks>https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2</remarks>
    /// <param name="token">The token to add</param>
    /// <param name="pos">The position to add it att</param>
    /// <param name="sequence">The sequence to add this token to</param>
    /// <param name="logits"></param>
    public void Add(LLamaToken token, LLamaPos pos, LLamaSeqId sequence, bool logits)
    {
        // Create a temporary span to contain 1 item without allocating
        Span<LLamaSeqId> sequences = stackalloc LLamaSeqId[1];
        sequences[0] = sequence;

        // Add it
        Add(token, pos, sequences, logits);
    }

    /// <summary>
    /// Set TokenCount to zero for this batch
    /// </summary>
    public void Clear()
    {
        TokenCount = 0;
    }
}