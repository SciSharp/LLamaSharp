﻿using System;
using System.Buffers;
using System.Runtime.InteropServices;
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
        public int ContextSize => ThrowIfDisposed().ContextSize;

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
            // Decrement refcount on model
            _model?.DangerousRelease();
            _model = null!;

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

        /// <summary>
        /// Create a new llama context with a clone of the current llama context state
        /// </summary>
        /// <param name="lparams"></param>
        /// <returns></returns>
        public SafeLLamaContextHandle Clone(LLamaContextParams lparams)
        {
            // Allocate space to read the state of the current context
            var stateSize = GetStateSize();
            var stateMemory = Marshal.AllocHGlobal((nint)stateSize);
            try
            {
                // Copy state from this context into memory
                GetState(stateMemory, stateSize);

                // Create a new context
                var newCtx = Create(ModelHandle, lparams);

                // Copy state into new context
                newCtx.SetState(stateMemory);

                return newCtx;
            }
            finally
            {
                Marshal.FreeHGlobal(stateMemory);
            }
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
        public bool Eval(ReadOnlySpan<int> tokens, int n_past, int n_threads)
        {
            unsafe
            {
                fixed (int* pinned = tokens)
                {
                    return NativeApi.llama_eval_with_pointer(this, pinned, tokens.Length, n_past, n_threads) == 0;
                }
            }
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
    }
}
