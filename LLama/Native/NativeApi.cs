using System;
using System.Runtime.InteropServices;

#pragma warning disable IDE1006 // Naming Styles

namespace LLama.Native
{
    /// <summary>
    /// Direct translation of the llama.cpp API
    /// </summary>
	public static partial class NativeApi
    {
        /// <summary>
        /// A method that does nothing. This is a native method, calling it will force the llama native dependencies to be loaded.
        /// </summary>
        /// <returns></returns>
        public static void llama_empty_call()
        {
            llama_max_devices();
        }

        /// <summary>
        /// Get the maximum number of devices supported by llama.cpp
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long llama_max_devices();

        /// <summary>
        /// Check if memory mapping is supported
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool llama_supports_mmap();

        /// <summary>
        /// Check if memory locking is supported
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool llama_supports_mlock();

        /// <summary>
        /// Check if GPU offload is supported
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool llama_supports_gpu_offload();

        /// <summary>
        /// Initialize the llama + ggml backend. Call once at the start of the program.
        ///
        /// This is private because LLamaSharp automatically calls it, and it's only valid to call it once!
        /// </summary>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_backend_init();

        // Note: this is not implemented because we don't have a definition for `ggml_numa_strategy` in C#. That definition doesn't
        //       exist because it's not in llama.h, it's in ggml.h which we don't currently build a wrapper for. If there's demand
        //       for better NUMA support that will need adding.
        ///// <summary>
        ///// Optional, enable NUMA optimisations
        ///// </summary>
        //[DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern void llama_numa_init(ggml_numa_strategy numa);

        /// <summary>
        /// Load session file
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path_session"></param>
        /// <param name="tokens_out"></param>
        /// <param name="n_token_capacity"></param>
        /// <param name="n_token_count_out"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool llama_state_load_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens_out, ulong n_token_capacity, out ulong n_token_count_out);

        /// <summary>
        /// Save session file
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path_session"></param>
        /// <param name="tokens"></param>
        /// <param name="n_token_count"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool llama_state_save_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens, ulong n_token_count);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe nuint llama_state_seq_save_file(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId seq_id, LLamaToken* tokens, nuint n_token_count);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe nuint llama_state_seq_load_file(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId dest_seq_id, LLamaToken* tokens_out, nuint n_token_capacity, out nuint n_token_count_out);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* llama_token_get_text(SafeLlamaModelHandle model, LLamaToken token);

        /// <summary>
        /// Set whether to use causal attention or not. If set to true, the model will only attend to the past tokens
        /// </summary>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_set_causal_attn(SafeLlamaModelHandle ctx, bool causal_attn);

        /// <summary>
        /// Set abort callback
        /// </summary>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_set_abort_callback(SafeLlamaModelHandle ctx, IntPtr /* ggml_abort_callback */ abort_callback, IntPtr abort_callback_data);

        /// <summary>
        /// Wait until all computations are finished. This is automatically done when using any of the functions to obtain computation results
        /// and is not necessary to call it explicitly in most cases.
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_synchronize(SafeLlamaModelHandle ctx);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float llama_token_get_score(SafeLlamaModelHandle model, LLamaToken token);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaTokenType llama_token_get_type(SafeLlamaModelHandle model, LLamaToken token);

