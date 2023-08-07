using System;
using System.Buffers;
using System.Text;
using LLama.Exceptions;

namespace LLama.Native
{
    /// <summary>
    /// A safe wrapper around a llama_context
    /// </summary>
    public class SafeLLamaContextHandle
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
        public int ContextSize => ThrowIfDisposed().ContextSize;

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingCount => ThrowIfDisposed().EmbeddingCount;

        /// <summary>
        /// This field guarantees that a reference to the model is held for as long as this handle is held
        /// </summary>
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
            // Decrement refcount on model
            _model?.DangerousRelease();
            _model = null;

            NativeApi.llama_free(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        private SafeLlamaModelHandle ThrowIfDisposed()
        {
            if (IsClosed)
                throw new ObjectDisposedException("Cannot use this `SafeLLamaContextHandle` - it has been disposed");
            if (_model == null || _model.IsClosed)
                throw new ObjectDisposedException("Cannot use this `SafeLLamaContextHandle` - `SafeLlamaModelHandle` has been disposed");

            return _model;
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
        /// Convert the given text into tokens
        /// </summary>
        /// <param name="text">The text to tokenize</param>
        /// <param name="add_bos">Whether the "BOS" token should be added</param>
        /// <param name="encoding">Encoding to use for the text</param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public int[] Tokenize(string text, bool add_bos, Encoding encoding)
        {
            ThrowIfDisposed();

            // Calculate number of bytes in string, this is a pessimistic estimate of token count. It can't
            // possibly be more than this.
            var count = encoding.GetByteCount(text) + (add_bos ? 1 : 0);

            // "Rent" an array to write results into (avoiding an allocation of a large array)
            var temporaryArray = ArrayPool<int>.Shared.Rent(count);
            try
            {
                // Do the actual conversion
                var n = NativeApi.llama_tokenize(this, text, encoding, temporaryArray, count, add_bos);
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
        /// Token logits obtained from the last call to llama_eval()
        /// The logits for the last token are stored in the last row
        /// Can be mutated in order to change the probabilities of the next token.<br />
        /// Rows: n_tokens<br />
        /// Cols: n_vocab
        /// </summary>
        /// <param name="ctx"></param>
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
        /// Convert a token into a string
        /// </summary>
        /// <param name="token"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string TokenToString(int token, Encoding encoding)
        {
            return ThrowIfDisposed().TokenToString(token, encoding);
        }

        /// <summary>
        /// Convert a token into a span of bytes that could be decoded into a string
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ReadOnlySpan<byte> TokenToSpan(int token)
        {
            return ThrowIfDisposed().TokenToSpan(token);
        }

        /// <summary>
        /// Run the llama inference to obtain the logits and probabilities for the next token.
        /// </summary>
        /// <param name="tokens">The provided batch of new tokens to process</param>
        /// <param name="n_past">the number of tokens to use from previous eval calls</param>
        /// <param name="n_threads"></param>
        /// <returns>Returns true on success</returns>
        public bool Eval(Memory<int> tokens, int n_past, int n_threads)
        {
            using var pin = tokens.Pin();
            unsafe
            {
                return NativeApi.llama_eval_with_pointer(this, (int*)pin.Pointer, tokens.Length, n_past, n_threads) == 0;
            }
        }
    }
}
