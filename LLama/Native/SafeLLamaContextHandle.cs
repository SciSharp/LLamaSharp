using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using LLama.Exceptions;

namespace LLama.Native
{
    /// <summary>
    /// A safe wrapper around a llama_context
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global (used implicitly in native API)
    public sealed class SafeLLamaContextHandle
        : SafeLLamaHandleBase
    {
        #region properties and fields
        /// <summary>
        /// Total number of tokens in vocabulary of this model
        /// </summary>
        public int VocabCount => ThrowIfDisposed().VocabCount;

        /// <summary>
        /// Total number of tokens in the context
        /// </summary>
        public uint ContextSize => NativeApi.llama_n_ctx(this);

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => ThrowIfDisposed().EmbeddingSize;

        /// <summary>
        /// Get the maximum batch size for this context
        /// </summary>
        public uint BatchSize => NativeApi.llama_n_batch(this);

        /// <summary>
        /// Get the model which this context is using
        /// </summary>
        public SafeLlamaModelHandle ModelHandle => ThrowIfDisposed();

        private SafeLlamaModelHandle? _model;
        #endregion

        #region construction/destruction
        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            llama_free(handle);
            SetHandle(IntPtr.Zero);

            // Decrement refcount on model
            _model?.DangerousRelease();
            _model = null!;

            return true;
        }

        private SafeLlamaModelHandle ThrowIfDisposed()
        {
            if (IsClosed)
                throw new ObjectDisposedException("Cannot use this `SafeLLamaContextHandle` - it has been disposed");
            if (_model == null || _model.IsClosed)
                throw new ObjectDisposedException("Cannot use this `SafeLLamaContextHandle` - `SafeLlamaModelHandle` has been disposed");

            return _model!;
        }

        /// <summary>
        /// Create a new llama_state for the given model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="lparams"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public static SafeLLamaContextHandle Create(SafeLlamaModelHandle model, LLamaContextParams lparams)
        {
            var ctx = llama_new_context_with_model(model, lparams);
            if (ctx == null)
                throw new RuntimeError("Failed to create context from model");

            // Increment the model reference count while this context exists.
            // DangerousAddRef throws if it fails, so there is no need to check "success"
            ctx._model = model;
            var success = false;
            ctx._model.DangerousAddRef(ref success);

            return ctx;
        }
        #endregion

        #region Native API
        static SafeLLamaContextHandle()
        {
            // This ensures that `NativeApi` has been loaded before calling the two native methods below
            NativeApi.llama_empty_call();
        }

        /// <summary>
        /// Create a new llama_context with the given model. **This should never be called directly! Always use SafeLLamaContextHandle.Create**!
        /// </summary>
        /// <param name="model"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern SafeLLamaContextHandle llama_new_context_with_model(SafeLlamaModelHandle model, LLamaContextParams @params);

        /// <summary>
        /// Frees all allocated memory in the given llama_context
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_free(IntPtr ctx);
        #endregion

        /// <summary>
        /// Token logits obtained from the last call to llama_decode
        /// The logits for the last token are stored in the last row
        /// Can be mutated in order to change the probabilities of the next token.<br />
        /// Rows: n_tokens<br />
        /// Cols: n_vocab
        /// </summary>
        /// <returns></returns>
        public Span<float> GetLogits()
        {
            var model = ThrowIfDisposed();

            unsafe
            {
                var logits = NativeApi.llama_get_logits(this);
                return new Span<float>(logits, model.VocabCount);
            }
        }

        /// <summary>
        /// Logits for the ith token. Equivalent to: llama_get_logits(ctx) + i*n_vocab
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Span<float> GetLogitsIth(int i)
        {
            var model = ThrowIfDisposed();

            unsafe
            {
                var logits = NativeApi.llama_get_logits_ith(this, i);
                return new Span<float>(logits, model.VocabCount);
            }
        }

        #region tokens
        /// <summary>
        /// Convert the given text into tokens
        /// </summary>
        /// <param name="text">The text to tokenize</param>
        /// <param name="add_bos">Whether the "BOS" token should be added</param>
        /// <param name="encoding">Encoding to use for the text</param>
        /// <param name="special">Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.</param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public LLamaToken[] Tokenize(string text, bool add_bos, bool special, Encoding encoding)
        {
            return ThrowIfDisposed().Tokenize(text, add_bos, special, encoding);
        }

