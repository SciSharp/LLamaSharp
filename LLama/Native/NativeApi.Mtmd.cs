using System;

namespace LLama.Native;

/// <summary>
/// P/Invoke surface for MTMD (multimodal) helpers exposed by llama.cpp.
/// </summary>
public static partial class NativeApi
{
    
    /// <summary>
    /// Native context parameters returned by <see cref="mtmd_context_params_default"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct mtmd_context_params
    {
        [MarshalAs(UnmanagedType.I1)] public bool use_gpu;
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
    }

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_default_marker", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_default_marker();

    /// <summary>
    /// Retrieve the default multimodal marker text.
    /// </summary>
    public static string? MtmdDefaultMarker()
        => mtmd_default_marker().PtrToString();

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_context_params_default", CallingConvention = CallingConvention.Cdecl)]
    internal static extern mtmd_context_params mtmd_context_params_default();

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_decode_use_non_causal", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool mtmd_decode_use_non_causal(IntPtr ctx);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_decode_use_mrope", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool mtmd_decode_use_mrope(IntPtr ctx);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_support_vision", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool mtmd_support_vision(IntPtr ctx);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_support_audio", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool mtmd_support_audio(IntPtr ctx);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_get_audio_bitrate", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_get_audio_bitrate(IntPtr ctx);

    // bitmap ------------------------------------------------------------

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_init", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_bitmap_init(uint nx, uint ny, IntPtr data);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_init_from_audio", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_bitmap_init_from_audio(ulong n_samples, IntPtr data);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_get_nx", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint mtmd_bitmap_get_nx(IntPtr bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_get_ny", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint mtmd_bitmap_get_ny(IntPtr bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_get_data", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_bitmap_get_data(IntPtr bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_get_n_bytes", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_bitmap_get_n_bytes(IntPtr bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_is_audio", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static extern bool mtmd_bitmap_is_audio(IntPtr bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_free", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void mtmd_bitmap_free(IntPtr bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_get_id", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_bitmap_get_id(IntPtr bitmap);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_bitmap_set_id", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void mtmd_bitmap_set_id_native(IntPtr bitmap, byte* id);

    /// <summary>
    /// Assign an identifier to a bitmap using a UTF-8 encoded string.
    /// </summary>
    internal static unsafe void mtmd_bitmap_set_id(IntPtr bitmap, string? id)
    {
        if (bitmap == IntPtr.Zero)
            throw new ArgumentNullException(nameof(bitmap));

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
    internal static extern int mtmd_input_chunk_get_type(IntPtr chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_tokens_text", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_input_chunk_get_tokens_text(IntPtr chunk, out UIntPtr n_tokens);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_tokens_image", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_input_chunk_get_tokens_image(IntPtr chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_n_tokens", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_input_chunk_get_n_tokens(IntPtr chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_id", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_input_chunk_get_id(IntPtr chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_get_n_pos", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_input_chunk_get_n_pos(IntPtr chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_copy", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_input_chunk_copy(IntPtr chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_input_chunk_free", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void mtmd_input_chunk_free(IntPtr chunk);

    // image_tokens ------------------------------------------------------

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_n_tokens", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_image_tokens_get_n_tokens(IntPtr image_tokens);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_nx", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_image_tokens_get_nx(IntPtr image_tokens);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_ny", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_image_tokens_get_ny(IntPtr image_tokens);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_id", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_image_tokens_get_id(IntPtr image_tokens);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_image_tokens_get_n_pos", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_image_tokens_get_n_pos(IntPtr image_tokens);

    // tokenize ----------------------------------------------------------

    /// <summary>
    /// Native text structure consumed by <see cref="mtmd_tokenize"/>.
    /// </summary>
    internal unsafe struct mtmd_input_text_native
    {
        public byte* text;
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
                add_special = addSpecial,
                parse_special = parseSpecial
            };
        }

        public void Dispose() => _text.Dispose();
    }

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_tokenize", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int mtmd_tokenize_native(
        IntPtr ctx,
        IntPtr output,
        mtmd_input_text_native* text,
        IntPtr[] bitmaps,
        UIntPtr n_bitmaps);

    internal static unsafe int mtmd_tokenize(IntPtr ctx, IntPtr output, in mtmd_input_text_native text, IntPtr[] bitmaps, UIntPtr n_bitmaps)
    {
        var temp = text;
        return mtmd_tokenize_native(ctx, output, &temp, bitmaps, n_bitmaps);
    }

    internal static unsafe int mtmd_tokenize(IntPtr ctx, IntPtr output, string text, bool addSpecial, bool parseSpecial, IntPtr[] bitmaps, UIntPtr n_bitmaps)
    {
        using var scope = new MtmdInputTextScope(text, addSpecial, parseSpecial);
        return mtmd_tokenize_native(ctx, output, &scope.Value, bitmaps, n_bitmaps);
    }

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_encode", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_encode(IntPtr ctx, IntPtr image_tokens);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_encode_chunk", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_encode_chunk(IntPtr ctx, IntPtr chunk);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_get_output_embd", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_get_output_embd(IntPtr ctx);

    // helper ------------------------------------------------------------

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_test_create_input_chunks", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_test_create_input_chunks();

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_bitmap_init_from_file", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe IntPtr mtmd_helper_bitmap_init_from_file_native(IntPtr ctx, byte* fname);

    internal static unsafe IntPtr mtmd_helper_bitmap_init_from_file(IntPtr ctx, string fname)
    {
        using var pinned = PinnedUtf8String.Create(fname) ?? throw new ArgumentNullException(nameof(fname));
        return mtmd_helper_bitmap_init_from_file_native(ctx, (byte*)pinned.Pointer);
    }

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_bitmap_init_from_buf", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr mtmd_helper_bitmap_init_from_buf(IntPtr ctx, IntPtr buf, UIntPtr len);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_get_n_tokens", CallingConvention = CallingConvention.Cdecl)]
    internal static extern UIntPtr mtmd_helper_get_n_tokens(IntPtr chunks);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_get_n_pos", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_helper_get_n_pos(IntPtr chunks);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_eval_chunks", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_helper_eval_chunks(
        IntPtr ctx,
        IntPtr lctx,
        IntPtr chunks,
        int n_past,
        int seq_id,
        int n_batch,
        [MarshalAs(UnmanagedType.I1)] bool logits_last,
        ref int new_n_past);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_eval_chunk_single", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_helper_eval_chunk_single(
        IntPtr ctx,
        IntPtr lctx,
        IntPtr chunk,
        int n_past,
        int seq_id,
        int n_batch,
        [MarshalAs(UnmanagedType.I1)] bool logits_last,
        ref int new_n_past);

    [DllImport(mtmdLibraryName, EntryPoint = "mtmd_helper_decode_image_chunk", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mtmd_helper_decode_image_chunk(
        IntPtr ctx,
        IntPtr lctx,
        IntPtr chunk,
        IntPtr encoded_embd,
        int n_past,
        int seq_id,
        int n_batch,
        ref int new_n_past);
}
