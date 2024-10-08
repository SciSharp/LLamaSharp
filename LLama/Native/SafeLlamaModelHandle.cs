using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using LLama.Exceptions;

namespace LLama.Native
{
    /// <summary>
    /// A reference to a set of llama model weights
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global (used implicitly in native API)
    public sealed class SafeLlamaModelHandle
        : SafeLLamaHandleBase
    {
        /// <summary>
        /// Total number of tokens in vocabulary of this model
        /// </summary>
        public int VocabCount => llama_n_vocab(this);

        /// <summary>
        /// Get the vocabulary type for this model
        /// </summary>
        public LLamaVocabType VocabType => llama_vocab_type(this);

        /// <summary>
        /// Get the rope (positional embedding) type for this model
        /// </summary>
        public LLamaRopeType RopeType => llama_rope_type(this);

        /// <summary>
        /// Total number of tokens in the context
        /// </summary>
        public int ContextSize => llama_n_ctx_train(this);

        /// <summary>
        /// Get the rope frequency this model was trained with
        /// </summary>
        public float RopeFrequency => llama_rope_freq_scale_train(this);

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => llama_n_embd(this);

        /// <summary>
        /// Get the size of this model in bytes
        /// </summary>
        public ulong SizeInBytes => llama_model_size(this);

        /// <summary>
        /// Get the number of parameters in this model
        /// </summary>
        public ulong ParameterCount => llama_model_n_params(this);

        /// <summary>
        /// Get the number of layers in this model
        /// </summary>
        public int LayerCount => llama_n_layers(this);

        ///// <summary>
        ///// Get the number of heads in this model
        ///// </summary>
        //public int HeadCount => llama_n_heads(this);

        /// <summary>
        /// Returns true if the model contains an encoder that requires llama_encode() call
        /// </summary>
        public bool HasEncoder => llama_model_has_encoder(this);

        /// <summary>
        /// Returns true if the model contains a decoder that requires llama_decode() call
        /// </summary>
        public bool HasDecoder => llama_model_has_decoder(this);

        /// <summary>
        /// Returns true if the model is recurrent (like Mamba, RWKV, etc.)
        /// </summary>
        public bool IsRecurrent => llama_model_is_recurrent(this);

        /// <summary>
        /// Get a description of this model
        /// </summary>
        public string Description
        {
            get
            {
                unsafe
                {
                    // Get description length
                    var size = llama_model_desc(this, null, 0);
                    var buf = new byte[size + 1];
                    fixed (byte* bufPtr = buf)
                    {
                        size = llama_model_desc(this, bufPtr, buf.Length);
                        return Encoding.UTF8.GetString(buf, 0, size);
                    }
                }
            }
        }

        /// <summary>
        /// Get the number of metadata key/value pairs
        /// </summary>
        /// <returns></returns>
        public int MetadataCount => llama_model_meta_count(this);

        private ModelTokens? _tokens;

        /// <summary>
        /// Get the special tokens of this model
        /// </summary>
        public ModelTokens Tokens => _tokens ??= new ModelTokens(this);

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            llama_free_model(handle);
            return true;
        }

        /// <summary>
        /// Load a model from the given file path into memory
        /// </summary>
        /// <param name="modelPath"></param>
        /// <param name="lparams"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public static SafeLlamaModelHandle LoadFromFile(string modelPath, LLamaModelParams lparams)
        {
            // Try to open the model file, this will check:
            // - File exists (automatically throws FileNotFoundException)
            // - File is readable (explicit check)
            // This provides better error messages that llama.cpp, which would throw an access violation exception in both cases.
            using (var fs = new FileStream(modelPath, FileMode.Open, FileAccess.Read))
                if (!fs.CanRead)
                    throw new InvalidOperationException($"Model file '{modelPath}' is not readable");

            var handle = llama_load_model_from_file(modelPath, lparams);
            if (handle.IsInvalid)
                throw new LoadWeightsFailedException(modelPath);

            return handle;
        }

        #region native API
        static SafeLlamaModelHandle()
        {
            // Ensure that `NativeApi` has been loaded
            NativeApi.llama_empty_call();
        }

        /// <summary>
        /// Load all of the weights of a model into memory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="params"></param>
        /// <returns>The loaded model, or null on failure.</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern SafeLlamaModelHandle llama_load_model_from_file(string path, LLamaModelParams @params);

