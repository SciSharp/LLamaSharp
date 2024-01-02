using System;
using System.Buffers;
using System.Text;
using LLama.Exceptions;

namespace LLama.Native
{
    /// <summary>
    /// A safe wrapper around a llama_context
    /// </summary>
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
        public int ContextSize => NativeApi.llama_n_ctx(this);

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => ThrowIfDisposed().EmbeddingSize;

        /// <summary>
        /// Get the model which this context is using
        /// </summary>
        public SafeLlamaModelHandle ModelHandle => ThrowIfDisposed();

        private SafeLlamaModelHandle? _model;
        #endregion

        #region construction/destruction
        /// <summary>
        /// Create a new SafeLLamaContextHandle
        /// </summary>
        /// <param name="handle">pointer to an allocated llama_context</param>
        /// <param name="model">the model which this context was created from</param>
        public SafeLLamaContextHandle(IntPtr handle, SafeLlamaModelHandle model)
            : base(handle)
        {
            // Increment the model reference count while this context exists
            _model = model;
            var success = false;
            _model.DangerousAddRef(ref success);
            if (!success)
                throw new RuntimeError("Failed to increment model refcount");
        }

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            NativeApi.llama_free(DangerousGetHandle());
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
            var ctx_ptr = NativeApi.llama_new_context_with_model(model, lparams);
            if (ctx_ptr == IntPtr.Zero)
                throw new RuntimeError("Failed to create context from model");

            return new(ctx_ptr, model);
        }
        #endregion

        /// <summary>
        /// Token logits obtained from the last call to llama_eval()
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
        public int[] Tokenize(string text, bool add_bos, bool special, Encoding encoding)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(text) && !add_bos)
                return Array.Empty<int>();

            // Calculate number of bytes in string, this is a pessimistic estimate of token count. It can't
            // possibly be more than this.
            var count = encoding.GetByteCount(text) + (add_bos ? 1 : 0);

            // "Rent" an array to write results into (avoiding an allocation of a large array)
            var temporaryArray = ArrayPool<int>.Shared.Rent(count);
            try
            {
                // Do the actual conversion
                var n = NativeApi.llama_tokenize(this, text, encoding, temporaryArray, count, add_bos, special);
                if (n < 0)
                {
                    throw new RuntimeError("Error happened during tokenization. It's possibly caused by wrong encoding. Please try to " +
                                           "specify the encoding.");
                }

                // Copy the results from the rented into an array which is exactly the right size
                var result = new int[n];
                Array.ConstrainedCopy(temporaryArray, 0, result, 0, n);

                return result;
            }
            finally
            {
                ArrayPool<int>.Shared.Return(temporaryArray);
            }
        }

        /// <summary>
        /// Convert a single llama token into bytes
        /// </summary>
        /// <param name="token">Token to decode</param>
        /// <param name="dest">A span to attempt to write into. If this is too small nothing will be written</param>
        /// <returns>The size of this token. **nothing will be written** if this is larger than `dest`</returns>
        public int TokenToSpan(int token, Span<byte> dest)
        {
            return ThrowIfDisposed().TokenToSpan(token, dest);
        }
#endregion

        /// <summary>
        /// Run the llama inference to obtain the logits and probabilities for the next token.
        /// </summary>
        /// <param name="tokens">The provided batch of new tokens to process</param>
        /// <param name="n_past">the number of tokens to use from previous eval calls</param>
        /// <returns>Returns true on success</returns>
        [Obsolete("use llama_decode() instead")]
        public bool Eval(ReadOnlySpan<int> tokens, int n_past)
        {
            unsafe
            {
                fixed (int* pinned = tokens)
                {
                    // the entire `eval` system needs replacing with the new batch system!
                    var ret = NativeApi.llama_eval(this, pinned, tokens.Length, n_past);
                    return ret == 0;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="batch"></param>
        /// <returns>Positive return values does not mean a fatal error, but rather a warning:<br />
        ///  - 0: success<br />
        ///  - 1: could not find a KV slot for the batch (try reducing the size of the batch or increase the context)<br />
        ///  - &lt; 0: error<br />
        /// </returns>
        public int Decode(LLamaBatchSafeHandle batch)
        {
            return NativeApi.llama_decode(this, batch.NativeBatch);
        }

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
    }
}
