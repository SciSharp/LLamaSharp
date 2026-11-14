using System;
using static LLama.Native.SafeMtmdInputChunk;

namespace LLama.Native;

/// <summary>
/// P/Invoke surface for MTMD (multimodal) helpers exposed by llama.cpp.
/// </summary>
public static partial class NativeApi
{

    /// <summary>
    /// Native context parameters returned by <see cref="mtmd_context_params_default"/>.
    /// </summary>
    /// <remarks>mtmd_context_params</remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct mtmd_context_params
    {
        [MarshalAs(UnmanagedType.I1)] public bool use_gpu;

        /// <summary>
        /// Device used by the multimodal encoder, or null for automatic selection.
        /// </summary>
        public IntPtr device;

        [MarshalAs(UnmanagedType.I1)] public bool print_timings;
        public int n_threads;
        public IntPtr image_marker;
        public IntPtr media_marker;
        public LLamaFlashAttentionType flash_attn_type;
        [MarshalAs(UnmanagedType.I1)] public bool warmup;
        public int image_min_tokens;
        public int image_max_tokens;

        private IntPtr /* ggml_backend_sched_eval_callback */ cb_eval;
        private IntPtr cb_eval_user_data;

        /// <summary>
        /// maximum number of output tokens in a batch
        /// (note: this is not a hard-limit, the first image will always be added even if it exceeds this limit)
        /// (default: 1024)
        /// </summary>
        public int batch_max_tokens;

        // Called with a progress value between 0.0 and 1.0. Pass NULL to disable.
        // If the provided progress_callback returns true, model loading continues.
        // If it returns false, model loading is immediately aborted.
        private IntPtr progress_callback;
        private IntPtr progress_callback_user_data;
    }

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_default_marker", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_default_marker();

    /// <summary>
    /// Retrieve the default multimodal marker text.
    /// </summary>
    public static string? MtmdDefaultMarker()
        => mtmd_default_marker().PtrToString();

    /// <summary>
    /// get the current marker string
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_context_params_default", CallingConvention = CallingConvention.Cdecl)]
    public static extern string mtmd_get_marker(SafeMtmdModelHandle ctx);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_context_params_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern mtmd_context_params mtmd_context_params_default();

    /// <summary>
    /// whether we need to set non-causal mask before llama_decode
    /// if chunk is nullptr, we assume the default case where chunk is an image chunk
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_decode_use_non_causal", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool mtmd_decode_use_non_causal(SafeMtmdModelHandle ctx);

    /// <summary>
    /// whether the current model use M-RoPE for llama_decode
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_decode_use_mrope", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool mtmd_decode_use_mrope(SafeMtmdModelHandle ctx);

    /// <summary>
    /// whether the current model supports vision input
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_support_vision", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool mtmd_support_vision(SafeMtmdModelHandle ctx);

    /// <summary>
    /// whether the current model supports audio input
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_support_audio", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool mtmd_support_audio(SafeMtmdModelHandle ctx);

    /// <summary>
    /// get audio sample rate in Hz, for example 16000 for Whisper
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_get_audio_sample_rate", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_get_audio_sample_rate(SafeMtmdModelHandle ctx);

