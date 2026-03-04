using System;
using System.Text;

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

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
        /// <summary>
        /// Call once at the end of the program - currently only used for MPI
        /// </summary>
        public static extern void llama_backend_free();
#pragma warning restore CS0626 // Method, operator, or accessor is marked external and has no attributes on it

        /// <summary>
        /// Get the maximum number of devices supported by llama.cpp
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long llama_max_devices();

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint llama_max_tensor_buft_overrides();

        /// <summary>
        /// Maximum number of parallel sequences
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long llama_max_parallel_sequences();

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
        /// Check if RPC offload is supported
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool llama_supports_rpc();

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint llama_n_ctx_seq(SafeLLamaContextHandle ctx);

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

        /// <summary>
        /// Saves the specified sequence as a file on specified filepath. Can later be loaded via <see cref="llama_state_load_file(SafeLLamaContextHandle, string, LLamaToken[], ulong, out ulong)"/>
        /// </summary>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe nuint llama_state_seq_save_file(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId seq_id, LLamaToken* tokens, nuint n_token_count);

        /// <summary>
        /// Loads a sequence saved as a file via <see cref="llama_state_save_file(SafeLLamaContextHandle, string, LLamaToken[], ulong)"/> into the specified sequence
        /// </summary>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe nuint llama_state_seq_load_file(SafeLLamaContextHandle ctx, string filepath, LLamaSeqId dest_seq_id, LLamaToken* tokens_out, nuint n_token_capacity, out nuint n_token_count_out);

        /// <summary>
        /// Set whether to use causal attention or not. If set to true, the model will only attend to the past tokens
        /// </summary>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_set_causal_attn(SafeLLamaContextHandle ctx, [MarshalAs(UnmanagedType.U1)] bool causalAttn);

        /// <summary>
        /// Set whether the context outputs embeddings or not
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="embeddings">If true, embeddings will be returned but logits will not</param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_set_embeddings(SafeLLamaContextHandle ctx, [MarshalAs(UnmanagedType.U1)] bool embeddings);

        /// <summary>
        /// Set abort callback
        /// </summary>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_set_abort_callback(SafeLLamaContextHandle ctx, IntPtr /* ggml_abort_callback */ abortCallback, IntPtr abortCallbackData);

        /// <summary>
        /// Get the n_seq_max for this context
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint llama_n_seq_max(SafeLLamaContextHandle ctx);

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
        /// <br />
        /// NOTE: This function does not use a jinja parser. It only support a pre-defined list of template.
        /// See more: https://github.com/ggml-org/llama.cpp/wiki/Templates-supported-by-llama_chat_apply_template
        /// </summary>
        /// <param name="tmpl">A Jinja template to use for this chat.</param>
        /// <param name="chat">Pointer to a list of multiple llama_chat_message</param>
        /// <param name="n_msg">Number of llama_chat_message in this chat</param>
        /// <param name="add_ass">Whether to end the prompt with the token(s) that indicate the start of an assistant message.</param>
        /// <param name="buf">A buffer to hold the output formatted prompt. The recommended alloc size is 2 * (total number of characters of all messages)</param>
        /// <param name="length">The size of the allocated buffer</param>
        /// <returns>The total number of bytes of the formatted prompt. If is it larger than the size of buffer, you may need to re-alloc it and then re-apply the template.</returns>
        public static unsafe int llama_chat_apply_template(byte* tmpl, LLamaChatMessage* chat, nuint n_msg, [MarshalAs(UnmanagedType.U1)] bool add_ass, byte* buf, int length)
        {
            return internal_llama_chat_apply_template(tmpl, chat, n_msg, add_ass, buf, length);

            [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_chat_apply_template")]
            static extern int internal_llama_chat_apply_template(byte* tmpl, LLamaChatMessage* chat, nuint n_msg, [MarshalAs(UnmanagedType.U1)] bool add_ass, byte* buf, int length);
        }

        /// <summary>
        /// Get list of built-in chat templates
        /// </summary>
        /// <param name="output"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int llama_chat_builtin_templates(char** output, nuint len);

        /// <summary>
        /// Print out timing information for this context
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_print_timings(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Print system information
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr llama_print_system_info();

        /// <summary>
        /// Convert a single token into text
        /// </summary>
        /// <param name="vocab"></param>
        /// <param name="llamaToken"></param>
        /// <param name="buffer">buffer to write string into</param>
        /// <param name="lstrip">User can skip up to 'lstrip' leading spaces before copying (useful when encoding/decoding multiple tokens with 'add_space_prefix')</param>
        /// <param name="special">If true, special tokens are rendered in the output</param>
        /// <returns>The length written, or if the buffer is too small a negative that indicates the length required</returns>
        public static int llama_token_to_piece(SafeLlamaModelHandle.Vocabulary vocab, LLamaToken llamaToken, Span<byte> buffer, int lstrip, bool special)
        {
            // Handle invalid tokens
            if ((int)llamaToken < 0)
                return 0;

            unsafe
            {
                fixed (byte* bufferPtr = buffer)
                {
                    return llama_token_to_piece_native(vocab.VocabNative, llamaToken, bufferPtr, buffer.Length, lstrip, special);
                }
            }

            [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_token_to_piece")]
            static extern unsafe int llama_token_to_piece_native(LLamaVocabNative* model, LLamaToken llamaToken, byte* buffer, int length, int lstrip, [MarshalAs(UnmanagedType.U1)] bool special);
        }

        /// <summary>
        /// Convert text into tokens
        /// </summary>
        /// <param name="model"></param>
        /// <param name="text"></param>
        /// <param name="text_len"></param>
        /// <param name="tokens">The tokens pointer must be large enough to hold the resulting tokens.</param>
        /// <param name="n_max_tokens"></param>
        /// <param name="add_special">add_special Allow to add BOS and EOS tokens if model is configured to do so.</param>
        /// <param name="parse_special">Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext. Does not insert a leading space.</param>
        /// <returns>Returns the number of tokens on success, no more than n_max_tokens.
        /// Returns a negative number on failure - the number of tokens that would have been returned. Returns INT32_MIN on overflow (e.g., tokenization result size exceeds int32_t limit)
        /// </returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe int llama_tokenize(LLamaVocabNative* model, byte* text, int text_len, LLamaToken* tokens, int n_max_tokens, [MarshalAs(UnmanagedType.U1)] bool add_special, [MarshalAs(UnmanagedType.U1)] bool parse_special);

        /// <summary>
        /// Convert the provided tokens into text (inverse of llama_tokenize()).
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tokens"></param>
        /// <param name="nTokens"></param>
        /// <param name="textOut">The char pointer must be large enough to hold the resulting text.</param>
        /// <param name="textLengthMax"></param>
        /// <param name="removeSpecial">remove_special Allow to remove BOS and EOS tokens if model is configured to do so.</param>
        /// <param name="unparseSpecial">unparse_special If true, special tokens are rendered in the output.</param>
        /// <returns>Returns the number of chars/bytes on success, no more than textLengthMax. Returns a negative number on failure - the number of chars/bytes that would have been returned.</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe int llama_detokenize(LLamaVocabNative* model, LLamaToken* tokens, int nTokens, byte* textOut, int textLengthMax, bool removeSpecial, bool unparseSpecial);

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
        public static extern unsafe int llama_set_adapter_cvec(SafeLLamaContextHandle ctx, float* data, nuint len, int n_embd, int il_start, int il_end);

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
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_split_path")]
        private static extern unsafe int llama_split_path_native(byte* split_path, nuint maxlen, byte* path_prefix, int split_no, int split_count);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_split_prefix")]
        private static extern unsafe int llama_split_prefix_native(byte* split_prefix, nuint maxlen, byte* split_path, int split_no, int split_count);

        private static byte[] EncodeNullTerminatedUtf8(string value, string paramName)
        {
            if (value is null)
                throw new ArgumentNullException(paramName);

            var bytes = Encoding.UTF8.GetBytes(value);
            var buffer = new byte[bytes.Length + 1];
            Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
            return buffer;
        }

        /// <summary>
        /// Build the fully-qualified path for a specific split file in a GGUF shard set.
        /// </summary>
        /// <param name="splitPathBuffer">Writable buffer that receives the UTF-8 encoded path.</param>
        /// <param name="pathPrefix">Base path (e.g. "/models/ggml-model-q4_0").</param>
        /// <param name="splitNo">Zero-based split index.</param>
        /// <param name="splitCount">Total number of splits.</param>
        /// <returns>Number of bytes written to <paramref name="splitPathBuffer"/>.</returns>
        public static int llama_split_path(Span<byte> splitPathBuffer, string pathPrefix, int splitNo, int splitCount)
        {
            if (splitPathBuffer.Length == 0)
                throw new ArgumentException("Buffer must not be empty.", nameof(splitPathBuffer));

            var pathPrefixBytes = EncodeNullTerminatedUtf8(pathPrefix, nameof(pathPrefix));

            unsafe
            {
                fixed (byte* splitPtr = splitPathBuffer)
                fixed (byte* prefixPtr = pathPrefixBytes)
                {
                    return llama_split_path_native(splitPtr, (nuint)splitPathBuffer.Length, prefixPtr, splitNo, splitCount);
                }
            }
        }

        /// <summary>
        /// Build the fully-qualified path for a specific split file in a GGUF shard set.
        /// </summary>
        /// <param name="pathPrefix">Base path (e.g. "/models/ggml-model-q4_0").</param>
        /// <param name="splitNo">Zero-based split index.</param>
        /// <param name="splitCount">Total number of splits.</param>
        /// <param name="maxLength">Maximum number of bytes to allocate for the resulting UTF-8 string.</param>
        /// <returns>UTF-8 decoded split path.</returns>
        public static string llama_split_path(string pathPrefix, int splitNo, int splitCount, int maxLength = 1024)
        {
            if (maxLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength));

            var buffer = new byte[maxLength];
            var written = llama_split_path((Span<byte>)buffer, pathPrefix, splitNo, splitCount);
            if (written <= 0)
                throw new InvalidOperationException("Failed to build split path using llama_split_path.");

            return Encoding.UTF8.GetString(buffer, 0, written);
        }

        /// <summary>
        /// Extract the shard prefix from a GGUF split path when the split metadata matches.
        /// </summary>
        /// <param name="splitPrefixBuffer">Writable buffer that receives the UTF-8 encoded prefix.</param>
        /// <param name="splitPath">Full path to a shard file.</param>
        /// <param name="splitNo">Zero-based split index.</param>
        /// <param name="splitCount">Total number of splits.</param>
        /// <returns>Number of bytes written to <paramref name="splitPrefixBuffer"/>.</returns>
        public static int llama_split_prefix(Span<byte> splitPrefixBuffer, string splitPath, int splitNo, int splitCount)
        {
            if (splitPrefixBuffer.Length == 0)
                throw new ArgumentException("Buffer must not be empty.", nameof(splitPrefixBuffer));

            var splitPathBytes = EncodeNullTerminatedUtf8(splitPath, nameof(splitPath));

            unsafe
            {
                fixed (byte* prefixPtr = splitPrefixBuffer)
                fixed (byte* pathPtr = splitPathBytes)
                {
                    return llama_split_prefix_native(prefixPtr, (nuint)splitPrefixBuffer.Length, pathPtr, splitNo, splitCount);
                }
            }
        }

        /// <summary>
        /// Extract the shard prefix from a GGUF split path when the split metadata matches.
        /// </summary>
        /// <param name="splitPath">Full path to a shard file.</param>
        /// <param name="splitNo">Zero-based split index.</param>
        /// <param name="splitCount">Total number of splits.</param>
        /// <param name="maxLength">Maximum number of bytes to allocate for the resulting UTF-8 string.</param>
        /// <returns>UTF-8 decoded split prefix.</returns>
        public static string llama_split_prefix(string splitPath, int splitNo, int splitCount, int maxLength = 1024)
        {
            if (maxLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength));

            var buffer = new byte[maxLength];
            var written = llama_split_prefix((Span<byte>)buffer, splitPath, splitNo, splitCount);
            if (written <= 0)
                throw new InvalidOperationException("Failed to extract split prefix using llama_split_prefix.");

            return Encoding.UTF8.GetString(buffer, 0, written);
        }

        //[DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        //todo: public static void llama_attach_threadpool(SafeLLamaContextHandle ctx, ggml_threadpool_t threadpool, ggml_threadpool_t threadpool_batch);

        //[DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        //todo: public static void llama_detach_threadpool(SafeLLamaContextHandle ctx);

        // SafeLLamaContextHandle already holds a back reference to the model, so this is never needed. Implementing it would be dangerous because
        // it would expose the raw pointer to the model, without properly wrapping it in a SafeLLamaModelHandle.
        //[DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        //public static void llama_model* llama_get_model(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Get the number of available backend devices
        /// </summary>
        /// <returns>Count of available backend devices</returns>
        [DllImport(ggmlLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern nuint ggml_backend_dev_count();

        /// <summary>
        /// Get a backend device by index
        /// </summary>
        /// <param name="i">Device index</param>
        /// <returns>Pointer to the backend device</returns>
        [DllImport(ggmlLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ggml_backend_dev_get(nuint i);

        /// <summary>
        /// Get the buffer type for a backend device
        /// </summary>
        /// <param name="dev">Backend device pointer</param>
        /// <returns>Pointer to the buffer type</returns>
        [DllImport(ggmlBaseLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ggml_backend_dev_buffer_type(IntPtr dev);

        /// <summary>
        /// Get the name of a buffer type
        /// </summary>
        /// <param name="buft">Buffer type pointer</param>
        /// <returns>Name of the buffer type</returns>
        [DllImport(ggmlBaseLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ggml_backend_buft_name(IntPtr buft);

        /// <summary>
        /// Fits mparams and cparams to free device memory (assumes system memory is unlimited)
        ///   - returns true if the parameters could be successfully modified to fit device memory
        ///   - this function is NOT thread safe because it modifies the global llama logger state
        ///   - only parameters that have the same value as in llama_default_model_params are modified
        ///     with the exception of the context size which is modified if and only if equal to 0
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mparams"></param>
        /// <param name="cparams"></param>
        /// <param name="tensor_split">Writable buffer for tensor split, needs at least llama_max_devices elements</param>
        /// <param name="tensor_buft_overrides">Writable buffer for overrides, needs at least llama_max_tensor_buft_overrides elements</param>
        /// <param name="margins">Margins of memory to leave per device in bytes</param>
        /// <param name="n_ctx_min">Minimum context size to set when trying to reduce memory use</param>
        /// <param name="log_level">Minimum log level to print during fitting, lower levels go to debug log</param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe LLamaParamsFitStatus llama_params_fit(
            string path,
            ref LLamaModelParams mparams,
            ref LLamaContextParams cparams,
            float* tensor_split,
            LLamaModelTensorBufferOverride* tensor_buft_overrides,
            nint* margins,
            uint n_ctx_min,
            int /* GGML_LOG_LEVEL */ log_level
        );

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long llama_time_us();

        
    }
}
