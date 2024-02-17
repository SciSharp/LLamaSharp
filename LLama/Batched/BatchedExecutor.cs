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

    internal LLamaBatch Batch { get; }

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
        Batch = new LLamaBatch();
        Context = model.CreateContext(contextParams);
        Epoch = 1;
    }

    /// <summary>
    /// Finalizer for BatchedExecutor
    /// </summary>
    ~BatchedExecutor()
    {
        Dispose();
    }

    /// <summary>
    /// Start a new <see cref="Conversation"/> with the given prompt
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public Conversation Prompt(string prompt)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(BatchedExecutor));

        var conversation = new Conversation(this, GetNextSequenceId(), 0);
        conversation.Prompt(prompt);

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

        var status = await Context.DecodeAsync(Batch, cancellation);

        // Only clear the batch if the result was ok. leaving all this state in place means that "Infer" can
        // be called again after a warning (e.g. NoKvSlot).
        if (status == DecodeResult.Ok)
        {
            Epoch++;
            Batch.Clear();
        }

        return status;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        GC.SuppressFinalize(this);

        Context.Dispose();
    }

    internal LLamaSeqId GetNextSequenceId()
    {
        return checked((LLamaSeqId)_nextSequenceId++);
    }
}