        /// <summary>
        /// Get the n_seq_max for this context
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint llama_n_seq_max(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Get the pooling type for this context
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaPoolingType llama_pooling_type(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Get the embeddings for the a specific sequence.
        /// Equivalent to: llama_get_embeddings(ctx) + ctx->output_ids[i]*n_embd
        /// </summary>
        /// <returns>A pointer to the first float in an embedding, length = ctx.EmbeddingSize</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe float* llama_get_embeddings_seq(SafeLLamaContextHandle ctx, LLamaSeqId id);

        /// <summary>
        /// Get the embeddings for the ith sequence.
        /// Equivalent to: llama_get_embeddings(ctx) + ctx->output_ids[i]*n_embd
        /// </summary>
        /// <returns>A pointer to the first float in an embedding, length = ctx.EmbeddingSize</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe float* llama_get_embeddings_ith(SafeLLamaContextHandle ctx, int i);

        /// <summary>
        /// Get all output token embeddings.
        /// When pooling_type == LLAMA_POOLING_TYPE_NONE or when using a generative model, the embeddings for which
        /// llama_batch.logits[i] != 0 are stored contiguously in the order they have appeared in the batch.
        /// shape: [n_outputs*n_embd]
        /// Otherwise, returns an empty span.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe float* llama_get_embeddings(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Apply chat template. Inspired by hf apply_chat_template() on python.
        /// Both "model" and "custom_template" are optional, but at least one is required. "custom_template" has higher precedence than "model"
        /// NOTE: This function does not use a jinja parser. It only support a pre-defined list of template. See more: https://github.com/ggerganov/llama.cpp/wiki/Templates-supported-by-llama_chat_apply_template
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tmpl">A Jinja template to use for this chat. If this is nullptr, the model’s default chat template will be used instead.</param>
        /// <param name="chat">Pointer to a list of multiple llama_chat_message</param>
        /// <param name="n_msg">Number of llama_chat_message in this chat</param>
        /// <param name="add_ass">Whether to end the prompt with the token(s) that indicate the start of an assistant message.</param>
        /// <param name="buf">A buffer to hold the output formatted prompt. The recommended alloc size is 2 * (total number of characters of all messages)</param>
        /// <param name="length">The size of the allocated buffer</param>
        /// <returns>The total number of bytes of the formatted prompt. If is it larger than the size of buffer, you may need to re-alloc it and then re-apply the template.</returns>
        public static unsafe int llama_chat_apply_template(SafeLlamaModelHandle? model, byte* tmpl, LLamaChatMessage* chat, nuint n_msg, bool add_ass, byte* buf, int length)
        {
            return internal_llama_chat_apply_template(model?.DangerousGetHandle() ?? IntPtr.Zero, tmpl, chat, n_msg, add_ass, buf, length);

            [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_chat_apply_template")]
            static extern int internal_llama_chat_apply_template(IntPtr model, byte* tmpl, LLamaChatMessage* chat, nuint n_msg, bool add_ass, byte* buf, int length);
        }

        /// <summary>
        /// Returns -1 if unknown, 1 for true or 0 for false.
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_add_bos_token(SafeLlamaModelHandle model);

        /// <summary>
        /// Returns -1 if unknown, 1 for true or 0 for false.
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_add_eos_token(SafeLlamaModelHandle model);

        /// <summary>
        /// Print out timing information for this context
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_print_timings(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Reset all collected timing information for this context
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_reset_timings(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Print system information
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr llama_print_system_info();

        /// <summary>
        /// Convert a single token into text
        /// </summary>
        /// <param name="model"></param>
        /// <param name="llamaToken"></param>
        /// <param name="buffer">buffer to write string into</param>
        /// <param name="special">If true, special tokens are rendered in the output</param>
        /// <returns>The length written, or if the buffer is too small a negative that indicates the length required</returns>
        public static int llama_token_to_piece(SafeLlamaModelHandle model, LLamaToken llamaToken, Span<byte> buffer, bool special)
        {
            unsafe
            {
                fixed (byte* bufferPtr = buffer)
                {
                    return llama_token_to_piece_native(model, llamaToken, bufferPtr, buffer.Length, special);
                }
            }

            [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_token_to_piece")]
            static extern unsafe int llama_token_to_piece_native(SafeLlamaModelHandle model, LLamaToken llamaToken, byte* buffer, int length, bool special);
        }

        /// <summary>
        /// Convert text into tokens
        /// </summary>
        /// <param name="model"></param>
        /// <param name="text"></param>
        /// <param name="text_len"></param>
        /// <param name="tokens"></param>
        /// <param name="n_max_tokens"></param>
        /// <param name="add_special"></param>
        /// <param name="parse_special">Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext. Does not insert a leading space.</param>
        /// <returns>Returns the number of tokens on success, no more than n_max_tokens.
        /// Returns a negative number on failure - the number of tokens that would have been returned
        /// </returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int llama_tokenize(SafeLlamaModelHandle model, byte* text, int text_len, LLamaToken* tokens, int n_max_tokens, bool add_special, bool parse_special);

        /// <summary>
        /// Register a callback to receive llama log messages
        /// </summary>
        /// <param name="logCallback"></param>
        [Obsolete("Use `NativeLogConfig.llama_log_set` instead")]
        public static void llama_log_set(NativeLogConfig.LLamaLogCallback logCallback)
        {
            NativeLogConfig.llama_log_set(logCallback);
        }
        
        /// <summary>
        /// Returns the number of tokens in the KV cache (slow, use only for debug)
        /// If a KV cell has multiple sequences assigned to it, it will be counted multiple times
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_get_kv_cache_token_count(SafeLLamaContextHandle ctx);
        
        /// <summary>
        /// Returns the number of used KV cells (i.e. have at least one sequence assigned to them)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_get_kv_cache_used_cells(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Clear the KV cache. Both cell info is erased and KV data is zeroed
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_kv_cache_clear(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Removes all tokens that belong to the specified sequence and have positions in [p0, p1)
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="seq"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns>Returns false if a partial sequence cannot be removed. Removing a whole sequence never fails</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool llama_kv_cache_seq_rm(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1);

        /// <summary>
        /// Copy all tokens that belong to the specified sequence to another sequence
        /// Note that this does not allocate extra KV cache memory - it simply assigns the tokens to the new sequence
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_kv_cache_seq_cp(SafeLLamaContextHandle ctx, LLamaSeqId src, LLamaSeqId dest, LLamaPos p0, LLamaPos p1);

        /// <summary>
        /// Removes all tokens that do not belong to the specified sequence
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="seq"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_kv_cache_seq_keep(SafeLLamaContextHandle ctx, LLamaSeqId seq);

        /// <summary>
        /// Adds relative position "delta" to all tokens that belong to the specified sequence and have positions in [p0, p1)
        /// If the KV cache is RoPEd, the KV data is updated accordingly:
        ///  - lazily on next llama_decode()
        ///  - explicitly with llama_kv_cache_update()
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="seq"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="delta"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_kv_cache_seq_add(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta);

        /// <summary>
        /// Integer division of the positions by factor of `d > 1`
        /// If the KV cache is RoPEd, the KV data is updated accordingly:
        ///   - lazily on next llama_decode()
        ///   - explicitly with llama_kv_cache_update()
        /// <br />
        /// p0 &lt; 0 : [0,  p1]
        /// <br />
        /// p1 &lt; 0 : [p0, inf)
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="seq"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="d"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_kv_cache_seq_div(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int d);

        /// <summary>
        /// Returns the largest position present in the KV cache for the specified sequence
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="seq"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaPos llama_kv_cache_seq_pos_max(SafeLLamaContextHandle ctx, LLamaSeqId seq);

        /// <summary>
        /// Allocates a batch of tokens on the heap
        /// Each token can be assigned up to n_seq_max sequence ids
        /// The batch has to be freed with llama_batch_free()
        /// If embd != 0, llama_batch.embd will be allocated with size of n_tokens * embd * sizeof(float)
        /// Otherwise, llama_batch.token will be allocated to store n_tokens llama_token
        /// The rest of the llama_batch members are allocated with size n_tokens
        /// All members are left uninitialized
        /// </summary>
        /// <param name="n_tokens"></param>
        /// <param name="embd"></param>
        /// <param name="n_seq_max">Each token can be assigned up to n_seq_max sequence ids</param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaNativeBatch llama_batch_init(int n_tokens, int embd, int n_seq_max);

        /// <summary>
        /// Frees a batch of tokens allocated with llama_batch_init()
        /// </summary>
        /// <param name="batch"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_batch_free(LLamaNativeBatch batch);

        /// <summary>
        /// Apply a loaded control vector to a llama_context, or if data is NULL, clear
        /// the currently loaded vector.
        /// n_embd should be the size of a single layer's control, and data should point
        /// to an n_embd x n_layers buffer starting from layer 1.
        /// il_start and il_end are the layer range the vector should apply to (both inclusive)
        /// See llama_control_vector_load in common to load a control vector.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <param name="n_embd"></param>
        /// <param name="il_start"></param>
        /// <param name="il_end"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int llama_control_vector_apply(SafeLLamaContextHandle ctx, float* data, nuint len, int n_embd, int il_start, int il_end);

        /// <summary>
        /// Build a split GGUF final path for this chunk.
        /// llama_split_path(split_path, sizeof(split_path), "/models/ggml-model-q4_0", 2, 4) => split_path = "/models/ggml-model-q4_0-00002-of-00004.gguf"
        /// </summary>
        /// <param name="split_path"></param>
        /// <param name="maxlen"></param>
        /// <param name="path_prefix"></param>
        /// <param name="split_no"></param>
        /// <param name="split_count"></param>
        /// <returns>Returns the split_path length.</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_split_path(string split_path, nuint maxlen, string path_prefix, int split_no, int split_count);

        /// <summary>
        /// Extract the path prefix from the split_path if and only if the split_no and split_count match.
        /// llama_split_prefix(split_prefix, 64, "/models/ggml-model-q4_0-00002-of-00004.gguf", 2, 4) => split_prefix = "/models/ggml-model-q4_0"
        /// </summary>
        /// <param name="split_prefix"></param>
        /// <param name="maxlen"></param>
        /// <param name="split_path"></param>
        /// <param name="split_no"></param>
        /// <param name="split_count"></param>
        /// <returns>Returns the split_prefix length.</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_split_prefix(string split_prefix, nuint maxlen, string split_path, int split_no, int split_count);
    }
}