        /// <summary>
        /// Apply a LoRA adapter to a loaded model
        /// path_base_model is the path to a higher quality model to use as a base for
        /// the layers modified by the adapter. Can be NULL to use the current loaded model.
        /// The model needs to be reloaded before applying a new adapter, otherwise the adapter
        /// will be applied on top of the previous one
        /// </summary>
        /// <param name="model"></param>
        /// <param name="path"></param>
        /// <param name="scale"></param>
        /// <param name="pathBase"></param>
        /// <param name="threads"></param>
        /// <returns>Returns 0 on success</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_model_apply_lora_from_file(SafeLlamaModelHandle model, string path, float scale, string? pathBase, int threads);

        /// <summary>
        /// Frees all allocated memory associated with a model
        /// </summary>
        /// <param name="model"></param>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void llama_free_model(IntPtr model);

        /// <summary>
        /// Get the number of metadata key/value pairs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_model_meta_count(SafeLlamaModelHandle model);

        /// <summary>
        /// Get metadata key name by index
        /// </summary>
        /// <param name="model">Model to fetch from</param>
        /// <param name="index">Index of key to fetch</param>
        /// <param name="dest">buffer to write result into</param>
        /// <returns>The length of the string on success (even if the buffer is too small). -1 is the key does not exist.</returns>
        private static int llama_model_meta_key_by_index(SafeLlamaModelHandle model, int index, Span<byte> dest)
        {
            unsafe
            {
                fixed (byte* destPtr = dest)
                {
                    return llama_model_meta_key_by_index_native(model, index, destPtr, dest.Length);
                }
            }

            [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_model_meta_key_by_index")]
            static extern unsafe int llama_model_meta_key_by_index_native(SafeLlamaModelHandle model, int index, byte* buf, nint bufSize);
        }

        /// <summary>
        /// Get metadata value as a string by index
        /// </summary>
        /// <param name="model">Model to fetch from</param>
        /// <param name="index">Index of val to fetch</param>
        /// <param name="dest">Buffer to write result into</param>
        /// <returns>The length of the string on success (even if the buffer is too small). -1 is the key does not exist.</returns>
        private static int llama_model_meta_val_str_by_index(SafeLlamaModelHandle model, int index, Span<byte> dest)
        {
            unsafe
            {
                fixed (byte* destPtr = dest)
                {
                    return llama_model_meta_val_str_by_index_native(model, index, destPtr, dest.Length);
                }
            }

            [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_model_meta_val_str_by_index")]
            static extern unsafe int llama_model_meta_val_str_by_index_native(SafeLlamaModelHandle model, int index, byte* buf, nint bufSize);
        }

        /// <summary>
        /// Get metadata value as a string by key name
        /// </summary>
        /// <param name="model"></param>
        /// <param name="key"></param>
        /// <param name="dest"></param>
        /// <returns>The length of the string on success, or -1 on failure</returns>
        private static int llama_model_meta_val_str(SafeLlamaModelHandle model, string key, Span<byte> dest)
        {
            var bytesCount = Encoding.UTF8.GetByteCount(key);
            var bytes = ArrayPool<byte>.Shared.Rent(bytesCount);

            unsafe
            {
                fixed (char* keyPtr = key)
                fixed (byte* bytesPtr = bytes)
                fixed (byte* destPtr = dest)
                {
                    // Convert text into bytes
                    Encoding.UTF8.GetBytes(keyPtr, key.Length, bytesPtr, bytesCount);

                    return llama_model_meta_val_str_native(model, bytesPtr, destPtr, dest.Length);
                }
            }

            [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "llama_model_meta_val_str")]
            static extern unsafe int llama_model_meta_val_str_native(SafeLlamaModelHandle model, byte* key, byte* buf, nint bufSize);
        }


        /// <summary>
        /// Get the number of tokens in the model vocabulary
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_n_vocab(SafeLlamaModelHandle model);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaVocabType llama_vocab_type(SafeLlamaModelHandle model);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaRopeType llama_rope_type(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the size of the context window for the model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_n_ctx_train(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the dimension of embedding vectors from this model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_n_embd(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the number of layers in this model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_n_layers(SafeLlamaModelHandle model);

        ///// <summary>
        ///// Get the number of heads in this model
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        //private static extern int llama_n_heads(SafeLlamaModelHandle model);

        /// <summary>
        /// Get a string describing the model type
        /// </summary>
        /// <param name="model"></param>
        /// <param name="buf"></param>
        /// <param name="bufSize"></param>
        /// <returns>The length of the string on success (even if the buffer is too small)., or -1 on failure</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int llama_model_desc(SafeLlamaModelHandle model, byte* buf, nint bufSize);

        /// <summary>
        /// Get the size of the model in bytes
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The size of the model</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong llama_model_size(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the number of parameters in this model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The functions return the length of the string on success, or -1 on failure</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong llama_model_n_params(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the model's RoPE frequency scaling factor
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern float llama_rope_freq_scale_train(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the "Beginning of sentence" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_bos(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the "End of sentence" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_eos(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the "classification" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_cls(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the "sentence separator" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_sep(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the "new line" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_nl(SafeLlamaModelHandle model);

        /// <summary>
        /// Get the "padding" token
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaToken llama_token_pad(SafeLlamaModelHandle model);

        /// <summary>
        /// codellama infill tokens, Beginning of infill prefix
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_token_prefix(SafeLlamaModelHandle model);

        /// <summary>
        /// codellama infill tokens, Beginning of infill middle
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_token_middle(SafeLlamaModelHandle model);

        /// <summary>
        /// codellama infill tokens, Beginning of infill suffix
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_token_suffix(SafeLlamaModelHandle model);

        /// <summary>
        /// codellama infill tokens, End of infill middle
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_token_eot(SafeLlamaModelHandle model);

        /// <summary>
        /// For encoder-decoder models, this function returns id of the token that must be provided
        /// to the decoder to start generating output sequence. For other models, it returns -1.
        /// </summary>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_model_decoder_start_token(SafeLlamaModelHandle model);

        /// <summary>
        /// Check if the token is supposed to end generation (end-of-generation, eg. EOS, EOT, etc.)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool llama_token_is_eog(SafeLlamaModelHandle model, LLamaToken token);

        /// <summary>
        /// Identify if Token Id is a control token or a render-able token
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool llama_token_is_control(SafeLlamaModelHandle model, LLamaToken token);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern LLamaTokenAttr llama_token_get_attr(SafeLlamaModelHandle model, LLamaToken token);

        //[DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        //private static extern GGMLTensor llama_get_model_tensor(SafeLlamaModelHandle model, string name);

        /// <summary>
        /// Returns true if the model contains an encoder that requires llama_encode() call
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool llama_model_has_encoder(SafeLlamaModelHandle model);

        /// <summary>
        /// Returns true if the model contains a decoder that requires llama_decode() call
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool llama_model_has_decoder(SafeLlamaModelHandle model);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr llama_lora_adapter_init(SafeLlamaModelHandle model, string path);

        /// <summary>
        /// Returns true if the model is recurrent (like Mamba, RWKV, etc.)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool llama_model_is_recurrent(SafeLlamaModelHandle model);
        #endregion

        #region LoRA
        /// <summary>
        /// Load a LoRA adapter from file. The adapter will be associated with this model but will not be applied
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public LoraAdapter LoadLoraFromFile(string path)
        {
            path = Path.GetFullPath(path);

            // Try to open the model file, this will check:
            // - File exists (automatically throws FileNotFoundException)
            // - File is readable (explicit check)
            // This provides better error messages that llama.cpp, which would throw an access violation exception in both cases.
            using (var fs = new FileStream(path, FileMode.Open))
                if (!fs.CanRead)
                    throw new InvalidOperationException($"LoRA file '{path}' is not readable");

            var ptr = llama_lora_adapter_init(this, path);
            return new LoraAdapter(this, path, ptr);
        }
        #endregion

        #region tokenize
        /// <summary>
        /// Convert a single llama token into bytes
        /// </summary>
        /// <param name="token">Token to decode</param>
        /// <param name="dest">A span to attempt to write into. If this is too small nothing will be written</param>
        /// <param name="lstrip">User can skip up to 'lstrip' leading spaces before copying (useful when encoding/decoding multiple tokens with 'add_space_prefix')</param>
        /// <param name="special">If true, special characters will be converted to text. If false they will be invisible.</param>
        /// <returns>The size of this token. **nothing will be written** if this is larger than `dest`</returns>
        public uint TokenToSpan(LLamaToken token, Span<byte> dest, int lstrip = 0, bool special = false)
        {
            var length = NativeApi.llama_token_to_piece(this, token, dest, lstrip, special);
            return (uint)Math.Abs(length);
        }

        /// <summary>
        /// Convert a sequence of tokens into characters.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="dest"></param>
        /// <param name="encoding"></param>
        /// <returns>The section of the span which has valid data in it.
        /// If there was insufficient space in the output span this will be
        /// filled with as many characters as possible, starting from the _last_ token.
        /// </returns>
        [Obsolete("Use a StreamingTokenDecoder instead")]
        internal Span<char> TokensToSpan(IReadOnlyList<LLamaToken> tokens, Span<char> dest, Encoding encoding)
        {
            var decoder = new StreamingTokenDecoder(encoding, this);
            decoder.AddRange(tokens);

            var str = decoder.Read();

            if (str.Length < dest.Length)
            {
                str.AsSpan().CopyTo(dest);
                return dest.Slice(0, str.Length);
            }
            else
            {
                str.AsSpan().Slice(str.Length - dest.Length).CopyTo(dest);
                return dest;
            }
        }

        /// <summary>
        /// Convert a string of text into tokens
        /// </summary>
        /// <param name="text"></param>
        /// <param name="addBos"></param>
        /// <param name="encoding"></param>
        /// <param name="special">Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.</param>
        /// <returns></returns>
        public LLamaToken[] Tokenize(string text, bool addBos, bool special, Encoding encoding)
        {
            // Early exit if there's no work to do
            if (text == string.Empty && !addBos)
                return [];

            // Convert string to bytes, adding one extra byte to the end (null terminator)
            var bytesCount = encoding.GetByteCount(text);
            var bytes = ArrayPool<byte>.Shared.Rent(bytesCount + 1);
            try
            {
                unsafe
                {
                    fixed (char* textPtr = text)
                    fixed (byte* bytesPtr = bytes)
                    {
                        // Convert text into bytes
                        encoding.GetBytes(textPtr, text.Length, bytesPtr, bytes.Length);

                        // Tokenize once with no output, to get the token count. Output will be negative (indicating that there was insufficient space)
                        var count = -NativeApi.llama_tokenize(this, bytesPtr, bytesCount, (LLamaToken*)IntPtr.Zero, 0, addBos, special);

                        // Tokenize again, this time outputting into an array of exactly the right size
                        var tokens = new LLamaToken[count];
                        fixed (LLamaToken* tokensPtr = tokens)
                        {
                            _ = NativeApi.llama_tokenize(this, bytesPtr, bytesCount, tokensPtr, count, addBos, special);
                            return tokens;
                        }
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes, true);
            }
        }
        #endregion

        #region context
        /// <summary>
        /// Create a new context for this model
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        public SafeLLamaContextHandle CreateContext(LLamaContextParams @params)
        {
            return SafeLLamaContextHandle.Create(this, @params);
        }
        #endregion

        #region metadata
        /// <summary>
        /// Get the metadata value for the given key
        /// </summary>
        /// <param name="key">The key to fetch</param>
        /// <returns>The value, null if there is no such key</returns>
        public Memory<byte>? MetadataValueByKey(string key)
        {
            // Check if the key exists, without getting any bytes of data
            var keyLength = llama_model_meta_val_str(this, key, []);
            if (keyLength < 0)
                return null;

            // get a buffer large enough to hold it
            var buffer = new byte[keyLength + 1];
            keyLength = llama_model_meta_val_str(this, key, buffer);
            Debug.Assert(keyLength >= 0);

            return buffer.AsMemory().Slice(0,keyLength);
        }

        /// <summary>
        /// Get the metadata key for the given index
        /// </summary>
        /// <param name="index">The index to get</param>
        /// <returns>The key, null if there is no such key or if the buffer was too small</returns>
        public Memory<byte>? MetadataKeyByIndex(int index)
        {
            // Check if the key exists, without getting any bytes of data
            var keyLength = llama_model_meta_key_by_index(this, index, Array.Empty<byte>());
            if (keyLength < 0)
                return null;

            // get a buffer large enough to hold it
            var buffer = new byte[keyLength + 1];
            keyLength = llama_model_meta_key_by_index(this, index, buffer);
            Debug.Assert(keyLength >= 0);

            return buffer.AsMemory().Slice(0, keyLength);
        }

        /// <summary>
        /// Get the metadata value for the given index
        /// </summary>
        /// <param name="index">The index to get</param>
        /// <returns>The value, null if there is no such value or if the buffer was too small</returns>
        public Memory<byte>? MetadataValueByIndex(int index)
        {
            // Check if the key exists, without getting any bytes of data
            var valueLength = llama_model_meta_val_str_by_index(this, index, Array.Empty<byte>());
            if (valueLength < 0)
                return null;

            // get a buffer large enough to hold it
            var buffer = new byte[valueLength + 1];
            valueLength = llama_model_meta_val_str_by_index(this, index, buffer);
            Debug.Assert(valueLength >= 0);

            return buffer.AsMemory().Slice(0, valueLength);
        }

        internal IReadOnlyDictionary<string, string> ReadMetadata()
        {
            var result = new Dictionary<string, string>();

            for (var i = 0; i < MetadataCount; i++)
            {
                var keyBytes = MetadataKeyByIndex(i);
                if (keyBytes == null)
                    continue;
                var key = Encoding.UTF8.GetStringFromSpan(keyBytes.Value.Span);

                var valBytes = MetadataValueByIndex(i);
                if (valBytes == null)
                    continue;
                var val = Encoding.UTF8.GetStringFromSpan(valBytes.Value.Span);

                result[key] = val;
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Get tokens for a model
        /// </summary>
        public sealed class ModelTokens
        {
            private readonly SafeLlamaModelHandle _model;
            private readonly string? _eot;
            private readonly string? _eos;

            internal ModelTokens(SafeLlamaModelHandle model)
            {
                _model = model;
                _eot = LLamaTokenToString(EOT, true);
                _eos = LLamaTokenToString(EOS, true);
            }

            private string? LLamaTokenToString(LLamaToken? token, bool isSpecialToken)
            {
                const int buffSize = 32;
                Span<byte> buff = stackalloc byte[buffSize];
                var tokenLength = _model.TokenToSpan(token ?? LLamaToken.InvalidToken, buff, special: isSpecialToken);
                
                if (tokenLength <= 0)
                {
                    return null;
                }
                
                // if the original buffer wasn't large enough, create a new one
                if (tokenLength > buffSize)
                {
                    buff = stackalloc byte[(int)tokenLength];
                    _ = _model.TokenToSpan(token ?? LLamaToken.InvalidToken, buff, special: isSpecialToken);
                }

                var slice = buff.Slice(0, (int)tokenLength);
                return Encoding.UTF8.GetStringFromSpan(slice);
            }

            private static LLamaToken? Normalize(LLamaToken token)
            {
                return token == -1 ? null : token;
            }

            /// <summary>
            /// Get the Beginning of Sentence token for this model
            /// </summary>
            public LLamaToken? BOS => Normalize(llama_token_bos(_model));

            /// <summary>
            /// Get the End of Sentence token for this model
            /// </summary>
            public LLamaToken? EOS => Normalize(llama_token_eos(_model));
            
            /// <summary>
            /// The textual representation of the end of speech special token for this model
            /// </summary>
            public string? EndOfSpeechToken => _eos;

            /// <summary>
            /// Get the newline token for this model
            /// </summary>
            public LLamaToken? Newline => Normalize(llama_token_nl(_model));

            /// <summary>
            /// Get the newline token for this model
            /// </summary>
            public LLamaToken? Pad => Normalize(llama_token_pad(_model));

            /// <summary>
            /// Get the classification token for this model
            /// </summary>
            public LLamaToken? CLS => Normalize(llama_token_cls(_model));

            /// <summary>
            /// Get the sentence separator token for this model
            /// </summary>
            public LLamaToken? SEP => Normalize(llama_token_sep(_model));

            /// <summary>
            /// Codellama beginning of infill prefix
            /// </summary>
            public LLamaToken? InfillPrefix => Normalize(llama_token_prefix(_model));

            /// <summary>
            /// Codellama beginning of infill middle
            /// </summary>
            public LLamaToken? InfillMiddle => Normalize(llama_token_middle(_model));

            /// <summary>
            /// Codellama beginning of infill suffix
            /// </summary>
            public LLamaToken? InfillSuffix => Normalize(llama_token_suffix(_model));

            /// <summary>
            /// Codellama end of infill middle
            /// </summary>
            public LLamaToken? EOT => Normalize(llama_token_eot(_model));

            /// <summary>
            /// For encoder-decoder models, this function returns id of the token that must be provided
            /// to the decoder to start generating output sequence.
            /// </summary>
            public LLamaToken? DecoderStartToken => Normalize(llama_model_decoder_start_token(_model));

            /// <summary>
            /// Returns the string representation of this model's end_of_text token
            /// </summary>
            public string? EndOfTurnToken => _eot;

            /// <summary>
            /// Check if the given token should end generation
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public bool IsEndOfGeneration(LLamaToken token)
            {
                return llama_token_is_eog(_model, token);
            }

            /// <summary>
            /// Check if the given token should end generation
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public bool IsControl(LLamaToken token)
            {
                return llama_token_is_control(_model, token);
            }

            /// <summary>
            /// Get attributes for the given token
            /// </summary>
            /// <param name="token"></param>
            /// <returns></returns>
            public LLamaTokenAttr GetAttributes(LLamaToken token)
            {
                return llama_token_get_attr(_model, token);
            }
        }
    }
}
