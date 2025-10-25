using System;
using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// Managed wrapper around a single <c>mtmd_input_chunk</c>. Instances can either own the
/// underlying native pointer (when created via <see cref="Copy"/>) or act as non-owning views
/// produced by the tokenizer.
/// </summary>
public sealed class SafeMtmdInputChunk : SafeLLamaHandleBase
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
    public IntPtr NativePtr
    {
        get
        {
            EnsureNotDisposed();
            return DangerousGetHandle();
        }
    }

    private SafeMtmdInputChunk(IntPtr handle, bool ownsHandle)
        : base(handle, ownsHandle)
    {
    }

    /// <summary>
    /// Wrap an existing chunk pointer without taking ownership.
    /// </summary>
    /// <param name="ptr">Pointer returned by the native tokenizer.</param>
    /// <returns>Managed wrapper, or <c>null</c> when the pointer is null.</returns>
    public static SafeMtmdInputChunk Wrap(IntPtr ptr)
        => ptr == IntPtr.Zero ? null : new SafeMtmdInputChunk(ptr, ownsHandle: false);

    /// <summary>
    /// Create an owning copy of the current chunk. The caller becomes responsible for disposal.
    /// </summary>
    /// <returns>Owning managed wrapper, or <c>null</c> if the native copy failed.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the current wrapper has been disposed.</exception>
    public SafeMtmdInputChunk Copy()
    {
        return WithHandle(ptr =>
        {
            var clone = NativeApi.mtmd_input_chunk_copy(ptr);
            return clone == IntPtr.Zero ? null : new SafeMtmdInputChunk(clone, ownsHandle: true);
        });
    }

    /// <summary>
    /// Chunk modality reported by the native helper.
    /// </summary>
    public SafeMtmdInputChunkType Type
    {
        get
        {
            return WithHandle(ptr => (SafeMtmdInputChunkType)NativeApi.mtmd_input_chunk_get_type(ptr));
        }
    }

    /// <summary>
    /// Number of tokens contained in this chunk.
    /// </summary>
    public ulong NTokens
    {
        get
        {
            return WithHandle(ptr => NativeApi.mtmd_input_chunk_get_n_tokens(ptr).ToUInt64());
        }
    }

    /// <summary>
    /// Identifier assigned by the tokenizer (if any).
    /// </summary>
    public string Id
    {
        get
        {
            return WithHandle(ptr =>
            {
                var idPtr = NativeApi.mtmd_input_chunk_get_id(ptr);
                return Marshal.PtrToStringAnsi(idPtr) ?? string.Empty;
            });
        }
    }

    /// <summary>
    /// Number of positional slots consumed by this chunk.
    /// </summary>
    public long NPos
    {
        get
        {
            return WithHandle(ptr => NativeApi.mtmd_input_chunk_get_n_pos(ptr));
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

        bool added = false;
        try
        {
            DangerousAddRef(ref added);
            UIntPtr nTokens;
            var tokensPtr = (uint*)NativeApi.mtmd_input_chunk_get_tokens_text(DangerousGetHandle(), out nTokens);
            if (tokensPtr == null)
                return ReadOnlySpan<uint>.Empty;

            var length = checked((int)nTokens.ToUInt64());
            return new ReadOnlySpan<uint>(tokensPtr, length);
        }
        finally
        {
            if (added)
                DangerousRelease();
        }
    }

    /// <summary>
    /// Releases the native chunk when ownership is held by this instance.
    /// </summary>
    protected override bool ReleaseHandle()
    {
        if (handle != IntPtr.Zero)
        {
            NativeApi.mtmd_input_chunk_free(handle);
            SetHandle(IntPtr.Zero);
        }

        return true;
    }

    private void EnsureNotDisposed()
    {
        if (IsClosed || IsInvalid)
            throw new ObjectDisposedException(nameof(SafeMtmdInputChunk));
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
                throw new ObjectDisposedException(nameof(SafeMtmdInputChunk));

            return action(ptr);
        }
        finally
        {
            if (added)
                DangerousRelease();
        }
    }
}
