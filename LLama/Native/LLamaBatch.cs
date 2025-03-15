using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.HighPerformance.Buffers;

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
    /// Keep a list of where logits can be sampled from
    /// </summary>
    private readonly List<(LLamaSeqId, int)> _logitPositions = new();

    /// <summary>
    /// Get the number of logit positions that will be generated from this batch
    /// </summary>
    internal int LogitPositionCount => _logitPositions.Count;

    /// <summary>
    /// The number of tokens in this batch
    /// </summary>
    public int TokenCount { get; private set; }

    /// <summary>
    /// Maximum number of tokens that can be added to this batch (automatically grows if exceeded)
    /// </summary>
    private int TokenCapacity { get; set; }

    /// <summary>
    /// Maximum number of sequences a token can be assigned to (automatically grows if exceeded)
    /// </summary>
    public int SequenceCapacity { get; private set; }

    /// <summary>
    /// Create a new batch for submitting inputs to llama.cpp
    /// </summary>
    public LLamaBatch()
    {
        // These can both be grown later, start off with reasonable numbers.
        const int tokensCapacity = 128;
        const int seqCapacity = 1;

        SequenceCapacity = seqCapacity;
        TokenCapacity = tokensCapacity;

        _logits = new byte[tokensCapacity];
        _tokens = new LLamaToken[tokensCapacity];
        _positions = new LLamaPos[tokensCapacity];

        _sequenceIdCount = new int[tokensCapacity];
        _sequenceIdsPtrs = new IntPtr[_sequenceIdCount.Length];

        _sequenceIds = new LLamaSeqId[tokensCapacity][];
        for (var i = 0; i < _sequenceIds.Length; i++)
            _sequenceIds[i] = new LLamaSeqId[SequenceCapacity];
    }

    #region grow
    private void GrowTokenCapacity()
    {
        var tokenCapacity = TokenCount * 2;
        TokenCapacity = tokenCapacity;

        Array.Resize(ref _logits, tokenCapacity);
        Array.Resize(ref _tokens, tokenCapacity);
        Array.Resize(ref _positions, tokenCapacity);

        Array.Resize(ref _sequenceIdCount, tokenCapacity);
        Array.Resize(ref _sequenceIdsPtrs, tokenCapacity);

        Array.Resize(ref _sequenceIds, tokenCapacity);
        for (var i = 0; i < _sequenceIds.Length; i++)
        {
            // Growing the array filled elements with null, temporarily violating the nullability contract!
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            _sequenceIds[i] ??= new LLamaSeqId[SequenceCapacity];
        }
    }

    private void GrowMaxSequences(int atLeast)
    {
        var seqCapacity = Math.Max(SequenceCapacity * 2, atLeast);
        SequenceCapacity = seqCapacity;

        for (var i = 0; i < _sequenceIds.Length; i++)
            Array.Resize(ref _sequenceIds[i], SequenceCapacity);
    }
    #endregion

    internal GroupDisposable ToNativeBatch(out LLamaNativeBatch batch)
    {
        // Sanity checking
#if DEBUG
        // Check every output logit position is generating logits for exactly one sequence
        foreach (var (seq, idx) in _logitPositions)
        {
            Debug.Assert(_logits[idx] != 0);
            Debug.Assert(_sequenceIdCount[idx] == 1);
            Debug.Assert(_sequenceIds[idx][0] == seq);
        }

        // Check every index, if it's generating logits it must be in the _logitPositions list. Otherwise it must not.
        for (var i = 0; i < _logits.Length; i++)
        {
            var actual = _logitPositions.Any(x => x.Item2 == i);
            var expected = _logits[i] != 0;
            Debug.Assert(actual == expected, $"Expected {actual} == {expected} @ index:{i}");
        }
#endif

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

    #region add
    /// <summary>
    /// Add a single token to the batch at the same position in several sequences
    /// </summary>
    /// <remarks>https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2</remarks>
    /// <param name="token">The token to add</param>
    /// <param name="pos">The position to add it att</param>
    /// <param name="sequences">The set of sequences to add this token to</param>
    /// <param name="logits"></param>
    /// <returns>The index that the token was added at. Use this for GetLogitsIth</returns>
    public int Add(LLamaToken token, LLamaPos pos, ReadOnlySpan<LLamaSeqId> sequences, bool logits)
    {
        // Grow capacity as necessary
        if (TokenCount == TokenCapacity)
            GrowTokenCapacity();
        if (sequences.Length > SequenceCapacity)
            GrowMaxSequences(sequences.Length);

        // Add the items to the arrays
        _tokens[TokenCount] = token;
        _positions[TokenCount] = pos;
        _sequenceIdCount[TokenCount] = sequences.Length;
        for (var i = 0; i < sequences.Length; i++)
            _sequenceIds[TokenCount][i] = sequences[i];
        _logits[TokenCount] = Convert.ToByte(logits);

        // Store this position in the logits lookup if necessary
        if (logits)
        {
            foreach (var sequence in sequences)
                _logitPositions.Add((sequence, TokenCount));
        }

        return TokenCount++;
    }

    /// <summary>
    /// Add a single token to the batch at the same position in several sequences
    /// </summary>
    /// <remarks>https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2</remarks>
    /// <param name="token">The token to add</param>
    /// <param name="pos">The position to add it att</param>
    /// <param name="sequences">The set of sequences to add this token to</param>
    /// <param name="logits"></param>
    /// <returns>The index that the token was added at. Use this for GetLogitsIth</returns>
    public int Add(LLamaToken token, LLamaPos pos, List<LLamaSeqId> sequences, bool logits)
    {
#if NET5_0_OR_GREATER
        var seqSpan = CollectionsMarshal.AsSpan(sequences);
        return Add(token, pos, seqSpan, logits);
#else
        // on netstandard2.0 we can't use CollectionsMarshal to get directly at the internal memory of
        // the list. Instead rent an array and copy the data into it. This avoids an allocation, but can't
        // avoid the copying.

        using var rented = SpanOwner<LLamaSeqId>.Allocate(sequences.Count);
        sequences.CopyTo(rented.Span);
        return Add(token, pos, rented.Span, logits);

#endif
    }

    /// <summary>
    /// Add a single token to the batch at a certain position for a single sequences
    /// </summary>
    /// <remarks>https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2</remarks>
    /// <param name="token">The token to add</param>
    /// <param name="pos">The position to add it att</param>
    /// <param name="sequence">The sequence to add this token to</param>
    /// <param name="logits"></param>
    /// <returns>The index that the token was added at. Use this for GetLogitsIth</returns>
    public int Add(LLamaToken token, LLamaPos pos, LLamaSeqId sequence, bool logits)
    {
        // Create a temporary span to contain 1 item without allocating
        Span<LLamaSeqId> sequences = stackalloc LLamaSeqId[1];
        sequences[0] = sequence;

        // Add it
        return Add(token, pos, sequences, logits);
    }

    /// <summary>
    /// Add a range of tokens to a single sequence, start at the given position.
    /// </summary>
    /// <param name="tokens">The tokens to add</param>
    /// <param name="start">The starting position to add tokens at</param>
    /// <param name="sequence">The sequence to add this token to</param>
    /// <param name="logitsLast">Whether the final token should generate logits</param>
    /// <returns>The index that the final token was added at. Use this for GetLogitsIth</returns>
    public int AddRange(ReadOnlySpan<LLamaToken> tokens, LLamaPos start, LLamaSeqId sequence, bool logitsLast)
    {
        var last = -1;
        for (var i = 0; i < tokens.Length; i++)
        {
            var logits = (i == tokens.Length - 1) & logitsLast;
            last = Add(tokens[i], start.Value + i, sequence, logits);
        }

        return last;
    }
    #endregion

    /// <summary>
    /// Set TokenCount to zero for this batch
    /// </summary>
    public void Clear()
    {
        TokenCount = 0;

        _logitPositions.Clear();

        Array.Clear(_logits, 0, _logits.Length);
        Array.Clear(_tokens, 0, _tokens.Length);
        Array.Clear(_positions, 0, _positions.Length);
        Array.Clear(_sequenceIdsPtrs, 0, _sequenceIdsPtrs.Length);

        foreach (var seqIds in _sequenceIds)
            Array.Clear(seqIds, 0, seqIds.Length);
    }

    /// <summary>
    /// Get the positions where logits can be sampled from
    /// </summary>
    /// <returns></returns>
    internal IReadOnlyList<(LLamaSeqId, int)> GetLogitPositions()
    {
        return _logitPositions;
    }
}