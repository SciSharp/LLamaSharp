using System;
using System.Collections.Generic;

namespace LLama.Native;

/// <summary>
/// Managed lifetime wrapper around a native <c>mtmd_input_chunks</c> collection returned by the tokenizer.
/// </summary>
public sealed class SafeMtmdInputChunks : IDisposable
{
    /// <summary>
    /// Raw pointer to the native chunk collection. Internal to allow other wrappers to interop safely.
    /// </summary>
    internal IntPtr NativePtr { get; private set; }

    private bool _disposed;

    internal SafeMtmdInputChunks(IntPtr ptr)
    {
        NativePtr = ptr;
    }

    /// <summary>
    /// Releases the native chunk collection and suppresses finalization.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        if (NativePtr != IntPtr.Zero)
        {
            NativeApi.mtmd_input_chunks_free(NativePtr);
            NativePtr = IntPtr.Zero;
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer to ensure native memory is reclaimed if Dispose is not called.
    /// </summary>
    ~SafeMtmdInputChunks()
    {
        Dispose();
    }

    /// <summary>
    /// Number of chunks currently held by the native collection.
    /// </summary>
    public ulong Size
    {
        get
        {
            EnsureNotDisposed();
            return NativeApi.mtmd_input_chunks_size(NativePtr).ToUInt64();
        }
    }

    /// <summary>
    /// Get a raw pointer to a chunk. The returned <see cref="IntPtr"/> is the <c>mtmd_input_chunk*</c>.
    /// Use <see cref="SafeMtmdInputChunk.Wrap"/> to create a managed wrapper if desired.
    /// </summary>
    /// <param name="index">Zero-based index of the chunk to retrieve.</param>
    /// <returns>Pointer to the requested chunk.</returns>
    /// <exception cref="ObjectDisposedException">The collection has already been disposed.</exception>
    /// <exception cref="IndexOutOfRangeException">The requested index is outside of the valid range.</exception>
    public IntPtr GetChunkPtr(ulong index)
    {
        EnsureNotDisposed();

        if (index >= Size) throw new IndexOutOfRangeException();
        return NativeApi.mtmd_input_chunks_get(NativePtr, (UIntPtr)index);
    }

    /// <summary>
    /// Enumerate the contained chunks as non-owning wrappers. Callers should dispose the returned chunk
    /// if they create a copy.
    /// </summary>
    /// <returns>Enumeration of chunk wrappers backed by the native collection.</returns>
    /// <exception cref="ObjectDisposedException">The collection has already been disposed.</exception>
    public IEnumerable<SafeMtmdInputChunk> Enumerate()
    {
        EnsureNotDisposed();

        for (ulong i = 0; i < Size; i++)
        {
            var chunk = SafeMtmdInputChunk.Wrap(GetChunkPtr(i));
            if (chunk != null)
            {
                // Yield a lightweight wrapper; ownership remains with the native collection.
                yield return chunk;
            }
        }
    }

    private void EnsureNotDisposed()
    {
        if (_disposed || NativePtr == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(SafeMtmdInputChunks));
    }
}
