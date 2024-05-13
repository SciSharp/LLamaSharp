using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    public unsafe partial class SafeLLamaContextHandle
    {
#if NETSTANDARD
        private unsafe delegate bool GgmlAbortCallback(void* data);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern SafeLLamaContextHandle llama_new_context_with_model_r(SafeLlamaModelHandle model, LLamaContextParams @params);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate SafeLLamaContextHandle llama_new_context_with_model_t(SafeLlamaModelHandle model, LLamaContextParams @params);
        private static SafeLLamaContextHandle llama_new_context_with_model(SafeLlamaModelHandle model, LLamaContextParams @params) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_new_context_with_model_r(model, @params) : NativeApi.GetLLamaExport<llama_new_context_with_model_t>("llama_new_context_with_model")(model, @params);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_free_r(IntPtr ctx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void llama_free_t(IntPtr ctx);
        private static void llama_free(IntPtr ctx)
        {
            if (NativeLibraryConfig.DynamicLoadingDisabled)
            {
                llama_free_r(ctx);
            }
            else
            {
                NativeApi.GetLLamaExport<llama_free_t>("llama_free")(ctx);
            }
        }

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_set_abort_callback_r(SafeLLamaContextHandle ctx, GgmlAbortCallback abort_callback, void* abort_callback_data);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void llama_set_abort_callback_t(SafeLLamaContextHandle ctx, GgmlAbortCallback abort_callback, void* abort_callback_data);
        private static void llama_set_abort_callback(SafeLLamaContextHandle ctx, GgmlAbortCallback abort_callback, void* abort_callback_data)
        {
            if (NativeLibraryConfig.DynamicLoadingDisabled)
            {
                llama_set_abort_callback_r(ctx, abort_callback, abort_callback_data);
            }
            else
            {
                NativeApi.GetLLamaExport<llama_set_abort_callback_t>("llama_set_abort_callback")(ctx, abort_callback, abort_callback_data);
            }
        }

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_decode_r(SafeLLamaContextHandle ctx, LLamaNativeBatch batch);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_decode_t(SafeLLamaContextHandle ctx, LLamaNativeBatch batch);
        private static int llama_decode(SafeLLamaContextHandle ctx, LLamaNativeBatch batch) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_decode_r(ctx, batch) : NativeApi.GetLLamaExport<llama_decode_t>("llama_decode")(ctx, batch);
        
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_set_n_threads_r(SafeLLamaContextHandle ctx, uint n_threads, uint n_threads_batch);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void llama_set_n_threads_t(SafeLLamaContextHandle ctx, uint n_threads, uint n_threads_batch);
        private static void llama_set_n_threads(SafeLLamaContextHandle ctx, uint n_threads, uint n_threads_batch)
        {
            if (NativeLibraryConfig.DynamicLoadingDisabled)
            {
                llama_set_n_threads_r(ctx, n_threads, n_threads_batch);
            }
            else
            {
                NativeApi.GetLLamaExport<llama_set_n_threads_t>("llama_set_n_threads")(ctx, n_threads, n_threads_batch);
            }
        }

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern float* llama_get_logits_r(SafeLLamaContextHandle ctx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float* llama_get_logits_t(SafeLLamaContextHandle ctx);
        private static float* llama_get_logits(SafeLLamaContextHandle ctx) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_get_logits_r(ctx) : NativeApi.GetLLamaExport<llama_get_logits_t>("llama_get_logits")(ctx);
        
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern float* llama_get_logits_ith_r(SafeLLamaContextHandle ctx, int i);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float* llama_get_logits_ith_t(SafeLLamaContextHandle ctx, int i);
        private static float* llama_get_logits_ith(SafeLLamaContextHandle ctx, int i) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_get_logits_ith_r(ctx, i) : NativeApi.GetLLamaExport<llama_get_logits_ith_t>("llama_get_logits_ith")(ctx, i);
        
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint llama_n_ctx_r(SafeLLamaContextHandle ctx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint llama_n_ctx_t(SafeLLamaContextHandle ctx);
        private static uint llama_n_ctx(SafeLLamaContextHandle ctx) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_n_ctx_r(ctx) : NativeApi.GetLLamaExport<llama_n_ctx_t>("llama_n_ctx")(ctx);


        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint llama_n_batch_r(SafeLLamaContextHandle ctx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint llama_n_batch_t(SafeLLamaContextHandle ctx);
        private static uint llama_n_batch(SafeLLamaContextHandle ctx) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_n_batch_r(ctx) : NativeApi.GetLLamaExport<llama_n_batch_t>("llama_n_batch")(ctx);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint llama_n_ubatch_r(SafeLLamaContextHandle ctx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint llama_n_ubatch_t(SafeLLamaContextHandle ctx);
        private static uint llama_n_ubatch(SafeLLamaContextHandle ctx) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_n_ubatch_r(ctx) : NativeApi.GetLLamaExport<llama_n_ubatch_t>("llama_n_ubatch")(ctx);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_set_rng_seed_r(SafeLLamaContextHandle ctx, uint seed);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void llama_set_rng_seed_t(SafeLLamaContextHandle ctx, uint seed);
        private static void llama_set_rng_seed(SafeLLamaContextHandle ctx, uint seed)
        {
            if (NativeLibraryConfig.DynamicLoadingDisabled)
            {
                llama_set_rng_seed_r(ctx, seed);
            }
            else
            {
                NativeApi.GetLLamaExport<llama_set_rng_seed_t>("llama_set_rng_seed")(ctx, seed);
            }
        }

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong llama_state_get_size_r(SafeLLamaContextHandle ctx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ulong llama_state_get_size_t(SafeLLamaContextHandle ctx);
        private static ulong llama_state_get_size(SafeLLamaContextHandle ctx) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_state_get_size_r(ctx) : NativeApi.GetLLamaExport<llama_state_get_size_t>("llama_state_get_size")(ctx);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong llama_state_get_data_r(SafeLLamaContextHandle ctx, byte* dest);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ulong llama_state_get_data_t(SafeLLamaContextHandle ctx, byte* dest);
        private static ulong llama_state_get_data(SafeLLamaContextHandle ctx, byte* dest) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_state_get_data_r(ctx, dest) : NativeApi.GetLLamaExport<llama_state_get_data_t>("llama_state_get_data")(ctx, dest);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong llama_state_set_data_r(SafeLLamaContextHandle ctx, byte* src);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ulong llama_state_set_data_t(SafeLLamaContextHandle ctx, byte* src);
        private static ulong llama_state_set_data(SafeLLamaContextHandle ctx, byte* src) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_state_set_data_r(ctx, src) : NativeApi.GetLLamaExport<llama_state_set_data_t>("llama_state_set_data")(ctx, src);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern nuint llama_state_seq_get_size_r(SafeLLamaContextHandle ctx, LLamaSeqId seq_id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate nuint llama_state_seq_get_size_t(SafeLLamaContextHandle ctx, LLamaSeqId seq_id);
        private static nuint llama_state_seq_get_size(SafeLLamaContextHandle ctx, LLamaSeqId seq_id) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_state_seq_get_size_r(ctx, seq_id) : NativeApi.GetLLamaExport<llama_state_seq_get_size_t>("llama_state_seq_get_size")(ctx, seq_id);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe nuint llama_state_seq_get_data_r(SafeLLamaContextHandle ctx, byte* dst, LLamaSeqId seq_id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate nuint llama_state_seq_get_data_t(SafeLLamaContextHandle ctx, byte* dst, LLamaSeqId seq_id);
        private static nuint llama_state_seq_get_data(SafeLLamaContextHandle ctx, byte* dst, LLamaSeqId seq_id) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_state_seq_get_data_r(ctx, dst, seq_id) : NativeApi.GetLLamaExport<llama_state_seq_get_data_t>("llama_state_seq_get_data")(ctx, dst, seq_id);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern nuint llama_state_seq_set_data_r(SafeLLamaContextHandle ctx, byte* src, LLamaSeqId dest_seq_id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate nuint llama_state_seq_set_data_t(SafeLLamaContextHandle ctx, byte* src, LLamaSeqId dest_seq_id);
        private static nuint llama_state_seq_set_data(SafeLLamaContextHandle ctx, byte* src, LLamaSeqId dest_seq_id) => NativeLibraryConfig.DynamicLoadingDisabled ? 
            llama_state_seq_set_data_r(ctx, src, dest_seq_id) : NativeApi.GetLLamaExport<llama_state_seq_set_data_t>("llama_state_seq_set_data")(ctx, src, dest_seq_id);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_kv_cache_defrag_r(SafeLLamaContextHandle ctx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void llama_kv_cache_defrag_t(SafeLLamaContextHandle ctx);
        private static void llama_kv_cache_defrag(SafeLLamaContextHandle ctx)
        {
            if (NativeLibraryConfig.DynamicLoadingDisabled)
            {
                llama_kv_cache_defrag_r(ctx);
            }
            else
            {
                NativeApi.GetLLamaExport<llama_kv_cache_defrag_t>("llama_kv_cache_defrag")(ctx);
            }
        }

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_kv_cache_update_r(SafeLLamaContextHandle ctx);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void llama_kv_cache_update_t(SafeLLamaContextHandle ctx);
        public static void llama_kv_cache_update(SafeLLamaContextHandle ctx)
        {
            if (NativeLibraryConfig.DynamicLoadingDisabled)
            {
                llama_kv_cache_update_r(ctx);
            }
            else
            {
                NativeApi.GetLLamaExport<llama_kv_cache_update_t>("llama_kv_cache_update")(ctx);
            }
        }
#endif
    }
}