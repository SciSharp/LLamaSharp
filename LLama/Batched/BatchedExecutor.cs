using System;
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
    
    private LLamaBatch _promptingBatch = new();
    private LLamaBatch _nextBatch = new();
    internal LLamaBatch Batch => _promptingBatch;

    /// <summary>
    /// Epoch is incremented every time Infer is called. Conversations can use this to keep track of
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
    public int BatchedTokenCount => Batch.TokenCount;

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
        
        // Swap over batches. This means the next batch can be filled with
        // tokens while inference is still running for the previous one.
        var batch = _promptingBatch;
        (_promptingBatch, _nextBatch) = (_nextBatch, _promptingBatch);

        var status = await Context.DecodeAsync(batch, cancellation);
        
        // If there was an error swap the previous batch back into place. This allows infer to be called again
        // after the issue has been fixed (e.g. some KV cache space has been freed) to "retry" this operation.
        if (status != DecodeResult.Ok)
        {
            (_promptingBatch, _nextBatch) = (_nextBatch, _promptingBatch);
            return status;
        }
        
        // Everything was ok, advance the epoch and clear the batch we just ran inference for.
        Epoch++;
        batch.Clear();
        
        return status;
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
}