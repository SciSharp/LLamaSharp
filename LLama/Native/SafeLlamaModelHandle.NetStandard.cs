using System;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    public partial class SafeLlamaModelHandle
    {
#if NETSTANDARD
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern SafeLlamaModelHandle llama_load_model_from_file_r(string path_model, LLamaModelParams @params);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate SafeLlamaModelHandle llama_load_model_from_file_t(string path_model, LLamaModelParams @params);
        private static SafeLlamaModelHandle llama_load_model_from_file(string path_model, LLamaModelParams @params) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_load_model_from_file_r(path_model, @params) : NativeApi.GetLLamaExport<llama_load_model_from_file_t>("llama_load_model_from_file")(path_model, @params);

        /// <summary>
        /// Apply a LoRA adapter to a loaded model
        /// path_base_model is the path to a higher quality model to use as a base for
        /// the layers modified by the adapter. Can be NULL to use the current loaded model.
        /// The model needs to be reloaded before applying a new adapter, otherwise the adapter
        /// will be applied on top of the previous one
        /// </summary>
        /// <param name="model_ptr"></param>
        /// <param name="path_lora"></param>
        /// <param name="scale"></param>
        /// <param name="path_base_model"></param>
        /// <param name="n_threads"></param>
        /// <returns>Returns 0 on success</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_model_apply_lora_from_file_r(SafeLlamaModelHandle model_ptr, string path_lora, float scale, string? path_base_model, int n_threads);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_model_apply_lora_from_file_t(SafeLlamaModelHandle model_ptr, string path_lora, float scale, string? path_base_model, int n_threads);
        private static int llama_model_apply_lora_from_file(SafeLlamaModelHandle model_ptr, string path_lora, float scale, string? path_base_model, int n_threads) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_model_apply_lora_from_file_r(model_ptr, path_lora, scale, path_base_model, n_threads) : NativeApi.GetLLamaExport<llama_model_apply_lora_from_file_t>("llama_model_apply_lora_from_file")(model_ptr, path_lora, scale, path_base_model, n_threads);

        /// <summary>
        /// Frees all allocated memory associated with a model
        /// </summary>
        /// <param name="model"></param>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_free_model_r(IntPtr model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void llama_free_model_t(IntPtr model);
        private static void llama_free_model(IntPtr model)
        {
            if (NativeLibraryConfig.DynamicLoadingDisabled)
            {
                llama_free_model_r(model);
            }
            else
            {
                NativeApi.GetLLamaExport<llama_free_model_t>("llama_free_model")(model);
            }
        }

        /// <summary>
        /// Get the number of metadata key/value pairs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_model_meta_count_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_model_meta_count_t(SafeLlamaModelHandle model);
        private static int llama_model_meta_count(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_model_meta_count_r(model) : NativeApi.GetLLamaExport<llama_model_meta_count_t>("llama_model_meta_count")(model);

        /// <summary>
        /// Get metadata key name by index
        /// </summary>
        /// <param name="model">Model to fetch from</param>
        /// <param name="index">Index of key to fetch</param>
        /// <param name="dest">buffer to write result into</param>
        /// <returns>The length of the string on success (even if the buffer is too small). -1 is the key does not exist.</returns>
        private static int llama_model_meta_key_by_index(SafeLlamaModelHandle model, int index, Span<byte> dest)
        {
            unsafe
            {
                fixed (byte* destPtr = dest)
                {
                    return llama_model_meta_key_by_index_native(model, index, destPtr, dest.Length);
                }
            }
        }

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_model_meta_key_by_index")]
        static extern unsafe int llama_model_meta_key_by_index_native_r(SafeLlamaModelHandle model, int index, byte* buf, long buf_size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe delegate int llama_model_meta_key_by_index_native_t(SafeLlamaModelHandle model, int index, byte* buf, long buf_size);
        static unsafe int llama_model_meta_key_by_index_native(SafeLlamaModelHandle model, int index, byte* buf, long buf_size) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_model_meta_key_by_index_native_r(model, index, buf, buf_size) : NativeApi.GetLLamaExport<llama_model_meta_key_by_index_native_t>("llama_model_meta_key_by_index")(model, index, buf, buf_size);

        /// <summary>
        /// Get metadata value as a string by index
        /// </summary>
        /// <param name="model">Model to fetch from</param>
        /// <param name="index">Index of val to fetch</param>
        /// <param name="dest">Buffer to write result into</param>
        /// <returns>The length of the string on success (even if the buffer is too small). -1 is the key does not exist.</returns>
        private static int llama_model_meta_val_str_by_index(SafeLlamaModelHandle model, int index, Span<byte> dest)
        {
            unsafe
            {
                fixed (byte* destPtr = dest)
                {
                    return llama_model_meta_val_str_by_index_native(model, index, destPtr, dest.Length);
                }
            }
        }

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_model_meta_val_str_by_index")]
        static extern unsafe int llama_model_meta_val_str_by_index_native_r(SafeLlamaModelHandle model, int index, byte* buf, long buf_size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe delegate int llama_model_meta_val_str_by_index_native_t(SafeLlamaModelHandle model, int index, byte* buf, long buf_size);
        static unsafe int llama_model_meta_val_str_by_index_native(SafeLlamaModelHandle model, int index, byte* buf, long buf_size) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_model_meta_val_str_by_index_native_r(model, index, buf, buf_size) : NativeApi.GetLLamaExport<llama_model_meta_val_str_by_index_native_t>("llama_model_meta_val_str_by_index")(model, index, buf, buf_size);

        /// <summary>
        /// Get metadata value as a string by key name
        /// </summary>
        /// <param name="model"></param>
        /// <param name="key"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <returns>The length of the string on success, or -1 on failure</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int llama_model_meta_val_str_r(SafeLlamaModelHandle model, byte* key, byte* buf, long buf_size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int llama_model_meta_val_str_t(SafeLlamaModelHandle model, byte* key, byte* buf, long buf_size);
        public unsafe static int llama_model_meta_val_str(SafeLlamaModelHandle model, byte* key, byte* buf, long buf_size) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_model_meta_val_str_r(model, key, buf, buf_size) : NativeApi.GetLLamaExport<llama_model_meta_val_str_t>("llama_model_meta_val_str")(model, key, buf, buf_size);

        /// <summary>
        /// Get the number of tokens in the model vocabulary
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_n_vocab_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_n_vocab_t(SafeLlamaModelHandle model);
        private static int llama_n_vocab(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_n_vocab_r(model) : NativeApi.GetLLamaExport<llama_n_vocab_t>("llama_n_vocab")(model);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaVocabType llama_vocab_type_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate LLamaVocabType llama_vocab_type_t(SafeLlamaModelHandle model);
        private static LLamaVocabType llama_vocab_type(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_vocab_type_r(model) : NativeApi.GetLLamaExport<llama_vocab_type_t>("llama_vocab_type")(model);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaRopeType llama_rope_type_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate LLamaRopeType llama_rope_type_t(SafeLlamaModelHandle model);
        private static LLamaRopeType llama_rope_type(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_rope_type_r(model) : NativeApi.GetLLamaExport<llama_rope_type_t>("llama_rope_type")(model);

        /// <summary>
        /// Get the size of the context window for the model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_n_ctx_train_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_n_ctx_train_t(SafeLlamaModelHandle model);
        private static int llama_n_ctx_train(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_n_ctx_train_r(model) : NativeApi.GetLLamaExport<llama_n_ctx_train_t>("llama_n_ctx_train")(model);

        /// <summary>
        /// Get the dimension of embedding vectors from this model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_n_embd_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_n_embd_t(SafeLlamaModelHandle model);
        private static int llama_n_embd(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_n_embd_r(model) : NativeApi.GetLLamaExport<llama_n_embd_t>("llama_n_embd")(model);

        /// <summary>
        /// Get the number of layers in this model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_n_layers_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_n_layers_t(SafeLlamaModelHandle model);
        private static int llama_n_layers(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_n_layers_r(model) : NativeApi.GetLLamaExport<llama_n_layers_t>("llama_n_layers")(model);

        /// <summary>
        /// Get a string describing the model type
        /// </summary>
        /// <param name="model"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <returns>The length of the string on success (even if the buffer is too small)., or -1 on failure</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int llama_model_desc_r(SafeLlamaModelHandle model, byte* buf, long buf_size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int llama_model_desc_t(SafeLlamaModelHandle model, byte* buf, long buf_size);
        private static unsafe int llama_model_desc(SafeLlamaModelHandle model, byte* buf, long buf_size) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_model_desc_r(model, buf, buf_size) : NativeApi.GetLLamaExport<llama_model_desc_t>("llama_model_desc")(model, buf, buf_size);

        /// <summary>
        /// Get the size of the model in bytes
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The size of the model</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong llama_model_size_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ulong llama_model_size_t(SafeLlamaModelHandle model);
        private static ulong llama_model_size(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_model_size_r(model) : NativeApi.GetLLamaExport<llama_model_size_t>("llama_model_size")(model);

        /// <summary>
        /// Get the number of parameters in this model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The functions return the length of the string on success, or -1 on failure</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong llama_model_n_params_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate ulong llama_model_n_params_t(SafeLlamaModelHandle model);
        private static ulong llama_model_n_params(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_model_n_params_r(model) : NativeApi.GetLLamaExport<llama_model_n_params_t>("llama_model_n_params")(model);

        /// <summary>
        /// Get the model's RoPE frequency scaling factor
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern float llama_rope_freq_scale_train_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float llama_rope_freq_scale_train_t(SafeLlamaModelHandle model);
        private static float llama_rope_freq_scale_train(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_rope_freq_scale_train_r(model) : NativeApi.GetLLamaExport<llama_rope_freq_scale_train_t>("llama_rope_freq_scale_train")(model);

        /// <summary>
        /// Get the "Beginning of sentence" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_bos_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate LLamaToken llama_token_bos_t(SafeLlamaModelHandle model);
        private static LLamaToken llama_token_bos(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_token_bos_r(model) : NativeApi.GetLLamaExport<llama_token_bos_t>("llama_token_bos")(model);

        /// <summary>
        /// Get the "End of sentence" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_eos_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate LLamaToken llama_token_eos_t(SafeLlamaModelHandle model);
        private static LLamaToken llama_token_eos(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_token_eos_r(model) : NativeApi.GetLLamaExport<llama_token_eos_t>("llama_token_eos")(model);

        /// <summary>
        /// Get the "classification" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_cls_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate LLamaToken llama_token_cls_t(SafeLlamaModelHandle model);
        private static LLamaToken llama_token_cls(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_token_cls_r(model) : NativeApi.GetLLamaExport<llama_token_cls_t>("llama_token_cls")(model);

        /// <summary>
        /// Get the "sentence separator" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_sep_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate LLamaToken llama_token_sep_t(SafeLlamaModelHandle model);
        private static LLamaToken llama_token_sep(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_token_sep_r(model) : NativeApi.GetLLamaExport<llama_token_sep_t>("llama_token_sep")(model);

        /// <summary>
        /// Get the "new line" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_nl_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate LLamaToken llama_token_nl_t(SafeLlamaModelHandle model);
        private static LLamaToken llama_token_nl(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_token_nl_r(model) : NativeApi.GetLLamaExport<llama_token_nl_t>("llama_token_nl")(model);

        /// <summary>
        /// codellama infill tokens, Beginning of infill prefix
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_token_prefix_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_token_prefix_t(SafeLlamaModelHandle model);
        private static int llama_token_prefix(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_token_prefix_r(model) : NativeApi.GetLLamaExport<llama_token_prefix_t>("llama_token_prefix")(model);

        /// <summary>
        /// codellama infill tokens, Beginning of infill middle
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_token_middle_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_token_middle_t(SafeLlamaModelHandle model);
        private static int llama_token_middle(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_token_middle_r(model) : NativeApi.GetLLamaExport<llama_token_middle_t>("llama_token_middle")(model);

        /// <summary>
        /// codellama infill tokens, Beginning of infill suffix
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_token_suffix_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_token_suffix_t(SafeLlamaModelHandle model);
        private static int llama_token_suffix(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_token_suffix_r(model) : NativeApi.GetLLamaExport<llama_token_suffix_t>("llama_token_suffix")(model);

        /// <summary>
        /// codellama infill tokens, End of infill middle
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_token_eot_r(SafeLlamaModelHandle model);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int llama_token_eot_t(SafeLlamaModelHandle model);
        private static int llama_token_eot(SafeLlamaModelHandle model) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_token_eot_r(model) : NativeApi.GetLLamaExport<llama_token_eot_t>("llama_token_eot")(model);

        /// <summary>
        /// Check if the token is supposed to end generation (end-of-generation, eg. EOS, EOT, etc.)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool llama_token_is_eog_r(SafeLlamaModelHandle model, LLamaToken token);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool llama_token_is_eog_t(SafeLlamaModelHandle model, LLamaToken token);
        private static bool llama_token_is_eog(SafeLlamaModelHandle model, LLamaToken token) => NativeLibraryConfig.DynamicLoadingDisabled ?
            llama_token_is_eog_r(model, token) : NativeApi.GetLLamaExport<llama_token_is_eog_t>("llama_token_is_eog")(model, token);
#endif
    }
}