﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
        #region Properties
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
        public int LayerCount => llama_n_embd(this);

        private string _modelDescription = null!;
        /// <summary>
        /// Get a description of this model
        /// </summary>
        public string Description
        {
            get
            {
                if (_modelDescription is null)
                {
                    unsafe
                    {
                        // Get description length
                        var size = llama_model_desc(this, null, 0);
                        var buf = new byte[size + 1];
                        fixed (byte* bufPtr = buf)
                        {
                            size = llama_model_desc(this, bufPtr, buf.Length);
                            _modelDescription = Encoding.UTF8.GetString(buf, 0, size) ?? string.Empty;
                        }
                    }
                }

                return _modelDescription;
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
        #endregion

        /// <inheritdoc />
        protected override bool ReleaseHandle()
        {
            llama_free_model(handle);
            return true;
        }

        // TODO: Move this to the model manager?
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
            using (var fs = new FileStream(modelPath, FileMode.Open))
            {
                if (!fs.CanRead)
                {
                    throw new InvalidOperationException($"Model file '{modelPath}' is not readable");
                }
            }

            var handle = llama_load_model_from_file(modelPath, lparams);
            if (handle.IsInvalid)
            {
                throw new LoadWeightsFailedException(modelPath);
            }

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
        /// <param name="path_model"></param>
        /// <param name="params"></param>
        /// <returns>The loaded model, or null on failure.</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern SafeLlamaModelHandle llama_load_model_from_file(string path_model, LLamaModelParams @params);

        /// <summary>
        /// Apply a LoRA adapter to a loaded model
        /// path_base_model is the path to a higher quality model to use as a base for
        /// the layers modified by the adapter. Can be NULL to use the current loaded model.
        /// The model needs to be reloaded before applying a new adapter, otherwise the adapter
        /// will be applied on top of the previous one
        /// </summary>
        /// <param name="model_ptr"></param>
        /// <param name="path_lora"></param>
        /// <param name="scale"></param>
        /// <param name="path_base_model"></param>
        /// <param name="n_threads"></param>
        /// <returns>Returns 0 on success</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_model_apply_lora_from_file(SafeLlamaModelHandle model_ptr, string path_lora, float scale, string? path_base_model, int n_threads);

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
            static extern unsafe int llama_model_meta_key_by_index_native(SafeLlamaModelHandle model, int index, byte* buf, long buf_size);
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
            static extern unsafe int llama_model_meta_val_str_by_index_native(SafeLlamaModelHandle model, int index, byte* buf, long buf_size);
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
            static extern unsafe int llama_model_meta_val_str_native(SafeLlamaModelHandle model, byte* key, byte* buf, long buf_size);
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

        /// <summary>
        /// Get a string describing the model type
        /// </summary>
        /// <param name="model"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <returns>The length of the string on success (even if the buffer is too small)., or -1 on failure</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int llama_model_desc(SafeLlamaModelHandle model, byte* buf, long buf_size);

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
        /// Check if the token is supposed to end generation (end-of-generation, eg. EOS, EOT, etc.)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool llama_token_is_eog(SafeLlamaModelHandle model, LLamaToken token);
        #endregion

        #region LoRA
        /// <summary>
        /// Apply a LoRA adapter to a loaded model
        /// </summary>
        /// <param name="lora"></param>
        /// <param name="scale"></param>
        /// <param name="modelBase">A path to a higher quality model to use as a base for the layers modified by the
        /// adapter. Can be NULL to use the current loaded model.</param>
        /// <param name="threads"></param>
        /// <exception cref="RuntimeError"></exception>
        public void ApplyLoraFromFile(string lora, float scale, string? modelBase = null, int? threads = null)
        {
            // Try to open the model file, this will check:
            // - File exists (automatically throws FileNotFoundException)
            // - File is readable (explicit check)
            // This provides better error messages that llama.cpp, which would throw an access violation exception in both cases.
            using (var fs = new FileStream(lora, FileMode.Open))
                if (!fs.CanRead)
                    throw new InvalidOperationException($"LoRA file '{lora}' is not readable");

            var err = llama_model_apply_lora_from_file(
                this,
                lora,
                scale,
                string.IsNullOrEmpty(modelBase) ? null : modelBase,
                threads ?? Math.Max(1, Environment.ProcessorCount / 2)
            );

            if (err != 0)
                throw new RuntimeError($"Failed to apply lora adapter (err={err}).");
        }
        #endregion

        #region tokenize
        /// <summary>
        /// Convert a single llama token into bytes
        /// </summary>
        /// <param name="token">Token to decode</param>
        /// <param name="dest">A span to attempt to write into. If this is too small nothing will be written</param>
        /// <param name="special">If true, special characters will be converted to text. If false they will be invisible.</param>
        /// <returns>The size of this token. **nothing will be written** if this is larger than `dest`</returns>
        public uint TokenToSpan(LLamaToken token, Span<byte> dest, bool special = false)
        {
            var length = NativeApi.llama_token_to_piece(this, token, dest, special);
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
        /// <param name="add_bos"></param>
        /// <param name="encoding"></param>
        /// <param name="special">Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.</param>
        /// <returns></returns>
        public LLamaToken[] Tokenize(string text, bool add_bos, bool special, Encoding encoding)
        {
            // Early exit if there's no work to do
            if (text == string.Empty && !add_bos)
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
                        var count = -NativeApi.llama_tokenize(this, bytesPtr, bytesCount, (LLamaToken*)IntPtr.Zero, 0, add_bos, special);

                        // Tokenize again, this time outputting into an array of exactly the right size
                        var tokens = new LLamaToken[count];
                        fixed (LLamaToken* tokensPtr = tokens)
                        {
                            _ = NativeApi.llama_tokenize(this, bytesPtr, bytesCount, tokensPtr, count, add_bos, special);
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

            return buffer.AsMemory().Slice(0, keyLength);
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
        }
    }
}
