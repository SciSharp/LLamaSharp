using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LLama.Abstractions;
using LLama.Native;

namespace LLama.Batched;

/// <summary>
/// A batched executor that can infer multiple separate "conversations" simultaneously.
/// </summary>
public sealed class BatchedExecutor
    : IDisposable
{
    private int _nextSequenceId;
    private readonly List<IBatch> _batchQueue = [];
    private int _batchQueueHead;
    private int _batchedTokenCount;
    private bool _batchedTokenCountDirty = true;
    // Skip compacting the queue until this many processed batches accumulate at the front.
    private const int CleanupThreshold = 16;
    
    /// <summary>
    /// Set to 1 using interlocked exchange while inference is running
    /// </summary>
    private int _inferenceLock = 0;

    /// <summary>
    /// Epoch is incremented twice every time Infer is called. Conversations can use this to keep track of
    /// whether they're waiting for inference, or can be sampled.
    /// </summary>
    internal ulong Epoch { get; private set; }

    /// <summary>
    /// The <see cref="LLamaContext"/> this executor is using
    /// </summary>
    public LLamaContext Context { get; }

    /// <summary>
    /// The <see cref="LLamaWeights"/> this executor is using
    /// </summary>
    public LLamaWeights Model { get; }
    
    /// <summary>
    /// Get the number of tokens in the batch, waiting for <see cref="Infer"/> to be called
    /// </summary>
    public int BatchedTokenCount
    {
        get
        {
            if (_batchedTokenCountDirty)
            {
                var total = 0;
                for (var i = _batchQueueHead; i < _batchQueue.Count; i++)
                    total += _batchQueue[i].ItemCount;
                _batchedTokenCount = total;
                _batchedTokenCountDirty = false;
            }

            return _batchedTokenCount;
        }
    }

    /// <summary>
    /// Number of batches in the queue, waiting for <see cref="Infer"/> to be called
    /// </summary>
    public int BatchQueueCount => _batchQueue.Count - _batchQueueHead;

    /// <summary>
    /// Check if this executor has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Create a new batched executor
    /// </summary>
    /// <param name="model">The model to use</param>
    /// <param name="contextParams">Parameters to create a new context</param>
    public BatchedExecutor(LLamaWeights model, IContextParams contextParams)
    {
        Model = model;
        Context = model.CreateContext(contextParams);
        Epoch = 1;
    }

    /// <summary>
    /// Start a new <see cref="Conversation"/>
    /// </summary>
    /// <returns></returns>
    public Conversation Create()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(BatchedExecutor));

        return new Conversation(this, GetNextSequenceId());
    }

    /// <summary>
    /// Load a conversation that was previously saved to a file. Once loaded the conversation will
    /// need to be prompted.
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public Conversation Load(string filepath)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(BatchedExecutor));

        var conversation = Create();
        conversation.Load(filepath);
        return conversation;
    }

    /// <summary>
    /// Load a conversation that was previously saved into memory. Once loaded the conversation will need to be prompted.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public Conversation Load(Conversation.State state)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(BatchedExecutor));

        var conversation = Create();
        conversation.Load(state);
        return conversation;
    }

    /// <summary>
    /// Run inference for all conversations in the batch which have pending tokens.
    ///
    /// If the result is `NoKvSlot` then there is not enough memory for inference, try disposing some conversation
    /// threads and running inference again.
    /// </summary>
    public async Task<DecodeResult> Infer(CancellationToken cancellation = default)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(BatchedExecutor));
        
        // If there's no work to do then we successfully completed all available work! immediately exit.
        var next = GetNextBatch();
        if (next == null)
            return DecodeResult.Ok;

        // This acts as a "lock" on inference, ensuring two inferences cannot run at once. First set the "_inferenceLock" field
        // to the "key" value iff it is currently 0. If it is not currently 0 this will throw an exception.
        var key = (int)(DateTime.UtcNow.Ticks & 0xFFFF_FFFF);
        if (Interlocked.CompareExchange(ref _inferenceLock, key, 0) != 0)
            throw new InvalidOperationException("Cannot start inference while it is already running");
        try
        {
            // Advance epoch by one. This ensures that _nothing_ can be sampled while inference is running.
            // Only do this if the epoch is odd. If it's even that means it was previously advanced by another
            // inference run, and this run is a retry.
            if ((Epoch & 1) == 1)
                Epoch++;

            // Run the actual inference. This is the slow bit!
            var status = await next.DecodeAsync(Context, cancellation);

            // If there was an error then early exit without incrementing the epoch. This allows infer to be called
            // again after the issue has been fixed (e.g. some KV cache space has been freed) to retry this operation.
            if (status != DecodeResult.Ok)
            {
                RequeueFront(next);
                return status;
            }
            
            // Everything was ok, advance the epoch
            Epoch++;
            CleanupQueue();
            
            return status;
        }
        finally
        {
            // Set "_inferenceLock" field back to zero iff it is currently the "key" value we set earlier. It should be
            // impossible for this to ever fail!
            var old = Interlocked.CompareExchange(ref _inferenceLock, 0, key);
            Debug.Assert(old == key);
        }
        
        IBatch? GetNextBatch()
        {
            if (_batchQueueHead >= _batchQueue.Count)
            {
                _batchQueue.Clear();
                _batchQueueHead = 0;
                return null;
            }

            var nextBatch = _batchQueue[_batchQueueHead];
            _batchQueueHead++;
            _batchedTokenCountDirty = true;
            return nextBatch;
        }

        void RequeueFront(IBatch batch)
        {
            Debug.Assert(_batchQueueHead > 0, "Cannot requeue batch when queue head is at zero.");
            _batchQueue[--_batchQueueHead] = batch;
            _batchedTokenCountDirty = true;
        }

        // Remove batches that have already been consumed so the head index does not grow without bound.
        void CleanupQueue()
        {
            if (_batchQueueHead == 0)
                return;

            if (_batchQueueHead >= _batchQueue.Count)
            {
                _batchQueue.Clear();
                _batchQueueHead = 0;
                return;
            }

            if (_batchQueueHead > CleanupThreshold && _batchQueueHead > _batchQueue.Count / 2)
            {
                _batchQueue.RemoveRange(0, _batchQueueHead);
                _batchQueueHead = 0;
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        Context.Dispose();
    }

    internal LLamaSeqId GetNextSequenceId()
    {
        return checked((LLamaSeqId)_nextSequenceId++);
    }
    
    /// <summary>
    /// Get a reference to a batch that tokens can be added to.
    /// </summary>
    /// <param name="minCapacity"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal (LLamaBatch batch, ulong epoch) GetTokenBatch(int minCapacity = 1)
    {
        if (minCapacity > Context.BatchSize)
            throw new ArgumentOutOfRangeException(nameof(minCapacity), $"Request batch capacity must be less than or equal to BatchSize ({Context.BatchSize})");

        // Find a batch with space for at least minCapacity tokens
        for (var i = _batchQueueHead; i < _batchQueue.Count; i++)
        {
            var item = _batchQueue[i];
            if (item is not TokenBatch { Batch: var batch })
                continue;

            var capacity = Context.BatchSize - batch.TokenCount;
            if (capacity < minCapacity)
                continue;

            if (batch.TokenCount < Context.BatchSize)
            {
                _batchedTokenCountDirty = true;
                return (batch, Epoch + (uint)(i - _batchQueueHead + 1) * 2);
            }
        }
        
        // Add a new batch to the end of the queue
        var end = new LLamaBatch();
        _batchQueue.Add(new TokenBatch(end));
        _batchedTokenCountDirty = true;
        return (end, Epoch + (uint)(_batchQueue.Count - _batchQueueHead) * 2);
    }
    
    /// <summary>
    /// Get a reference to a batch that embeddings can be added to.
    /// </summary>
    /// <param name="minCapacity"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal (LLamaBatchEmbeddings batch, ulong epoch) GetEmbeddingBatch(int minCapacity = 1)
    {
        if (minCapacity > Context.BatchSize)
            throw new ArgumentOutOfRangeException(nameof(minCapacity), $"Request batch capacity must be less than or equal to BatchSize ({Context.BatchSize})");
        
        // Find a batch with space for at least minCapacity embeddings
        for (var i = _batchQueueHead; i < _batchQueue.Count; i++)
        {
            var item = _batchQueue[i];
            if (item is not EmbeddingBatch { Batch: var batch })
                continue;
            
            var capacity = Context.BatchSize - batch.EmbeddingsCount;
            if (capacity < minCapacity)
                continue;
            
            if (batch.EmbeddingsCount < Context.BatchSize)
            {
                _batchedTokenCountDirty = true;
                return (batch, Epoch + (uint)(i - _batchQueueHead + 1) * 2);
            }
        }
        
        // Add a new batch to the end of the queue
        var end = new LLamaBatchEmbeddings(Context.EmbeddingSize);
        _batchQueue.Add(new EmbeddingBatch(end));
        _batchedTokenCountDirty = true;
        return (end, Epoch + (uint)(_batchQueue.Count - _batchQueueHead) * 2);
    }

    #region batches
    private interface IBatch
    {
        int ItemCount { get; }
        
        Task<DecodeResult> DecodeAsync(LLamaContext ctx, CancellationToken token);
    }
    
    private class TokenBatch(LLamaBatch batch)
        : IBatch
    {
        public readonly LLamaBatch Batch = batch;
        public int ItemCount => Batch.TokenCount;

        public Task<DecodeResult> DecodeAsync(LLamaContext ctx, CancellationToken token)
        {
            return ctx.DecodeAsync(Batch, token);
        }
    }
    
    private class EmbeddingBatch(LLamaBatchEmbeddings batch)
        : IBatch
    {
        public readonly LLamaBatchEmbeddings Batch = batch;
        public int ItemCount => Batch.EmbeddingsCount;

        public Task<DecodeResult> DecodeAsync(LLamaContext ctx, CancellationToken token)
        {
            return ctx.DecodeAsync(Batch, token);
        }
    }
    #endregion
}