        /// <summary>
        /// Convert a single llama token into bytes
        /// </summary>
        /// <param name="token">Token to decode</param>
        /// <param name="dest">A span to attempt to write into. If this is too small nothing will be written</param>
        /// <returns>The size of this token. **nothing will be written** if this is larger than `dest`</returns>
        public uint TokenToSpan(LLamaToken token, Span<byte> dest)
        {
            return ThrowIfDisposed().TokenToSpan(token, dest);
        }
        #endregion

        #region infer
        /// <summary>
        /// </summary>
        /// <param name="batch"></param>
        /// <returns>Positive return values does not mean a fatal error, but rather a warning:<br />
        ///  - 0: success<br />
        ///  - 1: could not find a KV slot for the batch (try reducing the size of the batch or increase the context)<br />
        ///  - &lt; 0: error<br />
        /// </returns>
        public DecodeResult Decode(LLamaBatch batch)
        {
            using (batch.ToNativeBatch(out var nb))
                return (DecodeResult)NativeApi.llama_decode(this, nb);
        }

        /// <summary>
        /// Decode a set of tokens in batch-size chunks.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="id"></param>
        /// <param name="batch"></param>
        /// <param name="n_past"></param>
        /// <returns>A tuple, containing the decode result and the number of tokens that have <b>not</b> been decoded yet.</returns>
        internal (DecodeResult, int) Decode(List<LLamaToken> tokens, LLamaSeqId id, LLamaBatch batch, ref int n_past)
        {
            var batchSize = checked((int)BatchSize);

            // Evaluate the prompt, in chunks smaller than the max batch size
            var n_left = tokens.Count;
            for (var i = 0; i < tokens.Count; i += batchSize)
            {
                var n_eval = tokens.Count - i;
                if (n_eval > batchSize)
                    n_eval = batchSize;

                batch.Clear();
                for (var j = 0; j < n_eval; j++)
                    batch.Add(tokens[i + j], n_past++, id, (i + j) == tokens.Count - 1);

                var returnCode = Decode(batch);
                if (returnCode != DecodeResult.Ok)
                    return (returnCode, n_left);

                n_left -= n_eval;
            }

            return (DecodeResult.Ok, 0);
        }
        #endregion

        #region state
        /// <summary>
        /// Get the size of the state, when saved as bytes
        /// </summary>
        public ulong GetStateSize()
        {
            return NativeApi.llama_get_state_size(this);
        }

        /// <summary>
        /// Get the raw state of this context, encoded as bytes. Data is written into the `dest` pointer.
        /// </summary>
        /// <param name="dest">Destination to write to</param>
        /// <param name="size">Number of bytes available to write to in dest (check required size with `GetStateSize()`)</param>
        /// <returns>The number of bytes written to dest</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if dest is too small</exception>
        public unsafe ulong GetState(byte* dest, ulong size)
        {
            return GetState(new IntPtr(dest), size);
        }

        /// <summary>
        /// Get the raw state of this context, encoded as bytes. Data is written into the `dest` pointer.
        /// </summary>
        /// <param name="dest">Destination to write to</param>
        /// <param name="size">Number of bytes available to write to in dest (check required size with `GetStateSize()`)</param>
        /// <returns>The number of bytes written to dest</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if dest is too small</exception>
        public ulong GetState(IntPtr dest, ulong size)
        {
            var required = GetStateSize();
            if (size < required)
                throw new ArgumentOutOfRangeException(nameof(size), $"Allocated space is too small, {size} < {required}");

            unsafe
            {
                return NativeApi.llama_copy_state_data(this, (byte*)dest.ToPointer());
            }
        }

        /// <summary>
        /// Set the raw state of this context
        /// </summary>
        /// <param name="src">The pointer to read the state from</param>
        /// <returns>Number of bytes read from the src pointer</returns>
        public unsafe ulong SetState(byte* src)
        {
            return SetState(new IntPtr(src));
        }

