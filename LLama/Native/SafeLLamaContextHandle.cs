using System;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// Run the llama inference to obtain the logits and probabilities for the next token.
        /// </summary>
        /// <param name="tokens">The provided batch of new tokens to process</param>
        /// <param name="n_past">the number of tokens to use from previous eval calls</param>
        /// <returns>Returns true on success</returns>
        [Obsolete("use llama_decode() instead")]
        public bool Eval(ReadOnlySpan<LLamaToken> tokens, int n_past)
        {
            unsafe
            {
                fixed (LLamaToken* pinned = tokens)
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
        public int Decode(LLamaBatch batch)
        {
            using (batch.ToNativeBatch(out var nb))
                return NativeApi.llama_decode(this, nb);
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
