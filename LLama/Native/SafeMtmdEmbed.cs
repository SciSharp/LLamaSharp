using System;
using System.IO;

namespace LLama.Native
{
    /// <summary>
    /// Managed wrapper around <c>mtmd_bitmap*</c> resources. Instances own the native pointer
    /// and ensure proper cleanup when disposed.
    /// </summary>
    public sealed class SafeMtmdEmbed : SafeLLamaHandleBase
    {
        /// <summary>
        /// Raw pointer to the native bitmap structure. Internal so other wrappers can interop.
        /// </summary>
        internal IntPtr NativePtr
        {
            get
            {
                EnsureNotDisposed();
                return DangerousGetHandle();
            }
        }

        private SafeMtmdEmbed(IntPtr ptr)
            : base(ptr, ownsHandle: true)
        {
            if (IsInvalid)
                throw new InvalidOperationException("Failed to create MTMD bitmap.");
        }

        /// <summary>
        /// Create an embedding from raw RGB bytes.
        /// </summary>
        /// <param name="nx">Width of the bitmap in pixels.</param>
        /// <param name="ny">Height of the bitmap in pixels.</param>
        /// <param name="rgbData">Packed RGB data (3 bytes per pixel).</param>
        /// <returns>Managed wrapper when initialization succeeds; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">The RGB buffer is null.</exception>
        public static SafeMtmdEmbed? FromRgbBytes(uint nx, uint ny, ReadOnlySpan<byte> rgbData)
        {
            if (rgbData.Length != nx * ny * 3)
                throw new ArgumentException("Pixel data must be exactly 3 bytes per pixel", nameof(rgbData));

            unsafe
            {
                fixed (byte* rgbDataPtr = rgbData)
                {
                    var native = NativeApi.mtmd_bitmap_init(nx, ny, rgbDataPtr);
                    return native == IntPtr.Zero ? null : new SafeMtmdEmbed(native);
                }
            }
        }

        /// <summary>
        /// Create an embedding from PCM audio samples.
        /// </summary>
        /// <param name="samples">Array of mono PCM samples in float format.</param>
        /// <returns>Managed wrapper when initialization succeeds; otherwise <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">The audio buffer is null.</exception>
        public static SafeMtmdEmbed? FromAudioSamples(ReadOnlySpan<float> samples)
        {
            unsafe
            {
                fixed (float* samplesPtr = samples)
                {
                    var native = NativeApi.mtmd_bitmap_init_from_audio((ulong)samples.Length, samplesPtr);
                    return native == IntPtr.Zero ? null : new SafeMtmdEmbed(native);
                }
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
            // Try to open the file, this will check:
            // - File exists (automatically throws FileNotFoundException)
            // - File is readable (explicit check)
            // This provides better error messages that llama.cpp, which would throw an access violation exception in both cases.
            var fullPath = Path.GetFullPath(path);
            using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                if (!fs.CanRead)
                    throw new InvalidOperationException($"Media file '{path}' is not readable");

            var native = NativeApi.mtmd_helper_bitmap_init_from_file(mtmdContext, fullPath);
            return native == IntPtr.Zero ? null : new SafeMtmdEmbed(native);
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
            if (data.IsEmpty)
                throw new ArgumentException("Media buffer must not be empty.", nameof(data));

            fixed (byte* bufferPtr = data)
            {
                var native = NativeApi.mtmd_helper_bitmap_init_from_buf(mtmdContext, bufferPtr, (nuint)data.Length);
                return native == IntPtr.Zero ? null : new SafeMtmdEmbed(native);
            }
        }

        /// <summary>
        /// Width of the bitmap in pixels (or number of samples for audio embeddings).
        /// </summary>
        public uint Nx => NativeApi.mtmd_bitmap_get_nx(this);

        /// <summary>
        /// Height of the bitmap in pixels. For audio embeddings this is typically <c>1</c>.
        /// </summary>
        public uint Ny => NativeApi.mtmd_bitmap_get_ny(this);

        /// <summary>
        /// Indicates whether the embedding stores audio data instead of image pixels.
        /// </summary>
        public bool IsAudio => NativeApi.mtmd_bitmap_is_audio(this);

        /// <summary>
        /// Optional identifier assigned to this embedding.
        /// </summary>
        public string? Id
        {
            get => NativeApi.mtmd_bitmap_get_id(this).PtrToString();
            set => NativeApi.mtmd_bitmap_set_id(this, value);
        }

        /// <summary>
        /// Zero-copy access to the underlying bitmap bytes. The span remains valid while this wrapper is alive.
        /// </summary>
        /// <returns>Read-only span exposing the native data buffer.</returns>
        /// <exception cref="ObjectDisposedException">The embedding has been disposed.</exception>
        public unsafe ReadOnlySpan<byte> GetDataSpan()
        {
            EnsureNotDisposed();

            var dataPtr = (byte*)NativeApi.mtmd_bitmap_get_data(this);
            var length = checked((int)NativeApi.mtmd_bitmap_get_n_bytes(this).ToUInt64());
            return dataPtr == null || length == 0
                ? ReadOnlySpan<byte>.Empty
                : new ReadOnlySpan<byte>(dataPtr, length);
        }

        /// <summary>
        /// Release the underlying native bitmap.
        /// </summary>
        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
            {
                NativeApi.mtmd_bitmap_free(handle);
                SetHandle(IntPtr.Zero);
            }

            return true;
        }

        private void EnsureNotDisposed()
        {
            if (IsClosed || IsInvalid)
                throw new ObjectDisposedException(nameof(SafeMtmdEmbed));
        }
    }
}
