using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LLama.Native;

/// <summary>
/// A chain of samplers
/// </summary>
public class SafeLLamaSamplerChainHandle
    : SafeLLamaSamplerHandle
{
    private readonly List<SafeLLamaSamplerHandle> _samplers = [ ];

    /// <summary>
    /// Get the number of samplers in this chain
    /// </summary>
    public int Count
    {
        get
        {
            return llama_sampler_chain_n(this);

            [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
            static extern int llama_sampler_chain_n(SafeLLamaSamplerHandle chain);
        }
    }

    /// <summary>
    /// Add a new sampler to the end of this chain
    /// </summary>
    /// <param name="sampler"></param>
    public void Add(SafeLLamaSamplerHandle sampler)
    {
        // Sanity check that the sampler isn't already in this chain
        lock (_samplers)
            if (_samplers.Contains(sampler))
                throw new ArgumentException("Cannot add a sampler to a chain twice");

        // Add the sampler to the chain
        llama_sampler_chain_add(this, sampler);

        // The chain now owns this sampler resource. Add to the reference counter so it cannot be disposed
        var success = false;
        sampler.DangerousAddRef(ref success);
        Debug.Assert(success);

        // Store a reference to the handle so we can retrieve it later
        lock (_samplers)
        {
            _samplers.Add(sampler);
        }

        // important: this takes ownership of the sampler object and will free it when llama_sampler_free is called on the chain
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern void llama_sampler_chain_add(SafeLLamaSamplerHandle chain, SafeLLamaSamplerHandle smpl);
    }

    /// <summary>
    /// Remove a sampler at the given index in this chain. The sampler will be disposed.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index)
    {
        RemoveAtAndReturn(index).Dispose();
    }

    /// <summary>
    /// Remove a sampler at the given index in this chain and return it. You must dispose the returned handle when you are done with it!
    /// </summary>
    /// <param name="index"></param>
    public SafeLLamaSamplerHandle RemoveAtAndReturn(int index)
    {
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException("Index must be > 0 and <= Count");

        // Remove the sampler from the chain, returning the pointer to this handle
        var ptr = llama_sampler_chain_remove(this, index);

        // Get the handle at that index and return it
        lock (_samplers)
        {
            var sampler = _samplers[index];
            _samplers.RemoveAt(index);
            Debug.Assert(ptr == sampler.DangerousGetHandle());
            return sampler;
        }

        // This is a tricky method to work with!
        // It can't return a handle, because that would create a second handle to these resources!
        // Instead it returns the raw pointer, and that can be looked up in the _samplers dictionary.
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_chain_remove(SafeLLamaSamplerHandle chain, int i);
    }

    /// <summary>
    /// Return the sampler at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public SafeLLamaSamplerHandle Get(int index)
    {
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException("Index must be > 0 and <= Count");

        // Get the pointer
        var ptr = llama_sampler_chain_get(this, index);

        // Find the handle (by looking up it's pointer) and return it.
        lock (_samplers)
        {
            var sampler = _samplers[index];
            Debug.Assert(ptr == sampler.DangerousGetHandle());
            return sampler;
        }
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        // Disposing the chain automatically disposes all sampler stages. Mark all of them as invalid.
        lock (_samplers)
        {
            foreach (var item in _samplers)
                item.SetHandleAsInvalid();
            _samplers.Clear();
        }

        return base.ReleaseHandle();
    }

    /// <inheritdoc />
    public override SafeLLamaSamplerHandle Clone()
    {
        // Create a new handle to own the clone
        var chain = new SafeLLamaSamplerChainHandle();

        // Clone the chain and move ownership across to the handle created above
        var invalidHandle = base.Clone();
        chain.SetHandle(invalidHandle.DangerousGetHandle());
        invalidHandle.SetHandleAsInvalid();
        
        // We've got a handle of the right type, but we still need to copy all the other bits. Cloning the chain created a load
        // of new resources which we don't have any reference to!
        for (var i = 0; i < Count; i++)
        {
            // Create a handle for this resource
            var sampler = new SafeLLamaSamplerHandle(llama_sampler_chain_get(this, i));
            chain._samplers.Add(sampler);

            // Bump the reference count, to account for the fact this sampler is owned by the chain
            var success = false;
            sampler.DangerousAddRef(ref success);
        }

        return chain;
    }

    #region Native API
    // This is a tricky method to work with!
    // It can't return a handle, because that would create a second handle to these resources.
    // Instead It returns the raw pointer, and that can be looked up in the _samplers dictionary.
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr llama_sampler_chain_get(SafeLLamaSamplerChainHandle chain, int i);
    #endregion
}

/// <summary>
/// A sampler modifies logits and may select tokens
/// </summary>
/// <remarks>llama_sampler</remarks>
public class SafeLLamaSamplerHandle
    : SafeLLamaHandleBase
{
    /// <summary>
    /// Get the name of this sampler
    /// </summary>
    public string Name => llama_sampler_name(this);

    /// <summary>
    /// Returns the seed used by the sampler if applicable, LLAMA_DEFAULT_SEED otherwise
    /// </summary>
    public uint Seed => llama_sampler_get_seed(this);

    internal SafeLLamaSamplerHandle(IntPtr ptr)
        : base(ptr, true)
    {
    }

    internal SafeLLamaSamplerHandle()
    {
    }

    #region create samplers
    /// <summary>
    /// Create a new sampler chain
    /// </summary>
    /// <param name="params"></param>
    /// <returns></returns>
    public static SafeLLamaSamplerChainHandle CreateChain(LLamaSamplerChainParams @params)
    {
        return llama_sampler_chain_init(@params);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerChainHandle llama_sampler_chain_init(LLamaSamplerChainParams @params);
    }


    /// <summary>
    /// Create a sampler which picks the most likely token
    /// </summary>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateGreedySampler()
    {
        return llama_sampler_init_greedy();

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_greedy();
    }

    /// <summary>
    /// Create a sampler which picks from the probability distribution of all tokens
    /// </summary>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateDistributionSampler(uint seed)
    {
        return llama_sampler_init_dist(seed);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_dist(uint seed);
    }

    /// <summary>
    /// Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
    /// </summary>
    /// <param name="seed"></param>
    /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
    /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
    /// <param name="m">The number of tokens considered in the estimation of `s_hat`. This is an arbitrary value that is used to calculate `s_hat`, which in turn helps to calculate the value of `k`. In the paper, they use `m = 100`, but you can experiment with different values to see how it affects the performance of the algorithm.</param>
    /// <param name="vocabCount"></param>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateMirostat1Sampler(int vocabCount, uint seed, float tau, float eta, int m)
    {
        return llama_sampler_init_mirostat(vocabCount, seed, tau, eta, m);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_mirostat(int nVocab, uint seed, float tau, float eta, int m);
    }

    /// <summary>
    /// Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
    /// </summary>
    /// <param name="seed"></param>
    /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
    /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateMirostat2Sampler(uint seed, float tau, float eta)
    {
        return llama_sampler_init_mirostat_v2(seed, tau, eta);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_mirostat_v2(uint seed, float tau, float eta);
    }


    /// <summary>
    /// Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
    /// </summary>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateSoftmax()
    {
        return llama_sampler_init_softmax();

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_softmax();
    }

    /// <summary>
    /// Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
    /// </summary>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateTopK(int k)
    {
        return llama_sampler_init_top_k(k);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_top_k(int k);
    }

    /// <summary>
    /// Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
    /// </summary>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateTopP(float p, nint minKeep)
    {
        return llama_sampler_init_top_p(p, minKeep);

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_top_p(float p, nint min_keep);
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Minimum P sampling as described in https://github.com/ggerganov/llama.cpp/pull/3841
    /// </summary>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateMinP(float p, nint minKeep)
    {
        return llama_sampler_init_min_p(p, minKeep);

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_min_p(float p, nint min_keep);
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Minimum P sampling as described in https://github.com/ggerganov/llama.cpp/pull/3841
    /// </summary>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateTailFree(float z, nint minKeep)
    {
        return llama_sampler_init_tail_free(z, minKeep);

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_tail_free(float p, nint min_keep);
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
    /// </summary>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateTypical(float p, nint minKeep)
    {
        return llama_sampler_init_typical(p, minKeep);

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_typical(float p, nint min_keep);
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Apply temperature to the logits
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateTemperature(float t)
    {
        return llama_sampler_init_temp(t);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_temp(float t);
    }

    /// <summary>
    /// Dynamic temperature implementation (a.k.a. entropy) described in the paper https://arxiv.org/abs/2309.02772.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="delta"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateDynamicTemperature(float t, float delta, float exponent)
    {
        return llama_sampler_init_temp_ext(t, delta, exponent);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_temp_ext(float t, float delta, float exponent);
    }

    /// <summary>
    /// Create a sampler which makes tokens impossible unless they match the grammar
    /// </summary>
    /// <param name="model"></param>
    /// <param name="grammar"></param>
    /// <param name="root">Root rule of the grammar</param>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateGrammar(SafeLlamaModelHandle model, string grammar, string root)
    {
        return llama_sampler_init_grammar(model, grammar, root);

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_grammar(SafeLlamaModelHandle model, string grammar_str, string grammar_root);
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Create a sampler that applies various repetition penalties
    /// </summary>
    /// <param name="vocabSize">Vocab size</param>
    /// <param name="eos">EOS token (if this model has one)</param>
    /// <param name="newline">Newline token</param>
    /// <param name="penaltyCount">How many tokens of history to consider when calculating penalties</param>
    /// <param name="repeat">Repetition penalty</param>
    /// <param name="freq">Frequency penalty</param>
    /// <param name="presence">Presence penalty</param>
    /// <param name="penalizeNewline">Whether or not to penalize the newline token</param>
    /// <param name="ignoreEOS">Whether ot not to ignore EOS token</param>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreatePenalties(
        int vocabSize, LLamaToken? eos, LLamaToken newline, int penaltyCount, float repeat, float freq, float presence, bool penalizeNewline, bool ignoreEOS
    )
    {
        return llama_sampler_init_penalties(vocabSize, eos ?? LLamaToken.InvalidToken, newline, penaltyCount, repeat, freq, presence, penalizeNewline, ignoreEOS);

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_init_penalties(
            int n_vocab,         // llama_n_vocab()
            LLamaToken special_eos_id,  // llama_token_eos()
            LLamaToken linefeed_id,     // llama_token_nl()
            int penalty_last_n,  // last n tokens to penalize (0 = disable penalty, -1 = context size)
            float penalty_repeat,  // 1.0 = disabled
            float penalty_freq,    // 0.0 = disabled
            float penalty_present, // 0.0 = disabled
            bool penalize_nl,     // consider newlines as a repeatable token
            bool ignore_eos       // ignore the end-of-sequence token
        );
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Create a sampler that applies a bias directly to the logits
    /// </summary>
    /// <param name="vocabSize"></param>
    /// <param name="biases"></param>
    /// <returns></returns>
    public static SafeLLamaSamplerHandle CreateLogitBias(int vocabSize, Span<LLamaLogitBias> biases)
    {
        unsafe
        {
            fixed (LLamaLogitBias* biasPtr = biases)
            {
                return llama_sampler_init_logit_bias(vocabSize, biases.Length, biasPtr);
            }
        }


        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe SafeLLamaSamplerHandle llama_sampler_init_logit_bias(
            int n_vocab,
            int n_logit_bias,
            LLamaLogitBias* logit_bias);
        // ReSharper restore InconsistentNaming
    }
    #endregion

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        if (handle != IntPtr.Zero)
            llama_sampler_free(handle);
        return true;
    }

    /// <summary>
    /// Create a clone of this sampler
    /// </summary>
    /// <returns></returns>
    public virtual SafeLLamaSamplerHandle Clone()
    {
        return llama_sampler_clone(this);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerHandle llama_sampler_clone(SafeLLamaSamplerHandle chain);
    }

    /// <summary>
    /// Apply this sampler to a set of candidates
    /// </summary>
    /// <param name="candidates"></param>
    public void Apply(ref LLamaTokenDataArrayNative candidates)
    {
        llama_sampler_apply(this, ref candidates);
    }

    /// <summary>
    /// Sample and accept a token from the idx-th output of the last evaluation
    /// </summary>
    /// <param name="context"></param>
    /// <param name="index"></param>
    public LLamaToken Sample(SafeLLamaContextHandle context, int index)
    {
        return llama_sampler_sample(this, context, index);
    }

    /// <summary>
    /// Reset the state of this sampler
    /// </summary>
    public void Reset()
    {
        llama_sampler_reset(this);
    }

    /// <summary>
    /// Accept a token and update the internal state of this sampler
    /// </summary>
    /// <param name="token"></param>
    public void Accept(LLamaToken token)
    {
        llama_sampler_accept(this, token);
    }

    #region Native API
    // ReSharper disable InconsistentNaming
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sampler_free(IntPtr model);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern string llama_sampler_name(SafeLLamaSamplerHandle smpl);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sampler_accept(SafeLLamaSamplerHandle smpl, LLamaToken token);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sampler_apply(SafeLLamaSamplerHandle smpl, ref LLamaTokenDataArrayNative cur_p);

    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sampler_reset(SafeLLamaSamplerHandle smpl);

    /// <summary>
    /// Returns the seed used by the sampler if applicable, LLAMA_DEFAULT_SEED otherwise
    /// </summary>
    /// <param name="smpl"></param>
    /// <returns></returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern uint llama_sampler_get_seed(SafeLLamaSamplerHandle smpl);

    /// <summary>
    /// Sample and accept a token from the idx-th output of the last evaluation
    ///
    /// Shorthand for:
    /// <code>
    ///    const auto * logits = llama_get_logits_ith(ctx, idx);
    ///    llama_token_data_array cur_p = { ... init from logits ... };
    ///    llama_sampler_apply(smpl, cur_p);
    ///    auto token = cur_p.data[cur_p.selected].id;
    ///    llama_sampler_accept(smpl, token);
    ///    return token;
    /// </code>
    /// </summary>
    /// <param name="smpl"></param>
    /// <param name="ctx"></param>
    /// <param name="idx"></param>
    /// <returns>The sampled token</returns>
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern LLamaToken llama_sampler_sample(SafeLLamaSamplerHandle smpl, SafeLLamaContextHandle ctx, int idx);
    // ReSharper restore InconsistentNaming
    #endregion
}

/// <summary>
/// 
/// </summary>
/// <remarks>llama_sampler_chain_params</remarks>
[StructLayout(LayoutKind.Sequential)]
public struct LLamaSamplerChainParams
{
    /// <summary>
    /// whether to measure performance timings
    /// </summary>
    public bool NoPerf
    {
        readonly get => Convert.ToBoolean(_no_perf);
        set => _no_perf = Convert.ToSByte(value);
    }
    private sbyte _no_perf;

    /// <summary>
    /// Get the default LLamaSamplerChainParams
    /// </summary>
    /// <returns></returns>
    public static LLamaSamplerChainParams Default()
    {
        return llama_sampler_chain_default_params();

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern LLamaSamplerChainParams llama_sampler_chain_default_params();
    }
}

/// <summary>
/// A bias to apply directly to a logit
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct LLamaLogitBias
{
    /// <summary>
    /// The token to apply the bias to
    /// </summary>
    public LLamaToken Token;

    /// <summary>
    /// The bias to add
    /// </summary>
    public float Bias;
}

//todo: custom sampler
/*
   // user code can implement the interface below in order to create custom llama_sampler
   struct llama_sampler_i {
       const char *           (*name)  (const SafeLLamaSamplerHandle smpl);                                 // can be NULL
       void                   (*accept)(      SafeLLamaSamplerHandle smpl, llama_token token);              // can be NULL
       void                   (*apply) (      SafeLLamaSamplerHandle smpl, llama_token_data_array * cur_p); // required
       void                   (*reset) (      SafeLLamaSamplerHandle smpl);                                 // can be NULL
       SafeLLamaSamplerHandle (*clone) (const SafeLLamaSamplerHandle smpl);                                 // can be NULL if ctx is NULL
       void                   (*free)  (      SafeLLamaSamplerHandle smpl);                                 // can be NULL if ctx is NULL
   };
 */