using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace LLama.Native
{
    /// <summary>
    /// Contains an array of LLamaTokenData, potentially sorted.
    /// </summary>
    public struct LLamaTokenDataArray
    {
        /// <summary>
        /// The LLamaTokenData
        /// </summary>
        public readonly Memory<LLamaTokenData> Data;

        /// <summary>
        /// Indicates if `data` is sorted by logits in descending order. If this is false the token data is in _no particular order_.
        /// </summary>
        public bool Sorted;

        /// <summary>
        /// Create a new LLamaTokenDataArray
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="isSorted"></param>
        public LLamaTokenDataArray(Memory<LLamaTokenData> tokens, bool isSorted = false)
        {
            Data = tokens;
            Sorted = isSorted;
        }

        /// <summary>
        /// Create a new LLamaTokenDataArray, copying the data from the given logits
        /// </summary>
        /// <param name="logits"></param>
        /// <returns></returns>
        public static LLamaTokenDataArray Create(ReadOnlySpan<float> logits)
        {
            return Create(logits, new LLamaTokenData[logits.Length]);
        }

        /// <summary>
        /// Create a new LLamaTokenDataArray, copying the data from the given logits into temporary memory.
        /// </summary>
        /// <remarks>The memory must not be modified while this <see cref="LLamaTokenDataArray"/> is in use.</remarks>
        /// <param name="logits"></param>
        /// <param name="buffer">Temporary memory which will be used to work on these logits. Must be at least as large as logits array</param>
        /// <returns></returns>
        public static LLamaTokenDataArray Create(ReadOnlySpan<float> logits, Memory<LLamaTokenData> buffer)
        {
            if (buffer.Length < logits.Length)
                throw new ArgumentException("temporary memory is shorter than logits span");

            // take a slice of the output buffer which is exactly the size we need.
            var candidates = buffer.Slice(0, logits.Length);
            var candidatesSpan = candidates.Span;

            for (var token = 0; token < logits.Length; token++)
                candidatesSpan[token] = new LLamaTokenData(token, logits[token], 0.0f);
            
            return new LLamaTokenDataArray(candidates);
        }
        
        /// <summary>
        /// Overwrite the logit values for all given tokens
        /// </summary>
        /// <param name="values">tuples of token and logit value to overwrite</param>
        public void OverwriteLogits(ReadOnlySpan<(LLamaToken token, float logit)> values)
        {
            if (values.Length == 0)
                return;

            var dataSpan = Data.Span;
            foreach (var (token, value) in values)
            {
                for (var i = 0; i < Data.Length; i++)
                {
                    if (dataSpan[i].id == token)
                    {
                        dataSpan[i].logit = value;
                        break;
                    }
                }   
            }
            Sorted = false;
        }

        #region sampling
        /// <summary>
        /// Apply grammar rules to candidate tokens
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="grammar"></param>
        public void ApplyGrammar(SafeLLamaContextHandle ctx, SafeLLamaGrammarHandle? grammar)
        {
            if (grammar == null)
                return;

            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                NativeApi.llama_sample_grammar(ctx, ref st, grammar);
                Sorted = st.sorted;
            }
        }

        /// <summary>
        /// Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
        /// </summary>
        /// <param name="context"></param>
        /// <param name="k">Number of tokens to keep</param>
        /// <param name="minKeep">Minimum number to keep</param>
        public void TopK(SafeLLamaContextHandle context, int k, ulong minKeep = 1)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                NativeApi.llama_sample_top_k(context, ref st, k, minKeep);
                Sorted = st.sorted;
            }
        }

        /// <summary>
        /// Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
        /// </summary>
        /// <param name="context"></param>
        /// <param name="p"></param>
        /// <param name="minKeep"></param>
        public void TopP(SafeLLamaContextHandle context, float p, ulong minKeep = 1)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                NativeApi.llama_sample_top_p(context, ref st, p, minKeep);
                Sorted = st.sorted;
            }
        }

        /// <summary>
        /// Minimum P sampling as described in https://github.com/ggerganov/llama.cpp/pull/3841
        /// </summary>
        /// <param name="context"></param>
        /// <param name="p">All tokens with probability greater than this will be kept</param>
        /// <param name="minKeep"></param>
        public void MinP(SafeLLamaContextHandle context, float p, ulong minKeep = 1)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                NativeApi.llama_sample_min_p(context, ref st, p, minKeep);
                Sorted = st.sorted;
            }
        }

        /// <summary>
        /// Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="z"></param>
        /// <param name="minKeep"></param>
        public void TailFree(SafeLLamaContextHandle context, float z, ulong minKeep = 1)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                NativeApi.llama_sample_tail_free(context, ref st, z, minKeep);
                Sorted = st.sorted;
            }
        }

        /// <summary>
        /// Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="p"></param>
        /// <param name="minKeep"></param>
        public void LocallyTypical(SafeLLamaContextHandle context, float p, ulong minKeep = 1)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                NativeApi.llama_sample_typical(context, ref st, p, minKeep);
                Sorted = st.sorted;
            }
        }

        /// <summary>
        /// Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
        /// Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="lastTokens"></param>
        /// <param name="penaltyRepeat"></param>
        /// <param name="penaltyFreq"></param>
        /// <param name="penaltyPresent"></param>
        public void RepetitionPenalty(SafeLLamaContextHandle context, ReadOnlySpan<LLamaToken> lastTokens, float penaltyRepeat, float penaltyFreq, float penaltyPresent)
        {
            unsafe
            {
                using (LLamaTokenDataArrayNative.Create(this, out var st))
                {
                    fixed (LLamaToken* lastTokensHandle = lastTokens)
                    {
                        NativeApi.llama_sample_repetition_penalties(context, ref st, lastTokensHandle, (ulong)lastTokens.Length, penaltyRepeat, penaltyFreq, penaltyPresent);
                        Sorted = st.sorted;
                    }
                }
            }
        }

        /// <summary>
        /// Apply classifier-free guidance to the logits as described in academic paper "Stay on topic with Classifier-Free Guidance" https://arxiv.org/abs/2306.17806
        /// </summary>
        /// <param name="context"></param>
        /// <param name="guidanceLogits">Logits extracted from a separate context from the same model.
        /// Other than a negative prompt at the beginning, it should have all generated and user input tokens copied from the main context.</param>
        /// <param name="guidance">Guidance strength. 0 means no guidance, higher values applies stronger guidance</param>
        public void Guidance(SafeLLamaContextHandle context, ReadOnlySpan<float> guidanceLogits, float guidance)
        {
            if (guidanceLogits.Length != Data.Length)
                throw new ArgumentException("Guidance logits count must equal vocabulary size", nameof(guidanceLogits));

            if (guidance < 0)
                throw new ArgumentOutOfRangeException(nameof(guidance), "Guidance strength must be greater than or equal to zero");

            // this method accepts 0 (no guidance), higher means more. llama.cpp expects 1 (no guidance), higher means more
            // Add one to move up to the llama.cpp baseline.
            guidance += 1;

            // We need logits array, which we don't have at this point.
            // Copy them to a temporary array, apply guidance, then copy them back.
            var logits = ArrayPool<float>.Shared.Rent(context.VocabCount);
            try
            {
                // Copy logits into a temporary array
                for (var i = 0; i < Data.Length; i++)
                {
                    ref var item = ref Data.Span[i];
                    logits[(int)item.id] = item.logit;
                }

                // Apply guidance
                NativeApi.llama_sample_apply_guidance(context, logits.AsSpan(0, context.VocabCount), guidanceLogits, guidance);

                // Copy logits back into data array
                for (var i = 0; i < Data.Length; i++)
                {
                    ref var item = ref Data.Span[i];
                    item.logit = logits[(int)item.id];
                }

                // No longer sorted since we just mutated logits!
                Sorted = false;
            }
            finally
            {
                ArrayPool<float>.Shared.Return(logits);
            }
        }

        /// <summary>
        /// Sample with temperature.
        /// As temperature increases, the prediction becomes more diverse but also vulnerable to hallucinations -- generating tokens that are sensible but not factual
        /// </summary>
        /// <param name="context"></param>
        /// <param name="temp"></param>
        public void Temperature(SafeLLamaContextHandle context, float temp)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                NativeApi.llama_sample_temp(context, ref st, temp);
                Sorted = st.sorted;
            }
        }

        /// <summary>
        /// Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
        /// </summary>
        /// <param name="context"></param>
        public void Softmax(SafeLLamaContextHandle context)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                NativeApi.llama_sample_softmax(context, ref st);
                Sorted = st.sorted;
            }
        }

        /// <summary>
        /// Randomly selects a token from the candidates based on their probabilities.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public LLamaToken SampleToken(SafeLLamaContextHandle context)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                var token = NativeApi.llama_sample_token(context, ref st);
                Sorted = st.sorted;
                return token;
            }
        }

        /// <summary>
        /// Selects the token with the highest probability.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public LLamaToken SampleTokenGreedy(SafeLLamaContextHandle context)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                var token = NativeApi.llama_sample_token_greedy(context, ref st);
                Sorted = st.sorted;
                return token;
            }
        }

        /// <summary>
        /// Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
        /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
        /// <param name="m">The number of tokens considered in the estimation of `s_hat`. This is an arbitrary value that is used to calculate `s_hat`, which in turn helps to calculate the value of `k`. In the paper, they use `m = 100`, but you can experiment with different values to see how it affects the performance of the algorithm.</param>
        /// <param name="mu">Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.</param>
        /// <returns></returns>
        public LLamaToken SampleTokenMirostat(SafeLLamaContextHandle context, float tau, float eta, int m, ref float mu)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                var token = NativeApi.llama_sample_token_mirostat(context, ref st, tau, eta, m, ref mu);
                Sorted = st.sorted;
                return token;
            }
        }

        /// <summary>
        /// Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
        /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
        /// <param name="mu">Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.</param>
        /// <returns></returns>
        public LLamaToken SampleTokenMirostat2(SafeLLamaContextHandle context, float tau, float eta, ref float mu)
        {
            using (LLamaTokenDataArrayNative.Create(this, out var st))
            {
                var token = NativeApi.llama_sample_token_mirostat_v2(context, ref st, tau, eta, ref mu);
                Sorted = st.sorted;
                return token;
            }
        }
        #endregion
    }

    /// <summary>
    /// Contains a pointer to an array of LLamaTokenData which is pinned in memory.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LLamaTokenDataArrayNative
    {
        /// <summary>
        /// A pointer to an array of LlamaTokenData
        /// </summary>
        /// <remarks>Memory must be pinned in place for all the time this LLamaTokenDataArrayNative is in use (i.e. `fixed` or `.Pin()`)</remarks>
        private unsafe LLamaTokenData* _data;

        /// <summary>
        /// Number of LLamaTokenData in the array
        /// </summary>
        public ulong size;
        
        /// <summary>
        /// A pointer to an array of LlamaTokenData
        /// </summary>
        public Span<LLamaTokenData> data
        {
            get
            {
                unsafe
                {
                    return new Span<LLamaTokenData>(_data, checked((int)size));
                }
            }
        }
        
        /// <summary>
        /// Indicates if the items in the array are sorted
        /// </summary>
        public bool sorted
        {
            get => Convert.ToBoolean(_sorted);
            set => _sorted = Convert.ToSByte(value);
        }
        private sbyte _sorted;

        /// <summary>
        /// Create a new LLamaTokenDataArrayNative around the data in the LLamaTokenDataArray 
        /// </summary>
        /// <param name="array">Data source</param>
        /// <param name="native">Created native array</param>
        /// <returns>A memory handle, pinning the data in place until disposed</returns>
        public static MemoryHandle Create(LLamaTokenDataArray array, out LLamaTokenDataArrayNative native)
        {
            var handle = array.Data.Pin();

            unsafe
            {
                native = new LLamaTokenDataArrayNative
                {
                    _data = (LLamaTokenData*)handle.Pointer,
                    size = (ulong)array.Data.Length,
                    sorted = array.Sorted
                };
            }

            return handle;
        }
    }
}
