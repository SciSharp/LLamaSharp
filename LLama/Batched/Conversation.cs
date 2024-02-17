using System;
using System.Collections.Generic;
using LLama.Native;

namespace LLama.Batched;

/// <summary>
/// A single conversation thread that can be prompted (adding tokens from the user) or inferred (extracting a token from the LLM)
/// </summary>
public sealed class Conversation
    : IDisposable
{
    private ulong _requiredEpoch;
    private LLamaPos _end;
    private int _batchIndex;
    private bool _disposed;

    /// <summary>
    /// The executor which this conversation belongs to
    /// </summary>
    public BatchedExecutor Executor { get; }

    /// <summary>
    /// Unique ID for this conversation
    /// </summary>
    public LLamaSeqId ConversationId { get; }

    /// <summary>
    /// Total number of tokens in this conversation, cannot exceed the context length.
    /// </summary>
    public int TokenCount => _end.Value;

    /// <summary>
    /// Indicates if this conversation has been disposed, nothing can be done with a disposed conversation
    /// </summary>
    public bool IsDisposed => _disposed || Executor.IsDisposed;

    /// <summary>
    /// Indicates if this conversation is waiting for inference to be run on the executor. "Prompt" and "Sample" cannot be called when this is true.
    /// </summary>
    public bool RequiresInference => _requiredEpoch > Executor.Epoch;

    /// <summary>
    /// Indicates that this conversation should be sampled.
    /// </summary>
    public bool RequiresSampling => _requiredEpoch == Executor.Epoch;

    #region construction/destruction
    internal Conversation(BatchedExecutor batch, LLamaSeqId id, LLamaPos end)
    {
        ConversationId = id;
        Executor = batch;

        _end = end;
    }

    /// <summary>
    /// Finalizer for Conversation
    /// </summary>
    ~Conversation()
    {
        Dispose();
    }

    /// <summary>
    /// End this conversation, freeing all resources used by it
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Dispose()
    {
        if (IsDisposed)
            return;
        _disposed = true;

        // Remove this conversation from the KV cache
        Executor.Context.NativeHandle.KvCacheRemove(ConversationId, 0, _end);

        // Prevent finalizer from running
        GC.SuppressFinalize(this);
    }

    private void AssertNotDisposed()
    {
        if (Executor.IsDisposed)
            throw new ObjectDisposedException(nameof(BatchedExecutor));
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Conversation));
    }
    #endregion

    /// <summary>
    /// Create a copy of the current conversation
    /// </summary>
    /// <remarks>The copy shares internal state, so consumes very little extra memory.</remarks>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public Conversation Fork()
    {
        AssertNotDisposed();

        if (RequiresInference)
            throw new CannotForkWhileRequiresInferenceException();

        // Create a new conversation which references the current position in this one
        var c = new Conversation(Executor, Executor.GetNextSequenceId(), _end)
        {
            _batchIndex = _batchIndex,
            _requiredEpoch = _requiredEpoch,
        };

        // Assign tokens to the new sequence
        NativeApi.llama_kv_cache_seq_cp(Executor.Context.NativeHandle, ConversationId, c.ConversationId, 0, _end);

        return c;
    }

    #region sample
    /// <summary>
    /// Get the logits from this conversation, ready for sampling
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="CannotSampleRequiresPromptException">Thrown if this conversation was not prompted before the previous call to infer</exception>
    /// <exception cref="CannotSampleRequiresInferenceException">Thrown if Infer() must be called on the executor</exception>
    public ReadOnlySpan<float> Sample()
    {
        AssertNotDisposed();

        if (_requiredEpoch < Executor.Epoch)
            throw new CannotSampleRequiresPromptException();
        if (_requiredEpoch > Executor.Epoch)
            throw new CannotSampleRequiresInferenceException();

        return Executor.Context.NativeHandle.GetLogitsIth(_batchIndex);
    }
    #endregion

    #region prompt
    private void AssertCanBePrompted()
    {
        AssertNotDisposed();

        if (RequiresInference)
            throw new AlreadyPromptedConversationException();
    }

    /// <summary>
    /// Add tokens to this conversation
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public void Prompt(string input)
    {
        AssertCanBePrompted();

        Prompt(Executor.Context.Tokenize(input));
    }

    /// <summary>
    /// Add tokens to this conversation
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Prompt(IReadOnlyList<LLamaToken> tokens)
    {
        AssertCanBePrompted();

        // Add the prompt to the batch
        for (var i = 0; i < tokens.Count; i++)
            _batchIndex = Executor.Batch.Add(tokens[i], _end++, ConversationId, i == tokens.Count - 1);

        // Mark this conversation as needing inference/sampling
        _requiredEpoch = Executor.Epoch + 1;
    }

    /// <summary>
    /// Add a single token to this conversation
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void Prompt(LLamaToken token)
    {
        AssertCanBePrompted();

        // Add this token as input
        _batchIndex = Executor.Batch.Add(token, _end++, ConversationId, true);

        // Mark this conversation as needing inference/sampling
        _requiredEpoch = Executor.Epoch + 1;
    }
    #endregion

    #region modify
    /// <summary>
    /// Directly modify the KV cache of this conversation
    /// </summary>
    /// <param name="modifier"></param>
    /// <exception cref="CannotModifyWhileRequiresInferenceException">Thrown if this method is called while <see cref="Conversation.RequiresInference"/> == true</exception>
    public void Modify(ModifyKvCache modifier)
    {
        AssertNotDisposed();

        if (RequiresInference)
            throw new CannotModifyWhileRequiresInferenceException();

        // do whatever the modification is
        _end = modifier.Invoke(_end, new KvAccessor(this));

        // Set the epoch down to zero, this ensures that this conversation
        // cannot be sampled until it is prompted again.
        _requiredEpoch = 0;
    }

    /// <summary>
    /// Provides direct access to the KV cache of a <see cref="Conversation"/>.
    /// See <see cref="Modify"/> for how to use this.
    /// </summary>
    public readonly ref struct KvAccessor
    {
        private readonly Conversation _conversation;

        internal KvAccessor(Conversation conversation)
        {
            _conversation = conversation;
        }

        #region remove
        /// <summary>
        /// Removes all tokens that have positions in [start, end)
        /// </summary>
        /// <param name="start">Start position (inclusive)</param>
        /// <param name="end">End position (exclusive)</param>
        public void Remove(LLamaPos start, LLamaPos end)
        {
            _conversation.Executor.Context.NativeHandle.KvCacheRemove(_conversation.ConversationId, start, end);
        }

        /// <summary>
        /// Removes all tokens starting from the given position
        /// </summary>
        /// <param name="start">Start position (inclusive)</param>
        /// <param name="count">Number of tokens</param>
        public void Remove(LLamaPos start, int count)
        {
            if (count <= 0)
                return;

            var end = start.Value + count;
            _conversation.Executor.Context.NativeHandle.KvCacheRemove(_conversation.ConversationId, start, end);
        }
        #endregion

        #region shift
        /// <summary>
        /// Adds relative position "delta" to all tokens that have positions in [p0, p1).
        /// If the KV cache is RoPEd, the KV data is updated
        /// accordingly
        /// </summary>
        /// <param name="start">Start position (inclusive)</param>
        /// <param name="end">End position (exclusive)</param>
        /// <param name="delta">Amount to add on to each token position</param>
        public void Shift(LLamaPos start, LLamaPos end, int delta)
        {
            _conversation.Executor.Context.NativeHandle.KvCacheSequenceShift(_conversation.ConversationId, start, end, delta);
        }
        #endregion

        #region divide
        /// <summary>
        /// Integer division of the positions by factor of `d > 1`.
        /// If the KV cache is RoPEd, the KV data is updated accordingly.
        /// </summary>
        /// <param name="start">Start position (inclusive). If less than zero, it is clamped to zero.</param>
        /// <param name="end">End position (exclusive). If less than zero, it is treated as "infinity".</param>
        /// <param name="divisor">Amount to divide each position by.</param>
        public void Divide(LLamaPos start, LLamaPos end, int divisor)
        {
            if (divisor <= 0)
                throw new ArgumentOutOfRangeException(nameof(divisor));

            _conversation.Executor.Context.NativeHandle.KvCacheSequenceDivide(_conversation.ConversationId, start, end, divisor);
        }
        #endregion
    }

    /// <summary>
    /// A function which can temporarily access the KV cache of a <see cref="Conversation"/> to modify it directly
    /// </summary>
    /// <param name="end">The current end token of this conversation</param>
    /// <param name="kv">An <see cref="KvAccessor"/> which allows direct access to modify the KV cache</param>
    /// <returns>The new end token position</returns>
    public delegate LLamaPos ModifyKvCache(LLamaPos end, KvAccessor kv);
    #endregion
}