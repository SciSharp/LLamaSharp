using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using LLama.Exceptions;

namespace LLama.Native
{
    using llama_token = Int32;
    public unsafe partial class NativeApi
    {
        static NativeApi()
        {
            try
            {
                llama_empty_call();
            }
            catch (DllNotFoundException)
            {
                throw new RuntimeError("The native library cannot be found. It could be one of the following reasons: \n" +
                    "1. No LLamaSharp backend was installed. Please search LLamaSharp.Backend and install one of them. \n" +
                    "2. You are using a device with only CPU but installed cuda backend. Please install cpu backend instead. \n" +
                    "3. The backend is not compatible with your system cuda environment. Please check and fix it. If the environment is " +
                    "expected not to be changed, then consider build llama.cpp from source or submit an issue to LLamaSharp.");
            }
            NativeApi.llama_init_backend();
        }
        private const string libraryName = "libllama";

        [DllImport("libllama", EntryPoint = "llama_mmap_supported")]
        public static extern bool llama_empty_call();

        [DllImport(libraryName)]
        public static extern LLamaContextParams llama_context_default_params();

        [DllImport(libraryName)]
        public static extern bool llama_mmap_supported();

        [DllImport(libraryName)]
        public static extern bool llama_mlock_supported();

        /// <summary>
        /// Various functions for loading a ggml llama model.
        /// Allocate (almost) all memory needed for the model.
        /// Return NULL on failure
        /// </summary>
        /// <param name="path_model"></param>
        /// <param name="params_"></param>
        /// <returns></returns>
        [DllImport(libraryName)]
        public static extern IntPtr llama_init_from_file(string path_model, LLamaContextParams params_);

        /// <summary>
        /// not great API - very likely to change. 
        /// Initialize the llama + ggml backend
        /// Call once at the start of the program
        /// </summary>
        [DllImport(libraryName)]
        public static extern void llama_init_backend();

        /// <summary>
        /// Frees all allocated memory
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(libraryName)]
        public static extern void llama_free(IntPtr ctx);

        /// <summary>
        /// Apply a LoRA adapter to a loaded model
        /// path_base_model is the path to a higher quality model to use as a base for
        /// the layers modified by the adapter. Can be NULL to use the current loaded model.
        /// The model needs to be reloaded before applying a new adapter, otherwise the adapter
        /// will be applied on top of the previous one
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path_lora"></param>
        /// <param name="path_base_model"></param>
        /// <param name="n_threads"></param>
        /// <returns>Returns 0 on success</returns>
        [DllImport(libraryName)]
        public static extern int llama_apply_lora_from_file(SafeLLamaContextHandle ctx, string path_lora, string path_base_model, int n_threads);

        /// <summary>
        /// Returns the number of tokens in the KV cache
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName)]
        public static extern int llama_get_kv_cache_token_count(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Sets the current rng seed.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="seed"></param>
        [DllImport(libraryName)]
        public static extern void llama_set_rng_seed(SafeLLamaContextHandle ctx, int seed);

        /// <summary>
        /// Returns the maximum size in bytes of the state (rng, logits, embedding
        /// and kv_cache) - will often be smaller after compacting tokens
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName)]
        public static extern ulong llama_get_state_size(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Copies the state to the specified destination address.
        /// Destination needs to have allocated enough memory.
        /// Returns the number of bytes copied
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        [DllImport(libraryName)]
        public static extern ulong llama_copy_state_data(SafeLLamaContextHandle ctx, byte[] dest);

        /// <summary>
        /// Set the state reading from the specified address
        /// Returns the number of bytes read
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        [DllImport(libraryName)]
        public static extern ulong llama_set_state_data(SafeLLamaContextHandle ctx, byte[] src);

        /// <summary>
        /// Load session file
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path_session"></param>
        /// <param name="tokens_out"></param>
        /// <param name="n_token_capacity"></param>
        /// <param name="n_token_count_out"></param>
        /// <returns></returns>
        [DllImport(libraryName)]
        public static extern bool llama_load_session_file(SafeLLamaContextHandle ctx, string path_session, llama_token[] tokens_out, ulong n_token_capacity, ulong* n_token_count_out);

        /// <summary>
        /// Save session file
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path_session"></param>
        /// <param name="tokens"></param>
        /// <param name="n_token_count"></param>
        /// <returns></returns>
        [DllImport(libraryName)]
        public static extern bool llama_save_session_file(SafeLLamaContextHandle ctx, string path_session, llama_token[] tokens, ulong n_token_count);

        /// <summary>
        /// Run the llama inference to obtain the logits and probabilities for the next token.
        /// tokens + n_tokens is the provided batch of new tokens to process
        /// n_past is the number of tokens to use from previous eval calls
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tokens"></param>
        /// <param name="n_tokens"></param>
        /// <param name="n_past"></param>
        /// <param name="n_threads"></param>
        /// <returns>Returns 0 on success</returns>
        [DllImport(libraryName)]
        public static extern int llama_eval(SafeLLamaContextHandle ctx, llama_token[] tokens, int n_tokens, int n_past, int n_threads);

        /// <summary>
        /// Convert the provided text into tokens.
        /// The tokens pointer must be large enough to hold the resulting tokens.
        /// Returns the number of tokens on success, no more than n_max_tokens
        /// Returns a negative number on failure - the number of tokens that would have been returned
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="text"></param>
        /// <param name="tokens"></param>
        /// <param name="n_max_tokens"></param>
        /// <param name="add_bos"></param>
        /// <returns></returns>
        [DllImport(libraryName)] 
        public static extern int llama_tokenize(SafeLLamaContextHandle ctx, string text, llama_token[] tokens, int n_max_tokens, bool add_bos);

        [DllImport(libraryName)]
        public static extern int llama_n_vocab(SafeLLamaContextHandle ctx);

        [DllImport(libraryName)]
        public static extern int llama_n_ctx(SafeLLamaContextHandle ctx);

        [DllImport(libraryName)]
        public static extern int llama_n_embd(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Token logits obtained from the last call to llama_eval()
        /// The logits for the last token are stored in the last row
        /// Can be mutated in order to change the probabilities of the next token
        /// Rows: n_tokens
        /// Cols: n_vocab
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName)]
        public static extern float* llama_get_logits(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Get the embeddings for the input
        /// shape: [n_embd] (1-dimensional)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(libraryName)]
        public static extern float* llama_get_embeddings(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Token Id -> String. Uses the vocabulary in the provided context
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="token"></param>
        /// <returns>Pointer to a string.</returns>
        [DllImport(libraryName)]
        public static extern IntPtr llama_token_to_str(SafeLLamaContextHandle ctx, llama_token token);

        [DllImport(libraryName)]
        public static extern llama_token llama_token_bos();

        [DllImport(libraryName)]
        public static extern llama_token llama_token_eos();

        [DllImport(libraryName)]
        public static extern llama_token llama_token_nl();

        [DllImport(libraryName)]
        public static extern void llama_print_timings(SafeLLamaContextHandle ctx);

        [DllImport(libraryName)]
        public static extern void llama_reset_timings(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Print system information
        /// </summary>
        /// <returns></returns>
        [DllImport(libraryName)]
        public static extern IntPtr llama_print_system_info();
    }
}
