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
using Microsoft.Win32.SafeHandles;
using LLama.Abstractions;

namespace LLama
{
    using llama_token = Int32;

    /// <summary>
    /// A llama_context, which holds all the context required to interact with a model
    /// </summary>
    public class LLamaContext
        : IDisposable
    {
        private readonly ILLamaLogger? _logger;
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
        /// <param name="encoding">Encoding to deal with text input.</param>
        /// <param name="logger">The logger.</param>
        [Obsolete("Use the LLamaWeights.CreateContext instead")]
        public LLamaContext(IModelParams @params, string encoding = "UTF-8", ILLamaLogger? logger = null)
        {
            Params = @params;

            _logger = logger;
            _encoding = Encoding.GetEncoding(encoding);

            _logger?.Log(nameof(LLamaContext), $"Initializing LLama model with params: {this.Params}", ILLamaLogger.LogLevel.Info);
            _ctx = Utils.InitLLamaContextFromModelParams(Params);
        }

        internal LLamaContext(SafeLLamaContextHandle nativeContext, IModelParams @params, Encoding encoding, ILLamaLogger? logger = null)
        {
            Params = @params;

            _logger = logger;
            _encoding = encoding;
            _ctx = nativeContext;
        }

        /// <summary>
        /// Create a new LLamaContext for the given LLamaWeights
        /// </summary>
        /// <param name="model"></param>
        /// <param name="params"></param>
        /// <param name="encoding"></param>
        /// <param name="logger"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public LLamaContext(LLamaWeights model, IModelParams @params, Encoding encoding, ILLamaLogger? logger = null)
        {
            if (model.NativeHandle.IsClosed)
                throw new ObjectDisposedException("Cannot create context, model weights have been disposed");

            Params = @params;

            _logger = logger;
            _encoding = encoding;

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

            // Create a blank new context for the model
            var ctx = new LLamaContext(SafeLLamaContextHandle.Create(NativeHandle.ModelHandle, lparams), Params, _encoding);

            // Copy across the state
            using var state = GetState();
            ctx.LoadState(state);

            return ctx;
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
            StringBuilder sb = new();
            foreach(var token in tokens)
                sb.Append(_ctx.TokenToString(token, _encoding));
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
            var stateSize = NativeApi.llama_get_state_size(_ctx);

            unsafe
            {
                var bigMemory = Marshal.AllocHGlobal((nint)stateSize);
                var smallMemory = IntPtr.Zero;
                try
                {
                    // Copy the state data into "big memory", discover the actual size required
                    var actualSize = NativeApi.llama_copy_state_data(_ctx, (byte*)bigMemory);

                    // Allocate a smaller buffer
                    smallMemory = Marshal.AllocHGlobal((nint)actualSize);

                    // Copy into the smaller buffer and free the large one to save excess memory usage
                    Buffer.MemoryCopy(bigMemory.ToPointer(), smallMemory.ToPointer(), actualSize, actualSize);
                    Marshal.FreeHGlobal(bigMemory);
                    bigMemory = IntPtr.Zero;

                    return new State(smallMemory);
                }
                catch
                {
                    if (bigMemory != IntPtr.Zero)
                        Marshal.FreeHGlobal(bigMemory);
                    if (smallMemory != IntPtr.Zero)
                        Marshal.FreeHGlobal(smallMemory);
                    throw;
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
                NativeApi.llama_set_state_data(_ctx, (byte*)state.DangerousGetHandle().ToPointer());
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
            var n_vocab = _ctx.VocabCount;
            var logits = _ctx.GetLogits();

            // Apply params.logit_bias map
            if(logitBias is not null)
            {
                foreach (var (key, value) in logitBias)
                {
                    logits[key] += value;
                }
            }

            var candidates = new LLamaTokenData[n_vocab];
            for (llama_token token_id = 0; token_id < n_vocab; token_id++)
                candidates[token_id] = new LLamaTokenData(token_id, logits[token_id], 0.0f);
            LLamaTokenDataArray candidates_p = new LLamaTokenDataArray(candidates);

            // Apply penalties
            float nl_logit = logits[NativeApi.llama_token_nl()];
            int lastTokensCount = lastTokens.Count();
            var last_n_repeat = Math.Min(Math.Min(lastTokensCount, repeatLastTokensCount), ContextSize);
            SamplingApi.llama_sample_repetition_penalty(_ctx, candidates_p,
                lastTokens.Skip(lastTokensCount - last_n_repeat).ToArray(),
                (ulong)last_n_repeat, repeatPenalty);
            SamplingApi.llama_sample_frequency_and_presence_penalties(_ctx, candidates_p,
                lastTokens.Skip(lastTokensCount - last_n_repeat).ToArray(),
                (ulong)last_n_repeat, alphaFrequency, alphaPresence);
            if (!penalizeNL)
            {
                logits[NativeApi.llama_token_nl()] = nl_logit;
            }

            return candidates_p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="pastTokensCount"></param>
        /// <returns>The updated `pastTokensCount`.</returns>
        /// <exception cref="RuntimeError"></exception>
        public llama_token Eval(llama_token[] tokens, llama_token pastTokensCount)
        {
            int total = tokens.Length;
            for(int i = 0; i < total; i += Params.BatchSize)
            {
                int n_eval = total - i;
                if(n_eval > Params.BatchSize)
                {
                    n_eval = Params.BatchSize;
                }

                if (!_ctx.Eval(tokens.AsMemory(i, n_eval), pastTokensCount, Params.Threads))
                {
                    _logger?.Log(nameof(LLamaContext), "Failed to eval.", ILLamaLogger.LogLevel.Error);
                    throw new RuntimeError("Failed to eval.");
                }

                pastTokensCount += n_eval;
            }
            return pastTokensCount;
        }

        internal IEnumerable<string> GenerateResult(IEnumerable<llama_token> ids)
        {
            foreach(var id in ids)
                yield return _ctx.TokenToString(id, _encoding);
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);

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
