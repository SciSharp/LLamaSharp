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

namespace LLama
{
    using llama_token = Int32;
    /// <summary>
    /// The abstraction of a LLama model, which holds the context in the native library.
    /// </summary>
    public class LLamaModel: IDisposable
    {
        // TODO: expose more properties.
        ILLamaLogger? _logger;
        Encoding _encoding;
        SafeLLamaContextHandle _ctx;
        /// <summary>
        /// The context size.
        /// </summary>
        public int ContextSize { get; }
        /// <summary>
        /// The model params set for this model.
        /// </summary>
        public ModelParams Params { get; set; }
        /// <summary>
        /// The native handle, which is used to be passed to the native APIs. Please avoid using it 
        /// unless you know what is the usage of the Native API.
        /// </summary>
        public SafeLLamaContextHandle NativeHandle => _ctx;
        /// <summary>
        /// The encoding set for this model to deal with text input.
        /// </summary>
        public Encoding Encoding => _encoding;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Params">Model params.</param>
        /// <param name="encoding">Encoding to deal with text input.</param>
        /// <param name="logger">The logger.</param>
        public LLamaModel(ModelParams Params, string encoding = "UTF-8", ILLamaLogger? logger = null)
        {
            _logger = logger;
            this.Params = Params;
            _encoding = Encoding.GetEncoding(encoding);
            _logger?.Log(nameof(LLamaModel), $"Initializing LLama model with params: {this.Params}", ILLamaLogger.LogLevel.Info);
            _ctx = Utils.InitLLamaContextFromModelParams(this.Params);
            ContextSize = NativeApi.llama_n_ctx(_ctx);
        }

        /// <summary>
        /// Tokenize a string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="addBos">Whether to add a bos to the text.</param>
        /// <returns></returns>
        public IEnumerable<llama_token> Tokenize(string text, bool addBos = true)
        {
            // TODO: reconsider whether to convert to array here.
            return Utils.Tokenize(_ctx, text, addBos, _encoding);
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
            {
                sb.Append(Utils.PtrToString(NativeApi.llama_token_to_str(_ctx, token), _encoding));
            }
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
        /// <param name="temperature"></param>
        /// <param name="mirostat"></param>
        /// <param name="mirostatTau"></param>
        /// <param name="mirostatEta"></param>
        /// <param name="topK"></param>
        /// <param name="topP"></param>
        /// <param name="tfsZ"></param>
        /// <param name="typicalP"></param>
        /// <returns></returns>
        public llama_token Sample(LLamaTokenDataArray candidates, float temperature = 0.8f, MirostatType mirostat = MirostatType.Disable, 
            float mirostatTau = 5.0f, float mirostatEta = 0.1f, int topK = 40, float topP = 0.95f, float tfsZ = 1.0f, float typicalP = 1.0f)
        {
            llama_token id = 0;
            if (temperature <= 0)
            {
                // Greedy sampling
                id = SamplingApi.llama_sample_token_greedy(_ctx, candidates);
            }
            else
            {
                if (mirostat == MirostatType.Mirostat)
                {
                    float mirostat_mu = 2.0f * mirostatTau;
                    const int mirostat_m = 100;
                    SamplingApi.llama_sample_temperature(_ctx, candidates, temperature);
                    id = SamplingApi.llama_sample_token_mirostat(_ctx, candidates, mirostatTau, mirostatEta, mirostat_m, ref mirostat_mu);
                }
                else if (mirostat == MirostatType.Mirostat2)
                {
                    float mirostat_mu = 2.0f * mirostatTau;
                    SamplingApi.llama_sample_temperature(_ctx, candidates, temperature);
                    id = SamplingApi.llama_sample_token_mirostat_v2(_ctx, candidates, mirostatTau, mirostatEta, ref mirostat_mu);
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
            var n_vocab = NativeApi.llama_n_vocab(_ctx);
            var logits = Utils.GetLogits(_ctx, n_vocab);

            // Apply params.logit_bias map
            if(logitBias is not null)
            {
                foreach (var (key, value) in logitBias)
                {
                    logits[key] += value;
                }
            }

            var candidates = new List<LLamaTokenData>();
            candidates.Capacity = n_vocab;
            for (llama_token token_id = 0; token_id < n_vocab; token_id++)
            {
                candidates.Add(new LLamaTokenData(token_id, logits[token_id], 0.0f));
            }

            LLamaTokenDataArray candidates_p = new LLamaTokenDataArray(candidates.ToArray(), (ulong)candidates.Count, false);

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

                if(Utils.Eval(_ctx, tokens, i, n_eval, pastTokensCount, Params.Threads) != 0)
                {
                    _logger?.Log(nameof(LLamaModel), "Failed to eval.", ILLamaLogger.LogLevel.Error);
                    throw new RuntimeError("Failed to eval.");
                }

                pastTokensCount += n_eval;
            }
            return pastTokensCount;
        }

        // TODO: add comment
        internal IEnumerable<string> GenerateResult(IEnumerable<llama_token> ids)
        {
            foreach(var id in ids)
            {
                yield return Utils.TokenToString(id, _ctx, _encoding);
            }
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            _ctx.Dispose();
        }

        /// <summary>
        /// The state of this model, which can be reloaded later
        /// </summary>
        public class State
            : SafeHandleZeroOrMinusOneIsInvalid
        {
            internal State(IntPtr memory)
                : base(true)
            {
                SetHandle(memory);
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
