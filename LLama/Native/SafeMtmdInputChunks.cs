using System;
using System.Collections.Generic;

namespace LLama.Native;

/// <summary>
/// Managed lifetime wrapper around a native <c>mtmd_input_chunks</c> collection returned by the tokenizer.
/// </summary>
public sealed class SafeMtmdInputChunks : SafeLLamaHandleBase
{
    /// <summary>
    /// Raw pointer to the native chunk collection. Internal to allow other wrappers to interop safely.
    /// </summary>
    internal IntPtr NativePtr
    {
        get
        {
            EnsureNotDisposed();
            return DangerousGetHandle();
        }
    }

    internal SafeMtmdInputChunks(IntPtr ptr)
        : base(ptr, ownsHandle: true)
    {
        if (IsInvalid)
            throw new InvalidOperationException("Native MTMD chunk collection pointer is null.");
    }

    /// <summary>
    /// Releases the native chunk collection.
    /// </summary>
    protected override bool ReleaseHandle()
    {
        if (handle != IntPtr.Zero)
        {
            NativeApi.mtmd_input_chunks_free(handle);
            SetHandle(IntPtr.Zero);
        }

        return true;
    }

    /// <summary>
    /// Number of chunks currently held by the native collection.
    /// </summary>
    public ulong Size
    {
        get
        {
            return WithHandle(ptr => NativeApi.mtmd_input_chunks_size(ptr).ToUInt64());
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
        return WithHandle(ptr =>
        {
            var size = NativeApi.mtmd_input_chunks_size(ptr).ToUInt64();
            if (index >= size)
                throw new IndexOutOfRangeException();

            return NativeApi.mtmd_input_chunks_get(ptr, (UIntPtr)index);
        });
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

        var count = Size;
        for (ulong i = 0; i < count; i++)
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
        if (IsClosed || IsInvalid)
            throw new ObjectDisposedException(nameof(SafeMtmdInputChunks));
    }

    private T WithHandle<T>(Func<IntPtr, T> action)
    {
        EnsureNotDisposed();

        bool added = false;
        try
        {
            DangerousAddRef(ref added);
            var ptr = DangerousGetHandle();
            if (ptr == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(SafeMtmdInputChunks));

            return action(ptr);
        }
        finally
        {
            if (added)
                DangerousRelease();
        }
    }
}
