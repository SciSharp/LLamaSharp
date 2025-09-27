using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// Managed wrapper around <c>mtmd_bitmap*</c> resources. Instances own the native pointer
    /// and ensure proper cleanup when disposed.
    /// </summary>
    public sealed class SafeMtmdEmbed : IDisposable
    {
        /// <summary>
        /// Raw pointer to the native bitmap structure. Internal so other wrappers can interop.
        /// </summary>
        internal IntPtr NativePtr { get; private set; }

        private bool _disposed;

        private SafeMtmdEmbed(IntPtr ptr)
        {
            NativePtr = ptr != IntPtr.Zero
                ? ptr
                : throw new InvalidOperationException("Failed to create MTMD bitmap.");
        }

        /// <summary>
        /// Create an embedding from raw RGB bytes.
        /// </summary>
        /// <param name="nx">Width of the bitmap in pixels.</param>
        /// <param name="ny">Height of the bitmap in pixels.</param>
        /// <param name="rgbData">Packed RGB data (3 bytes per pixel).</param>
        /// <returns>Managed wrapper when initialization succeeds; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">The RGB buffer is null.</exception>
        public static SafeMtmdEmbed? FromRgbBytes(uint nx, uint ny, byte[] rgbData)
        {
            if (rgbData == null)
                throw new ArgumentNullException(nameof(rgbData));

            var handle = GCHandle.Alloc(rgbData, GCHandleType.Pinned);
            try
            {
                var native = NativeApi.mtmd_bitmap_init(nx, ny, handle.AddrOfPinnedObject());
                return native == IntPtr.Zero ? null : new SafeMtmdEmbed(native);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        /// <summary>
        /// Create an embedding from PCM audio samples.
        /// </summary>
        /// <param name="samples">Array of mono PCM samples in float format.</param>
        /// <returns>Managed wrapper when initialization succeeds; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">The audio buffer is null.</exception>
        public static SafeMtmdEmbed? FromAudioSamples(float[] samples)
        {
            if (samples == null)
                throw new ArgumentNullException(nameof(samples));

            var handle = GCHandle.Alloc(samples, GCHandleType.Pinned);
            try
            {
                var native = NativeApi.mtmd_bitmap_init_from_audio((ulong)samples.Length, handle.AddrOfPinnedObject());
                return native == IntPtr.Zero ? null : new SafeMtmdEmbed(native);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        /// <summary>
        /// Create an embedding by decoding a media file using libmtmd helpers.
        /// </summary>
        /// <param name="mtmdContext">Model context that provides the decoder configuration.</param>
        /// <param name="path">Path to the media file on disk.</param>
        /// <returns>Managed wrapper when decoding succeeds; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">The context is null.</exception>
        /// <exception cref="ArgumentException">The path is null or whitespace.</exception>
        /// <exception cref="FileNotFoundException">The supplied file does not exist.</exception>
        public static SafeMtmdEmbed? FromMediaFile(SafeMtmdModelHandle mtmdContext, string path)
        {
            if (mtmdContext == null)
                throw new ArgumentNullException(nameof(mtmdContext));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Media file not found.", fullPath);

            bool added = false;
            var ctxPtr = IntPtr.Zero;
            try
            {
                // Hold a strong reference to the native context while the helper decodes the media file.
                mtmdContext.DangerousAddRef(ref added);
                ctxPtr = mtmdContext.DangerousGetHandle();
                var native = NativeApi.mtmd_helper_bitmap_init_from_file(ctxPtr, fullPath);
                return native == IntPtr.Zero ? null : new SafeMtmdEmbed(native);
            }
            finally
            {
                if (added)
                    mtmdContext.DangerousRelease();
            }
        }

        /// <summary>
        /// Create an embedding from an in-memory media buffer (image/audio/video).
        /// </summary>
        /// <param name="mtmdContext">Model context that provides the decoder configuration.</param>
        /// <param name="data">Binary buffer containing the encoded media.</param>
        /// <returns>Managed wrapper when decoding succeeds; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">The context is null.</exception>
        /// <exception cref="ArgumentException">The buffer is empty.</exception>
        public static unsafe SafeMtmdEmbed? FromMediaBuffer(SafeMtmdModelHandle mtmdContext, ReadOnlySpan<byte> data)
        {
            if (mtmdContext == null)
                throw new ArgumentNullException(nameof(mtmdContext));
            if (data.IsEmpty)
                throw new ArgumentException("Buffer must not be empty.", nameof(data));

            bool added = false;
            var ctxPtr = IntPtr.Zero;
            try
            {
                // Keep the context alive while the native helper processes the buffer.
                mtmdContext.DangerousAddRef(ref added);
                ctxPtr = mtmdContext.DangerousGetHandle();

                fixed (byte* bufferPtr = data)
                {
                    var native = NativeApi.mtmd_helper_bitmap_init_from_buf(ctxPtr, new IntPtr(bufferPtr), (UIntPtr)data.Length);
                    return native == IntPtr.Zero ? null : new SafeMtmdEmbed(native);
                }
            }
            finally
            {
                if (added)
                    mtmdContext.DangerousRelease();
            }
        }

        /// <summary>
        /// Width of the bitmap in pixels (or number of samples for audio embeddings).
        /// </summary>
        public uint Nx
        {
            get
            {
                EnsureNotDisposed();
                return NativeApi.mtmd_bitmap_get_nx(NativePtr);
            }
        }

        /// <summary>
        /// Height of the bitmap in pixels. For audio embeddings this is typically <c>1</c>.
        /// </summary>
        public uint Ny
        {
            get
            {
                EnsureNotDisposed();
                return NativeApi.mtmd_bitmap_get_ny(NativePtr);
            }
        }

        /// <summary>
        /// Indicates whether the embedding stores audio data instead of image pixels.
        /// </summary>
        public bool IsAudio
        {
            get
            {
                EnsureNotDisposed();
                return NativeApi.mtmd_bitmap_is_audio(NativePtr);
            }
        }

        /// <summary>
        /// Optional identifier assigned to this embedding.
        /// </summary>
        public string? Id
        {
            get
            {
                EnsureNotDisposed();
                var ptr = NativeApi.mtmd_bitmap_get_id(NativePtr);
                return NativeApi.PtrToStringUtf8(ptr);
            }
            set
            {
                EnsureNotDisposed();
                NativeApi.mtmd_bitmap_set_id(NativePtr, value);
            }
        }

        /// <summary>
        /// Zero-copy access to the underlying bitmap bytes. The span remains valid while this wrapper is alive.
        /// </summary>
        /// <returns>Read-only span exposing the native data buffer.</returns>
        /// <exception cref="ObjectDisposedException">The embedding has been disposed.</exception>
        public unsafe ReadOnlySpan<byte> GetDataSpan()
        {
            EnsureNotDisposed();

            var dataPtr = (byte*)NativeApi.mtmd_bitmap_get_data(NativePtr);
            var length = checked((int)NativeApi.mtmd_bitmap_get_n_bytes(NativePtr).ToUInt64());
            return dataPtr == null || length == 0 ? ReadOnlySpan<byte>.Empty : new ReadOnlySpan<byte>(dataPtr, length);
        }

        /// <summary>
        /// Release the underlying native bitmap.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if (NativePtr != IntPtr.Zero)
            {
                NativeApi.mtmd_bitmap_free(NativePtr);
                NativePtr = IntPtr.Zero;
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer to ensure native resources are reclaimed when Dispose is not invoked.
        /// </summary>
        ~SafeMtmdEmbed() => Dispose();

        private void EnsureNotDisposed()
        {
            if (_disposed || NativePtr == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(SafeMtmdEmbed));
        }
    }
}
