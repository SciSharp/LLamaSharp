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
    /// Native text structure consumed by <see cref="NativeApi.mtmd_tokenize(LLama.Native.SafeMtmdModelHandle,System.IntPtr,in LLama.Native.NativeApi.mtmd_input_text_native,System.IntPtr[],System.UIntPtr)"/>.
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

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_test_create_input_chunks", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_test_create_input_chunks();

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_bitmap_init_from_file", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe mtmd_helper_bitmap_wrapper mtmd_helper_bitmap_init_from_file_native(SafeMtmdModelHandle ctx, byte* fname, bool placeholder);

    internal static unsafe IntPtr mtmd_helper_bitmap_init_from_file(SafeMtmdModelHandle ctx, string fname)
    {
        using var pinned = PinnedUtf8String.Create(fname) ?? throw new ArgumentNullException(nameof(fname));
        return mtmd_helper_bitmap_init_from_file_native(ctx, (byte*)pinned.Pointer, false).bitmap;
    }

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_bitmap_init_from_buf", CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe mtmd_helper_bitmap_wrapper mtmd_helper_bitmap_init_from_buf_native(SafeMtmdModelHandle ctx, byte* buf, nuint len, bool placeholder);

    internal static unsafe IntPtr mtmd_helper_bitmap_init_from_buf(SafeMtmdModelHandle ctx, byte* buf, nuint len)
    {
        return mtmd_helper_bitmap_init_from_buf_native(ctx, buf, len, false).bitmap;
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
}
