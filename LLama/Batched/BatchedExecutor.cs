using System;
using System.Collections.Generic;
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
    private readonly List<LLamaBatch> _batchQueue = [ ];
    
    /// <summary>
    /// Held while inference is running
    /// </summary>
    private readonly object _inferenceLock = new();

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
    public int BatchedTokenCount => _batchQueue.Sum(a => a.TokenCount);

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
        
        // Take the inference lock, if this fails it's because inference is already running.
        if (!Monitor.TryEnter(_inferenceLock))
            throw new InvalidOperationException("Cannot start inference while it is already running");
        try
        {
            // Advance epoch by one. This ensures that _nothing_ can be sampled while inference is running.
            // Only do this if the epoch is odd. If it's even that means it was previously advanced by another
            // inference run, and this run is a retry.
            if ((Epoch & 1) == 1)
                Epoch++;

            // Run the actual inference. This is the slow bit!
            var status = await Context.DecodeAsync(next, cancellation);

            // If there was an error then early exit without incrementing the epoch. This allows infer to be called
            // again after the issue has been fixed (e.g. some KV cache space has been freed) to retry this operation.
            if (status != DecodeResult.Ok)
            {
                _batchQueue.Insert(0, next);
                return status;
            }
            
            // Everything was ok, advance the epoch and clear the batch we just ran inference for.
            Epoch++;
            next.Clear();
            
            return status;
        }
        finally
        {
            Monitor.Exit(_inferenceLock);
        }
        
        LLamaBatch? GetNextBatch()
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
            var capacity = Context.BatchSize - _batchQueue[i].TokenCount;
            if (capacity < minCapacity)
                continue;

            if (_batchQueue[i].TokenCount < Context.BatchSize)
                return (_batchQueue[i], Epoch + (uint)(i + 1) * 2);
        }
        
        // Add a new batch to the end of the queue
        var end = new LLamaBatch();
        _batchQueue.Add(end);
        return (end, Epoch + (uint)_batchQueue.Count * 2);
    }
}