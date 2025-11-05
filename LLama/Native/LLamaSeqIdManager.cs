using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LLama.Native;

/// <summary>
/// Provides <see cref="LLamaSeqId"/> management for LLama models.
/// Based on the provided max sequence count, it allocates and recycles <see cref="LLamaSeqId"/> for use in model operations.
/// </summary>
/// <remarks>
/// The class is thread-safe and allows multiple concurrent requests for sequence IDs.
/// </remarks>
public sealed class LLamaSeqIdManager : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private readonly ConcurrentBag<int> _availableIds;

    /// <summary>
    /// Constructs a new <see cref="LLamaSeqIdManager"/> with the specified maximum sequence count.
    /// </summary>
    /// <param name="maxSeqCount">maximum number of sequence IDs to manage.</param>
    public LLamaSeqIdManager(uint maxSeqCount)
    {
        var max = Math.Max((int)maxSeqCount, 1); // Ensure at least one sequence ID is available
        _semaphore = new SemaphoreSlim(initialCount: max, maxCount: max);
        _availableIds = [];
        for (var i = 0; i < max; i++)
        {
            _availableIds.Add(i);
        }
    }

    /// <summary>
    /// Returns the next available sequence ID.
    /// Callers will asynchronously wait if none are available.
    /// </summary>
    /// <param name="timeout">optional timeout for acquiring a sequence ID. If null, waits indefinitely.</param>
    /// <param name="cancellationToken">cancellation token to cancel the wait operation.</param>
    /// <returns>The next available sequence ID.</returns>
    public async Task<LLamaSeqId> NextAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(timeout ?? TimeSpan.FromMilliseconds(-1), cancellationToken).ConfigureAwait(false);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_availableIds.TryTake(out var seqId))
            {
                return (LLamaSeqId)seqId;
            }

            throw new InvalidOperationException("No sequence ID available despite semaphore release");
        }
        catch
        {
            _semaphore.Release();
            throw;
        }
    }

    /// <summary>
    /// Returns a sequence ID to the manager, making it available for reuse.
    /// </summary>
    /// <remarks>
    /// It's the caller's responsibility to ensure the sequence ID is in a valid state for reuse.
    /// </remarks>
    /// <param name="seqId">sequence ID to return.</param>
    public void Return(LLamaSeqId seqId)
    {
        _availableIds.Add(seqId.Value);
        _semaphore.Release();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _semaphore.Dispose();
    }
}