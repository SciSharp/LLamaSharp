
using System.Runtime.InteropServices;
using System;

namespace LLama.Native;

public unsafe partial class NativeApi
{
#if NETSTANDARD
#region NativeApi.cs
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern long llama_max_devices_r();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate long llama_max_device_t();
    public static long llama_max_devices() => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_max_devices_r() : _llamaNativeLibraryHolder.LoadFunction<llama_max_device_t>("llama_max_devices")();

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool llama_supports_mmap_r();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private delegate bool llama_supports_mmap_t();
    public static bool llama_supports_mmap() => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_supports_mmap_r() : _llamaNativeLibraryHolder.LoadFunction<llama_supports_mmap_t>("llama_supports_mmap")();

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool llama_supports_mlock_r();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private delegate bool llama_supports_mlock_t();
    public static bool llama_supports_mlock() => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_supports_mlock_r() : _llamaNativeLibraryHolder.LoadFunction<llama_supports_mlock_t>("llama_supports_mlock")();

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool llama_supports_gpu_offload_r();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private delegate bool llama_supports_gpu_offload_t();
    public static bool llama_supports_gpu_offload() => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_supports_gpu_offload_r() : _llamaNativeLibraryHolder.LoadFunction<llama_supports_gpu_offload_t>("llama_supports_gpu_offload")();

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_backend_init_r();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_backend_init_t();
    public static void llama_backend_init()
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_backend_init_r();
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_backend_init_t>("llama_backend_init")();
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool llama_state_load_file_r(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens_out, ulong n_token_capacity, out ulong n_token_count_out);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private delegate bool llama_state_load_file_t(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens_out, ulong n_token_capacity, out ulong n_token_count_out);
    public static bool llama_state_load_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens_out, ulong n_token_capacity, out ulong n_token_count_out) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_state_load_file_r(ctx, path_session, tokens_out, n_token_capacity, out n_token_count_out) : _llamaNativeLibraryHolder.LoadFunction<llama_state_load_file_t>("llama_state_load_file")(ctx, path_session, tokens_out, n_token_capacity, out n_token_count_out);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool llama_state_save_file_r(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens, ulong n_token_count);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private delegate bool llama_state_save_file_t(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens, ulong n_token_count);
    public static bool llama_state_save_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens, ulong n_token_count) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_state_save_file_r(ctx, path_session, tokens, n_token_count) : _llamaNativeLibraryHolder.LoadFunction<llama_state_save_file_t>("llama_state_save_file")(ctx, path_session, tokens, n_token_count);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe nuint llama_state_seq_save_file_r(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId seq_id, LLamaToken* tokens, nuint n_token_count);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nuint llama_state_seq_save_file_t(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId seq_id, LLamaToken* tokens, nuint n_token_count);
    public static unsafe nuint llama_state_seq_save_file(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId seq_id, LLamaToken* tokens, nuint n_token_count) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_state_seq_save_file_r(ctx, filepath, seq_id, tokens, n_token_count) : _llamaNativeLibraryHolder.LoadFunction<llama_state_seq_save_file_t>("llama_state_seq_save_file")(ctx, filepath, seq_id, tokens, n_token_count);
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe nuint llama_state_seq_load_file_r(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId dest_seq_id, LLamaToken* tokens_out, nuint n_token_capacity, out nuint n_token_count_out);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nuint llama_state_seq_load_file_t(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId dest_seq_id, LLamaToken* tokens_out, nuint n_token_capacity, out nuint n_token_count_out);
    public static unsafe nuint llama_state_seq_load_file(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId dest_seq_id, LLamaToken* tokens_out, nuint n_token_capacity, out nuint n_token_count_out) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_state_seq_load_file_r(ctx, filepath, dest_seq_id, tokens_out, n_token_capacity, out n_token_count_out) : _llamaNativeLibraryHolder.LoadFunction<llama_state_seq_load_file_t>("llama_state_seq_load_file")(ctx, filepath, dest_seq_id, tokens_out, n_token_capacity, out n_token_count_out);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe byte* llama_token_get_text_r(SafeLlamaModelHandle model, LLamaToken token);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate byte* llama_token_get_text_t(SafeLlamaModelHandle model, LLamaToken token);
    public static unsafe byte* llama_token_get_text(SafeLlamaModelHandle model, LLamaToken token) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_token_get_text_r(model, token) : _llamaNativeLibraryHolder.LoadFunction<llama_token_get_text_t>("llama_token_get_text")(model, token);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_set_causal_attn_r(SafeLlamaModelHandle ctx, bool causal_attn);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_set_causal_attn_t(SafeLlamaModelHandle ctx, bool causal_attn);
    public static void llama_set_causal_attn(SafeLlamaModelHandle ctx, bool causal_attn)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_set_causal_attn_r(ctx, causal_attn);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_set_causal_attn_t>("llama_set_causal_attn")(ctx, causal_attn);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_set_abort_callback_r(SafeLlamaModelHandle ctx, IntPtr /* ggml_abort_callback */ abort_callback, IntPtr abort_callback_data);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_set_abort_callback_t(SafeLlamaModelHandle ctx, IntPtr /* ggml_abort_callback */ abort_callback, IntPtr abort_callback_data);
    public static void llama_set_abort_callback(SafeLlamaModelHandle ctx, IntPtr /* ggml_abort_callback */ abort_callback, IntPtr abort_callback_data)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_set_abort_callback_r(ctx, abort_callback, abort_callback_data);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_set_abort_callback_t>("llama_set_abort_callback")(ctx, abort_callback, abort_callback_data);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_synchronize_r(SafeLlamaModelHandle ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_synchronize_t(SafeLlamaModelHandle ctx);
    public static void llama_synchronize(SafeLlamaModelHandle ctx)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_synchronize_r(ctx);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_synchronize_t>("llama_synchronize")(ctx);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern float llama_token_get_score_r(SafeLlamaModelHandle model, LLamaToken token);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate float llama_token_get_score_t(SafeLlamaModelHandle model, LLamaToken token);
    public static float llama_token_get_score(SafeLlamaModelHandle model, LLamaToken token) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_token_get_score_r(model, token) : _llamaNativeLibraryHolder.LoadFunction<llama_token_get_score_t>("llama_token_get_score")(model, token);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern LLamaTokenType llama_token_get_type_r(SafeLlamaModelHandle model, LLamaToken token);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate LLamaTokenType llama_token_get_type_t(SafeLlamaModelHandle model, LLamaToken token);
    public static LLamaTokenType llama_token_get_type(SafeLlamaModelHandle model, LLamaToken token) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_token_get_type_r(model, token) : _llamaNativeLibraryHolder.LoadFunction<llama_token_get_type_t>("llama_token_get_type")(model, token);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint llama_n_seq_max_r(SafeLLamaContextHandle ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint llama_n_seq_max_t(SafeLLamaContextHandle ctx);
    public static uint llama_n_seq_max(SafeLLamaContextHandle ctx) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_n_seq_max_r(ctx) : _llamaNativeLibraryHolder.LoadFunction<llama_n_seq_max_t>("llama_n_seq_max")(ctx);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern LLamaPoolingType llama_pooling_type_r(SafeLLamaContextHandle ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate LLamaPoolingType llama_pooling_type_t(SafeLLamaContextHandle ctx);
    public static LLamaPoolingType llama_pooling_type(SafeLLamaContextHandle ctx) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_pooling_type_r(ctx) : _llamaNativeLibraryHolder.LoadFunction<llama_pooling_type_t>("llama_pooling_type")(ctx);
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe float* llama_get_embeddings_seq_r(SafeLLamaContextHandle ctx, LLamaSeqId id);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate float* llama_get_embeddings_seq_t(SafeLLamaContextHandle ctx, LLamaSeqId id);
    public static unsafe float* llama_get_embeddings_seq(SafeLLamaContextHandle ctx, LLamaSeqId id) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_get_embeddings_seq_r(ctx, id) : _llamaNativeLibraryHolder.LoadFunction<llama_get_embeddings_seq_t>("llama_get_embeddings_seq")(ctx, id);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe float* llama_get_embeddings_ith_r(SafeLLamaContextHandle ctx, int i);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate float* llama_get_embeddings_ith_t(SafeLLamaContextHandle ctx, int i);
    public static unsafe float* llama_get_embeddings_ith(SafeLLamaContextHandle ctx, int i) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_get_embeddings_ith_r(ctx, i) : _llamaNativeLibraryHolder.LoadFunction<llama_get_embeddings_ith_t>("llama_get_embeddings_ith")(ctx, i);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe float* llama_get_embeddings_r(SafeLLamaContextHandle ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate float* llama_get_embeddings_t(SafeLLamaContextHandle ctx);
    public static unsafe float* llama_get_embeddings(SafeLLamaContextHandle ctx) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_get_embeddings_r(ctx) : _llamaNativeLibraryHolder.LoadFunction<llama_get_embeddings_t>("llama_get_embeddings")(ctx);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int llama_add_bos_token_r(SafeLlamaModelHandle model);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int llama_add_bos_token_t(SafeLlamaModelHandle model);
    public static int llama_add_bos_token(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_add_bos_token_r(model) : _llamaNativeLibraryHolder.LoadFunction<llama_add_bos_token_t>("llama_add_bos_token")(model);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int llama_add_eos_token_r(SafeLlamaModelHandle model);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int llama_add_eos_token_t(SafeLlamaModelHandle model);
    public static int llama_add_eos_token(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_add_eos_token_r(model) : _llamaNativeLibraryHolder.LoadFunction<llama_add_eos_token_t>("llama_add_eos_token")(model);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_print_timings_r(SafeLLamaContextHandle ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_print_timings_t(SafeLLamaContextHandle ctx);
    public static void llama_print_timings(SafeLLamaContextHandle ctx)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_print_timings_r(ctx);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_print_timings_t>("llama_print_timings")(ctx);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_reset_timings_r(SafeLLamaContextHandle ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_reset_timings_t(SafeLLamaContextHandle ctx);
    public static void llama_reset_timings(SafeLLamaContextHandle ctx)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_reset_timings_r(ctx);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_reset_timings_t>("llama_reset_timings")(ctx);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr llama_print_system_info_r();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr llama_print_system_info_t();
    public static IntPtr llama_print_system_info() => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_print_system_info_r() : _llamaNativeLibraryHolder.LoadFunction<llama_print_system_info_t>("llama_print_system_info")();

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int llama_tokenize_r(SafeLlamaModelHandle model, byte* text, int text_len, LLamaToken* tokens, int n_max_tokens, bool add_special, bool parse_special);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int llama_tokenize_t(SafeLlamaModelHandle model, byte* text, int text_len, LLamaToken* tokens, int n_max_tokens, bool add_special, bool parse_special);
    public static unsafe int llama_tokenize(SafeLlamaModelHandle model, byte* text, int text_len, LLamaToken* tokens, int n_max_tokens, bool add_special, bool parse_special) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_tokenize_r(model, text, text_len, tokens, n_max_tokens, add_special, parse_special) : _llamaNativeLibraryHolder.LoadFunction<llama_tokenize_t>("llama_tokenize")(model, text, text_len, tokens, n_max_tokens, add_special, parse_special);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int llama_get_kv_cache_token_count_r(SafeLLamaContextHandle ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int llama_get_kv_cache_token_count_t(SafeLLamaContextHandle ctx);
    public static int llama_get_kv_cache_token_count(SafeLLamaContextHandle ctx) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_get_kv_cache_token_count_r(ctx) : _llamaNativeLibraryHolder.LoadFunction<llama_get_kv_cache_token_count_t>("llama_get_kv_cache_token_count")(ctx);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int llama_get_kv_cache_used_cells_r(SafeLLamaContextHandle ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int llama_get_kv_cache_used_cells_t(SafeLLamaContextHandle ctx);
    public static int llama_get_kv_cache_used_cells(SafeLLamaContextHandle ctx) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_get_kv_cache_used_cells_r(ctx) : _llamaNativeLibraryHolder.LoadFunction<llama_get_kv_cache_used_cells_t>("llama_get_kv_cache_used_cells")(ctx);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_kv_cache_clear_r(SafeLLamaContextHandle ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_kv_cache_clear_t(SafeLLamaContextHandle ctx);
    public static void llama_kv_cache_clear(SafeLLamaContextHandle ctx)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_kv_cache_clear_r(ctx);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_kv_cache_clear_t>("llama_kv_cache_clear")(ctx);

        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern byte llama_kv_cache_seq_rm_r(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate byte llama_kv_cache_seq_rm_t(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1);
    public static byte llama_kv_cache_seq_rm(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_kv_cache_seq_rm_r(ctx, seq, p0, p1) : _llamaNativeLibraryHolder.LoadFunction<llama_kv_cache_seq_rm_t>("llama_kv_cache_seq_rm")(ctx, seq, p0, p1);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_kv_cache_seq_cp_r(SafeLLamaContextHandle ctx, LLamaSeqId src, LLamaSeqId dest, LLamaPos p0, LLamaPos p1);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_kv_cache_seq_cp_t(SafeLLamaContextHandle ctx, LLamaSeqId src, LLamaSeqId dest, LLamaPos p0, LLamaPos p1);
    public static void llama_kv_cache_seq_cp(SafeLLamaContextHandle ctx, LLamaSeqId src, LLamaSeqId dest, LLamaPos p0, LLamaPos p1)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_kv_cache_seq_cp_r(ctx, src, dest, p0, p1);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_kv_cache_seq_cp_t>("llama_kv_cache_seq_cp")(ctx, src, dest, p0, p1);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_kv_cache_seq_keep_r(SafeLLamaContextHandle ctx, LLamaSeqId seq);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_kv_cache_seq_keep_t(SafeLLamaContextHandle ctx, LLamaSeqId seq);
    public static void llama_kv_cache_seq_keep(SafeLLamaContextHandle ctx, LLamaSeqId seq)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_kv_cache_seq_keep_r(ctx, seq);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_kv_cache_seq_keep_t>("llama_kv_cache_seq_keep")(ctx, seq);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_kv_cache_seq_add_r(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_kv_cache_seq_add_t(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta);
    public static void llama_kv_cache_seq_add(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_kv_cache_seq_add_r(ctx, seq, p0, p1, delta);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_kv_cache_seq_add_t>("llama_kv_cache_seq_add")(ctx, seq, p0, p1, delta);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_kv_cache_seq_div_r(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int d);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_kv_cache_seq_div_t(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int d);
    public static void llama_kv_cache_seq_div(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int d)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_kv_cache_seq_div_r(ctx, seq, p0, p1, d);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_kv_cache_seq_div_t>("llama_kv_cache_seq_div")(ctx, seq, p0, p1, d);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern LLamaPos llama_kv_cache_seq_pos_max_r(SafeLLamaContextHandle ctx, LLamaSeqId seq);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate LLamaPos llama_kv_cache_seq_pos_max_t(SafeLLamaContextHandle ctx, LLamaSeqId seq);
    public static LLamaPos llama_kv_cache_seq_pos_max(SafeLLamaContextHandle ctx, LLamaSeqId seq) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_kv_cache_seq_pos_max_r(ctx, seq) : _llamaNativeLibraryHolder.LoadFunction<llama_kv_cache_seq_pos_max_t>("llama_kv_cache_seq_pos_max")(ctx, seq);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern LLamaNativeBatch llama_batch_init_r(int n_tokens, int embd, int n_seq_max);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate LLamaNativeBatch llama_batch_init_t(int n_tokens, int embd, int n_seq_max);
    public static LLamaNativeBatch llama_batch_init(int n_tokens, int embd, int n_seq_max) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_batch_init_r(n_tokens, embd, n_seq_max) : _llamaNativeLibraryHolder.LoadFunction<llama_batch_init_t>("llama_batch_init")(n_tokens, embd, n_seq_max);
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_batch_free_r(LLamaNativeBatch batch);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_batch_free_t(LLamaNativeBatch batch);
    public static void llama_batch_free(LLamaNativeBatch batch)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_batch_free_r(batch);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_batch_free_t>("llama_batch_free")(batch);
        }
    }
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int llama_control_vector_apply_r(SafeLLamaContextHandle ctx, float* data, nuint len, int n_embd, int il_start, int il_end);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int llama_control_vector_apply_t(SafeLLamaContextHandle ctx, float* data, nuint len, int n_embd, int il_start, int il_end);
    public static unsafe int llama_control_vector_apply(SafeLLamaContextHandle ctx, float* data, nuint len, int n_embd, int il_start, int il_end) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_control_vector_apply_r(ctx, data, len, n_embd, il_start, il_end) : _llamaNativeLibraryHolder.LoadFunction<llama_control_vector_apply_t>("llama_control_vector_apply")(ctx, data, len, n_embd, il_start, il_end);
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int llama_split_path_r(string split_path, nuint maxlen, string path_prefix, int split_no, int split_count);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int llama_split_path_t(string split_path, nuint maxlen, string path_prefix, int split_no, int split_count);
    public static int llama_split_path(string split_path, nuint maxlen, string path_prefix, int split_no, int split_count) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_split_path_r(split_path, maxlen, path_prefix, split_no, split_count) : _llamaNativeLibraryHolder.LoadFunction<llama_split_path_t>("llama_split_path")(split_path, maxlen, path_prefix, split_no, split_count);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int llama_split_prefix_r(string split_prefix, nuint maxlen, string split_path, int split_no, int split_count);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int llama_split_prefix_t(string split_prefix, nuint maxlen, string split_path, int split_no, int split_count);
    public static int llama_split_prefix(string split_prefix, nuint maxlen, string split_path, int split_no, int split_count) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_split_prefix_r(split_prefix, maxlen, split_path, split_no, split_count) : _llamaNativeLibraryHolder.LoadFunction<llama_split_prefix_t>("llama_split_prefix")(split_prefix, maxlen, split_path, split_no, split_count);

#endregion

#region NativeApi.BeamSearch.cs
    public delegate void LLamaBeamSearchCallback(IntPtr callback_data, LLamaBeamsState state);
    
    [DllImport(libraryName, EntryPoint = "llama_beam_search", CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_beam_search_r(SafeLLamaContextHandle ctx, LLamaBeamSearchCallback callback, IntPtr callback_data, ulong n_beams, int n_past, int n_predict, int n_threads);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_beam_search_t(SafeLLamaContextHandle ctx, LLamaBeamSearchCallback callback, IntPtr callback_data, ulong n_beams, int n_past, int n_predict, int n_threads);
    public static void llama_beam_search(SafeLLamaContextHandle ctx, LLamaBeamSearchCallback callback, IntPtr callback_data, ulong n_beams, int n_past, int n_predict, int n_threads)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_beam_search_r(ctx, callback, callback_data, n_beams, n_past, n_predict, n_threads);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_beam_search_t>("llama_beam_search")(ctx, callback, callback_data, n_beams, n_past, n_predict, n_threads);
        }
    }
#endregion

#region NativeApi.Grammar.cs

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe SafeLLamaGrammarHandle llama_grammar_init_r(LLamaGrammarElement** rules, ulong n_rules, ulong start_rule_index);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate SafeLLamaGrammarHandle llama_grammar_init_t(LLamaGrammarElement** rules, ulong n_rules, ulong start_rule_index);
    public static unsafe SafeLLamaGrammarHandle llama_grammar_init(LLamaGrammarElement** rules, ulong n_rules, ulong start_rule_index) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_grammar_init_r(rules, n_rules, start_rule_index) : _llamaNativeLibraryHolder.LoadFunction<llama_grammar_init_t>("llama_grammar_init")(rules, n_rules, start_rule_index);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_grammar_free_r(IntPtr grammar);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_grammar_free_t(IntPtr grammar);
    public static void llama_grammar_free(IntPtr grammar)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_grammar_free_r(grammar);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_grammar_free_t>("llama_grammar_free")(grammar);
        }
    }
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern SafeLLamaGrammarHandle llama_grammar_copy_r(SafeLLamaGrammarHandle grammar);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate SafeLLamaGrammarHandle llama_grammar_copy_t(SafeLLamaGrammarHandle grammar);
    public static SafeLLamaGrammarHandle llama_grammar_copy(SafeLLamaGrammarHandle grammar) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_grammar_copy_r(grammar) : _llamaNativeLibraryHolder.LoadFunction<llama_grammar_copy_t>("llama_grammar_copy")(grammar);

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sample_grammar_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, SafeLLamaGrammarHandle grammar);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_grammar_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, SafeLLamaGrammarHandle grammar);
    public static void llama_sample_grammar(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, SafeLLamaGrammarHandle grammar)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_grammar_r(ctx, ref candidates, grammar);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_grammar_t>("llama_sample_grammar")(ctx, ref candidates, grammar);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_grammar_accept_token_r(SafeLLamaContextHandle ctx, SafeLLamaGrammarHandle grammar, LLamaToken token);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_grammar_accept_token_t(SafeLLamaContextHandle ctx, SafeLLamaGrammarHandle grammar, LLamaToken token);
    public static void llama_grammar_accept_token(SafeLLamaContextHandle ctx, SafeLLamaGrammarHandle grammar, LLamaToken token)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_grammar_accept_token_r(ctx, grammar, token);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_grammar_accept_token_t>("llama_grammar_accept_token")(ctx, grammar, token);
        }
    }

#endregion

#region NativeApi.LLava.cs

    [DllImport(llavaLibraryName, EntryPoint = "llava_image_embed_get_image_size", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool llava_validate_embed_size_r(SafeLLamaContextHandle ctxLlama, SafeLlavaModelHandle ctxClip);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private delegate bool llava_validate_embed_size_t(SafeLLamaContextHandle ctxLlama, SafeLlavaModelHandle ctxClip);
    public static bool llava_validate_embed_size(SafeLLamaContextHandle ctxLlama, SafeLlavaModelHandle ctxClip) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llava_validate_embed_size_r(ctxLlama, ctxClip) : _llavaNativeLibraryHolder.LoadFunction<llava_validate_embed_size_t>("llava_validate_embed_size")(ctxLlama, ctxClip);
    
    [DllImport(llavaLibraryName, EntryPoint = "llava_image_embed_make_with_bytes", CallingConvention = CallingConvention.Cdecl)]
    private static extern SafeLlavaImageEmbedHandle llava_image_embed_make_with_bytes_r(SafeLlavaModelHandle ctxClip, int nThreads, byte[] imageBytes, int imageBytesLength);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate SafeLlavaImageEmbedHandle llava_image_embed_make_with_bytes_t(SafeLlavaModelHandle ctxClip, int nThreads, byte[] imageBytes, int imageBytesLength);
    public static SafeLlavaImageEmbedHandle llava_image_embed_make_with_bytes(SafeLlavaModelHandle ctxClip, int nThreads, byte[] imageBytes, int imageBytesLength) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llava_image_embed_make_with_bytes_r(ctxClip, nThreads, imageBytes, imageBytesLength) : _llavaNativeLibraryHolder.LoadFunction<llava_image_embed_make_with_bytes_t>("llava_image_embed_make_with_bytes")(ctxClip, nThreads, imageBytes, imageBytesLength);

    [DllImport(llavaLibraryName, EntryPoint = "llava_image_embed_make_with_filename", CallingConvention = CallingConvention.Cdecl)]
    private static extern SafeLlavaImageEmbedHandle llava_image_embed_make_with_filename_r(SafeLlavaModelHandle ctxClip, int nThreads, [MarshalAs(UnmanagedType.LPStr)] string imagePath);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate SafeLlavaImageEmbedHandle llava_image_embed_make_with_filename_t(SafeLlavaModelHandle ctxClip, int nThreads, [MarshalAs(UnmanagedType.LPStr)] string imagePath);
    public static SafeLlavaImageEmbedHandle llava_image_embed_make_with_filename(SafeLlavaModelHandle ctxClip, int nThreads, string imagePath) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llava_image_embed_make_with_filename_r(ctxClip, nThreads, imagePath) : _llavaNativeLibraryHolder.LoadFunction<llava_image_embed_make_with_filename_t>("llava_image_embed_make_with_filename")(ctxClip, nThreads, imagePath);
    
    [DllImport(llavaLibraryName, EntryPoint = "llava_image_embed_free", CallingConvention = CallingConvention.Cdecl)]
    private static extern void llava_image_embed_free_r(IntPtr embed);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llava_image_embed_free_t(IntPtr embed);
    public static void llava_image_embed_free(IntPtr embed)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llava_image_embed_free_r(embed);
        }
        else
        {
            _llavaNativeLibraryHolder.LoadFunction<llava_image_embed_free_t>("llava_image_embed_free")(embed);
        }
    }
    
    [DllImport(llavaLibraryName, EntryPoint = "llava_eval_image_embed", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool llava_eval_image_embed_r(SafeLLamaContextHandle ctxLlama, SafeLlavaImageEmbedHandle embed, int nBatch, ref int nPast);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private delegate bool llava_eval_image_embed_t(SafeLLamaContextHandle ctxLlama, SafeLlavaImageEmbedHandle embed, int nBatch, ref int nPast);
    public static bool llava_eval_image_embed(SafeLLamaContextHandle ctxLlama, SafeLlavaImageEmbedHandle embed, int nBatch, ref int nPast) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llava_eval_image_embed_r(ctxLlama, embed, nBatch, ref nPast) : _llavaNativeLibraryHolder.LoadFunction<llava_eval_image_embed_t>("llava_eval_image_embed")(ctxLlama, embed, nBatch, ref nPast);
        
#endregion

#region  NativeApi.Quantize.cs

[DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
public static extern uint llama_model_quantize_r(string fname_inp, string fname_out, ref LLamaModelQuantizeParams param);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
private delegate uint llama_model_quantize_t(string fname_inp, string fname_out, ref LLamaModelQuantizeParams param);
public static uint llama_model_quantize(string fname_inp, string fname_out, ref LLamaModelQuantizeParams param) => NativeLibraryConfig.DynamicLoadingDisabled ? 
    llama_model_quantize_r(fname_inp, fname_out, ref param) : _llamaNativeLibraryHolder.LoadFunction<llama_model_quantize_t>("llama_model_quantize")(fname_inp, fname_out, ref param);

#endregion

#region  NativeApi.Sampling.cs

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void llama_sample_repetition_penalties_r(SafeLLamaContextHandle ctx,
                                                                ref LLamaTokenDataArrayNative candidates,
                                                                LLamaToken* last_tokens, ulong last_tokens_size,
                                                                float penalty_repeat,
                                                                float penalty_freq,
                                                                float penalty_present);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_repetition_penalties_t(SafeLLamaContextHandle ctx,
                                                                ref LLamaTokenDataArrayNative candidates,
                                                                LLamaToken* last_tokens, ulong last_tokens_size,
                                                                float penalty_repeat,
                                                                float penalty_freq,
                                                                float penalty_present);
    public static unsafe void llama_sample_repetition_penalties(SafeLLamaContextHandle ctx,
                                                                ref LLamaTokenDataArrayNative candidates,
                                                                LLamaToken* last_tokens, ulong last_tokens_size,
                                                                float penalty_repeat,
                                                                float penalty_freq,
                                                                float penalty_present)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_repetition_penalties_r(ctx, ref candidates, last_tokens, last_tokens_size, penalty_repeat, penalty_freq, penalty_present);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_repetition_penalties_t>("llama_sample_repetition_penalties")(ctx, ref candidates, last_tokens, last_tokens_size, penalty_repeat, penalty_freq, penalty_present);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void llama_sample_apply_guidance_r(SafeLLamaContextHandle ctx, float* logits, float* logits_guidance, float scale);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_apply_guidance_t(SafeLLamaContextHandle ctx, float* logits, float* logits_guidance, float scale);
    public static void llama_sample_apply_guidance(SafeLLamaContextHandle ctx, float* logits, float* logits_guidance, float scale)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_apply_guidance_r(ctx, logits, logits_guidance, scale);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_apply_guidance_t>("llama_sample_apply_guidance")(ctx, logits, logits_guidance, scale);
        }
    }
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sample_softmax_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_softmax_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);
    public static void llama_sample_softmax(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_softmax_r(ctx, ref candidates);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_softmax_t>("llama_sample_softmax")(ctx, ref candidates);
        }
    }
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sample_top_k_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, int k, ulong min_keep);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_top_k_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, int k, ulong min_keep);
    public static void llama_sample_top_k(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, int k, ulong min_keep)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_top_k_r(ctx, ref candidates, k, min_keep);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_top_k_t>("llama_sample_top_k")(ctx, ref candidates, k, min_keep);
        }
    }
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sample_top_p_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_top_p_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep);
    public static void llama_sample_top_p(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_top_p_r(ctx, ref candidates, p, min_keep);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_top_p_t>("llama_sample_top_p")(ctx, ref candidates, p, min_keep);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sample_min_p_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_min_p_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep);
    public static void llama_sample_min_p(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_min_p_r(ctx, ref candidates, p, min_keep);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_min_p_t>("llama_sample_min_p")(ctx, ref candidates, p, min_keep);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sample_tail_free_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float z, ulong min_keep);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_tail_free_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float z, ulong min_keep);
    public static void llama_sample_tail_free(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float z, ulong min_keep)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_tail_free_r(ctx, ref candidates, z, min_keep);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_tail_free_t>("llama_sample_tail_free")(ctx, ref candidates, z, min_keep);
        }
    }
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sample_typical_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_typical_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep);
    public static void llama_sample_typical(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float p, ulong min_keep)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_typical_r(ctx, ref candidates, p, min_keep);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_typical_t>("llama_sample_typical")(ctx, ref candidates, p, min_keep);
        }
    }
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sample_typical_v2_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float min_temp, float max_temp, float exponent_val);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_typical_v2_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float min_temp, float max_temp, float exponent_val);
    public static void llama_sample_typical(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float min_temp, float max_temp, float exponent_val)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_typical_v2_r(ctx, ref candidates, min_temp, max_temp, exponent_val);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_typical_v2_t>("llama_sample_typical")(ctx, ref candidates, min_temp, max_temp, exponent_val);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sample_temp_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float temp);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void llama_sample_temp_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float temp);
    public static void llama_sample_temp(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float temp)
    {
        if (NativeLibraryConfig.DynamicLoadingDisabled)
        {
            llama_sample_temp_r(ctx, ref candidates, temp);
        }
        else
        {
            _llamaNativeLibraryHolder.LoadFunction<llama_sample_temp_t>("llama_sample_temp")(ctx, ref candidates, temp);
        }
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern LLamaToken llama_sample_token_mirostat_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float tau, float eta, int m, ref float mu);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate LLamaToken llama_sample_token_mirostat_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float tau, float eta, int m, ref float mu);
    public static LLamaToken llama_sample_token_mirostat(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float tau, float eta, int m, ref float mu) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_sample_token_mirostat_r(ctx, ref candidates, tau, eta, m, ref mu) : _llamaNativeLibraryHolder.LoadFunction<llama_sample_token_mirostat_t>("llama_sample_token_mirostat")(ctx, ref candidates, tau, eta, m, ref mu);
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern LLamaToken llama_sample_token_mirostat_v2_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float tau, float eta, ref float mu);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate LLamaToken llama_sample_token_mirostat_v2_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float tau, float eta, ref float mu);
    public static LLamaToken llama_sample_token_mirostat_v2(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates, float tau, float eta, ref float mu) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_sample_token_mirostat_v2_r(ctx, ref candidates, tau, eta, ref mu) : _llamaNativeLibraryHolder.LoadFunction<llama_sample_token_mirostat_v2_t>("llama_sample_token_mirostat_v2")(ctx, ref candidates, tau, eta, ref mu);
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern LLamaToken llama_sample_token_greedy_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate LLamaToken llama_sample_token_greedy_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);
    public static LLamaToken llama_sample_token_greedy(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_sample_token_greedy_r(ctx, ref candidates) : _llamaNativeLibraryHolder.LoadFunction<llama_sample_token_greedy_t>("llama_sample_token_greedy")(ctx, ref candidates);
    
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern LLamaToken llama_sample_token_r(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate LLamaToken llama_sample_token_t(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates);
    public static LLamaToken llama_sample_token(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative candidates) => NativeLibraryConfig.DynamicLoadingDisabled ? 
        llama_sample_token_r(ctx, ref candidates) : _llamaNativeLibraryHolder.LoadFunction<llama_sample_token_t>("llama_sample_token")(ctx, ref candidates);

#endregion

    public static unsafe int llama_chat_apply_template(SafeLlamaModelHandle? model, byte* tmpl, LLamaChatMessage* chat, nuint n_msg, bool add_ass, byte* buf, int length)
    {
        return internal_llama_chat_apply_template(model?.DangerousGetHandle() ?? IntPtr.Zero, tmpl, chat, n_msg, add_ass, buf, length);
    }

    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_chat_apply_template")]
    static unsafe extern int internal_llama_chat_apply_template_r(IntPtr model, byte* tmpl, LLamaChatMessage* chat, nuint n_msg, bool add_ass, byte* buf, int length);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate int internal_llama_chat_apply_template_t(IntPtr model, byte* tmpl, LLamaChatMessage* chat, nuint n_msg, bool add_ass, byte* buf, int length);
    unsafe static int internal_llama_chat_apply_template(IntPtr model, byte* tmpl, LLamaChatMessage* chat, nuint n_msg, bool add_ass, byte* buf, int length) =>
        NativeLibraryConfig.DynamicLoadingDisabled ? internal_llama_chat_apply_template_r(model, tmpl, chat, n_msg, add_ass, buf, length)
        : _llamaNativeLibraryHolder.LoadFunction<internal_llama_chat_apply_template_t>("llama_chat_apply_template")(model, tmpl, chat, n_msg, add_ass, buf, length);

    public static int llama_token_to_piece(SafeLlamaModelHandle model, LLamaToken llamaToken, Span<byte> buffer, bool special)
    {
        unsafe
        {
            fixed (byte* bufferPtr = buffer)
            {
                return llama_token_to_piece_native(model, llamaToken, bufferPtr, buffer.Length, special);
            }
        }
    }
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_token_to_piece")]
    static extern unsafe int llama_token_to_piece_native_r(SafeLlamaModelHandle model, LLamaToken llamaToken, byte* buffer, int length, bool special);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int llama_token_to_piece_native_t(SafeLlamaModelHandle model, LLamaToken llamaToken, byte* buffer, int length, bool special);
    static int llama_token_to_piece_native(SafeLlamaModelHandle model, LLamaToken llamaToken, byte* buffer, int length, bool special) =>
        NativeLibraryConfig.DynamicLoadingDisabled ? llama_token_to_piece_native_r(model, llamaToken, buffer, length, special)
        : _llamaNativeLibraryHolder.LoadFunction<llama_token_to_piece_native_t>("llama_token_to_piece")(model, llamaToken, buffer, length, special);
#endif
}