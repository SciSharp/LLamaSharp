using System;
using System.IO;
using LLama.Exceptions;


namespace LLama.Native
{
    /// <summary>
    /// Wrapper to the Multi Modal Weights handle. This wrapper manages the low level
    /// operations.
    /// </summary>
    public sealed class SafeMtmdModelHandle : SafeLLamaHandleBase
    {
        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            mtmd_free(DangerousGetHandle());
            SetHandle(IntPtr.Zero);
            return true;
        }

        /// <summary>
        /// Load a multimodal projection model from disk and bind it to the supplied text model.
        /// </summary>
        /// <param name="modelPath">Path to the MMP (Multi-Modal Projections) file.</param>
        /// <param name="textModel">Text model that provides tokenizer weights for the multimodal helper.</param>
        /// <param name="mtmdCtxParams">Optional context parameters; defaults are used when <c>null</c>.</param>
        /// <returns>Safe handle for the MTMD model.</returns>
        /// <exception cref="InvalidOperationException">The file exists but is not readable by the current process.</exception>
        /// <exception cref="LoadWeightsFailedException">The native loader failed to initialize the MTMD model.</exception>
        public static SafeMtmdModelHandle LoadFromFile(string modelPath, LLamaWeights textModel, MtmdContextParams? mtmdCtxParams)
        {
            // Try to open the model file, this will check:
            // - File exists (automatically throws FileNotFoundException)
            // - File is readable (explicit check)
            // This provides better error messages that llama.cpp, which would throw an access violation exception in both cases.
            using (var fs = new FileStream(modelPath, FileMode.Open, FileAccess.Read))
                if (!fs.CanRead)
                    throw new InvalidOperationException($"Mtmd Model file '{modelPath}' is not readable");

            using var pathUtf8 = PinnedUtf8String.Create(modelPath) ?? throw new ArgumentNullException(nameof(modelPath));

            unsafe
            {
                SafeMtmdModelHandle handle;
                if (mtmdCtxParams is null)
                {
                    var nativeParams = NativeApi.mtmd_context_params_default();
                    handle = mtmd_init_from_file((byte*)pathUtf8.Pointer, textModel.NativeHandle, nativeParams);
                }
                else
                {
                    using var nativeParamsScope = mtmdCtxParams.ToNativeScope();
                    handle = mtmd_init_from_file((byte*)pathUtf8.Pointer, textModel.NativeHandle, nativeParamsScope.Value);
                }

                if (handle.IsInvalid)
                    throw new LoadWeightsFailedException(modelPath);

                return handle;
            }
        }

        /// <summary>
        /// Create a media embedding from disk.
        /// </summary>
        /// <param name="path">Path to the media file on disk.</param>
        /// <returns>Safe handle to the prepared media embedding.</returns>
        /// <exception cref="ObjectDisposedException">The model handle has been disposed.</exception>
        /// <exception cref="RuntimeError">The native loader failed to ingest the file contents.</exception>
        public SafeMtmdEmbed CreateMediaEmbedFromFile(string path)
        {
            EnsureNotDisposed();

            return SafeMtmdEmbed.FromMediaFile(this, path)
                ?? throw new RuntimeError($"Failed to load media '{path}'.");
        }

        /// <summary>
        /// Create a media embedding from an in-memory buffer.
        /// </summary>
        /// <param name="buffer">Binary buffer containing the encoded media data.</param>
        /// <returns>Safe handle to the prepared media embedding.</returns>
        /// <exception cref="ObjectDisposedException">The model handle has been disposed.</exception>
        /// <exception cref="RuntimeError">The native loader failed to ingest the buffer contents.</exception>
        public SafeMtmdEmbed CreateMediaEmbedFromBuffer(ReadOnlySpan<byte> buffer)
        {
            EnsureNotDisposed();

            return SafeMtmdEmbed.FromMediaBuffer(this, buffer)
                ?? throw new RuntimeError("Failed to load media from buffer.");
        }

