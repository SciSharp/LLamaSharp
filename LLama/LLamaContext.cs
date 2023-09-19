using LLama.Exceptions;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using LLama.Common;
using System.Runtime.InteropServices;
using LLama.Extensions;
using LLama.Abstractions;
using Microsoft.Extensions.Logging;

namespace LLama
{
    using llama_token = Int32;

    /// <summary>
    /// A llama_context, which holds all the context required to interact with a model
    /// </summary>
    public sealed class LLamaContext
        : IDisposable
    {
        private readonly ILogger? _logger;
        private readonly Encoding _encoding;
        private readonly SafeLLamaContextHandle _ctx;

        /// <summary>
        /// Total number of tokens in vocabulary of this model
        /// </summary>
        public int VocabCount => _ctx.VocabCount;

        /// <summary>
        /// Total number of tokens in the context
        /// </summary>
        public int ContextSize => _ctx.ContextSize;

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => _ctx.EmbeddingSize;

        /// <summary>
        /// Get the number of tokens in the KV Cache for this context
        /// </summary>
        public int KVCacheTokenCount => _ctx.KVCacheTokenCount;

        /// <summary>
        /// The model params set for this model.
        /// </summary>
        public IModelParams Params { get; set; }

        /// <summary>
        /// The native handle, which is used to be passed to the native APIs
        /// </summary>
        /// <remarks>Be careful how you use this!</remarks>
        public SafeLLamaContextHandle NativeHandle => _ctx;

        /// <summary>
        /// The encoding set for this model to deal with text input.
        /// </summary>
        public Encoding Encoding => _encoding;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="params">Model params.</param>
        /// <param name="logger">The logger.</param>
        [Obsolete("Use the LLamaWeights.CreateContext instead")]
        public LLamaContext(IModelParams @params, ILogger? logger = null)
        {
            Params = @params;

            _logger = logger;
            _encoding = @params.Encoding;

            _logger?.LogInformation($"[LLamaContext] Initializing LLama model with params: {this.Params}");
            _ctx = Utils.InitLLamaContextFromModelParams(Params);
        }

        internal LLamaContext(SafeLLamaContextHandle nativeContext, IModelParams @params, ILogger? logger = null)
        {
            Params = @params;

            _logger = logger;
            _encoding = @params.Encoding;
            _ctx = nativeContext;
        }

        /// <summary>
        /// Create a new LLamaContext for the given LLamaWeights
        /// </summary>
        /// <param name="model"></param>
        /// <param name="params"></param>
        /// <param name="logger"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public LLamaContext(LLamaWeights model, IModelParams @params, ILogger? logger = null)
        {
            if (model.NativeHandle.IsClosed)
                throw new ObjectDisposedException("Cannot create context, model weights have been disposed");

            Params = @params;

            _logger = logger;
            _encoding = @params.Encoding;

            using var pin = @params.ToLlamaContextParams(out var lparams);
            _ctx = SafeLLamaContextHandle.Create(model.NativeHandle, lparams);
        }

        /// <summary>
        /// Create a copy of the current state of this context
        /// </summary>
        /// <returns></returns>
        public LLamaContext Clone()
        {
            using var pin = Params.ToLlamaContextParams(out var lparams);
            var clone = _ctx.Clone(lparams);
            return  new LLamaContext(clone, Params);
        }

        /// <summary>
        /// Tokenize a string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="addBos">Whether to add a bos to the text.</param>
        /// <returns></returns>
        public llama_token[] Tokenize(string text, bool addBos = true)
        {
            return _ctx.Tokenize(text, addBos, _encoding);
        }

        /// <summary>
        /// Detokenize the tokens to text.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public string DeTokenize(IEnumerable<llama_token> tokens)
        {
            var sb = new StringBuilder();
            foreach (var token in tokens)
                _ctx.TokenToString(token, _encoding, sb);

            return sb.ToString();
        }

        /// <summary>
        /// Save the state to specified path.
        /// </summary>
        /// <param name="filename"></param>
        public void SaveState(string filename)
        {
            // Delete that file before overwriting it
            if (File.Exists(filename))
                File.Delete(filename);

            // Estimate size of state to write to disk, this is always equal to or greater than the actual size
            var estimatedStateSize = (long)NativeApi.llama_get_state_size(_ctx);

            // Map the file and write the bytes directly to it. This saves copying the bytes into a C# array
            long writtenBytes;
            using (var file = MemoryMappedFile.CreateFromFile(filename, FileMode.Create, null, estimatedStateSize))
            using (var view = file.CreateViewAccessor(0, estimatedStateSize))
            {
                unsafe
                {
                    byte* ptr = null;
                    view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    writtenBytes = (long)NativeApi.llama_copy_state_data(_ctx, ptr);
                    view.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }

            // Truncate the file to the actual size of data that was written
            using (var fileStream = new FileStream(filename, FileMode.Open))
                fileStream.SetLength(writtenBytes);
        }

        /// <summary>
        /// Get the state data as a byte array.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use `GetState` instead, this supports larger states (over 2GB)")]
        public byte[] GetStateData()
        {
            var stateSize = NativeApi.llama_get_state_size(_ctx);
            byte[] stateMemory = new byte[stateSize];
            NativeApi.llama_copy_state_data(_ctx, stateMemory);
            return stateMemory;
        }

        /// <summary>
        /// Get the state data as an opaque handle
        /// </summary>
        /// <returns></returns>
        public State GetState()
        {
            var stateSize = _ctx.GetStateSize();

            unsafe
            {
                // Allocate a chunk of memory large enough to hold the entire state
                var memory = Marshal.AllocHGlobal((nint)stateSize);
                try
                {
                    // Copy the state data into memory, discover the actual size required
                    var actualSize = _ctx.GetState(memory, stateSize);

                    // Shrink to size
                    memory = Marshal.ReAllocHGlobal(memory, (nint)actualSize);

                    // Wrap memory in a "state"
                    var state = new State(memory);

                    // Set memory to zero, to prevent it being freed in finally block
                    memory = IntPtr.Zero;

                    return state;
                }
                finally
                {
                    if (memory != IntPtr.Zero)
                        Marshal.FreeHGlobal(memory);
                }
            }
        }

        /// <summary>
        /// Load the state from specified path.
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="RuntimeError"></exception>
        public void LoadState(string filename)
        {
            // Map state file into memory and pass that pointer directly to `llama_set_state_data` to load from
            using (var file = MemoryMappedFile.CreateFromFile(filename, FileMode.Open, null))
            using (var view = file.CreateViewAccessor())
            {
                unsafe
                {
                    byte* ptr = null;
                    view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    NativeApi.llama_set_state_data(_ctx, ptr);
                    view.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }
        }

        /// <summary>
        /// Load the state from memory.
        /// </summary>
        /// <param name="stateData"></param>
        /// <exception cref="RuntimeError"></exception>
        public void LoadState(byte[] stateData)
        {
            int stateSize = (int)NativeApi.llama_get_state_size(_ctx);
            if (stateData.Length > stateSize)
            {
                throw new RuntimeError("Failed to validate state size.");
            }
            NativeApi.llama_set_state_data(_ctx, stateData);
        }

        /// <summary>
        /// Load the state from memory.
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="RuntimeError"></exception>
        public void LoadState(State state)
        {
            unsafe
            {
                _ctx.SetState((byte*)state.DangerousGetHandle().ToPointer());
            }
        }

        /// <summary>
        /// Perform the sampling. Please don't use it unless you fully know what it does.
        /// </summary>
        /// <param name="candidates"></param>
        /// <param name="mirostat_mu"></param>
        /// <param name="temperature"></param>
        /// <param name="mirostat"></param>
        /// <param name="mirostatTau"></param>
        /// <param name="mirostatEta"></param>
        /// <param name="topK"></param>
        /// <param name="topP"></param>
        /// <param name="tfsZ"></param>
        /// <param name="typicalP"></param>
        /// <param name="grammar"></param>
        /// <returns></returns>
        public llama_token Sample(LLamaTokenDataArray candidates, ref float? mirostat_mu, float temperature = 0.8f, MirostatType mirostat = MirostatType.Disable, 
                                  float mirostatTau = 5.0f, float mirostatEta = 0.1f, int topK = 40, float topP = 0.95f, float tfsZ = 1.0f, float typicalP = 1.0f,
                                  SafeLLamaGrammarHandle? grammar = null)
        {
            llama_token id;

            if (grammar != null)
            {
                SamplingApi.llama_sample_grammar(_ctx, candidates, grammar);
            }

            if (temperature <= 0)
            {
                // Greedy sampling
                id = SamplingApi.llama_sample_token_greedy(_ctx, candidates);
            }
            else
            {
                var mu = mirostat_mu ?? (2 * mirostatTau);
                {
                    if (mirostat == MirostatType.Mirostat)
                    {
                        const int mirostat_m = 100;
                        SamplingApi.llama_sample_temperature(_ctx, candidates, temperature);
                        id = SamplingApi.llama_sample_token_mirostat(_ctx, candidates, mirostatTau, mirostatEta, mirostat_m, ref mu);
                    }
                    else if (mirostat == MirostatType.Mirostat2)
                    {
                        SamplingApi.llama_sample_temperature(_ctx, candidates, temperature);
                        id = SamplingApi.llama_sample_token_mirostat_v2(_ctx, candidates, mirostatTau, mirostatEta, ref mu);
                    }
                    else
                    {
                        // Temperature sampling
                        SamplingApi.llama_sample_top_k(_ctx, candidates, topK, 1);
                        SamplingApi.llama_sample_tail_free(_ctx, candidates, tfsZ, 1);
                        SamplingApi.llama_sample_typical(_ctx, candidates, typicalP, 1);
                        SamplingApi.llama_sample_top_p(_ctx, candidates, topP, 1);
                        SamplingApi.llama_sample_temperature(_ctx, candidates, temperature);
                        id = SamplingApi.llama_sample_token(_ctx, candidates);
                    }
                }
                mirostat_mu = mu;
            }

            if (grammar != null)
            {
                NativeApi.llama_grammar_accept_token(_ctx, grammar, id);
            }

            return id;
        }

        /// <summary>
        /// Apply the penalty for the tokens. Please don't use it unless you fully know what it does.
        /// </summary>
        /// <param name="lastTokens"></param>
        /// <param name="logitBias"></param>
        /// <param name="repeatLastTokensCount"></param>
        /// <param name="repeatPenalty"></param>
        /// <param name="alphaFrequency"></param>
        /// <param name="alphaPresence"></param>
        /// <param name="penalizeNL"></param>
        /// <returns></returns>
        public LLamaTokenDataArray ApplyPenalty(IEnumerable<llama_token> lastTokens, Dictionary<llama_token, float>? logitBias = null, 
            int repeatLastTokensCount = 64, float repeatPenalty = 1.1f, float alphaFrequency = .0f, float alphaPresence = .0f, 
            bool penalizeNL = true)
        {
            var logits = _ctx.GetLogits();

            // Apply params.logit_bias map
            if (logitBias is not null)
            {
                foreach (var (key, value) in logitBias)
                    logits[key] += value;
            }

            // Save the newline logit value
            var nl_token = NativeApi.llama_token_nl(_ctx);
            var nl_logit = logits[nl_token];

            // Convert logits into token candidates
            var candidates_p = LLamaTokenDataArray.Create(logits);

            // Extract most recently returned tokens
            var last_n_repeat = Math.Min(ContextSize, repeatLastTokensCount);
            var last_n_array = lastTokens.TakeLast(last_n_repeat).ToArray();

            // Apply penalties to candidates
            SamplingApi.llama_sample_repetition_penalty(_ctx, candidates_p, last_n_array, repeatPenalty);
            SamplingApi.llama_sample_frequency_and_presence_penalties(_ctx, candidates_p, last_n_array, alphaFrequency, alphaPresence);

            // Restore newline token logit value if necessary
            if (!penalizeNL)
            {
                var candidatesSpan = candidates_p.data.Span;
                for (var i = 0; i < candidates_p.data.Length; i++)
                {
                    ref var item = ref candidatesSpan[i];
                    if (item.id == nl_token)
                        item.logit = nl_logit;
                }
                candidates_p.sorted = false;
            }

            return candidates_p;
        }

        #region eval overloads
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="pastTokensCount"></param>
        /// <returns>The updated `pastTokensCount`.</returns>
        /// <exception cref="RuntimeError"></exception>
        public int Eval(llama_token[] tokens, llama_token pastTokensCount)
        {
            return Eval(tokens.AsSpan(), pastTokensCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="pastTokensCount"></param>
        /// <returns>The updated `pastTokensCount`.</returns>
        /// <exception cref="RuntimeError"></exception>
        public int Eval(List<llama_token> tokens, llama_token pastTokensCount)
        {
#if NET5_0_OR_GREATER
            var span = CollectionsMarshal.AsSpan(tokens);
            return Eval(span, pastTokensCount);
#else
            // on netstandard2.0 we can't use CollectionsMarshal to get directly at the internal memory of
            // the list. Instead rent an array and copy the data into it. This avoids an allocation, but can't
            // avoid the copying.

            var rented = System.Buffers.ArrayPool<llama_token>.Shared.Rent(tokens.Count);
            try
            {
                tokens.CopyTo(rented, 0);
                return Eval(rented, pastTokensCount);
            }
            finally
            {
                System.Buffers.ArrayPool<llama_token>.Shared.Return(rented);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="pastTokensCount"></param>
        /// <returns>The updated `pastTokensCount`.</returns>
        /// <exception cref="RuntimeError"></exception>
        public int Eval(ReadOnlyMemory<llama_token> tokens, llama_token pastTokensCount)
        {
            return Eval(tokens.Span, pastTokensCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="pastTokensCount"></param>
        /// <returns>The updated `pastTokensCount`.</returns>
        /// <exception cref="RuntimeError"></exception>
        public int Eval(ReadOnlySpan<llama_token> tokens, llama_token pastTokensCount)
        {
            var total = tokens.Length;
            for(var i = 0; i < total; i += Params.BatchSize)
            {
                var n_eval = total - i;
                if (n_eval > Params.BatchSize)
                {
                    n_eval = Params.BatchSize;
                }

                if (!_ctx.Eval(tokens.Slice(i, n_eval), pastTokensCount, Params.Threads))
                {
                    _logger?.LogError($"[LLamaContext] Failed to eval.");
                    throw new RuntimeError("Failed to eval.");
                }

                pastTokensCount += n_eval;
            }
            return pastTokensCount;
        }
#endregion

        /// <summary>
        /// Convert a token into a string
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string TokenToString(llama_token token)
        {
            return NativeHandle.TokenToString(token, Encoding);
        }

        /// <summary>
        /// Append a single token to a string builder
        /// </summary>
        /// <param name="token">Token to decode</param>
        /// <param name="dest">string builder to append the result to</param>
        public void TokenToString(llama_token token, StringBuilder dest)
        {
            NativeHandle.TokenToString(token, Encoding, dest);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _ctx.Dispose();
        }

        /// <summary>
        /// The state of this model, which can be reloaded later
        /// </summary>
        public class State
            : SafeLLamaHandleBase
        {
            internal State(IntPtr memory)
                : base(memory)
            {
            }

            /// <inheritdoc />
            protected override bool ReleaseHandle()
            {
                Marshal.FreeHGlobal(handle);
                return true;
            }
        }
    }
}
