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
using System.Threading.Tasks;
using LLama.Extensions;
using LLama.Abstractions;
using LLama.Sampling;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace LLama
{
    /// <summary>
    /// A llama_context, which holds all the context required to interact with a model
    /// </summary>
    public sealed class LLamaContext
        : IDisposable
    {
        private readonly ILogger? _logger;

        /// <summary>
        /// Total number of tokens in vocabulary of this model
        /// </summary>
        public int VocabCount => NativeHandle.VocabCount;

        /// <summary>
        /// Total number of tokens in the context
        /// </summary>
        public uint ContextSize => NativeHandle.ContextSize;

        /// <summary>
        /// Dimension of embedding vectors
        /// </summary>
        public int EmbeddingSize => NativeHandle.EmbeddingSize;

        /// <summary>
        /// The context params set for this context
        /// </summary>
        public IContextParams Params { get; }

        /// <summary>
        /// The native handle, which is used to be passed to the native APIs
        /// </summary>
        /// <remarks>Be careful how you use this!</remarks>
        public SafeLLamaContextHandle NativeHandle { get; }

        /// <summary>
        /// The encoding set for this model to deal with text input.
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// Create a new LLamaContext for the given LLamaWeights
        /// </summary>
        /// <param name="model"></param>
        /// <param name="params"></param>
        /// <param name="logger"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        public LLamaContext(LLamaWeights model, IContextParams @params, ILogger? logger = null)
        {
            if (model.NativeHandle.IsClosed)
                throw new ObjectDisposedException("Cannot create context, model weights have been disposed");

            Params = @params;

            _logger = logger;
            Encoding = @params.Encoding;

            @params.ToLlamaContextParams(out var lparams);
            NativeHandle = SafeLLamaContextHandle.Create(model.NativeHandle, lparams);
        }

        /// <summary>
        /// Set the seed for the RNG
        /// </summary>
        /// <param name="seed"></param>
        public void SetSeed(uint seed)
        {
            NativeHandle.SetSeed(seed);
        }

        /// <summary>
        /// Tokenize a string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="addBos">Whether to add a bos to the text.</param>
        /// <param name="special">Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.</param>
        /// <returns></returns>
        public LLamaToken[] Tokenize(string text, bool addBos = true, bool special = false)
        {
            return NativeHandle.Tokenize(text, addBos, special, Encoding);
        }

        /// <summary>
        /// Detokenize the tokens to text.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        [Obsolete("Use a `StreamingTokenDecoder` instead")]
        public string DeTokenize(IReadOnlyList<LLamaToken> tokens)
        {
            // Do **not** use this method as an example of how to correctly use the StreamingTokenDecoder!
            // It should be kept around for the entire time you are decoding one stream of tokens.

            var decoder = new StreamingTokenDecoder(this);
            decoder.AddRange(tokens);
            return decoder.Read();
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
            var estimatedStateSize = (long)NativeApi.llama_get_state_size(NativeHandle);

            // Map the file and write the bytes directly to it. This saves copying the bytes into a C# array
            long writtenBytes;
            using (var file = MemoryMappedFile.CreateFromFile(filename, FileMode.Create, null, estimatedStateSize))
            using (var view = file.CreateViewAccessor(0, estimatedStateSize))
            {
                unsafe
                {
                    byte* ptr = null;
                    view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    writtenBytes = (long)NativeApi.llama_copy_state_data(NativeHandle, ptr);
                    view.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }

            // Truncate the file to the actual size of data that was written
            using (var fileStream = new FileStream(filename, FileMode.Open))
                fileStream.SetLength(writtenBytes);
        }

        /// <summary>
        /// Get the state data as an opaque handle, which can be loaded later using <see cref="LoadState(State)"/>
        /// </summary>
        /// <remarks>Use <see cref="SaveState"/> if you intend to save this state to disk.</remarks>
        /// <returns></returns>
        public State GetState()
        {
            var stateSize = NativeHandle.GetStateSize();

            // Allocate a chunk of memory large enough to hold the entire state
            var memory = Marshal.AllocHGlobal((nint)stateSize);
            try
            {
                // Copy the state data into memory, discover the actual size required
                var actualSize = NativeHandle.GetState(memory, stateSize);

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
                    NativeApi.llama_set_state_data(NativeHandle, ptr);
                    view.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }
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
                NativeHandle.SetState((byte*)state.DangerousGetHandle().ToPointer());
            }
        }

        /// <summary>
        /// Sample a single token from this context, using the given sampling pipeline
        /// </summary>
        /// <param name="pipeline">The pipeline to use to process the logits and to select a token</param>
        /// <param name="lastTokens">The tokens recently returned from the model</param>
        /// <returns>The selected token</returns>
        public LLamaToken Sample(ISamplingPipeline pipeline, ReadOnlySpan<LLamaToken> lastTokens)
        {
            var token = pipeline.Sample(NativeHandle, NativeHandle.GetLogits(), lastTokens);
            pipeline.Accept(NativeHandle, token);
            return token;
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
        /// <param name="minP"></param>
        /// <returns></returns>
        public LLamaToken Sample(LLamaTokenDataArray candidates, ref float? mirostat_mu, float temperature, MirostatType mirostat, 
                                 float mirostatTau, float mirostatEta, int topK, float topP, float tfsZ, float typicalP,
                                 SafeLLamaGrammarHandle? grammar, float minP)
        {
            LLamaToken id;

            if (grammar != null)
            {
                candidates.ApplyGrammar(NativeHandle, grammar);
            }

            if (temperature <= 0)
            {
                // Greedy sampling
                id = candidates.SampleTokenGreedy(NativeHandle);
            }
            else
            {
                var mu = mirostat_mu ?? (2 * mirostatTau);
                {
                    if (mirostat == MirostatType.Mirostat)
                    {
                        const int mirostat_m = 100;
                        candidates.Temperature(NativeHandle, temperature);
                        id = candidates.SampleTokenMirostat(NativeHandle, mirostatTau, mirostatEta, mirostat_m, ref mu);
                    }
                    else if (mirostat == MirostatType.Mirostat2)
                    {
                        candidates.Temperature(NativeHandle, temperature);
                        id = candidates.SampleTokenMirostat2(NativeHandle, mirostatTau, mirostatEta, ref mu);
                    }
                    else
                    {
                        candidates.TopK(NativeHandle, topK);
                        candidates.TailFree(NativeHandle, tfsZ);
                        candidates.LocallyTypical(NativeHandle, typicalP);
                        candidates.TopP(NativeHandle, topP);
                        candidates.MinP(NativeHandle, minP);
                        candidates.Temperature(NativeHandle, temperature);
                        id = candidates.SampleToken(NativeHandle);
                    }
                }
                mirostat_mu = mu;
            }

            grammar?.AcceptToken(NativeHandle, id);

            return id;
        }

        /// <summary>
        /// Apply the penalty for the tokens. Please don't use it unless you fully know what it does.
        /// </summary>
        /// <param name="logits_i"></param>
        /// <param name="lastTokens"></param>
        /// <param name="logitBias"></param>
        /// <param name="repeatLastTokensCount"></param>
        /// <param name="repeatPenalty"></param>
        /// <param name="alphaFrequency"></param>
        /// <param name="alphaPresence"></param>
        /// <param name="penalizeNL"></param>
        /// <returns></returns>
        public LLamaTokenDataArray ApplyPenalty(int logits_i, IEnumerable<LLamaToken> lastTokens, Dictionary<LLamaToken, float>? logitBias = null, 
                                                int repeatLastTokensCount = 64, float repeatPenalty = 1.1f, float alphaFrequency = .0f, float alphaPresence = .0f, 
                                                bool penalizeNL = true)
        {
            var logits = NativeHandle.GetLogitsIth(logits_i);

            // Apply params.logit_bias map
            if (logitBias is not null)
            {
                foreach (var (key, value) in logitBias)
                    logits[(int)key] += value;
            }

            // Save the newline logit value
            var nl_token = NativeApi.llama_token_nl(NativeHandle.ModelHandle);
            var nl_logit = logits[(int)nl_token];

            // Convert logits into token candidates
            var candidates_p = LLamaTokenDataArray.Create(logits);

            // Extract most recently returned tokens
            var last_n_repeat = Math.Min((int)ContextSize, repeatLastTokensCount);
            var last_n_array = lastTokens.TakeLast(last_n_repeat).ToArray();

            // Apply penalties to candidates
            candidates_p.RepetitionPenalty(NativeHandle, last_n_array, repeatPenalty, alphaFrequency, alphaPresence);

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
        /// </summary>
        /// <param name="batch"></param>
        public DecodeResult Decode(LLamaBatch batch)
        {
            if (batch.TokenCount == 0)
                return 0;
            if (batch.TokenCount > Params.BatchSize)
                throw new ArgumentException("Input contains more tokens than configured batch size", nameof(batch));

            return (DecodeResult)NativeHandle.Decode(batch);
        }

        /// <summary>
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="cancellationToken"></param>
        public Task<DecodeResult> DecodeAsync(LLamaBatch batch, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Decode(batch), cancellationToken);
        }
        #endregion

        /// <inheritdoc />
        public void Dispose()
        {
            NativeHandle.Dispose();
        }

        /// <summary>
        /// The state of this model, which can be reloaded later
        /// </summary>
        public class State
            : SafeLLamaHandleBase
        {
            internal State(IntPtr memory)
                : base(memory, true)
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