        /// <summary>
        /// Tokenize a prompt without any media embeddings.
        /// </summary>
        /// <param name="text">Prompt text to tokenize.</param>
        /// <param name="addSpecial">Whether to append special tokens automatically.</param>
        /// <param name="parseSpecial">Whether special tokens should be treated as user-provided text.</param>
        /// <param name="chunks">Receives the native chunk collection when tokenization succeeds.</param>
        /// <returns>Zero on success; otherwise the native mtmd tokenize error code.</returns>
        /// <exception cref="ObjectDisposedException">The model handle has been disposed.</exception>
        public int Tokenize(string text, bool addSpecial, bool parseSpecial, out SafeMtmdInputChunks? chunks)
        {
            return Tokenize(text, addSpecial, parseSpecial, ReadOnlySpan<SafeMtmdEmbed>.Empty, out chunks);
        }

        /// <summary>
        /// Tokenize a prompt alongside the provided media embeddings.
        /// The caller retains ownership of <paramref name="embeds"/>.
        /// </summary>
        /// <param name="text">Prompt text to tokenize.</param>
        /// <param name="addSpecial">Whether to append special tokens automatically.</param>
        /// <param name="parseSpecial">Whether special tokens should be treated as user-provided text.</param>
        /// <param name="embeds">Media embeddings to include in the multimodal prompt.</param>
        /// <param name="chunks">Receives the native chunk collection when tokenization succeeds.</param>
        /// <returns>Zero on success; otherwise the native mtmd tokenize error code.</returns>
        /// <exception cref="ObjectDisposedException">The model handle has been disposed.</exception>
        /// <exception cref="RuntimeError">The native tokenizer failed to allocate output chunks.</exception>
        public int Tokenize(string text, bool addSpecial, bool parseSpecial, ReadOnlySpan<SafeMtmdEmbed> embeds, out SafeMtmdInputChunks? chunks)
        {
            EnsureNotDisposed();

            chunks = null;
            var output = NativeApi.mtmd_input_chunks_init();
            if (output == IntPtr.Zero)
                throw new RuntimeError("Failed to allocate mtmd_input_chunks.");

            var bitmapHandles = new IntPtr[embeds.Length];
            for (var i = 0; i < embeds.Length; i++)
            {
                var embed = embeds[i] ?? throw new ArgumentNullException(nameof(embeds), "Embeds cannot contain null.");
                bitmapHandles[i] = embed.NativePtr;
            }

            var result = NativeApi.mtmd_tokenize(this, output, text, addSpecial, parseSpecial, bitmapHandles, (UIntPtr)bitmapHandles.Length);
            if (result == 0)
            {
                chunks = new SafeMtmdInputChunks(output);
            }
            else
            {
                NativeApi.mtmd_input_chunks_free(output);
            }

            return result;
        }

        /// <summary>
        /// Evaluate a batch of chunks using the helper (mirrors mtmd-helper eval logic).
        /// </summary>
        /// <param name="chunks">Chunk collection produced by <see cref="Tokenize(string, bool, bool, out SafeMtmdInputChunks?)"/>.</param>
        /// <param name="llamaContext">Context handle that receives the evaluated tokens.</param>
        /// <param name="nPast">Number of past tokens; updated when evaluation succeeds.</param>
        /// <param name="seqId">Sequence identifier used for KV cache management.</param>
        /// <param name="nBatch">Maximum number of tokens to evaluate in a single batch.</param>
        /// <param name="logitsLast">Whether to request logits for the last token only.</param>
        /// <returns>Zero on success; otherwise the native helper error code.</returns>
        /// <exception cref="ArgumentNullException">Thrown when required handles are null.</exception>
        public int EvaluateChunks(SafeMtmdInputChunks chunks, SafeLLamaContextHandle llamaContext, ref int nPast, int seqId, int nBatch, bool logitsLast)
        {
            EnsureNotDisposed();

            if (chunks == null)
                throw new ArgumentNullException(nameof(chunks));
            if (llamaContext == null)
                throw new ArgumentNullException(nameof(llamaContext));

            var newNPast = nPast;
            var result = NativeApi.mtmd_helper_eval_chunks(
                this,
                llamaContext,
                chunks,
                nPast,
                seqId,
                nBatch,
                logitsLast,
                ref newNPast);

            if (result == 0)
                nPast = newNPast;

            return result;
        }

