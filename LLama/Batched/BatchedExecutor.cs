using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LLama.Abstractions;
using LLama.Exceptions;
using LLama.Native;

namespace LLama.Batched;

/// <summary>
/// A batched executor that can infer multiple separate "conversations" simultaneously.
/// </summary>
public sealed class BatchedExecutor
    : IDisposable
{
    /// <summary>
    /// Tracks the sequence IDs currently in use by active conversations.
    /// This pool ensures that IDs are reused and never exceed the native backend's SeqMax allocation.
    /// </summary>
    private readonly HashSet<int> _activeSequenceIds = new();

    #region speculative
    private readonly Dictionary<LLamaSeqId, Conversation> _activeConversations = new();

    internal void RegisterConversation(Conversation conv)
    {
        lock (_activeSequenceIds)
        {
            _activeConversations[conv.ConversationId] = conv;
        }
    }
    #endregion

    /// <summary>
    /// Allocates the lowest available Sequence ID for a new conversation.
    /// </summary>
    /// <returns>A unique sequence ID bounded by the maximum number of concurrent active conversations.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no sequence IDs can be allocated.</exception>
    internal LLamaSeqId GetNextSequenceId()
    {
        // LOCK REQUIRED: Prevent race conditions if multiple conversations are created simultaneously
        lock (_activeSequenceIds)
        {
            // Linearly search for the lowest available ID.
            // Because IDs are recycled when conversations are disposed, this will naturally 
            // stay bounded below the host's maximum concurrency limit (SeqMax).
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (!_activeSequenceIds.Contains(i))
                {
                    _activeSequenceIds.Add(i);
                    return (LLamaSeqId)i;
                }
            }
        }

        // Fallback safety (practically unreachable unless int.MaxValue concurrent users are active)
        throw new InvalidOperationException("Failed to allocate a Sequence ID.");
    }

    /// <summary>
    /// Returns a Sequence ID to the pool so it can be reused by future conversations.
    /// This should be called exactly once when a Conversation is being disposed.
    /// </summary>
    /// <param name="id">The sequence ID to release.</param>
    internal void ReleaseSequenceId(LLamaSeqId id)
    {
        // LOCK REQUIRED: Prevent race conditions against GetNextSequenceId
        lock (_activeSequenceIds)
        {
            // Remove the ID from the active set, making it available for the next GetNextSequenceId() call
            _activeSequenceIds.Remove((int)id);

            // speculative
            _activeConversations.Remove(id);
        }
    }

    private readonly List<IBatch> _batchQueue = [];
    private string? _mtmdMarker;
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
    /// The optional <see cref="MtmdWeights"/> this executor is using
    /// </summary>
    public MtmdWeights? ClipModel { get; }

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

    // speculative
    /// <summary>
    /// The secondary context used to generate proposed tokens during speculative decoding.
    /// <para>In Dual-Model speculation, this context executes the smaller draft model. In Multi-Token Prediction (MTP) mode, it executes the MTP projection heads using the primary target model's weights.</para>
    /// </summary>
    public LLamaContext? DraftContext { get; }
    /// <summary>
    /// The internal native wrapper that orchestrates the speculative verification loop, handling draft proposals, target evaluations, and sequence rollbacks.
    /// </summary>
    private readonly LLama.Speculative.SpeculativeDecoder? _specDecoder;


    /// <summary>
    /// Creates a new batched executor capable of processing multiple concurrent conversation streams, with optional multimodal (CLIP) and speculative decoding acceleration.
    /// <para><b>Dual-Model Speculation:</b> If using two different models, the target and draft models must share the exact same tokenizer architecture and vocabulary size. A mismatch will cause immediate cache desynchronization crashes.</para>
    /// <para><b>Performance Note:</b> Speculative decoding requires both models (or the full MTP model) to fit entirely within GPU VRAM. Speedups are primarily seen on larger, memory-bandwidth-bound target models (e.g., 8B+).</para>
    /// </summary>
    /// <param name="model">The weights of the primary target model.</param>
    /// <param name="contextParams">The context parameters used to initialize the primary target context.</param>
    /// <param name="clipModel">The optional CLIP model weights to enable multimodal (image-to-text) capabilities.</param>
    /// <param name="draftModel">The weights of the draft model. <br/><b>Important:</b> In MTP mode, the executor will internally re-use the target <paramref name="model"/> weights for the draft context, therefore this may be <c>null</c>.</param>
    /// <param name="draftParams">The context parameters for the draft model. In MTP mode, ensure the <c>ContextType</c> property is explicitly set to <c>LLamaContextType.Mtp</c>.</param>
    /// <param name="draftTokens">The budget of draft tokens to propose per burst. Keep this modest (e.g., 2-4 for Dual-Model) or match the exact number of projection heads for MTP models. Set to 0 to disable speculation.</param>
    /// <param name="useMtp">Set to <c>true</c> to enable Multi-Token Prediction (Self-Speculation) for supported architectures (e.g., DeepSeek-R1, Qwen3.5-MTP). Requires <c>LoadMtp = true</c> in the target model parameters.</param>
    public BatchedExecutor(
        LLamaWeights model,
        IContextParams contextParams,
        MtmdWeights? clipModel = null,
        LLamaWeights? draftModel = null,
        IContextParams? draftParams = null,
        int draftTokens = 0,
        bool useMtp = false)
    {
        Model = model;
        Context = model.CreateContext(contextParams);
        ClipModel = clipModel;
        Epoch = 1;

        if (draftTokens > 0)
        {
            if (useMtp)
            {
                // MTP uses the target context directly. No second context needed!
                _specDecoder = new LLama.Speculative.SpeculativeDecoder(Context.NativeHandle, Context.NativeHandle, draftTokens, useMtp);
            }
            else
            {
                // Standard draft models require their own distinct context
                LLamaWeights activeDraftWeights = draftModel ?? model;
                DraftContext = activeDraftWeights.CreateContext(draftParams ?? contextParams);
                _specDecoder = new LLama.Speculative.SpeculativeDecoder(Context.NativeHandle, DraftContext.NativeHandle, draftTokens, useMtp);
            }
        }
    }

    /// <summary>
    /// Start a new <see cref="Conversation"/>
    /// </summary>
    /// <returns></returns>
    public Conversation Create()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(BatchedExecutor));

        // speculative
        var conv = new Conversation(this, GetNextSequenceId());
        RegisterConversation(conv);
        return conv;
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

            DecodeResult status;

            // speculative
            // Only use speculative decoding if the batch is exactly 1 token (Generation Phase)
            if (_specDecoder != null && next is TokenBatch tb && tb.Batch.TokenCount == 1)
            {
                try
                {
                    var results = _specDecoder.Decode(tb.Batch);
                    status = DecodeResult.Ok;

                    foreach (var result in results)
                    {
                        if (result.count > 0)
                        {
                            lock (_activeSequenceIds)
                            {
                                if (_activeConversations.TryGetValue((LLamaSeqId)result.seq_id, out var conv))
                                    conv.EnqueueSpeculativeTokens(result.tokens, result.count);
                            }
                        }
                    }
                }
                catch (LLamaDecodeError)
                {
                    status = DecodeResult.DecodeFailed;
                }
            }
            else
            {
                // STANDARD EXECUTION (Prefill Phase / Prompts)
                status = await next.DecodeAsync(Context, cancellation);

                // If this is a prompt, we MUST manually sync the draft model's KV cache so it doesn't fall behind!
                if (status == DecodeResult.Ok && DraftContext != null && _specDecoder != null && next is TokenBatch tbPrompt)
                {
                    await DraftContext.DecodeAsync(tbPrompt.Batch, cancellation);
                }
            }

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

        // speculative
        _specDecoder?.Dispose();
        DraftContext?.Dispose();

        Context.Dispose();
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
