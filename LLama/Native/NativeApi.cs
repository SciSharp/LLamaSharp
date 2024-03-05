using LLama.LLava;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable IDE1006 // Naming Styles

namespace LLama.Native
{
    /// <summary>
    /// Callback from llama.cpp with log messages
    /// </summary>
    /// <param name="level"></param>
    /// <param name="message"></param>
	public delegate void LLamaLogCallback(LLamaLogLevel level, string message);

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
        /// Create a LLamaModelParams with default values
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaModelParams llama_model_default_params();

        /// <summary>
        /// Create a LLamaContextParams with default values
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaContextParams llama_context_default_params();

        /// <summary>
        /// Create a LLamaModelQuantizeParams with default values
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaModelQuantizeParams llama_model_quantize_default_params();

        /// <summary>
        /// Check if memory mapping is supported
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool llama_supports_mmap();

        /// <summary>
        /// Check if memory locking is supported
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool llama_supports_mlock();

        /// <summary>
        /// Check if GPU offload is supported
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool llama_supports_gpu_offload();

        /// <summary>
        /// Initialize the llama + ggml backend
        /// Call once at the start of the program
        /// </summary>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_backend_init(bool numa);

        /// <summary>
        /// Sets the current rng seed.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="seed"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_set_rng_seed(SafeLLamaContextHandle ctx, uint seed);

        /// <summary>
        /// Returns the maximum size in bytes of the state (rng, logits, embedding
        /// and kv_cache) - will often be smaller after compacting tokens
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong llama_get_state_size(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Copies the state to the specified destination address.
        /// Destination needs to have allocated enough memory.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dest"></param>
        /// <returns>the number of bytes copied</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe ulong llama_copy_state_data(SafeLLamaContextHandle ctx, byte* dest);

        /// <summary>
        /// Set the state reading from the specified address
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="src"></param>
        /// <returns>the number of bytes read</returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe ulong llama_set_state_data(SafeLLamaContextHandle ctx, byte* src);

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
        public static extern bool llama_load_session_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens_out, ulong n_token_capacity, out ulong n_token_count_out);

        /// <summary>
        /// Save session file
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path_session"></param>
        /// <param name="tokens"></param>
        /// <param name="n_token_count"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool llama_save_session_file(SafeLLamaContextHandle ctx, string path_session, LLamaToken[] tokens, ulong n_token_count);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe byte* llama_token_get_text(SafeLlamaModelHandle model, LLamaToken token);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float llama_token_get_score(SafeLlamaModelHandle model, LLamaToken token);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaTokenType llama_token_get_type(SafeLlamaModelHandle model, LLamaToken token);

