using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    private readonly List<IBatch> _batchQueue = [ ];
    private string? _mtmdMarker;
    
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
    public int BatchedTokenCount => _batchQueue.Sum(a => a.ItemCount);

    /// <summary>
    /// Number of batches in the queue, waiting for <see cref="Infer"/> to be called
    /// </summary>
    public int BatchQueueCount => _batchQueue.Count;

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
        : this(model, contextParams, null)
    {
    }

    public BatchedExecutor(LLamaWeights model, IContextParams contextParams, MtmdWeights? clipModel)
    {
        Model = model;
        Context = model.CreateContext(contextParams);
        ClipModel = clipModel;
        Epoch = 1;
    }

    public MtmdWeights? ClipModel { get; }

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
                _batchQueue.Insert(0, next);
                return status;
            }
            
            // Everything was ok, advance the epoch
            Epoch++;
            
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
            if (_batchQueue.Count == 0)
                return null;
            
            var nextBatch = _batchQueue[0];
            _batchQueue.RemoveAt(0);
            return nextBatch;
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
        for (var i = 0; i < _batchQueue.Count; i++)
        {
            var item = _batchQueue[i];
            if (item is not TokenBatch { Batch: var batch })
                continue;

            var capacity = Context.BatchSize - batch.TokenCount;
            if (capacity < minCapacity)
                continue;

            if (batch.TokenCount < Context.BatchSize)
                return (batch, Epoch + (uint)(i + 1) * 2);
        }
        
        // Add a new batch to the end of the queue
        var end = new LLamaBatch();
        _batchQueue.Add(new TokenBatch(end));
        return (end, Epoch + (uint)_batchQueue.Count * 2);
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
        for (var i = 0; i < _batchQueue.Count; i++)
        {
            var item = _batchQueue[i];
            if (item is not EmbeddingBatch { Batch: var batch })
                continue;
            
            var capacity = Context.BatchSize - batch.EmbeddingsCount;
            if (capacity < minCapacity)
                continue;
            
            if (batch.EmbeddingsCount < Context.BatchSize)
                return (batch, Epoch + (uint)(i + 1) * 2);
        }
        
        // Add a new batch to the end of the queue
        var end = new LLamaBatchEmbeddings(Context.EmbeddingSize);
        _batchQueue.Add(new EmbeddingBatch(end));
        return (end, Epoch + (uint)_batchQueue.Count * 2);
    }

    internal ulong QueueMtmdBatch(Conversation conversation, Conversation.MtmdChunkSequence sequence)
    {
        if (ClipModel is null)
            throw new InvalidOperationException("This batched executor is not configured for multimodal inference.");

        var batch = new MtmdChunkBatch(ClipModel, conversation, sequence);
        _batchQueue.Add(batch);
        return Epoch + (uint)_batchQueue.Count * 2;
    }

    internal string GetMtmdMarker()
    {
        if (ClipModel is null)
            throw new InvalidOperationException("This batched executor is not configured for multimodal inference.");
        return _mtmdMarker ??= NativeApi.MtmdDefaultMarker() ?? "<media>";
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

    private class MtmdChunkBatch : IBatch
    {
        private readonly MtmdWeights _clipModel;
        private readonly Conversation _conversation;
        private readonly Conversation.MtmdChunkSequence _sequence;

        public MtmdChunkBatch(MtmdWeights clipModel, Conversation conversation, Conversation.MtmdChunkSequence sequence)
        {
            _clipModel = clipModel;
            _conversation = conversation;
            _sequence = sequence;
        }

        public int ItemCount => Math.Max(1, _sequence.TotalTokens);

        public Task<DecodeResult> DecodeAsync(LLamaContext ctx, CancellationToken token)
        {
            try
            {
                var nPast = _conversation.GetMtmdPast();
                var status = _clipModel.EvaluateChunks(_sequence.Chunks, ctx.NativeHandle, ref nPast,
                    (int)_conversation.ConversationId, checked((int)ctx.BatchSize), logitsLast: true);
                if (status != 0)
                {
                    _conversation.OnMtmdEvaluationFailed(status);
                    return Task.FromResult(DecodeResult.DecodeFailed);
                }

                _conversation.OnMtmdEvaluationCompleted(nPast, _sequence);
                return Task.FromResult(DecodeResult.Ok);
            }
            catch
            {
                _conversation.OnMtmdEvaluationFailed(-1);
                return Task.FromResult(DecodeResult.DecodeFailed);
            }
        }
    }
    #endregion
}
