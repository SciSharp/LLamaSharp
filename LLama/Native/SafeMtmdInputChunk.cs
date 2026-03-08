using System;

namespace LLama.Native;

/// <summary>
/// Managed wrapper around a single <c>mtmd_input_chunk</c>. Instances can either own the
/// underlying native pointer (when created via <see cref="Copy"/>) or act as non-owning views
/// produced by the tokenizer.
/// </summary>
public sealed class SafeMtmdInputChunk
    : SafeLLamaHandleBase
{
    /// <summary>
    /// Chunk modality returned by the native tokenizer.
    /// </summary>
    public enum SafeMtmdInputChunkType
    {
        /// <summary>
        /// Chunk contains text
        /// </summary>
        Text = 0,

        /// <summary>
        /// Chunk contains an image
        /// </summary>
        Image = 1,

        /// <summary>
        /// Chunk contains audio
        /// </summary>
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
    public static SafeMtmdInputChunk? Wrap(IntPtr ptr)
        => ptr == IntPtr.Zero ? null : new SafeMtmdInputChunk(ptr, ownsHandle: false);

    /// <summary>
    /// Create an owning copy of the current chunk. The caller becomes responsible for disposal.
    /// </summary>
    /// <returns>Owning managed wrapper, or <c>null</c> if the native copy failed.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the current wrapper has been disposed.</exception>
    public SafeMtmdInputChunk? Copy()
    {
        var clone = NativeApi.mtmd_input_chunk_copy(this);
        return clone == IntPtr.Zero ? null : new SafeMtmdInputChunk(clone, ownsHandle: true);
    }

    /// <summary>
    /// Chunk modality reported by the native helper.
    /// </summary>
    public SafeMtmdInputChunkType Type => NativeApi.mtmd_input_chunk_get_type(this);

    /// <summary>
    /// Number of tokens contained in this chunk.
    /// </summary>
    public ulong NTokens => NativeApi.mtmd_input_chunk_get_n_tokens(this).ToUInt64();

    /// <summary>
    /// Identifier assigned by the tokenizer (if any).
    /// </summary>
    public string Id => NativeApi.mtmd_input_chunk_get_id(this).PtrToStringWithDefault(string.Empty);

    /// <summary>
    /// Number of positional slots consumed by this chunk.
    /// </summary>
    public long NPos => NativeApi.mtmd_input_chunk_get_n_pos(this);

    /// <summary>
    /// Zero-copy view over the chunk's token buffer. The span remains valid only while the native chunk is alive.
    /// </summary>
    /// <returns>Read-only span exposing the chunk's tokens.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the wrapper has been disposed.</exception>
    public unsafe ReadOnlySpan<int> GetTextTokensSpan()
    {
        EnsureNotDisposed();

        var tokensPtr = (int*)NativeApi.mtmd_input_chunk_get_tokens_text(this, out var nTokens);
        if (tokensPtr == null)
            return ReadOnlySpan<int>.Empty;

        var length = checked((int)nTokens.ToUInt64());
        return new ReadOnlySpan<int>(tokensPtr, length);
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
}