    // bitmap ------------------------------------------------------------

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_init", CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe IntPtr mtmd_bitmap_init(uint nx, uint ny, byte* data);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_init_from_audio", CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe IntPtr mtmd_bitmap_init_from_audio(ulong n_samples, float* data);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_get_nx", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint mtmd_bitmap_get_nx(SafeMtmdEmbed bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_get_ny", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint mtmd_bitmap_get_ny(SafeMtmdEmbed bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_get_data", CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe byte* mtmd_bitmap_get_data(SafeMtmdEmbed bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_get_n_bytes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_bitmap_get_n_bytes(SafeMtmdEmbed bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_is_audio", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool mtmd_bitmap_is_audio(SafeMtmdEmbed bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_free", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void mtmd_bitmap_free(IntPtr bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_get_id", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_bitmap_get_id(SafeMtmdEmbed bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_set_id", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void mtmd_bitmap_set_id_native(SafeMtmdEmbed bitmap, byte* id);

    /// <summary>
    /// Assign an identifier to a bitmap using a UTF-8 encoded string.
    /// </summary>
    internal static unsafe void mtmd_bitmap_set_id(SafeMtmdEmbed bitmap, string? id)
    {
        if (id is null)
        {
            mtmd_bitmap_set_id_native(bitmap, null);
            return;
        }

        using var pinned = PinnedUtf8String.Create(id) ?? throw new ArgumentNullException(nameof(id));
        mtmd_bitmap_set_id_native(bitmap, (byte*)pinned.Pointer);
    }

    // input_chunks ------------------------------------------------------

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunks_init", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_input_chunks_init();

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunks_size", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_input_chunks_size(IntPtr chunks);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunks_get", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_input_chunks_get(IntPtr chunks, UIntPtr idx);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunks_free", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void mtmd_input_chunks_free(IntPtr chunks);

    // input_chunk -------------------------------------------------------

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_type", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SafeMtmdInputChunkType mtmd_input_chunk_get_type(SafeMtmdInputChunk chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_tokens_text", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_input_chunk_get_tokens_text(SafeMtmdInputChunk chunk, out UIntPtr n_tokens);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_tokens_image", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_input_chunk_get_tokens_image(IntPtr chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_n_tokens", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_input_chunk_get_n_tokens(SafeMtmdInputChunk chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_id", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_input_chunk_get_id(SafeMtmdInputChunk chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_n_pos", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_input_chunk_get_n_pos(SafeMtmdInputChunk chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_copy", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_input_chunk_copy(SafeMtmdInputChunk chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_free", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void mtmd_input_chunk_free(IntPtr chunk);

    // image_tokens ------------------------------------------------------

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_n_tokens", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_image_tokens_get_n_tokens(IntPtr image_tokens);

    [Obsolete("use mtmd_image_tokens_get_decoder_pos() instead")]
    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_nx", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_image_tokens_get_nx(IntPtr image_tokens);

    [Obsolete("use mtmd_image_tokens_get_decoder_pos() instead")]
    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_ny", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_image_tokens_get_ny(IntPtr image_tokens);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_id", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_image_tokens_get_id(IntPtr image_tokens);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_n_pos", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_image_tokens_get_n_pos(IntPtr image_tokens);

    [StructLayout(LayoutKind.Explicit)]
    internal struct mtmd_decoder_pos
    {
        [FieldOffset(0)]
        uint t;

        [FieldOffset(4)]
        uint x;

        [FieldOffset(8)]
        uint y;

        [FieldOffset(12)]
        uint z;
    };

    /// <summary>
    /// get position for decoder attention, to be used by M-RoPE models
    /// </summary>
    /// <param name="image_tokens"></param>
    /// <param name="pos_0">pos_0 is the absolute position of the first token</param>
    /// <param name="i">i is the index of the embedding token, ranging from 0 to mtmd_image_tokens_get_n_tokens() - 1</param>
    /// <returns>return relative position (for example, embedding 0 will have position (0, 0, 0); remember to adjust it to the current absolute position)</returns>
    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_decoder_pos", CallingConvention = CallingConvention.Cdecl)]
    internal static extern mtmd_decoder_pos mtmd_image_tokens_get_decoder_pos(IntPtr image_tokens, LLamaPos pos_0, nuint i);

    // tokenize ----------------------------------------------------------

    /// <summary>
    /// Native text structure consumed by NativeApi.mtmd_tokenize
    /// </summary>
    internal unsafe struct mtmd_input_text_native
    {
        public byte* text;
        public nuint text_len;
        [MarshalAs(UnmanagedType.I1)] public bool add_special;
        [MarshalAs(UnmanagedType.I1)] public bool parse_special;
    }

    /// <summary>
    /// Utility scope that pins managed text while invoking the native tokenizer.
    /// </summary>
    internal readonly unsafe ref struct MtmdInputTextScope
    {
        public readonly mtmd_input_text_native Value;
        private readonly PinnedUtf8String _text;

        public MtmdInputTextScope(string text, bool addSpecial, bool parseSpecial)
        {
            _text = PinnedUtf8String.Create(text) ?? throw new ArgumentNullException(nameof(text));
            Value = new mtmd_input_text_native
            {
                text = (byte*)_text.Pointer,
                text_len = _text.Length,
                add_special = addSpecial,
                parse_special = parseSpecial
            };
        }

        public void Dispose() => _text.Dispose();
    }

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_tokenize", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int mtmd_tokenize_native(
        SafeMtmdModelHandle ctx,
        IntPtr output,
        mtmd_input_text_native* text,
        IntPtr[] bitmaps,
        UIntPtr n_bitmaps);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_tokenize", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int mtmd_tokenize_native(
        SafeMtmdModelHandle ctx,
        IntPtr output,
        mtmd_input_text_native* text,
        SafeMtmdEmbed[] bitmaps,
        UIntPtr n_bitmaps);

    internal static unsafe int mtmd_tokenize(SafeMtmdModelHandle ctx, IntPtr output, in mtmd_input_text_native text, IntPtr[] bitmaps, nuint n_bitmaps)
    {
        var temp = text;
        return mtmd_tokenize_native(ctx, output, &temp, bitmaps, n_bitmaps);
    }

    internal static unsafe int mtmd_tokenize(SafeMtmdModelHandle ctx, IntPtr output, string text, bool addSpecial, bool parseSpecial, IntPtr[] bitmaps, nuint n_bitmaps)
    {
        using var scope = new MtmdInputTextScope(text, addSpecial, parseSpecial);
        return mtmd_tokenize_native(ctx, output, &scope.Value, bitmaps, n_bitmaps);
    }

    /// <summary>
    /// text chunk will be ignored silently, only media chunk will be encoded
    /// returns 0 on success
    /// returns 1 on generic error
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="chunk"></param>
    /// <returns></returns>
    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_encode_chunk", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_encode_chunk(IntPtr ctx, IntPtr chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_get_output_embd", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_get_output_embd(IntPtr ctx);

    // helper ------------------------------------------------------------

    internal struct mtmd_helper_bitmap_wrapper
    {
        public IntPtr /* mtmd_bitmap* */ bitmap;
        public IntPtr /* mtmd_helper_video* */ video_ctx;
    };

    /// <summary>
    /// Native video decoding defaults used by bitmap loading helpers.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct mtmd_helper_video_init_params
    {
        /// <summary>
        /// Desired output frame rate.
        /// </summary>
        public float fps_target;

        /// <summary>
        /// Optional directory containing ffmpeg and ffprobe.
        /// </summary>
        public IntPtr ffmpeg_bin_dir;

        /// <summary>
        /// Interval between timestamps in milliseconds.
        /// </summary>
        public long timestamp_interval_ms;

        /// <summary>
        /// Whether to omit the soundtrack.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool no_soundtrack;
    }

    /// <summary>
    /// Native options passed by value to bitmap loading helpers.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct mtmd_helper_init_opt
    {
        /// <summary>
        /// Video decoding options.
        /// </summary>
        public mtmd_helper_video_init_params video_params;
    }

    /// <summary>
    /// Get bitmap loading defaults from the native library.
    /// </summary>
    [DllImport(mtmdLibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern mtmd_helper_init_opt mtmd_helper_init_opt_default();

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_test_create_input_chunks", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_test_create_input_chunks();

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_bitmap_init_from_file", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe mtmd_helper_bitmap_wrapper mtmd_helper_bitmap_init_from_file_native(SafeMtmdModelHandle ctx, byte* fname, bool placeholder, mtmd_helper_init_opt opt);

    internal static unsafe IntPtr mtmd_helper_bitmap_init_from_file(SafeMtmdModelHandle ctx, string fname)
    {
        using var pinned = PinnedUtf8String.Create(fname) ?? throw new ArgumentNullException(nameof(fname));
        return mtmd_helper_bitmap_init_from_file_native(ctx, (byte*)pinned.Pointer, false, mtmd_helper_init_opt_default()).bitmap;
    }

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_bitmap_init_from_buf", CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe mtmd_helper_bitmap_wrapper mtmd_helper_bitmap_init_from_buf_native(SafeMtmdModelHandle ctx, byte* buf, nuint len, bool placeholder, mtmd_helper_init_opt opt);

    internal static unsafe IntPtr mtmd_helper_bitmap_init_from_buf(SafeMtmdModelHandle ctx, byte* buf, nuint len)
    {
        return mtmd_helper_bitmap_init_from_buf_native(ctx, buf, len, false, mtmd_helper_init_opt_default()).bitmap;
    }

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_get_n_tokens", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_helper_get_n_tokens(SafeMtmdInputChunks chunks);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_get_n_pos", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_helper_get_n_pos(SafeMtmdInputChunks chunks);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_image_get_decoder_pos", CallingConvention = CallingConvention.Cdecl)]
    // helper to get the list of relative positions corresponding to the embedding tokens, to be used by M-RoPE
    // out_pos must have length == mtmd_helper_get_n_tokens(image)
    internal static extern void mtmd_helper_image_get_decoder_pos(
        IntPtr /* mtmd_image_tokens* */ image,
        LLamaPos pos_0,
        IntPtr /* mtmd_decoder_pos* */ out_pos
    );

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_eval_chunks", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_helper_eval_chunks(
        SafeMtmdModelHandle ctx,
        SafeLLamaContextHandle lctx,
        SafeMtmdInputChunks chunks,
        int n_past,
        int seq_id,
        int n_batch,
        [MarshalAs(UnmanagedType.I1)] bool logits_last,
        ref int new_n_past);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_eval_chunk_single", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_helper_eval_chunk_single(
        SafeMtmdModelHandle ctx,
        SafeLLamaContextHandle lctx,
        IntPtr chunk,
        int n_past,
        int seq_id,
        int n_batch,
        [MarshalAs(UnmanagedType.I1)] bool logits_last,
        ref int new_n_past);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_decode_image_chunk", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_helper_decode_image_chunk(
        SafeMtmdModelHandle ctx,
        SafeLLamaContextHandle lctx,
        IntPtr chunk,
        IntPtr encoded_embd,
        int n_past,
        int seq_id,
        int n_batch,
        ref int new_n_past,
        IntPtr /* mtmd_helper_post_decode_callback */ callback,
        IntPtr user_data
    );
    
    /*
     * // EXPERIMENTAL API to get mmproj's capabilities without initializing the full context
       // This is only intended to be used by llama-server, breaking changes is expected
       struct mtmd_caps {
           bool inp_vision;
           bool inp_audio;
       };
       MTMD_API struct mtmd_caps mtmd_get_cap_from_file(const char * mmproj_fname);
     */
    
    /*
     * // batch encoding API
       // chunks are not owned by the batch, they will not be freed by mtmd_batch_free()
       // batch is valid for a given context, cannot be shared across contexts
       MTMD_API mtmd_batch * mtmd_batch_init(mtmd_context * ctx);
       MTMD_API void         mtmd_batch_free(mtmd_batch * batch);
       
       // only media chunks are allowed, text chunks will be rejected
       // returns 0 on success
       // returns 1 on generic error
       // returns 2 if the batch is too large (chunk won't be added)
       // returns 3 if it cannot be batched with the existing chunks in the batch
       MTMD_API int32_t mtmd_batch_add_chunk(mtmd_batch * batch, const mtmd_input_chunk * chunk);
       
       // returns 0 on success
       // returns 1 on generic error
       MTMD_API int32_t mtmd_batch_encode(mtmd_batch * batch);
       MTMD_API float * mtmd_batch_get_output_embd(mtmd_batch * batch, const mtmd_input_chunk * chunk);
     */
    
    /*
     * //
       // video input helpers (requires ffmpeg/ffprobe installed on the system)
       // the notion of video only exists at the helper level, it is not visible to the core mtmd library
       //
       // NOTE: this implementation is model-agnostic, it can be used with any vision-capable model
       //       however, it may not be accurate for some specific models
       //       (this is expected for now, to keep the implementation simple)
       //
       
       struct mtmd_helper_video_info {
           uint32_t width;
           uint32_t height;
           float    fps;      // effective fps (fps_target if set, else original video fps)
           int32_t  n_frames; // estimated total frames at effective fps (-1 if unknown)
       };
       
       struct mtmd_helper_video_init_params {
           float fps_target;            // desired output fps; <= 0 means use the video's native fps, defaulted to 4.0f
           const char * ffmpeg_bin_dir; // directory containing ffmpeg/ffprobe binaries; NULL means search PATH
           int64_t timestamp_interval_ms; // interval for adding timestamp as text chunk (example: "[10m50.5s]"); <= 0 means no timestamp, defaulted to 5000ms
           // TODO @ngxson : allow "placeholder" bitmap output for counting tokens
       };
       
       MTMD_API struct mtmd_helper_video_init_params mtmd_helper_video_init_params_default(void);
       
       // returns NULL on failure (ffprobe not found, file unreadable, etc.)
       MTMD_API mtmd_helper_video * mtmd_helper_video_init(
                           struct mtmd_context * mctx,
                           const char * path,
                           struct mtmd_helper_video_init_params params);
       
       // Same as mtmd_helper_video_init(), but reads from an in-memory buffer.
       // The buffer is copied internally; the caller does not need to keep it alive.
       // Note: pipe input is not seekable, so seeking will use output-side seeking
       // (ffmpeg decodes and discards frames up to the target position).
       MTMD_API mtmd_helper_video * mtmd_helper_video_init_from_buf(
                           struct mtmd_context * mctx,
                           const unsigned char * buf, size_t len,
                           struct mtmd_helper_video_init_params params);
       MTMD_API void mtmd_helper_video_free(mtmd_helper_video * ctx);
       MTMD_API struct mtmd_helper_video_info mtmd_helper_video_get_info(const mtmd_helper_video * ctx);
       
       // Read the next item from the video stream; exactly one of out_bitmap or out_text is set per call.
       // *out_bitmap - heap-allocated; caller must free with mtmd_bitmap_free()
       // *out_text   - heap-allocated (always via strdup/malloc); caller must free with free()
       // returns 0 on success, -1 on EOF, -2 on error
       MTMD_API int32_t mtmd_helper_video_read_next(mtmd_helper_video * ctx,
                   mtmd_bitmap ** out_bitmap,
                   char ** out_text);
     */

    /*
     * // Bitmap and chunk helpers (added in v0.9.0, not yet wrapped)
       MTMD_API bool                  mtmd_bitmap_is_voice   (const mtmd_bitmap * bitmap);

       // on a frame: video frames must be both mergeable and is_video
       // on a lazy bitmap: the expanded chunks are one video, wrapped by the model's video begin/end tokens
       //                   with MTMD_VIDEO_TEMPLATE_LFM3_OMNI, its audio bitmaps are the pieces of one soundtrack, in order
       MTMD_API void         mtmd_bitmap_set_is_video(mtmd_bitmap * bitmap, bool is_video);

       // if true, this bitmap can be merged (temporal merge) with an adjacent mergeable bitmap by certain video input models
       MTMD_API void         mtmd_bitmap_set_mergeable(mtmd_bitmap * bitmap, bool mergeable);

       // similar to mtmd_input_chunk_copy, but returns a placeholder chunk
       MTMD_API mtmd_input_chunk * mtmd_input_chunk_get_placeholder(const mtmd_input_chunk * chunk);

       // save/load an input chunk to/from a buffer (useful for KV save/load)
       // important: only chunk's metadata will be saved, the actual image/audio data will not be saved
       // the loaded chunk will always be a placeholder, cannot be used for mtmd_encode() or mtmd_batch_encode()
       // out_buf can be nullptr (to query expected_out_len)
       // returns 0 on success, non-zero on failure
       MTMD_API int32_t            mtmd_input_chunk_save(const mtmd_input_chunk * chunk, char * out_buf, size_t out_len, size_t * expected_out_len);

       // returns nullptr on failure
       MTMD_API mtmd_input_chunk * mtmd_input_chunk_load(const char * buf, size_t len);
     */

    /*
     * // Tokenize from mixed text/bitmap parts (added in v0.9.0, not yet wrapped)
       struct mtmd_input_part {
           // only text or bitmap can be set, not both
           const struct mtmd_input_text * text;
           const struct mtmd_bitmap * bitmap;
       };

       // same as mtmd_tokenize(), but takes an array of mtmd_input_part
       // use cases:
       // - when you don't want to use media markers (they will be tokenized as normal text)
       // - when you want to control parse_special for each text part
       // note: per-part add_special will be ignored
       // return 1 if a part has both text and bitmap set (or neither)
       MTMD_API int32_t mtmd_tokenize_from_parts(const mtmd_context * ctx,
                                                 mtmd_input_chunks * output,
                                                 const mtmd_input_part * const * parts,
                                                 size_t n_parts,
                                                 bool add_special);
     */

    /*
     * // Recommended video configuration (added in v0.9.0, not yet wrapped)
       // how the frames and timestamps of a video are laid out in the prompt
       enum mtmd_video_template {
           MTMD_VIDEO_TEMPLATE_DEFAULT,   // "Video:" prefix, then a "[XmY.YYs]" timestamp every N ms
           MTMD_VIDEO_TEMPLATE_LFM3,      // "<X.X seconds>" before every group of 2 frames
           MTMD_VIDEO_TEMPLATE_LFM3_OMNI, // same as LFM3, then the soundtrack of each group, as an audio bitmap after its frames
           MTMD_VIDEO_TEMPLATE_QWEN3VL,   // "<X.X seconds>" before every group of 2 frames, no markers around the whole video
       };

       // recommended video input for the current model, given the source video
       // fields set to 0 mean "no preference, use the caller's default"
       struct mtmd_video_config {
           enum mtmd_video_template tmpl;
           float   fps;          // frame sampling rate
           int32_t n_frames;     // number of frames to sample
           int32_t frame_width;  // size to resize the frames to
           int32_t frame_height;
       };

       // src_n_frames and src_fps can be <= 0 if unknown
       MTMD_API struct mtmd_video_config mtmd_get_video_config(const mtmd_context * ctx,
                                                               int32_t src_width, int32_t src_height,
                                                               int32_t src_n_frames, float src_fps);
     */

    /*
     * // Realtime audio streaming API (added in v0.9.0, not yet wrapped)
       struct mtmd_audio_stream_params {
           int32_t step_ms; // audio encoded per encoder call, rounded up to the model step (default: 320)

           // tokens that open the stream, they go out with the first rows (default: none)
           // with the prompt of its "asr" chat template, LFM3-Audio transcribes live
           const llama_token * prompt;
           size_t              n_prompt;
       };

       // whether the current model supports mtmd_audio_stream
       MTMD_API bool mtmd_support_audio_stream(const mtmd_context * ctx);

       MTMD_API struct mtmd_audio_stream_params mtmd_audio_stream_params_default(void);

       // returns nullptr on failure
       MTMD_API mtmd_audio_stream * mtmd_audio_stream_init(mtmd_context * ctx, struct mtmd_audio_stream_params params);

       MTMD_API void                mtmd_audio_stream_free(mtmd_audio_stream * stream);

       // samples are PCM F32 at mtmd_get_audio_sample_rate()
       // returns 0 on success, 1 if the stream has ended
       MTMD_API int32_t mtmd_audio_stream_push(mtmd_audio_stream * stream, const float * samples, size_t n_samples);

       // pad the stream so that all pending rows can be produced
       // end = false: the stream stays open (use it on a pause, to get a partial transcript)
       // end = true:  no more push() is accepted
       // thread-safe across streams
       // returns 0 on success, 1 if the stream has ended
       MTMD_API int32_t mtmd_audio_stream_flush(mtmd_audio_stream * stream, bool end);

       // produce the next decoder rows
       // last_token is the token sampled from the previous row, LLAMA_TOKEN_NULL before the first sample
       // thread-safe across streams
       // return value:
       //   > 0 number of rows ready, read them with mtmd_audio_stream_get_output_embd()
       //   0   more audio is needed
       //   -1  end of stream, nothing more will be produced
       //   -2  error
       MTMD_API int32_t mtmd_audio_stream_step(mtmd_audio_stream * stream, llama_token last_token);

       // audio rows whose text has been sampled (prefix rows included), step() updates it from last_token
       MTMD_API int64_t mtmd_audio_stream_get_n_rows(const mtmd_audio_stream * stream);

       // audio rows up to the end of the last flush
       // once get_n_rows() reaches it, the text of the flushed audio has been sampled
       MTMD_API int64_t mtmd_audio_stream_get_n_rows_flushed(const mtmd_audio_stream * stream);

       // rows of the last step, the reading size (in bytes) is n_rows * llama_model_n_embd_inp(model) * sizeof(float)
       MTMD_API const float * mtmd_audio_stream_get_output_embd(const mtmd_audio_stream * stream);

       // true if the rows of the last step start a new sequence: the caller drops its sequence before decoding them
       MTMD_API bool mtmd_audio_stream_is_seq_start(const mtmd_audio_stream * stream);

       // bias for the text the caller samples while it transcribes an audio stream opened with a prompt (see mtmd_audio_stream_params)
       // writes up to n_max entries to out, returns the size of the bias
       MTMD_API int32_t mtmd_helper_audio_stream_get_logit_bias(const mtmd_context * mctx, const struct llama_model * model,
                                                                llama_logit_bias * out, int32_t n_max);
     */

    /*
     * // Audio generation API (added in v0.9.0, not yet wrapped)
       // represent the pipeline type
       enum mtmd_gen_audio_type {
           MTMD_GEN_AUDIO_TYPE_NONE, // not supported
           MTMD_GEN_AUDIO_TYPE_QWEN3TTS,
           MTMD_GEN_AUDIO_TYPE_POCKETTTS,
           MTMD_GEN_AUDIO_TYPE_SOPRANO,
           MTMD_GEN_AUDIO_TYPE_LFM3A,
           MTMD_GEN_AUDIO_TYPE_CHATTERBOX,
           MTMD_GEN_AUDIO_TYPE_DRAMA3,
       };

       struct mtmd_gen_audio_info {
           enum mtmd_gen_audio_type type;
           int32_t sample_rate; // in Hz, for example 24000 for qwen3tts
           const char * model_variant; // name of the weight variant, can be nullptr if not applicable
           int32_t n_voices; // predefined voices, see mtmd_gen_audio_voice_name()
       };

       struct mtmd_gen_state_params {
           uint32_t seed; // UINT32_MAX for random
       };

       enum mtmd_gen_process_type {
           MTMD_GEN_PROCESS_TYPE_GEN_CODE, // h_state to semantic (codes, mel-spectrogram, etc.)
           MTMD_GEN_PROCESS_TYPE_GEN_WAV,  // convert semantic to PCM audio
                                           // for qwen3tts, this is code2wav
                                           // for pocket-tts, this is mimi decoder
                                           // for lfm3a, this is the codec decoder + ISTFT, n_codes == 0 flushes the tail
       };

       struct mtmd_gen_inp {
           enum mtmd_gen_process_type type;

           // for MTMD_GEN_PROCESS_TYPE_GEN_CODE
           int32_t code0;  // the sampled codebook 0 entry from backbone
           float * embd;   // the hidden state from backbone, must have n_text_embd elements
           int32_t top_k;
           float   top_p;
           float   temp; // sampling temperature, or noise scale for flow-matching decoders (0 = no noise)
           float   penalty_repeat; // repetition penalty of the sampled codes, <= 1 is off
           int32_t penalty_last_n; // frames the repetition penalty looks back on

           // for MTMD_GEN_PROCESS_TYPE_GEN_WAV
           // pass either codes (discrete) or feats (continuous), depending on the pipeline
           // lfm3a GEN_CODE also takes codes: the codes revealed so far, -1 for a masked codebook
           int32_t * codes;
           size_t    n_codes;
           const float * feats;
           size_t        n_feats;
       };

       struct mtmd_gen_out {
           // note: output memory is owned by the state, valid until the next process() call with this state

           // for MTMD_GEN_PROCESS_TYPE_GEN_CODE
           const int32_t * codes;
           size_t          n_codes;
           const float * feats; // continuous counterpart of codes
           size_t        n_feats;
           const float * embd; // the generated hidden state, to be fed back to backbone
                               // it must have n_text_embd elements
           bool is_eos; // only set by pipelines having the EOS head inside mmproj

           // for MTMD_GEN_PROCESS_TYPE_GEN_WAV
           const float * audio;
           size_t        n_samples;
       };

       MTMD_API struct mtmd_gen_audio_info mtmd_gen_audio_get_info(const mtmd_context * ctx);

       // predefined voice with this name as a voice bitmap, to pass as speaker reference
       // returns nullptr if there is no such voice, the caller must free it
       MTMD_API mtmd_bitmap * mtmd_gen_audio_get_voice(const mtmd_context * ctx, const char * name);

       // name of the idx-th predefined voice, nullptr when idx is out of range
       MTMD_API const char * mtmd_gen_audio_voice_name(const mtmd_context * ctx, int32_t idx);

       // read a named tensor of the gen audio model as F32, returns the element count (0 if not found), out may be null to query the size
       MTMD_API size_t mtmd_gen_audio_read_tensor(mtmd_context * ctx, const char * name, float * out, size_t n_max);

       MTMD_API struct mtmd_gen_state_params mtmd_gen_state_params_default(void);

       // returns nullptr if the model does not support audio generation
       MTMD_API mtmd_gen_state * mtmd_gen_state_init(mtmd_context * ctx, struct mtmd_gen_state_params params);

       MTMD_API void             mtmd_gen_state_free(mtmd_gen_state * state);

       // clear the decoder state (cold start), keep the RNG stream
       MTMD_API void             mtmd_gen_state_reset(mtmd_gen_state * state);

       // copy the gen-side reference data of the last mtmd_encode_chunk() call into the state (speaker clip or predefined voice)
       // returns non-zero if the last encode produced no such data
       MTMD_API int32_t          mtmd_gen_state_set_ref(mtmd_context * ctx, mtmd_gen_state * state);

       // defaults tuned for the loaded pipeline, callers override only what they care about
       MTMD_API struct mtmd_gen_inp mtmd_gen_inp_default(const mtmd_context * ctx);

       // run one step and update the state in place
       // the caller drives the backbone and collects the audio frames
       // thread-safe across gen states
       MTMD_API int32_t mtmd_gen_audio_process(mtmd_context * ctx,
                                       mtmd_gen_state * state,
                                       const struct mtmd_gen_inp * inp,
                                       struct mtmd_gen_out * out);
     */

    /*
     * // Model capabilities helper (added in v0.9.0, not yet wrapped)
       struct mtmd_helper_cap {
           bool chat; // false for tts-only models (no chat template) and realtime transcription models (token delay)

           // input capabilities
           bool vision;
           bool audio;
           bool video; // also depending on MTMD_VIDEO build option

           // realtime capabilities
           bool audio_stream; // realtime transcription, see mtmd_audio_stream_init()

           // output capabilities
           bool text_gen; // false for an audio-only model (tts-only, no chat template)
           bool audio_gen;
           bool audio_gen_spk_ref; // accepts a speaker reference audio for voice cloning
       };

       // mctx can be NULL (text-only model)
       MTMD_API struct mtmd_helper_cap mtmd_helper_get_capabilities(const struct llama_model * model, const struct mtmd_context * mctx);
     */

    /*
     * // Audio generation helper (added in v0.9.0, not yet wrapped)
       // what a response that continues a sequence (n_past > 0) outputs
       enum mtmd_helper_gen_audio_out_mode {
           MTMD_HELPER_GEN_AUDIO_OUT_AUTO,  // the model switches between text and audio itself
           MTMD_HELPER_GEN_AUDIO_OUT_TEXT,
           MTMD_HELPER_GEN_AUDIO_OUT_AUDIO,
           MTMD_HELPER_GEN_AUDIO_OUT_MIXED, // interleaved text and audio
       };

       enum mtmd_helper_gen_audio_outtype {
           MTMD_HELPER_GEN_AUDIO_OUTTYPE_PCM, // raw PCM
           MTMD_HELPER_GEN_AUDIO_OUTTYPE_WAV, // WAV PCM 16-bit LE, mono
       };

       struct mtmd_helper_gen_audio_inp {
           bool stream; // if true, output() must be called after each step_gen()
           llama_seq_id seq_id;

           // the text to speak; for a model with a "tts" chat template (e.g. LFM3-Audio), the prompt rendered from it
           const char * prompt;
           size_t       prompt_len;

           // > 0: continue a sequence of n_past positions with raw text (LFM3-Audio only)
           int32_t n_past;
           enum mtmd_helper_gen_audio_out_mode out_mode; // n_past > 0 only (default: auto)

           mtmd_bitmap * speaker_ref; // optional, can be NULL: audio to clone, or a voice bitmap
           const char * lang; // optional, can be NULL

           int32_t  top_k;
           float    top_p;
           float    temp; // audio frame temperature if the model supports it, < 0 = model default, 0 = greedy
           uint32_t seed; // UINT32_MAX for random (default: random)

           enum mtmd_helper_gen_audio_outtype out_type;
       };

       MTMD_API struct mtmd_helper_gen_audio_inp mtmd_helper_gen_audio_inp_default(void);

       MTMD_API mtmd_helper_gen_audio * mtmd_helper_gen_audio_init(
                                           struct llama_context * lctx,
                                           struct mtmd_context * mctx);

       MTMD_API void mtmd_helper_gen_audio_free(mtmd_helper_gen_audio * ctx);

       MTMD_API void mtmd_helper_gen_audio_reset(mtmd_helper_gen_audio * ctx);

       MTMD_API int32_t mtmd_helper_gen_audio_set_input(
                               mtmd_helper_gen_audio * ctx,
                               const struct mtmd_helper_gen_audio_inp * inp);

       // processes at most n_batch prompt tokens per call
       // returns: >0 = number of prompt tokens remaining, 0 = done, <0 = error
       MTMD_API int32_t mtmd_helper_gen_audio_step_prompt(
                               mtmd_helper_gen_audio * ctx,
                               int32_t n_batch);

       // generates one frame; must only be called after step_prompt() has returned 0
       // sampled can be LLAMA_TOKEN_NULL for pipelines with no discrete backbone token
       // out_stop (optional) is set on end-of-speech, the caller must then stop the loop
       // h_state_out is valid until next step_gen() or reset() call, null if no frame is generated
       MTMD_API int32_t mtmd_helper_gen_audio_step_gen(
                               mtmd_helper_gen_audio * ctx,
                               llama_token sampled,
                               const float *  h_state_in,
                               const float ** h_state_out,
                               bool * out_stop);

       // true if the next step_gen() takes a text token: the caller samples it from the logits and passes it as sampled
       // false if the next step generates an audio frame from h_state_in
       MTMD_API bool mtmd_helper_gen_audio_next_is_text(mtmd_helper_gen_audio * ctx);

       // bias for the text tokens the caller samples (e.g. no switch to audio in a text-only response), n_bias is set to its size
       MTMD_API const llama_logit_bias * mtmd_helper_gen_audio_get_logit_bias(mtmd_helper_gen_audio * ctx, size_t * n_bias);

       // out_data valid until next get_output() or reset() call
       // out_n_samples (optional, can be NULL) receives the number of generated PCM samples
       // if inp->stream is true: returns only audio produced since the previous call, and
       // *out_data_len == 0 whenever a full window_frames batch hasn't accumulated yet
       MTMD_API int32_t mtmd_helper_gen_audio_get_output(
                               mtmd_helper_gen_audio * ctx,
                               int32_t * out_sample_rate,
                               const char ** out_data,
                               size_t * out_data_len,
                               int64_t * out_n_samples);

       // forces any buffered codes through code2wav now, regardless of window_frames;
       // call once when generation has ended, before the last get_output() in stream mode
       MTMD_API int32_t mtmd_helper_gen_audio_flush(mtmd_helper_gen_audio * ctx);
     */

    /*
     * // Streaming audio resampler (added in v1.0.0, not yet wrapped)
       // returns NULL on failure
       MTMD_API mtmd_helper_resampler * mtmd_helper_resampler_init(int32_t rate_in, int32_t rate_out);

       MTMD_API void                    mtmd_helper_resampler_free(mtmd_helper_resampler * ctx);

       // *out is valid until the next call
       // returns the number of output samples (can be 0), < 0 on error
       MTMD_API int64_t mtmd_helper_resampler_process(mtmd_helper_resampler * ctx, const float * in, size_t n_in, const float ** out);
     */
}
