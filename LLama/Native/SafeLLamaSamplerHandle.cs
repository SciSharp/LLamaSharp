using System;

namespace LLama.Native;

/// <summary>
/// A chain of sampler stages that can be used to select tokens from logits.
/// </summary>
/// <remarks>Wraps a handle returned from `llama_sampler_chain_init`. Other samplers are owned by this chain and are never directly exposed.</remarks>
public class SafeLLamaSamplerChainHandle
    : SafeLLamaHandleBase
{
    /// <summary>
    /// Get the number of samplers in this chain
    /// </summary>
    public int Count
    {
        get
        {
            return llama_sampler_chain_n(this);

            [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
            static extern int llama_sampler_chain_n(SafeLLamaSamplerChainHandle chain);
        }
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        if (handle != IntPtr.Zero)
            llama_sampler_free(handle);
        return true;
    }

    /// <summary>
    /// Apply this sampler to a set of candidates
    /// </summary>
    /// <param name="candidates"></param>
    public void Apply(ref LLamaTokenDataArrayNative candidates)
    {
        llama_sampler_apply(this, ref candidates);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        // ReSharper disable once InconsistentNaming
        static extern void llama_sampler_apply(SafeLLamaSamplerChainHandle smpl, ref LLamaTokenDataArrayNative cur_p);
    }

    /// <summary>
    /// Sample and accept a token from the idx-th output of the last evaluation. Shorthand for:
    ///
    /// <code>
    ///    var logits = ctx.GetLogitsIth(idx);
    ///    var token_data_array = LLamaTokenDataArray.Create(logits);
    ///    using LLamaTokenDataArrayNative.Create(token_data_array, out var native_token_data);
    ///    sampler_chain.Apply(native_token_data);
    ///    var token = native_token_data.Data.Span[native_token_data.Selected];
    ///    sampler_chain.Accept(token);
    ///    return token;
    /// </code>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="index"></param>
    public LLamaToken Sample(SafeLLamaContextHandle context, int index)
    {
        return llama_sampler_sample(this, context, index);

        // <summary>
        // Sample and accept a token from the idx-th output of the last evaluation
        //
        // Shorthand for:
        // <code>
        //    const auto * logits = llama_get_logits_ith(ctx, idx);
        //    llama_token_data_array cur_p = { ... init from logits ... };
        //    llama_sampler_apply(smpl, cur_p);
        //    auto token = cur_p.data[cur_p.selected].id;
        //    llama_sampler_accept(smpl, token);
        //    return token;
        // </code>
        // </summary>
        // <param name="smpl"></param>
        // <param name="ctx"></param>
        // <param name="idx"></param>
        // <returns>The sampled token</returns>
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern LLamaToken llama_sampler_sample(SafeLLamaSamplerChainHandle smpl, SafeLLamaContextHandle ctx, int idx);
    }

    /// <summary>
    /// Reset the state of this sampler
    /// </summary>
    public void Reset()
    {
        llama_sampler_reset(this);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern void llama_sampler_reset(SafeLLamaSamplerChainHandle smpl);
    }

    /// <summary>
    /// Accept a token and update the internal state of this sampler
    /// </summary>
    /// <param name="token"></param>
    public void Accept(LLamaToken token)
    {
        llama_sampler_accept(this, token);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern void llama_sampler_accept(SafeLLamaSamplerChainHandle smpl, LLamaToken token);
    }

    /// <summary>
    /// Get the name of the sampler at the given index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string GetName(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        return llama_sampler_name(llama_sampler_chain_get(this, index));

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern string llama_sampler_name(IntPtr smpl);
    }

    /// <summary>
    /// Get the seed of the sampler at the given index if applicable. returns LLAMA_DEFAULT_SEED otherwise
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public uint GetSeed(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        return llama_sampler_get_seed(llama_sampler_chain_get(this, index));

        // Returns the seed used by the sampler if applicable, LLAMA_DEFAULT_SEED otherwise
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern uint llama_sampler_get_seed(IntPtr smpl);
    }

    #region add/remove samplers
    /// <summary>
    /// Create a new sampler chain
    /// </summary>
    /// <param name="params"></param>
    /// <returns></returns>
    public static SafeLLamaSamplerChainHandle Create(LLamaSamplerChainParams @params)
    {
        return llama_sampler_chain_init(@params);

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern SafeLLamaSamplerChainHandle llama_sampler_chain_init(LLamaSamplerChainParams @params);
    }


    /// <summary>
    /// Clone a sampler stage from another chain and add it to this chain
    /// </summary>
    /// <param name="src">The chain to clone a stage from</param>
    /// <param name="index">The index of the stage to clone</param>
    public void AddClone(SafeLLamaSamplerChainHandle src, int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        llama_sampler_chain_add(
            this,
            llama_sampler_clone(
                llama_sampler_chain_get(src, index)
            )
        );

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_clone(IntPtr chain);
    }

    /// <summary>
    /// Remove a sampler stage from this chain
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Remove(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        llama_sampler_free(llama_sampler_chain_remove(this, index));

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_chain_remove(SafeLLamaSamplerChainHandle chain, int i);
    }


    /// <summary>
    /// Add a sampler which picks the most likely token.
    /// </summary>
    /// <returns></returns>
    public void AddGreedySampler()
    {
        llama_sampler_chain_add(this, llama_sampler_init_greedy());

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_greedy();
    }

    /// <summary>
    /// Add a sampler which picks from the probability distribution of all tokens
    /// </summary>
    /// <returns></returns>
    public void AddDistributionSampler(uint seed)
    {
        llama_sampler_chain_add(this, llama_sampler_init_dist(seed));

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_dist(uint seed);
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
    public void AddMirostat1Sampler(int vocabCount, uint seed, float tau, float eta, int m)
    {
        llama_sampler_chain_add(this, llama_sampler_init_mirostat(vocabCount, seed, tau, eta, m));

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_mirostat(int nVocab, uint seed, float tau, float eta, int m);
    }

    /// <summary>
    /// Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
    /// </summary>
    /// <param name="seed"></param>
    /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
    /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
    /// <returns></returns>
    public void AddMirostat2Sampler(uint seed, float tau, float eta)
    {
        llama_sampler_chain_add(this, llama_sampler_init_mirostat_v2(seed, tau, eta));

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_mirostat_v2(uint seed, float tau, float eta);
    }


    /// <summary>
    /// Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
    /// </summary>
    /// <returns></returns>
    public void AddSoftmax()
    {
        llama_sampler_chain_add(this, llama_sampler_init_softmax());

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_softmax();
    }

    /// <summary>
    /// Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
    /// </summary>
    /// <returns></returns>
    public void AddTopK(int k)
    {
        llama_sampler_chain_add(this, llama_sampler_init_top_k(k));

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_top_k(int k);
    }

    /// <summary>
    /// Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
    /// </summary>
    /// <returns></returns>
    public void AddTopP(float p, nint minKeep)
    {
        llama_sampler_chain_add(this, llama_sampler_init_top_p(p, minKeep));

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_top_p(float p, nint min_keep);
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Minimum P sampling as described in https://github.com/ggerganov/llama.cpp/pull/3841
    /// </summary>
    /// <returns></returns>
    public void AddMinP(float p, nint minKeep)
    {
        llama_sampler_chain_add(this, llama_sampler_init_min_p(p, minKeep));

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_min_p(float p, nint min_keep);
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Minimum P sampling as described in https://github.com/ggerganov/llama.cpp/pull/3841
    /// </summary>
    /// <returns></returns>
    public void AddTailFree(float z, nint minKeep)
    {
        llama_sampler_chain_add(this, llama_sampler_init_tail_free(z, minKeep));

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_tail_free(float p, nint min_keep);
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
    /// </summary>
    /// <returns></returns>
    public void AddTypical(float p, nint minKeep)
    {
        llama_sampler_chain_add(this, llama_sampler_init_typical(p, minKeep));

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_typical(float p, nint min_keep);
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Apply temperature to the logits
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public void AddTemperature(float t)
    {
        llama_sampler_chain_add(this, llama_sampler_init_temp(t));

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_temp(float t);
    }

    /// <summary>
    /// Dynamic temperature implementation (a.k.a. entropy) described in the paper https://arxiv.org/abs/2309.02772.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="delta"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public void AddDynamicTemperature(float t, float delta, float exponent)
    {
        llama_sampler_chain_add(this, llama_sampler_init_temp_ext(t, delta, exponent));

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_temp_ext(float t, float delta, float exponent);
    }

    /// <summary>
    /// Create a sampler which makes tokens impossible unless they match the grammar
    /// </summary>
    /// <param name="model"></param>
    /// <param name="grammar"></param>
    /// <param name="root">Root rule of the grammar</param>
    /// <returns></returns>
    public void AddGrammar(SafeLlamaModelHandle model, string grammar, string root)
    {
        llama_sampler_chain_add(this, llama_sampler_init_grammar(model, grammar, root));

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_grammar(SafeLlamaModelHandle model, string grammar_str, string grammar_root);
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
    public void AddPenalties(
        int vocabSize, LLamaToken? eos, LLamaToken newline, int penaltyCount, float repeat, float freq, float presence, bool penalizeNewline, bool ignoreEOS
    )
    {
        llama_sampler_chain_add(this, llama_sampler_init_penalties(vocabSize, eos ?? LLamaToken.InvalidToken, newline, penaltyCount, repeat, freq, presence, penalizeNewline, ignoreEOS));

        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_init_penalties(
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
    public void AddLogitBias(int vocabSize, Span<LLamaLogitBias> biases)
    {
        unsafe
        {
            fixed (LLamaLogitBias* biasPtr = biases)
            {
                llama_sampler_chain_add(this, llama_sampler_init_logit_bias(vocabSize, biases.Length, biasPtr));
            }
        }


        // ReSharper disable InconsistentNaming
        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe IntPtr llama_sampler_init_logit_bias(
            int n_vocab,
            int n_logit_bias,
            LLamaLogitBias* logit_bias);
        // ReSharper restore InconsistentNaming
    }
    #endregion

    #region Native API
    // ReSharper disable InconsistentNaming
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sampler_free(IntPtr model);

    // important: this takes ownership of the sampler object and will free it when llama_sampler_free is called on the chain
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void llama_sampler_chain_add(SafeLLamaSamplerChainHandle chain, IntPtr smpl);

    // This is a tricky method to work with!
    // It can't return a handle, because that would create a second handle to these resources.
    // Instead It returns the raw pointer, and that can be looked up in the _samplers dictionary.
    [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr llama_sampler_chain_get(SafeLLamaSamplerChainHandle chain, int i);
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

/* todo: Custom sampler stuff
/// <summary>
/// 
/// </summary>
/// <remarks>llama_sampler_i</remarks>
public struct LLamaSamplerINative
{
    // Delegate definitions for the function pointers
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate string NameDelegate(IntPtr smpl);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AcceptDelegate(IntPtr smpl, LLamaToken token);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ApplyDelegate(IntPtr smpl, ref LLamaTokenDataArrayNative cur_p);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ResetDelegate(IntPtr smpl);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr CloneDelegate(IntPtr smpl);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FreeDelegate(IntPtr smpl);

    // Struct fields corresponding to function pointers
    public NameDelegate name;
    public AcceptDelegate accept;
    public ApplyDelegate apply;
    public ResetDelegate reset;
    public CloneDelegate clone;
    public FreeDelegate free;
}

/// <summary>
/// 
/// </summary>
/// <remarks>llama_sampler</remarks>
internal unsafe struct LLamaSamplerNative
{
    public LLamaSamplerINative* iface;
    public IntPtr ctx;
}

internal class CustomSamplerWrapper
{
    public GCHandle Handle;
    public ICustomSampler Sampler;
}

/// <summary>
/// A custom sampler stage for modifying logits or selecting a token
/// </summary>
public interface ICustomSampler
{
    /// <summary>
    /// The name of this stage
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Apply this stage to a set of logits
    /// </summary>
    /// <param name="tokenData"></param>
    void Apply(ref LLamaTokenDataArrayNative tokenData);

    /// <summary>
    /// Update the internal state of the sampler when a token is chosen
    /// </summary>
    /// <param name="token"></param>
    void Accept(LLamaToken token);

    /// <summary>
    /// Reset the internal state of this sampler
    /// </summary>
    void Reset();

    /// <summary>
    /// Create a clone of this sampler
    /// </summary>
    ICustomSampler Clone();

    /// <summary>
    /// Free all unmanaged resources held by this sampler
    /// </summary>
    void Free();
}
*/