        /// <summary>
        /// Evaluate a single chunk helper.
        /// </summary>
        /// <param name="chunkPtr">Pointer to the chunk to evaluate.</param>
        /// <param name="llamaContext">Context handle that receives the evaluated tokens.</param>
        /// <param name="nPast">Number of past tokens; updated when evaluation succeeds.</param>
        /// <param name="seqId">Sequence identifier used for KV cache management.</param>
        /// <param name="nBatch">Maximum number of tokens to evaluate in a single batch.</param>
        /// <param name="logitsLast">Whether to request logits for the last token only.</param>
        /// <returns>Zero on success; otherwise the native helper error code.</returns>
        /// <exception cref="ArgumentNullException">Thrown when required handles are null.</exception>
        public int EvaluateChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, ref int nPast, int seqId, int nBatch, bool logitsLast)
        {
            EnsureNotDisposed();

            if (chunkPtr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(chunkPtr));
            if (llamaContext == null)
                throw new ArgumentNullException(nameof(llamaContext));

            var newNPast = nPast;
            var result = NativeApi.mtmd_helper_eval_chunk_single(
                this,
                llamaContext,
                chunkPtr,
                nPast,
                seqId,
                nBatch,
                logitsLast,
                ref newNPast);

            if (result == 0)
                nPast = newNPast;

            return result;
        }

        /// <summary>
        /// Decode a prepared image chunk whose embedding is already computed.
        /// </summary>
        /// <param name="chunkPtr">Pointer to the chunk whose embedding should be decoded.</param>
        /// <param name="llamaContext">Context handle used for decoding.</param>
        /// <param name="encodedEmbeddings">Pointer to the pre-computed embedding data.</param>
        /// <param name="nPast">Number of past tokens; updated when evaluation succeeds.</param>
        /// <param name="seqId">Sequence identifier used for KV cache management.</param>
        /// <param name="nBatch">Maximum number of tokens to evaluate in a single batch.</param>
        /// <returns>Zero on success; otherwise the native helper error code.</returns>
        /// <exception cref="ArgumentNullException">Thrown when required handles are null.</exception>
        public int DecodeImageChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, IntPtr encodedEmbeddings, ref int nPast, int seqId, int nBatch)
        {
            EnsureNotDisposed();

            if (chunkPtr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(chunkPtr));

            var newNPast = nPast;
            var result = NativeApi.mtmd_helper_decode_image_chunk(
                this,
                llamaContext,
                chunkPtr,
                encodedEmbeddings,
                nPast,
                seqId,
                nBatch,
                ref newNPast);

            if (result == 0)
                nPast = newNPast;

            return result;
        }

        #region native API
        [DllImport(NativeApi.mtmdLibraryName, EntryPoint = "mtmd_init_from_file", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe SafeMtmdModelHandle mtmd_init_from_file(byte* path, SafeLlamaModelHandle textModel, NativeApi.mtmd_context_params ctxParams);

        [DllImport(NativeApi.mtmdLibraryName, EntryPoint = "mtmd_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void mtmd_free(IntPtr ctx);
        #endregion   
        
        /// <summary>
        /// Finalizer to ensure native resources are released if Dispose was not called.
        /// </summary>
        ~SafeMtmdModelHandle()
        {
            Dispose();
        }

        /// <summary>
        /// Indicates whether the model decodes using the non-causal path.
        /// </summary>
        public bool DecodeUseNonCausal() => NativeApi.mtmd_decode_use_non_causal(this);

        /// <summary>
        /// Indicates whether the model decodes using multi-scale RoPE.
        /// </summary>
        public bool DecodeUseMRope() => NativeApi.mtmd_decode_use_mrope(this);

        /// <summary>
        /// Indicates whether the model supports vision inputs.
        /// </summary>
        public bool SupportVision() => NativeApi.mtmd_support_vision(this);

        /// <summary>
        /// Indicates whether the model supports audio inputs.
        /// </summary>
        public bool SupportAudio() => NativeApi.mtmd_support_audio(this);

        /// <summary>
        /// Gets the audio bitrate advertised by the model.
        /// </summary>
        public int GetAudioBitrate() => NativeApi.mtmd_get_audio_bitrate(this);

        private void EnsureNotDisposed()
        {
            if (IsInvalid || IsClosed)
                throw new ObjectDisposedException(nameof(SafeMtmdModelHandle));
        }
    }
}