        /// <summary>
        /// Set the raw state of this context
        /// </summary>
        /// <param name="src">The pointer to read the state from</param>
        /// <returns>Number of bytes read from the src pointer</returns>
        public ulong SetState(IntPtr src)
        {
            unsafe
            {
                return NativeApi.llama_set_state_data(this, (byte*)src.ToPointer());
            }
        }
        #endregion

        /// <summary>
        /// Set the RNG seed
        /// </summary>
        /// <param name="seed"></param>
        public void SetSeed(uint seed)
        {
            NativeApi.llama_set_rng_seed(this, seed);
        }

        /// <summary>
        /// Set the number of threads used for decoding
        /// </summary>
        /// <param name="threads">n_threads is the number of threads used for generation (single token)</param>
        /// <param name="threadsBatch">n_threads_batch is the number of threads used for prompt and batch processing (multiple tokens)</param>
        public void SetThreads(uint threads, uint threadsBatch)
        {
            NativeApi.llama_set_n_threads(this, threads, threadsBatch);
        }

        #region KV Cache Management
        /// <summary>
        /// Get a new KV cache view that can be used to debug the KV cache
        /// </summary>
        /// <param name="maxSequences"></param>
        /// <returns></returns>
        public LLamaKvCacheViewSafeHandle KvCacheGetDebugView(int maxSequences = 4)
        {
            return LLamaKvCacheViewSafeHandle.Allocate(this, maxSequences);
        }

        /// <summary>
        /// Count the number of used cells in the KV cache (i.e. have at least one sequence assigned to them)
        /// </summary>
        /// <returns></returns>
        public int KvCacheCountCells()
        {
            return NativeApi.llama_get_kv_cache_used_cells(this);
        }

        /// <summary>
        /// Returns the number of tokens in the KV cache (slow, use only for debug)
        /// If a KV cell has multiple sequences assigned to it, it will be counted multiple times
        /// </summary>
        /// <returns></returns>
        public int KvCacheCountTokens()
        {
            return NativeApi.llama_get_kv_cache_token_count(this);
        }

        /// <summary>
        /// Clear the KV cache
        /// </summary>
        public void KvCacheClear()
        {
            NativeApi.llama_kv_cache_clear(this);
        }

        /// <summary>
        /// Removes all tokens that belong to the specified sequence and have positions in [p0, p1)
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public void KvCacheRemove(LLamaSeqId seq, LLamaPos p0, LLamaPos p1)
        {
            NativeApi.llama_kv_cache_seq_rm(this, seq, p0, p1);
        }

        /// <summary>
        /// Copy all tokens that belong to the specified sequence to another sequence. Note that
        /// this does not allocate extra KV cache memory - it simply assigns the tokens to the
        /// new sequence
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public void KvCacheSequenceCopy(LLamaSeqId src, LLamaSeqId dest, LLamaPos p0, LLamaPos p1)
        {
            NativeApi.llama_kv_cache_seq_cp(this, src, dest, p0, p1);
        }

        /// <summary>
        /// Removes all tokens that do not belong to the specified sequence
        /// </summary>
        /// <param name="seq"></param>
        public void KvCacheSequenceKeep(LLamaSeqId seq)
        {
            NativeApi.llama_kv_cache_seq_keep(this, seq);
        }

        /// <summary>
        /// Adds relative position "delta" to all tokens that belong to the specified sequence
        /// and have positions in [p0, p1. If the KV cache is RoPEd, the KV data is updated
        /// accordingly
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="delta"></param>
        public void KvCacheSequenceShift(LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta)
        {
            NativeApi.llama_kv_cache_seq_shift(this, seq, p0, p1, delta);
        }

        /// <summary>
        /// Integer division of the positions by factor of `d > 1`.
        /// If the KV cache is RoPEd, the KV data is updated accordingly.<br />
        /// p0 &lt; 0 : [0,  p1]<br />
        /// p1 &lt; 0 : [p0, inf)
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="divisor"></param>
        public void KvCacheSequenceDivide(LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int divisor)
        {
            NativeApi.llama_kv_cache_seq_div(this, seq, p0, p1, divisor);
        }
        #endregion
    }
}
