using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LLama.Abstractions;
using LLama.Native;

namespace LLama.Batched;

struct AsyncMutex : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AsyncMutex()
    {
    }

    public void Wait()
    {
        _semaphore.Wait();
    }

    public void Release()
    {
        _semaphore.Release(1);
    }

    public async Task WithMutexAsync(Func<Task> t)
    {
        await _semaphore.WaitAsync();

        try
        {
            await t();
        }
        finally
        {
            _semaphore.Release(1);
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}

/// <summary>
/// A batched executor that can infer multiple separate "conversations" simultaneously.
/// </summary>
public sealed class BatchedExecutor
    : IDisposable
{
    private int _nextSequenceId;

    private readonly AsyncMutex _mutex = new AsyncMutex();

    private ConcurrentQueue<LLamaBatch> _batchQueue;
    private readonly ConcurrentDictionary<(ulong Epoch, int BatchIndex), float[]> _logitCache;

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
    public int BatchedTokenCount
    {
        get
        {
            var count = 0;
            foreach (var batch in _batchQueue)
                count += batch.TokenCount;
            return count;
        }
    }

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
        _batchQueue = new ConcurrentQueue<LLamaBatch>();
        Context = model.CreateContext(contextParams);
        Epoch = 1;
        _logitCache = new ConcurrentDictionary<(ulong Epoch, int BatchIndex), float[]>();
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
    /// <param name="processAll">If true processes the whole Queue, otherwise only single batch</param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public async Task<DecodeResult> Infer(bool processAll = true, CancellationToken cancellation = default)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(BatchedExecutor));
        DecodeResult status = DecodeResult.Ok;
        await _mutex.WithMutexAsync(async () =>
        {
            if (processAll)
            {
                while (_batchQueue.TryPeek(out var batch))
                {
                    status = await Context.DecodeAsync(batch, cancellation);
                    if (status != DecodeResult.Ok)
                        break;
                    if (status == DecodeResult.Ok)
                    {
                        Epoch++;
                        _batchQueue.TryDequeue(out _);
                        AddToLogitCache(batch);
                        batch.Clear();
                    }
                }
                
            }
            else
            {
                if (_batchQueue.TryPeek(out var batch))
                {
                    var status = await Context.DecodeAsync(batch, cancellation);
                    if (status == DecodeResult.Ok)
                    {
                        Epoch++;
                        _batchQueue.TryDequeue(out _);
                        AddToLogitCache(batch);
                        batch.Clear();
                    }
                }
            }
        });
        return status;
    }

    internal float[] GetLogits(ulong epoch, int batchIndex)
    {
        if (_logitCache.TryGetValue((epoch, batchIndex), out var logits))
            return logits;
        throw new InvalidOperationException("Logits not found in cache");
    }

    /// <summary>
    /// Add a token to the batch for the given conversation.
    /// </summary>
    /// <param name="conversation"></param>
    /// <param name="token"></param>
    /// <param name="endIndex"></param>
    /// <param name="ConversationId"></param>
    /// <param name="logits"></param>
    /// <returns></returns>
    internal (int batchIndex, ulong requiredEpochs) AddTokenToConversation(
        LLamaToken token, LLamaPos endIndex, LLamaSeqId ConversationId, bool logits)
    {
        _mutex.Wait();
        if (!_batchQueue.TryPeek(out var batch) || batch.TokenCount >= Context.Params.BatchSize)
        {
            batch = new LLamaBatch();
            _batchQueue.Enqueue(batch);
        }
        var batchIndex = batch.Add(token, endIndex, ConversationId, logits);
        var requiredEpochs = (ulong) _batchQueue.Count;
        _mutex.Release();
        return (batchIndex, requiredEpochs);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        foreach (var batch in _batchQueue)
            batch.Clear();
        _batchQueue = null;
        _mutex.Dispose();
        GC.SuppressFinalize(this);

        Context.Dispose();
    }

    internal LLamaSeqId GetNextSequenceId()
    {
        var id = checked((LLamaSeqId)_nextSequenceId);
        Interlocked.Increment(ref _nextSequenceId);
        return id;
    }

    private void AddToLogitCache(LLamaBatch batch)
    {
        foreach (var kv in batch.Logits.Select((v, k) => (Key: k, Value: v)))
        {
            if (kv.Value)
            {
                _logitCache.TryAdd(
                    (Epoch, kv.Key),
                    Context.NativeHandle.GetLogitsIth(kv.Key).ToArray()
                );
            }
        }
    }
}