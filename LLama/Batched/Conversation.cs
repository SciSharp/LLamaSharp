using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CommunityToolkit.HighPerformance.Buffers;
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
    private bool _disposed;

    /// <summary>
    /// Indicates if this conversation has been "forked" and may share logits with another conversation.
    /// </summary>
    private bool _forked;

    /// <summary>
    /// Stores the indices to sample from. Contains <see cref="_batchSampleCount"/> valid items.
    /// </summary>
    private int[] _batchSampleIndices = new int[4];
    private int _batchSampleCount;

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
    internal Conversation(BatchedExecutor batch, LLamaSeqId id)
    {
        ConversationId = id;
        Executor = batch;
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
        Executor.Context.NativeHandle.MemorySequenceRemove(ConversationId, -1, -1);

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

        // Create a new conversation which references the current position in this one
        var c = new Conversation(Executor, Executor.GetNextSequenceId())
        {
            // Because these values are copied to the forked conversation it means that it will share the exact same output
            // logits next time sampling is done. This is a problem, because the sampling process is allowed to modify those
            // logits, so sampling one conversation may mess up the fork! Setting the "forked" flag on both sequences ensures
            // they both copy the logits before the next sampling run, to fix this issue.
            _requiredEpoch = _requiredEpoch,
            _batchSampleIndices = _batchSampleIndices.ToArray(),
            _batchSampleCount = _batchSampleCount,
            _forked = true,

            _end = _end,
        };

        // Setting this flag means that logits will be copied next time sampling is called, ensuring that the forked
        // conversation doesn't share logits with this one.
        _forked = true;

        // Assign tokens to the new sequence
        Executor.Context.NativeHandle.MemorySequenceCopy(ConversationId, c.ConversationId, 0, _end);

        return c;
    }

    #region sample
    /// <summary>
    /// Get the index in the context which each token can be sampled from, the return value of this function get be used to retrieve logits
    /// (<see cref="SafeLLamaContextHandle.GetLogitsIth"/>) or to sample a token (<see cref="SafeLLamaSamplerChainHandle.Sample"/>.
    /// </summary>
    /// <param name="offset">How far from the <b>end</b> of the previous prompt should logits be sampled. Any value other than 0 requires
    /// allLogits to have been set during prompting.<br />
    /// For example if 5 tokens were supplied in the last prompt call:
    /// <list type="bullet">
    ///     <item>The logits of the first token can be accessed with 4</item>
    ///     <item>The logits of the second token can be accessed with 3</item>
    ///     <item>The logits of the third token can be accessed with 2</item>
    ///     <item>The logits of the fourth token can be accessed with 1</item>
    ///     <item>The logits of the fifth token can be accessed with 0</item>
    /// </list>
    /// </param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="CannotSampleRequiresPromptException">Thrown if this conversation was not prompted before the previous call to infer</exception>
    /// <exception cref="CannotSampleRequiresInferenceException">Thrown if Infer() must be called on the executor</exception>
    public int GetSampleIndex(int offset = 0)
    {
        AssertNotDisposed();

        if (_requiredEpoch < Executor.Epoch)
            throw new CannotSampleRequiresPromptException();
        if (_requiredEpoch > Executor.Epoch)
            throw new CannotSampleRequiresInferenceException();
        if (offset >= _batchSampleCount)
            throw new ArgumentException("Cannot sample offset more than the previous prompt count", nameof(offset));

        return _batchSampleIndices[_batchSampleCount - offset - 1];
    }

    /// <summary>
    /// Get the logits from this conversation, ready for sampling
    /// </summary>
    /// <param name="offset">How far from the <b>end</b> of the previous prompt should logits be sampled. Any value other than 0 requires allLogits to have been set during prompting</param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="CannotSampleRequiresPromptException">Thrown if this conversation was not prompted before the previous call to infer</exception>
    /// <exception cref="CannotSampleRequiresInferenceException">Thrown if Infer() must be called on the executor</exception>
    public Span<float> Sample(int offset = 0)
    {
        AssertNotDisposed();

        if (_requiredEpoch < Executor.Epoch)
            throw new CannotSampleRequiresPromptException();
        if (_requiredEpoch > Executor.Epoch)
            throw new CannotSampleRequiresInferenceException();
        if (offset >= _batchSampleCount)
            throw new ArgumentException("Cannot sample offset more than the previous prompt count", nameof(offset));

        var index = GetSampleIndex(offset);
        var span = Executor.Context.NativeHandle.GetLogitsIth(index);

        // If necessary copy the span, to protect it from modification. This is only done when
        // this conversation has been forked in this epoch.
        if (_forked)
            span = span.ToArray();

        return span;
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
    /// <param name="tokens"></param>
    /// <param name="allLogits">If true, generate logits for all tokens. If false, only generate logits for the last token.</param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="AlreadyPromptedConversationException"></exception>
    public void Prompt(List<LLamaToken> tokens, bool allLogits = false)
    {
        AssertCanBePrompted();

#if NET6_0_OR_GREATER
        var span = CollectionsMarshal.AsSpan(tokens);
        Prompt(span, allLogits);
#else
        // Borrow an array and copy tokens into it
        using var span = SpanOwner<LLamaToken>.Allocate(tokens.Count);

        for (var i = 0; i < tokens.Count; i++)
            span.Span[i] = tokens[i];

        Prompt(span.Span);

#endif
    }
    
    /// <summary>
    /// Add tokens to this conversation
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="allLogits">If true, generate logits for all tokens. If false, only generate logits for the last token.</param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="AlreadyPromptedConversationException"></exception>
    public void Prompt(ReadOnlySpan<LLamaToken> tokens, bool allLogits = false)
    {
        AssertCanBePrompted();

        // No point doing anything if there is no actual prompt!
        if (tokens.Length == 0)
            return;

        // Add the prompt to the batch
        if (allLogits)
        {
            if (_batchSampleIndices.Length < tokens.Length)
                _batchSampleIndices = new int[tokens.Length];
            
            _batchSampleCount = tokens.Length;
            
            // We need to add all tokens to a single batch, so they can all be sampled at once.
            // Request a batch with sufficient space.
            (var batch, _requiredEpoch) = Executor.GetTokenBatch(tokens.Length);
            
            // Add everything to that batch
            for (var i = 0; i < tokens.Length; i++)
                _batchSampleIndices[i] = batch.Add(tokens[i], _end++, ConversationId, true);
        }
        else
        {
            _batchSampleCount = 1;
            
            while (tokens.Length > 0)
            {
                // Get a batch with capacity for at least 1 token
                (var batch, _requiredEpoch) = Executor.GetTokenBatch();
                
                // Add as many tokens as possible
                var count = Math.Min(tokens.Length, checked((int)Executor.Context.BatchSize) - batch.TokenCount);
                for (var i = 0; i < count; i++)
                    _batchSampleIndices[0] = batch.Add(tokens[i], _end++, ConversationId, i == tokens.Length - 1);
                
                // Slice the array to remove tokens we've already added to a batch
                tokens = tokens.Slice(count);
            }
        }

        // Unset the forked flag. Since this conversation has just been prompted it's no longer
        // sharing anything with any other conversations.
        _forked = false;
    }

    /// <summary>
    /// Add a single token to this conversation
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="AlreadyPromptedConversationException"></exception>
    public void Prompt(LLamaToken token)
    {
        AssertCanBePrompted();
        
        Span<LLamaToken> span = [ token ];
        Prompt(span);
    }

    /// <summary>
    /// Prompt this conversation with an image embedding
    /// </summary>
    /// <param name="embedding"></param>
    public void Prompt(SafeLlavaImageEmbedHandle embedding)
    {
        AssertCanBePrompted();

        if (embedding.Model.EmbeddingDimensions != Executor.Model.EmbeddingSize)
            throw new ArgumentException($"Embedding dimension mismatch between image embedding ({embedding.Model.EmbeddingDimensions}) and model ({Executor.Model.EmbeddingSize})");

        for (var i = 0; i < embedding.Model.PatchCount; i++)
        {
            // Get a batch with space
            (var batch, _requiredEpoch) = Executor.GetEmbeddingBatch();
                
            batch.Add(
                (i, embedding),
                static (Span<float> dest, (int index, SafeLlavaImageEmbedHandle embedding) tup) => tup.embedding.GetEmbedding(dest, tup.index),
                _end++,
                ConversationId,
                i == embedding.Model.PatchCount - 1
            );
        }
    }

    /// <summary>
    /// Prompt this conversation with embeddings
    /// </summary>
    /// <param name="embeddings">The raw values of the embeddings. This span must divide equally by the embedding size of this model.</param>
    public void Prompt(ReadOnlySpan<float> embeddings)
    {
        AssertCanBePrompted();

        var dim = Executor.Model.EmbeddingSize;
        var count = embeddings.Length / dim;
        if (count * dim != embeddings.Length)
            throw new ArgumentException($"Incorrect embeddings span size, length ({embeddings.Length}) must be divisible by embedding dimensions ({Executor.Model.EmbeddingSize})");

        while (embeddings.Length > 0)
        {
            // Get a batch with space
            (var batch, _requiredEpoch) = Executor.GetEmbeddingBatch();

            // Add 1 embedding to the batch
            batch.Add(
                embeddings.Slice(0, dim),
                _end++,
                ConversationId,
                embeddings.Length == dim
            );

            // Advance to next embedding
            embeddings = embeddings.Slice(dim);
        }
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
            _conversation.Executor.Context.NativeHandle.MemorySequenceRemove(_conversation.ConversationId, start, end);
        }

        /// <summary>
        /// Removes <see cref="count"/> tokens starting from <see cref="start"/>
        /// </summary>
        /// <param name="start">Start position (inclusive)</param>
        /// <param name="count">Number of tokens</param>
        public void Remove(LLamaPos start, int count)
        {
            if (count <= 0)
                return;

            var end = start.Value + count;
            _conversation.Executor.Context.NativeHandle.MemorySequenceRemove(_conversation.ConversationId, start, end);
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
        public void Add(LLamaPos start, LLamaPos end, int delta)
        {
            _conversation.Executor.Context.NativeHandle.MemorySequenceAdd(_conversation.ConversationId, start, end, delta);
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

            _conversation.Executor.Context.NativeHandle.MemorySequenceDivide(_conversation.ConversationId, start, end, divisor);
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

    #region save/load
    private void AssertCanLoad()
    {
        AssertNotDisposed();
        if (_end.Value > 0)
            throw new InvalidOperationException("Cannot load into a non-empty conversation");
    }

    private void AssertCanSave()
    {
        AssertNotDisposed();
        if (RequiresInference)
            throw new CannotSaveWhileRequiresInferenceException();
    }


    /// <summary>
    /// Save the complete state of this conversation to a file. if the file already exists it will be overwritten.
    /// </summary>
    /// <param name="filepath"></param>
    /// <exception cref="CannotSaveWhileRequiresInferenceException"></exception>
    public void Save(string filepath)
    {
        AssertCanSave();

        // Prepare extra state to put into file header
        var state = GetState();
        var bytes = JsonSerializer.SerializeToUtf8Bytes(state);

        // Save extra state along with the KV cache
        Executor.Context.SaveState(filepath, ConversationId, bytes);
    }

    /// <summary>
    /// Save the complete state of this conversation in system memory.
    /// </summary>
    /// <returns></returns>
    public State Save()
    {
        AssertCanSave();

        return new PrivateState(
            Executor.Context.GetState(ConversationId),
            GetState()
        );
    }


    /// <summary>
    /// Load state from a file
    /// This should only ever be called by the BatchedExecutor, on a newly created conversation object!
    /// </summary>
    /// <param name="filepath"></param>
    /// <exception cref="InvalidOperationException"></exception>
    internal void Load(string filepath)
    {
        AssertCanLoad();

        // Load the state from file into the KV cache
        Executor.Context.LoadState(filepath, ConversationId, out var header);

        // deserialize the extra state in the file header
        var state = JsonSerializer.Deserialize<SerializableConversationState>(header);
        if (state == null)
        {
            Dispose();
            throw new InvalidOperationException("Failed to deserialize - deserialized header state was null");
        }

        Load(state);
    }

    /// <summary>
    /// Load state from a previously saved state.
    /// This should only ever be called by the BatchedExecutor, on a newly created conversation object!
    /// </summary>
    /// <param name="state"></param>
    internal void Load(State state)
    {
        AssertCanLoad();

        // There is only one class that extends State and it is PrivateState, so this cast is safe.
        var priv = (PrivateState)state;

        // Load the state from file into the KV cache
        Executor.Context.LoadState(priv.SequenceState, ConversationId);

        Load(priv.ConversationState);
    }


    private void Load(SerializableConversationState state)
    {
        if (state.Version != 1)
            throw new InvalidOperationException("Failed to deserialize - mismatched version number");

        // Load extra conversation state
        _end = state.TokenCount;
    }

    private SerializableConversationState GetState()
    {
        return new SerializableConversationState(
            Version: 1,
            TokenCount: TokenCount
        );
    }


    private record SerializableConversationState(int Version, int TokenCount);

    private sealed class PrivateState
        : State
    {
        public readonly LLamaContext.SequenceState SequenceState;
        public readonly SerializableConversationState ConversationState;

        public override ulong Size => SequenceState.Size;

        public PrivateState(LLamaContext.SequenceState sequenceState, SerializableConversationState conversationState)
        {
            SequenceState = sequenceState;
            ConversationState = conversationState;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(State));
            IsDisposed = true;

            SequenceState.Dispose();
        }
    }

    /// <summary>
    /// In memory saved state of a <see cref="Conversation"/>
    /// </summary>
    public abstract class State
        : IDisposable
    {
        /// <summary>
        /// Indicates if this state has been disposed
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Get the size in bytes of this state object
        /// </summary>
        public abstract ulong Size { get; }

        /// <inheritdoc />
        public abstract void Dispose();

        /// <summary>
        /// Internal constructor prevent anyone outside of LLamaSharp extending this class
        /// </summary>
        internal State()
        {
        }
    }
    #endregion
}