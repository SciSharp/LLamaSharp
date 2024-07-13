using System;
using System.Collections.Generic;

namespace LLama.Native;

/// <summary>
/// An embeddings batch allows submitting embeddings to multiple sequences simultaneously
/// </summary>
public class LLamaBatchEmbeddings
{
    private byte[] _logits;
    
    private float[] _embeddings;
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
    /// Size of an individual embedding
    /// </summary>
    public int EmbeddingDimensions { get; }

    /// <summary>
    /// The number of items in this batch
    /// </summary>
    public int EmbeddingsCount { get; private set; }
    
    /// <summary>
    /// Maximum number of items that can be added to this batch (automatically grows if exceeded)
    /// </summary>
    private int EmbeddingsCapacity { get; set; }
    
    /// <summary>
    /// Maximum number of sequences an item can be assigned to (automatically grows if exceeded)
    /// </summary>
    public int SequenceCapacity { get; private set; }

    /// <summary>
    /// Create a new batch for submitting inputs to llama.cpp
    /// </summary>
    public LLamaBatchEmbeddings(int embeddingDimensions)
    {
        // These can both be grown later, start off with reasonable numbers.
        const int embeddingsCapacity = 128;
        const int seqCapacity = 1;
        
        EmbeddingDimensions = embeddingDimensions;
        SequenceCapacity = seqCapacity;
        EmbeddingsCapacity = embeddingsCapacity;
        
        _logits = new byte[embeddingsCapacity];
        _embeddings = new float[embeddingsCapacity * embeddingDimensions];
        _positions = new LLamaPos[embeddingsCapacity];
        
        _sequenceIdCount = new int[embeddingsCapacity];
        _sequenceIdsPtrs = new IntPtr[_sequenceIdCount.Length];
        
        _sequenceIds = new LLamaSeqId[embeddingsCapacity][];
        for (var i = 0; i < _sequenceIds.Length; i++)
            _sequenceIds[i] = new LLamaSeqId[SequenceCapacity];
    }
    
    #region grow
    private void GrowEmbeddingsCapacity()
    {
        var embeddings = EmbeddingsCount * 2;
        EmbeddingsCapacity = embeddings;
        
        Array.Resize(ref _logits, embeddings);
        Array.Resize(ref _embeddings, embeddings * EmbeddingDimensions);
        Array.Resize(ref _positions, embeddings);
        
        Array.Resize(ref _sequenceIdCount, embeddings);
        Array.Resize(ref _sequenceIdsPtrs, embeddings);
        
        Array.Resize(ref _sequenceIds, embeddings);
        for (var i = 0; i < _sequenceIds.Length; i++)
        {
            // Growing the array filled elements with null, temporarily violating the nullability contract!
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            _sequenceIds[i] ??= new LLamaSeqId[SequenceCapacity];
        }
    }
    
    private void GrowMaxSequences(int atLeast)
    {
        var seqCount = Math.Max(SequenceCapacity * 2, atLeast);
        SequenceCapacity = seqCount;
        
        for (var i = 0; i < _sequenceIds.Length; i++)
            Array.Resize(ref _sequenceIds[i], SequenceCapacity);
    }
    #endregion

    internal GroupDisposable ToNativeBatch(out LLamaNativeBatch batch)
    {
        // This group holds all of the memory pins
        var group = new GroupDisposable();
        
        unsafe
        {
            batch = new LLamaNativeBatch
            {
                n_tokens = EmbeddingsCount,
                logits = (byte*)group.Add(_logits.AsMemory().Pin()).Pointer,
                
                n_seq_id = (int*)group.Add(_sequenceIdCount.AsMemory().Pin()).Pointer,
                pos = (LLamaPos*)group.Add(_positions.AsMemory().Pin()).Pointer,
                seq_id = (LLamaSeqId**)group.Add(_sequenceIdsPtrs.AsMemory().Pin()).Pointer,
                
                embd = (float*)group.Add(_embeddings.AsMemory().Pin()).Pointer,
                tokens = null,
            };
            
            // Create pointers to each of the arrays in turns
            for (var i = 0; i < _sequenceIdsPtrs.Length; i++)
                _sequenceIdsPtrs[i] = (IntPtr)group.Add(_sequenceIds[i].AsMemory().Pin()).Pointer;
        }
        
        return group;
    }

    #region Add
    /// <summary>
    /// Add a single embedding to the batch at the same position in several sequences
    /// </summary>
    /// <remarks>https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2</remarks>
    /// <param name="embedding">The embedding to add</param>
    /// <param name="pos">The position to add it att</param>
    /// <param name="sequences">The set of sequences to add this token to</param>
    /// <param name="logits"></param>
    /// <returns>The index that the token was added at. Use this for GetLogitsIth</returns>
    public int Add(ReadOnlySpan<float> embedding, LLamaPos pos, ReadOnlySpan<LLamaSeqId> sequences, bool logits)
    {
        if (embedding.Length != EmbeddingDimensions)
            throw new ArgumentException($"Embedding must have correct dimension (expected {EmbeddingDimensions}, actual {embedding.Length})", nameof(embedding));
        
        // Span<float> cannot be passed as a type parameter. Split the span up into a pointer/length to sneak
        // it through. This is only safe because we know the pointer will not be used after this call has returned.
        unsafe
        {
            fixed (float* srcPtr = embedding)
            {
                return Add(
                    ((IntPtr)srcPtr, embedding.Length),
                    (dest, src) => new Span<float>((float*)src.Item1.ToPointer(), src.Length).CopyTo(dest),
                    pos,
                    sequences,
                    logits
                );
            }
        }
    }