        /// <summary>
        /// Get the size of the context window for the model for this context
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint llama_n_ctx(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Get the batch size for this context
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint llama_n_batch(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Token logits obtained from the last call to llama_decode
        /// The logits for the last token are stored in the last row
        /// Can be mutated in order to change the probabilities of the next token.<br />
        /// Rows: n_tokens<br />
        /// Cols: n_vocab
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe float* llama_get_logits(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Logits for the ith token. Equivalent to: llama_get_logits(ctx) + i*n_vocab
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe float* llama_get_logits_ith(SafeLLamaContextHandle ctx, int i);


        /// <summary>
        /// Get the embeddings for the input
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static Span<float> llama_get_embeddings(SafeLLamaContextHandle ctx)
        {
            unsafe
            {
                var ptr = llama_get_embeddings_native(ctx);
                return new Span<float>(ptr, ctx.EmbeddingSize);
            }

            [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_get_embeddings")]
            static extern unsafe float* llama_get_embeddings_native(SafeLLamaContextHandle ctx);
        }

        /// <summary>
        /// Get the "Beginning of sentence" token
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaToken llama_token_bos(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the "End of sentence" token
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaToken llama_token_eos(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the "new line" token
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern LLamaToken llama_token_nl(SafeLlamaModelHandle model);

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
        /// codellama infill tokens, Beginning of infill prefix
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_token_prefix(SafeLlamaModelHandle model);

        /// <summary>
        /// codellama infill tokens, Beginning of infill middle
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_token_middle(SafeLlamaModelHandle model);

        /// <summary>
        /// codellama infill tokens, Beginning of infill suffix
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_token_suffix(SafeLlamaModelHandle model);

        /// <summary>
        /// codellama infill tokens, End of infill middle
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_token_eot(SafeLlamaModelHandle model);

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
        /// <returns>The length written, or if the buffer is too small a negative that indicates the length required</returns>
        public static int llama_token_to_piece(SafeLlamaModelHandle model, LLamaToken llamaToken, Span<byte> buffer)
        {
            unsafe
            {
                fixed (byte* bufferPtr = buffer)
                {
                    return llama_token_to_piece_native(model, llamaToken, bufferPtr, buffer.Length);
                }
            }

            [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_token_to_piece")]
            static extern unsafe int llama_token_to_piece_native(SafeLlamaModelHandle model, LLamaToken llamaToken, byte* buffer, int length);
        }

        /// <summary>
        /// Convert text into tokens
        /// </summary>
        /// <param name="model"></param>
        /// <param name="text"></param>
        /// <param name="text_len"></param>
        /// <param name="tokens"></param>
        /// <param name="n_max_tokens"></param>
        /// <param name="add_bos"></param>
        /// <param name="special">Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext. Does not insert a leading space.</param>
        /// <returns>Returns the number of tokens on success, no more than n_max_tokens.
        /// Returns a negative number on failure - the number of tokens that would have been returned
        /// </returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int llama_tokenize(SafeLlamaModelHandle model, byte* text, int text_len, LLamaToken* tokens, int n_max_tokens, bool add_bos, bool special);

        /// <summary>
        /// Register a callback to receive llama log messages
        /// </summary>
        /// <param name="logCallback"></param>
		[DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_log_set(LLamaLogCallback logCallback);

        /// <summary>
        /// Clear the KV cache
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
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_kv_cache_seq_rm(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1);

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
        /// If the KV cache is RoPEd, the KV data is updated accordingly
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="seq"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="delta"></param>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_kv_cache_seq_shift(SafeLLamaContextHandle ctx, LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta);

        /// <summary>
        /// Integer division of the positions by factor of `d > 1`
        /// If the KV cache is RoPEd, the KV data is updated accordingly
        /// p0 &lt; 0 : [0,  p1]
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
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="batch"></param>
        /// <returns>Positive return values does not mean a fatal error, but rather a warning:<br />
        ///  - 0: success<br />
        ///  - 1: could not find a KV slot for the batch (try reducing the size of the batch or increase the context)<br />
        ///  - &lt; 0: error<br />
        /// </returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int llama_decode(SafeLLamaContextHandle ctx, LLamaNativeBatch batch);

        /// <summary>
        /// Set the number of threads used for decoding
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="n_threads">n_threads is the number of threads used for generation (single token)</param>
        /// <param name="n_threads_batch">n_threads_batch is the number of threads used for prompt and batch processing (multiple tokens)</param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void llama_set_n_threads(SafeLLamaContextHandle ctx, uint n_threads, uint n_threads_batch);

        /// <summary>
        /// Get vocab type from model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public extern static LLamaVocabType llama_vocab_type(SafeLlamaModelHandle model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="n_tokens"></param>
        /// <param name="pos_0"></param>
        /// <param name="seq_id"></param>
        /// <returns></returns>

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        unsafe public extern static LLamaNativeBatch llama_batch_get_one(LLamaToken* tokens, int n_tokens, ref int pos_0, int seq_id);
        /// <summary>
        /// Set the Init time to ggml process
        /// </summary>
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ggml_time_init();

        /// <summary>
        /// Load clip model
        /// </summary>
        /// <param name="clip_model_path">Clip model path</param>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [DllImport(llavaLibName, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr clip_model_load(string clip_model_path, int verbosity = 1);

        /// <summary>
        /// Free the clip context
        /// </summary>
        /// <param name="ctx">Clip context</param>
        [DllImport(llavaLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clip_free(IntPtr ctx);

        /// <summary>
        /// Read an image from bytes
        /// </summary>
        /// <param name="ctx_clip"></param>
        /// <param name="n_threads"></param>
        /// <param name="bytes"></param>
        /// <param name="image_bytes_length"></param>
        /// <returns></returns>
        [DllImport(llavaLibName, CallingConvention = CallingConvention.Cdecl)]
        public extern static LLavaImageEmbed llava_image_embed_make_with_bytes(IntPtr ctx_clip, int n_threads, byte[] bytes, int image_bytes_length);

        /// <summary>
        /// Embed an image and get the token length
        /// </summary>
        /// <param name="ctx_llama"></param>
        /// <param name="image_embed"></param>
        /// <param name="n_batch"></param>
        /// <param name="n_past"></param>
        /// <returns></returns>
        [DllImport(llavaLibName, CallingConvention = CallingConvention.Cdecl)]
        public extern unsafe static bool llava_eval_image_embed(SafeLLamaContextHandle ctx_llama, LLavaImageEmbed image_embed, int n_batch, ref int n_past);

    }
}
