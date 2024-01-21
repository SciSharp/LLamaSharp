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
            throw new CannotForkWhileRequiresInference();

        // Assign tokens to the new sequence
        var id2 = Executor.GetNextSequenceId();
        NativeApi.llama_kv_cache_seq_cp(Executor.Context.NativeHandle, ConversationId, id2, 0, _end);

        // Create a new conversation which references the current position in this one
        var c = new Conversation(Executor, id2, _end)
        {
            _batchIndex = _batchIndex,
            _requiredEpoch = _requiredEpoch,
        };

        return c;
    }

    /// <summary>
    /// Rewind this conversation back to an earlier state
    /// </summary>
    /// <param name="tokens"></param>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="CannotForkWhileRequiresInference"></exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if `tokens` parameter is larger than NTokens</exception>
    public void Rewind(int tokens)
    {
        AssertNotDisposed();

        if (tokens > TokenCount)
            throw new ArgumentOutOfRangeException(nameof(tokens), "Cannot rewind more than the total number of tokens");

        // Remove those tokens from KV
        Executor.Context.NativeHandle.KvCacheRemove(ConversationId, _end.Value - tokens, _end);

        // Adjust "end" marker back
        _end = _end.Value - tokens;
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
}