    /// <summary>
    /// Add a single embedding to the batch for a single sequence
    /// </summary>
    /// <param name="embedding"></param>
    /// <param name="pos"></param>
    /// <param name="sequence"></param>
    /// <param name="logits"></param>
    /// <returns>The index that the token was added at. Use this for GetLogitsIth</returns>
    public int Add(ReadOnlySpan<float> embedding, LLamaPos pos, LLamaSeqId sequence, bool logits)
    {
        Span<LLamaSeqId> seqs = stackalloc LLamaSeqId[] { sequence };
        return Add(
            embedding,
            pos,
            seqs,
            logits
        );
    }

    /// <summary>
    /// Called by embeddings batch to write embeddings into a destination span
    /// </summary>
    /// <typeparam name="TParam">Type of user data parameter passed in</typeparam>
    /// <param name="dest">Destination to write data to. Entire destination must be filled!</param>
    /// <param name="parameter">User data parameter passed in</param>
    public delegate void WriteEmbeddingsDelegate<in TParam>(Span<float> dest, TParam parameter);

    /// <summary>
    /// Add a single embedding to the batch at the same position in several sequences
    /// </summary>
    /// <remarks>https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2</remarks>
    /// <typeparam name="TParam">Type of userdata passed to write delegate</typeparam>
    /// <param name="parameter">Userdata passed to write delegate</param>
    /// <param name="write">Delegate called once to write data into a span</param>
    /// <param name="pos">Position to write this embedding to</param>
    /// <param name="sequences">All sequences to assign this embedding to</param>
    /// <param name="logits">Whether logits should be generated for this embedding</param>
    /// <returns>The index that the token was added at. Use this for GetLogitsIth</returns>
    public int Add<TParam>(TParam parameter, WriteEmbeddingsDelegate<TParam> write, LLamaPos pos, ReadOnlySpan<LLamaSeqId> sequences, bool logits)
    {
        // Grow capacity as necessary
        if (EmbeddingsCount == EmbeddingsCapacity)
            GrowEmbeddingsCapacity();
        if (sequences.Length > SequenceCapacity)
            GrowMaxSequences(sequences.Length);

        // Add the items to the arrays
        write(_embeddings.AsSpan(EmbeddingsCount * EmbeddingDimensions, EmbeddingDimensions), parameter);
        _positions[EmbeddingsCount] = pos;
        _sequenceIdCount[EmbeddingsCount] = sequences.Length;
        for (var i = 0; i < sequences.Length; i++)
            _sequenceIds[EmbeddingsCount][i] = sequences[i];
        _logits[EmbeddingsCount] = Convert.ToByte(logits);
        
        // Store this position in the logits lookup if necessary
        if (logits)
        {
            foreach (var sequence in sequences)
                _logitPositions.Add((sequence, EmbeddingsCount));
        }
        
        return EmbeddingsCount++;
    }
    
    /// <summary>
    /// Add a single embedding to the batch at a position for one sequence
    /// </summary>
    /// <remarks>https://github.com/ggerganov/llama.cpp/blob/ad939626577cd25b462e8026cc543efb71528472/common/common.cpp#L829C2-L829C2</remarks>
    /// <typeparam name="TParam">Type of userdata passed to write delegate</typeparam>
    /// <param name="parameter">Userdata passed to write delegate</param>
    /// <param name="write">Delegate called once to write data into a span</param>
    /// <param name="pos">Position to write this embedding to</param>
    /// <param name="sequence">Sequence to assign this embedding to</param>
    /// <param name="logits">Whether logits should be generated for this embedding</param>
    /// <returns>The index that the token was added at. Use this for GetLogitsIth</returns>
    public int Add<TParam>(TParam parameter, WriteEmbeddingsDelegate<TParam> write, LLamaPos pos, LLamaSeqId sequence, bool logits)
    {
        Span<LLamaSeqId> seqs = stackalloc LLamaSeqId[] { sequence };
        return Add(
            parameter,
            write,
            pos,
            seqs,
            logits
        );
    }
    #endregion

    /// <summary>
    /// Set EmbeddingsCount to zero for this batch
    /// </summary>
    public void Clear()
    {
        EmbeddingsCount = 0;
    }
    
    /// <summary>
    /// Get the positions where logits can be sampled from
    /// </summary>
    /// <returns></returns>
    internal Span<(LLamaSeqId, int)> GetLogitPositions(Span<(LLamaSeqId, int)> dest)
    {
        for (var i = 0; i < _logitPositions.Count; i++)
            dest[i] = _logitPositions[i];
        
        return dest.Slice(0, _logitPositions.Count);
    }
}