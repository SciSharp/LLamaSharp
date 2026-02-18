using System;
using System.Text;

namespace LLama.Native;

/// <summary>
/// Managed representation of the native <c>mtmd_context_params</c> structure used to configure multimodal helpers.
/// </summary>
public class MtmdContextParams
{
    /// <summary>
    /// Whether GPU acceleration should be requested when available.
    /// </summary>
    public bool UseGpu { get; set; }

    /// <summary>
    /// Whether timing information should be emitted by the native helper.
    /// </summary>
    public bool PrintTimings { get; set; }

    /// <summary>
    /// Number of worker threads to dedicate to preprocessing and tokenization.
    /// </summary>
    public int NThreads { get; set; }

    /// <summary>
    /// Marker token inserted into the text stream to reference an image embedding (deprecated by mtmd).
    /// </summary>
    public string? ImageMarker { get; set; }

    /// <summary>
    /// Marker token inserted into the text stream to reference a generic media embedding.
    /// </summary>
    public string? MediaMarker { get; set; }

    /// <summary>
    /// Flash attention policy forwarded to mtmd encoders.
    /// </summary>
    public LLamaFlashAttentionType FlashAttentionType { get; set; }

    /// <summary>
    /// Whether to run a warmup encode pass after initialization.
    /// </summary>
    public bool Warmup { get; set; }

    /// <summary>
    /// Minimum number of image tokens for dynamic resolution (use -1 to read metadata).
    /// </summary>
    public int ImageMinTokens { get; set; }

    /// <summary>
    /// Maximum number of image tokens for dynamic resolution (use -1 to read metadata).
    /// </summary>
    public int ImageMaxTokens { get; set; }

    /// <summary>
    /// Create a managed copy of the native defaults returned by <see cref="NativeApi.mtmd_context_params_default"/>.
    /// </summary>
    public static MtmdContextParams Default()
    {
        var native = NativeApi.mtmd_context_params_default();
        return new MtmdContextParams
        {
            UseGpu = native.use_gpu,
            PrintTimings = native.print_timings,
            NThreads = native.n_threads,
            ImageMarker = native.image_marker.PtrToString(),
            MediaMarker = native.media_marker.PtrToString(),
            FlashAttentionType = native.flash_attn_type,
            Warmup = native.warmup,
            ImageMinTokens = native.image_min_tokens,
            ImageMaxTokens = native.image_max_tokens
        };
    }


    /// <summary>
    /// Convert the managed representation to a native structure, pinning strings for the duration of the scope.
    /// </summary>
    internal NativeScope ToNativeScope() => new(this);

    internal readonly struct NativeScope : IDisposable
    {
        public NativeApi.mtmd_context_params Value { get; }

        private readonly PinnedUtf8String? _imageMarker;
        private readonly PinnedUtf8String? _mediaMarker;

        public NativeScope(MtmdContextParams managed)
        {
            _imageMarker = PinnedUtf8String.Create(managed.ImageMarker);
            _mediaMarker = PinnedUtf8String.Create(managed.MediaMarker);

            var native = NativeApi.mtmd_context_params_default();
            native.use_gpu = managed.UseGpu;
            native.print_timings = managed.PrintTimings;
            native.n_threads = managed.NThreads;
            native.flash_attn_type = managed.FlashAttentionType;
            native.warmup = managed.Warmup;
            native.image_min_tokens = managed.ImageMinTokens;
            native.image_max_tokens = managed.ImageMaxTokens;

            if (_imageMarker is not null)
                native.image_marker = _imageMarker.Pointer;
            if (_mediaMarker is not null)
                native.media_marker = _mediaMarker.Pointer;

            Value = native;
        }

        public void Dispose()
        {
            _imageMarker?.Dispose();
            _mediaMarker?.Dispose();
        }
    }
}

/// <summary>
/// Helper that pins a managed string as UTF-8 for the lifetime of the instance.
/// </summary>
internal sealed class PinnedUtf8String : IDisposable
{
    private readonly byte[]? _buffer;
    private readonly GCHandle _handle;

    private PinnedUtf8String(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        _buffer = new byte[bytes.Length + 1];
        Buffer.BlockCopy(bytes, 0, _buffer, 0, bytes.Length);
        _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
    }

    public static PinnedUtf8String? Create(string? value) => value is null ? null : new PinnedUtf8String(value);

    public IntPtr Pointer
    {
        get
        {
            if (_buffer is null || !_handle.IsAllocated)
                return IntPtr.Zero;

            return _handle.AddrOfPinnedObject();
        }
    }

    public void Dispose()
    {
        if (_buffer is not null && _handle.IsAllocated)
            _handle.Free();
    }
}
