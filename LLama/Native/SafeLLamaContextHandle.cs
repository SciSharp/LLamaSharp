using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
        /// The underlying vocabulary for the model
        /// </summary>
        /// <returns></returns>
        public LLamaVocabType LLamaVocabType => ThrowIfDisposed().VocabType;

        /// <summary>
        /// Total number of tokens in the context
        /// </summary>
        public uint ContextSize => llama_n_ctx(this);

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => ThrowIfDisposed().EmbeddingSize;

        /// <summary>
        /// Get the maximum batch size for this context
        /// </summary>
        public uint BatchSize => llama_n_batch(this);

        /// <summary>
        /// Get the physical maximum batch size for this context
        /// </summary>
        public uint UBatchSize => llama_n_ubatch(this);

        /// <summary>
        /// Get or set the number of threads used for generation of a single token.
        /// </summary>
        public int GenerationThreads
        {
            get => llama_n_threads(this);
            set => llama_set_n_threads(this, value, BatchThreads);
        }

        /// <summary>
        /// Get or set the number of threads used for prompt and batch processing (multiple token).
        /// </summary>
        public int BatchThreads
        {
            get => llama_n_threads_batch(this);
            set => llama_set_n_threads(this, GenerationThreads, value);
        }

        /// <summary>
        /// Get the pooling type for this context
        /// </summary>
        public LLamaPoolingType PoolingType => llama_pooling_type(this);

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

        /// <summary>
        /// Set a callback which can abort computation
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="abort_callback"></param>
        /// <param name="abort_callback_data"></param>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void llama_set_abort_callback(SafeLLamaContextHandle ctx, GgmlAbortCallback abort_callback, void* abort_callback_data);

        /// <summary>
        /// If this returns true computation is cancelled
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private unsafe delegate bool GgmlAbortCallback(void* data);

        /// <summary>
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="batch"></param>
        /// <returns>Positive return values does not mean a fatal error, but rather a warning:<br />
        ///  - 0: success<br />
        ///  - 1: could not find a KV slot for the batch (try reducing the size of the batch or increase the context)<br />
        ///  - &lt; 0: error<br />
        /// </returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_decode(SafeLLamaContextHandle ctx, LLamaNativeBatch batch);

        /// <summary>
        /// Processes a batch of tokens with the encoder part of the encoder-decoder model. Stores the encoder output
        /// internally for later use by the decoder cross-attention layers.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="batch"></param>
        /// <returns>0 = success <br />&lt; 0 = error</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_encode(SafeLLamaContextHandle ctx, LLamaNativeBatch batch);

        /// <summary>
        /// Set the number of threads used for decoding
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="n_threads">n_threads is the number of threads used for generation (single token)</param>
        /// <param name="n_threads_batch">n_threads_batch is the number of threads used for prompt and batch processing (multiple tokens)</param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_set_n_threads(SafeLLamaContextHandle ctx, int n_threads, int n_threads_batch);

        /// <summary>
        /// Get the number of threads used for generation of a single token.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_n_threads(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Get the number of threads used for prompt and batch processing (multiple token).
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_n_threads_batch(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Token logits obtained from the last call to llama_decode
        /// The logits for the last token are stored in the last row
        /// Can be mutated in order to change the probabilities of the next token.<br />
        /// Rows: n_tokens<br />
        /// Cols: n_vocab
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe float* llama_get_logits(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Logits for the ith token. Equivalent to: llama_get_logits(ctx) + i*n_vocab
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe float* llama_get_logits_ith(SafeLLamaContextHandle ctx, int i);

        /// <summary>
        /// Get the size of the context window for the model for this context
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint llama_n_ctx(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Get the batch size for this context
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint llama_n_batch(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Get the ubatch size for this context
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint llama_n_ubatch(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Returns the **actual** size in bytes of the state (logits, embedding and kv_cache).
        /// Only use when saving the state, not when restoring it, otherwise the size may be too small.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern nuint llama_state_get_size(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Copies the state to the specified destination address.
        /// Destination needs to have allocated enough memory.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dest"></param>
        /// <param name="size"></param>
        /// <returns>the number of bytes copied</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe nuint llama_state_get_data(SafeLLamaContextHandle ctx, byte* dest, nuint size);

        /// <summary>
        /// Set the state reading from the specified address
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="src"></param>
        /// <param name="size"></param>
        /// <returns>the number of bytes read</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe nuint llama_state_set_data(SafeLLamaContextHandle ctx, byte* src, nuint size);

        /// <summary>
        /// Get the exact size needed to copy the KV cache of a single sequence
        /// </summary>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern nuint llama_state_seq_get_size(SafeLLamaContextHandle ctx, LLamaSeqId seqId);

        /// <summary>
        /// Copy the KV cache of a single sequence into the specified buffer
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dst"></param>
        /// <param name="size"></param>
        /// <param name="seqId"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe nuint llama_state_seq_get_data(SafeLLamaContextHandle ctx, byte* dst, nuint size, LLamaSeqId seqId);

        /// <summary>
        /// Copy the sequence data (originally copied with `llama_state_seq_get_data`) into the specified sequence
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="src"></param>
        /// <param name="size"></param>
        /// <param name="destSeqId"></param>
        /// <returns>
        ///  - Positive: Ok
        ///  - Zero: Failed to load
        /// </returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe nuint llama_state_seq_set_data(SafeLLamaContextHandle ctx, byte* src, nuint size, LLamaSeqId destSeqId);

        /// <summary>
        /// Defragment the KV cache. This will be applied:
        ///   - lazily on next llama_decode()
        ///   - explicitly with llama_kv_cache_update()
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_kv_cache_defrag(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Apply the KV cache updates (such as K-shifts, defragmentation, etc.)
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_kv_cache_update(SafeLLamaContextHandle ctx);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaPerfContextTimings llama_perf_context(SafeLLamaContextHandle ctx);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_perf_context_print(SafeLLamaContextHandle ctx);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_perf_context_reset(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Wait until all computations are finished. This is automatically done when using any of the functions to obtain computation results
        /// and is not necessary to call it explicitly in most cases.
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_synchronize(SafeLLamaContextHandle ctx);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_lora_adapter_set(SafeLLamaContextHandle context, IntPtr adapter, float scale);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_lora_adapter_remove(SafeLLamaContextHandle context, IntPtr adapter);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_lora_adapter_clear(SafeLLamaContextHandle context);

        /// <summary>
        /// Get the pooling type for this context
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaPoolingType llama_pooling_type(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Get the embeddings for the a specific sequence.
        /// Equivalent to: llama_get_embeddings(ctx) + ctx->output_ids[i]*n_embd
        /// </summary>
        /// <returns>A pointer to the first float in an embedding, length = ctx.EmbeddingSize</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe float* llama_get_embeddings_seq(SafeLLamaContextHandle ctx, LLamaSeqId id);

        /// <summary>
        /// Get the embeddings for the ith sequence.
        /// Equivalent to: llama_get_embeddings(ctx) + ctx->output_ids[i]*n_embd
        /// </summary>
        /// <returns>A pointer to the first float in an embedding, length = ctx.EmbeddingSize</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe float* llama_get_embeddings_ith(SafeLLamaContextHandle ctx, int i);
        #endregion

        #region LoRA
        /// <summary>
        /// Add a LoRA adapter to this context
        /// </summary>
        /// <param name="lora"></param>
        /// <param name="scale"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="RuntimeError"></exception>
        public void AddLoraAdapter(LoraAdapter lora, float scale)
        {
            if (lora.Model != ModelHandle)
                throw new ArgumentException("Cannot add LoRA adapter which was loaded for a different model");
            if (!lora.Loaded)
                throw new ArgumentException("Cannot add LoRA adapter which has been unloaded");

            var err = llama_lora_adapter_set(this, lora.Pointer, scale);
            if (err != 0)
                throw new RuntimeError("Failed to set lora adapter");
        }

        /// <summary>
        /// Remove a LoRA adapter from this context
        /// </summary>
        /// <param name="lora"></param>
        /// <returns>Indicates if the lora was in this context and was remove</returns>
        public bool RemoveLoraAdapter(LoraAdapter lora)
        {
            if (lora.Model != ModelHandle)
                return false;

            var err = llama_lora_adapter_remove(this, lora.Pointer);
            return err == 0;
        }

        /// <summary>
        /// Remove all LoRA adapters from this context
        /// </summary>
        public void ClearLoraAdapters()
        {
            llama_lora_adapter_clear(this);
        }
        #endregion

        #region GetLogits
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
                var logits = llama_get_logits(this);
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
                var logits = llama_get_logits_ith(this, i);
                if (logits == null)
                    throw new GetLogitsInvalidIndexException(i);

                return new Span<float>(logits, model.VocabCount);
            }
        }
        #endregion

        #region GetEmbeddings()
        /// <summary>
        /// Get the embeddings for the ith sequence.
        /// Equivalent to: llama_get_embeddings(ctx) + ctx->output_ids[i]*n_embd
        /// </summary>
        /// <returns>A pointer to the first float in an embedding, length = ctx.EmbeddingSize</returns>
        public Span<float> GetEmbeddingsIth(LLamaPos pos)
        {
            var model = ThrowIfDisposed();

            unsafe
            {
                var embd = llama_get_embeddings_ith(this, pos.Value);
                Debug.Assert(embd != null);
                return new Span<float>(embd, model.EmbeddingSize);
            }
        }

        /// <summary>
        /// Get the embeddings for the a specific sequence.
        /// Equivalent to: llama_get_embeddings(ctx) + ctx->output_ids[i]*n_embd
        /// </summary>
        /// <returns>A pointer to the first float in an embedding, length = ctx.EmbeddingSize</returns>
        public Span<float> GetEmbeddingsSeq(LLamaSeqId seq)
        {
            var model = ThrowIfDisposed();

            unsafe
            {
                var embd = llama_get_embeddings_seq(this, seq);
                Debug.Assert(embd != null);
                return new Span<float>(embd, model.EmbeddingSize);
            }
        }
        #endregion

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
        /// This object exists to ensure there is only ever 1 inference running at a time. This is a workaround for thread safety issues in llama.cpp itself.
        /// Most notably CUDA, which seems to use some global singleton resources and will crash if multiple inferences are run (even against different models).
        /// 
        /// For more information see these issues:
        ///  - https://github.com/SciSharp/LLamaSharp/issues/596
        ///  - https://github.com/ggerganov/llama.cpp/issues/3960
        ///
        /// If these are ever resolved this lock can probably be removed.
        /// </summary>
        private static readonly object GlobalInferenceLock = new();

        /// <summary>
        /// Wait until all computations are finished. This is automatically done when using any of the functions to obtain computation results
        /// and is not necessary to call it explicitly in most cases.
        /// </summary>
        public void Synchronize()
        {
            lock (GlobalInferenceLock)
                llama_synchronize(this);
        }

        /// <summary>
        /// Processes a batch of tokens with the encoder part of the encoder-decoder model. Stores the encoder output
        /// internally for later use by the decoder cross-attention layers.
        /// </summary>
        /// <param name="batch"></param>
        /// <returns>0 = success <br />&lt; 0 = error</returns>
        public DecodeResult Encode(LLamaBatch batch)
        {
            if (batch.TokenCount == 0)
                return DecodeResult.Ok;

            lock (GlobalInferenceLock)
                using (batch.ToNativeBatch(out var nb))
                    return (DecodeResult)llama_encode(this, nb);
        }

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
            if (batch.TokenCount == 0)
                return DecodeResult.Ok;

            lock (GlobalInferenceLock)
                using (batch.ToNativeBatch(out var nb))
                    return (DecodeResult)llama_decode(this, nb);
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
            if (tokens.Count == 0)
                return (DecodeResult.Ok, 0);

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
        
        /// <summary>
        /// </summary>
        /// <param name="batch"></param>
        /// <returns>Positive return values does not mean a fatal error, but rather a warning:<br />
        ///  - 0: success<br />
        ///  - 1: could not find a KV slot for the batch (try reducing the size of the batch or increase the context)<br />
        ///  - &lt; 0: error<br />
        /// </returns>
        public DecodeResult Decode(LLamaBatchEmbeddings batch)
        {
            if (batch.EmbeddingsCount == 0)
                return DecodeResult.Ok;
            
            lock (GlobalInferenceLock)
                using (batch.ToNativeBatch(out var nb))
                    return (DecodeResult)llama_decode(this, nb);
        }
        #endregion

        #region state
        /// <summary>
        /// Get the size of the state, when saved as bytes
        /// </summary>
        public nuint GetStateSize()
        {
            return llama_state_get_size(this);
        }

        /// <summary>
        /// Get the size of the KV cache for a single sequence ID, when saved as bytes
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public nuint GetStateSize(LLamaSeqId sequence)
        {
            return llama_state_seq_get_size(this, sequence);
        }

        /// <summary>
        /// Get the raw state of this context, encoded as bytes. Data is written into the `dest` pointer.
        /// </summary>
        /// <param name="dest">Destination to write to</param>
        /// <param name="size">Number of bytes available to write to in dest (check required size with `GetStateSize()`)</param>
        /// <returns>The number of bytes written to dest</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if dest is too small</exception>
        public unsafe nuint GetState(byte* dest, nuint size)
        {
            var required = GetStateSize();
            if (size < required)
                throw new ArgumentOutOfRangeException(nameof(size), $"Allocated space is too small, {size} < {required}");

            return llama_state_get_data(this, dest, size);
        }

        /// <summary>
        /// Get the raw state of a single sequence from this context, encoded as bytes. Data is written into the `dest` pointer.
        /// </summary>
        /// <param name="dest">Destination to write to</param>
        /// <param name="size">Number of bytes available to write to in dest (check required size with `GetStateSize()`)</param>
        /// <param name="sequence">The sequence to get state data for</param>
        /// <returns>The number of bytes written to dest</returns>
        public unsafe nuint GetState(byte* dest, nuint size, LLamaSeqId sequence)
        {
            var required = GetStateSize(sequence);
            if (size < required)
                throw new ArgumentOutOfRangeException(nameof(size), $"Allocated space is too small, {size} < {required}");

            return llama_state_seq_get_data(this, dest, size, sequence);
        }

        /// <summary>
        /// Set the raw state of this context
        /// </summary>
        /// <param name="src">The pointer to read the state from</param>
        /// <param name="size">Number of bytes that can be safely read from the pointer</param>
        /// <returns>Number of bytes read from the src pointer</returns>
        public unsafe nuint SetState(byte* src, nuint size)
        {
            return llama_state_set_data(this, src, size);
        }

        /// <summary>
        /// Set the raw state of a single sequence
        /// </summary>
        /// <param name="src">The pointer to read the state from</param>
        /// <param name="sequence">Sequence ID to set</param>
        /// <param name="size">Number of bytes that can be safely read from the pointer</param>
        /// <returns>Number of bytes read from the src pointer</returns>
        public unsafe nuint SetState(byte* src, nuint size, LLamaSeqId sequence)
        {
            return llama_state_seq_set_data(this, src, size, sequence);
        }
        #endregion

        #region timing
        /// <summary>
        /// Get performance information
        /// </summary>
        /// <returns></returns>
        public LLamaPerfContextTimings GetTimings()
        {
            return llama_perf_context(this);
        }
        
        /// <summary>
        /// Reset all performance information for this context
        /// </summary>
        public void ResetTimings()
        {
            llama_perf_context_reset(this);
        }
        #endregion

        #region KV Cache Management
        /// <summary>
        /// Apply KV cache updates (such as K-shifts, defragmentation, etc.)
        /// </summary>
        public void KvCacheUpdate()
        {
            llama_kv_cache_update(this);
        }

        /// <summary>
        /// Defragment the KV cache. This will be applied:
        ///   - lazily on next llama_decode()
        ///   - explicitly with llama_kv_cache_update()
        /// </summary>
        /// <returns></returns>
        public void KvCacheDefrag()
        {
            llama_kv_cache_defrag(this);
        }

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
        /// Clear the KV cache - both cell info is erased and KV data is zeroed
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
        public void KvCacheSequenceAdd(LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta)
        {
            NativeApi.llama_kv_cache_seq_add(this, seq, p0, p1, delta);
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

        /// <summary>
        /// Returns the largest position present in the KV cache for the specified sequence
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        public LLamaPos KvCacheMaxPosition(LLamaSeqId seq)
        {
            return NativeApi.llama_kv_cache_seq_pos_max(this, seq);
        }
        #endregion
    }
}
