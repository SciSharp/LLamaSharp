using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// Managed wrapper around a single <c>mtmd_input_chunk</c>. Instances can either own the
/// underlying native pointer (when created via <see cref="Copy"/>) or act as non-owning views
/// produced by the tokenizer.
/// </summary>
public sealed class SafeMtmdInputChunk : IDisposable
{
    /// <summary>
    /// Chunk modality returned by the native tokenizer.
    /// </summary>
    public enum SafeMtmdInputChunkType
    {
        Text = 0,
        Image = 1,
        Audio = 2
    }

    /// <summary>
    /// Raw pointer to the native chunk structure.
    /// </summary>
    public IntPtr NativePtr { get; private set; }

    private bool _ownsPtr;
    private bool _disposed;

    private SafeMtmdInputChunk(IntPtr ptr, bool owns)
    {
        NativePtr = ptr;
        _ownsPtr = owns;
    }

    /// <summary>
    /// Wrap an existing chunk pointer without taking ownership.
    /// </summary>
    /// <param name="ptr">Pointer returned by the native tokenizer.</param>
    /// <returns>Managed wrapper, or <c>null</c> when the pointer is null.</returns>
    public static SafeMtmdInputChunk Wrap(IntPtr ptr)
        => ptr == IntPtr.Zero ? null : new SafeMtmdInputChunk(ptr, false);

    /// <summary>
    /// Create an owning copy of the current chunk. The caller becomes responsible for disposal.
    /// </summary>
    /// <returns>Owning managed wrapper, or <c>null</c> if the native copy failed.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the current wrapper has been disposed.</exception>
    public SafeMtmdInputChunk Copy()
    {
        EnsureNotDisposed();

        var p = NativeApi.mtmd_input_chunk_copy(NativePtr);
        return p == IntPtr.Zero ? null : new SafeMtmdInputChunk(p, true);
    }

    /// <summary>
    /// Chunk modality reported by the native helper.
    /// </summary>
    public SafeMtmdInputChunkType Type
    {
        get
        {
            EnsureNotDisposed();
            return (SafeMtmdInputChunkType)NativeApi.mtmd_input_chunk_get_type(NativePtr);
        }
    }

    /// <summary>
    /// Number of tokens contained in this chunk.
    /// </summary>
    public ulong NTokens
    {
        get
        {
            EnsureNotDisposed();
            return NativeApi.mtmd_input_chunk_get_n_tokens(NativePtr).ToUInt64();
        }
    }

    /// <summary>
    /// Identifier assigned by the tokenizer (if any).
    /// </summary>
    public string Id
    {
        get
        {
            EnsureNotDisposed();
            return Marshal.PtrToStringAnsi(NativeApi.mtmd_input_chunk_get_id(NativePtr)) ?? string.Empty;
        }
    }

    /// <summary>
    /// Number of positional slots consumed by this chunk.
    /// </summary>
    public long NPos
    {
        get
        {
            EnsureNotDisposed();
            return NativeApi.mtmd_input_chunk_get_n_pos(NativePtr);
        }
    }

    /// <summary>
    /// Zero-copy view over the chunk's token buffer. The span remains valid only while the native chunk is alive.
    /// </summary>
    /// <returns>Read-only span exposing the chunk's tokens.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the wrapper has been disposed.</exception>
    public unsafe ReadOnlySpan<uint> GetTextTokensSpan()
    {
        EnsureNotDisposed();

        UIntPtr n;
        var p = (uint*)NativeApi.mtmd_input_chunk_get_tokens_text(NativePtr, out n);
        return p == null ? ReadOnlySpan<uint>.Empty : new ReadOnlySpan<uint>(p, checked((int)n.ToUInt64()));
    }

    /// <summary>
    /// Release the underlying native resources if this instance owns them.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        if (_ownsPtr && NativePtr != IntPtr.Zero)
        {
            NativeApi.mtmd_input_chunk_free(NativePtr);
        }

        NativePtr = IntPtr.Zero;
        _ownsPtr = false;
        _disposed = true;

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizer to ensure native memory is reclaimed when Dispose is not called by owners.
    /// </summary>
    ~SafeMtmdInputChunk() => Dispose();

    private void EnsureNotDisposed()
    {
        if (_disposed || NativePtr == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(SafeMtmdInputChunk));
    }
}
