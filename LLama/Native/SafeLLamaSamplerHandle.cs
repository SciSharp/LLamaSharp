using System;
using System.Runtime.CompilerServices;
using System.Text;

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

        return Marshal.PtrToStringAnsi(llama_sampler_name(llama_sampler_chain_get(this, index))) ?? "Unknown Name";

        [DllImport(NativeApi.libraryName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr llama_sampler_name(IntPtr smpl);
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
        if (index < 0 || index >= src.Count)
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
    /// Add a custom sampler stage
    /// </summary>
    /// <typeparam name="TSampler"></typeparam>
    /// <param name="sampler"></param>
    public void AddCustom<TSampler>(TSampler sampler)
        where TSampler : class, ICustomSampler
    {
        unsafe
        {
            var samplerHandle = CustomSamplerHandle.Create(sampler);
            llama_sampler_chain_add(this, (IntPtr)samplerHandle.GetLLamaSamplerPointer());
        }
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
    /// <param name="ignoreEOS">Whether or not to ignore EOS token</param>
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

/// <summary>
/// 
/// </summary>
/// <remarks>llama_sampler_i</remarks>
[StructLayout(LayoutKind.Sequential)]
internal struct LLamaSamplerINative
{
    /// <summary>
    /// Get the name of this sampler
    /// </summary>
    /// <param name="smpl"></param>
    /// <returns></returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate byte* NameDelegate(ref LLamaSamplerNative smpl);

    /// <summary>
    /// Update internal sampler state after a token has been chosen
    /// </summary>
    /// <param name="smpl"></param>
    /// <param name="token"></param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AcceptDelegate(ref LLamaSamplerNative smpl, LLamaToken token);

    /// <summary>
    /// Apply this sampler to a set of logits
    /// </summary>
    /// <param name="smpl"></param>
    /// <param name="cur_p"></param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ApplyDelegate(ref LLamaSamplerNative smpl, ref LLamaTokenDataArrayNative cur_p);

    /// <summary>
    /// Reset the internal state of this sampler
    /// </summary>
    /// <param name="smpl"></param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ResetDelegate(ref LLamaSamplerNative smpl);

    /// <summary>
    /// Create a clone of this sampler
    /// </summary>
    /// <param name="smpl"></param>
    /// <returns></returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate LLamaSamplerNative* CloneDelegate(ref LLamaSamplerNative smpl);

    /// <summary>
    /// Free all resources held by this sampler
    /// </summary>
    /// <param name="smpl"></param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FreeDelegate(ref LLamaSamplerNative smpl);

    public unsafe delegate*<byte*> Name;
    public unsafe delegate*<LLamaSamplerNative*, LLamaToken, void> Accept;
    public unsafe delegate*<LLamaSamplerNative*, LLamaTokenDataArrayNative*, void> Apply;
    public unsafe delegate*<LLamaSamplerNative*, void> Reset;
    public unsafe delegate*<LLamaSamplerNative*, IntPtr> Clone;
    public unsafe delegate*<LLamaSamplerNative*, void> Free;
}

/// <summary>
/// 
/// </summary>
/// <remarks>llama_sampler</remarks>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct LLamaSamplerNative
{
    /// <summary>
    /// Holds the function pointers which make up the actual sampler
    /// </summary>
    public LLamaSamplerINative* Interface;

    /// <summary>
    /// Any additional context this sampler needs, may be anything. We will use it
    /// to hold a GCHandle.
    /// </summary>
    public IntPtr Context;
}

internal class CustomSamplerHandle
{
    /// <summary>
    /// This GCHandle roots this object, preventing it from being freed.
    /// </summary>
    private GCHandle _gcHandle;

    /// <summary>
    /// A reference to the user code which implements the custom sampler
    /// </summary>
    private readonly ICustomSampler _sampler;

    private unsafe LLamaSamplerNative* _samplerNativePtr;
    private unsafe LLamaSamplerINative* _samplerNativeInterfacePtr;
    private unsafe byte* _samplerNamePtr;

    private CustomSamplerHandle(ICustomSampler sampler)
    {
        _sampler = sampler;
    }

    public static CustomSamplerHandle Create(ICustomSampler sampler)
    {
        var nameArr = Encoding.UTF8.GetBytes(sampler.Name + '\0');

        var handle = new CustomSamplerHandle(sampler);
        handle._gcHandle = GCHandle.Alloc(handle);

        unsafe
        {
            // Allocate space for a `LLamaSamplerINative` struct. So we can pass pointers to it.
            handle._samplerNativeInterfacePtr = (LLamaSamplerINative*)Marshal.AllocHGlobal(sizeof(LLamaSamplerINative));
            handle._samplerNativeInterfacePtr->Name = (delegate*<byte*>)Marshal.GetFunctionPointerForDelegate<LLamaSamplerINative.NameDelegate>(Name);
            handle._samplerNativeInterfacePtr->Accept = (delegate*<LLamaSamplerNative*, LLamaToken, void>)Marshal.GetFunctionPointerForDelegate<LLamaSamplerINative.AcceptDelegate>(Accept);
            handle._samplerNativeInterfacePtr->Apply = (delegate*<LLamaSamplerNative*, LLamaTokenDataArrayNative*, void>)Marshal.GetFunctionPointerForDelegate<LLamaSamplerINative.ApplyDelegate>(Apply);
            handle._samplerNativeInterfacePtr->Reset = (delegate*<LLamaSamplerNative*, void>)Marshal.GetFunctionPointerForDelegate<LLamaSamplerINative.ResetDelegate>(Reset);
            handle._samplerNativeInterfacePtr->Clone = (delegate*<LLamaSamplerNative*, IntPtr>)Marshal.GetFunctionPointerForDelegate<LLamaSamplerINative.CloneDelegate>(Clone);
            handle._samplerNativeInterfacePtr->Free = (delegate*<LLamaSamplerNative*, void>)Marshal.GetFunctionPointerForDelegate<LLamaSamplerINative.FreeDelegate>(Free);

            // Allocate space for a `LLamaSamplerNative` struct. So we can pass pointers to it.
            handle._samplerNativePtr = (LLamaSamplerNative*)Marshal.AllocHGlobal(sizeof(LLamaSamplerNative));
            handle._samplerNativePtr->Context = (IntPtr)handle._gcHandle;
            handle._samplerNativePtr->Interface = handle._samplerNativeInterfacePtr;

            // Allocate space for the name string
            handle._samplerNamePtr = (byte*)Marshal.AllocHGlobal(nameArr.Length);
            nameArr.AsSpan().CopyTo(new Span<byte>(handle._samplerNamePtr, nameArr.Length));
        }

        return handle;
    }

    /// <summary>
    /// Get a pointer to a `llama_sampler` (LLamaSamplerNative) struct, suitable for passing to `llama_sampler_chain_add`
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public unsafe LLamaSamplerNative* GetLLamaSamplerPointer()
    {
        return _samplerNativePtr;
    }

    private static CustomSamplerHandle GetSampler(ref LLamaSamplerNative smpl)
    {
        return (CustomSamplerHandle)GCHandle.FromIntPtr(smpl.Context).Target!;
    }

    private static unsafe byte* Name(ref LLamaSamplerNative smpl)
    {
        return GetSampler(ref smpl)._samplerNamePtr;
    }

    private static void Accept(ref LLamaSamplerNative smpl, LLamaToken token)
    {
        GetSampler(ref smpl)._sampler.Accept(token);
    }

    private static void Apply(ref LLamaSamplerNative smpl, ref LLamaTokenDataArrayNative candidates)
    {
        GetSampler(ref smpl)._sampler.Apply(ref candidates);
    }

    private static void Reset(ref LLamaSamplerNative smpl)
    {
        GetSampler(ref smpl)._sampler.Reset();
    }

    private static unsafe LLamaSamplerNative* Clone(ref LLamaSamplerNative smpl)
    {
        var sampler = GetSampler(ref smpl);

        return Create(sampler._sampler.Clone()).GetLLamaSamplerPointer();
    }

    private static unsafe void Free(ref LLamaSamplerNative smpl)
    {
        var sampler = GetSampler(ref smpl);

        if (sampler._samplerNativePtr != null)
        {
            Marshal.FreeHGlobal((IntPtr)sampler._samplerNativePtr);
            sampler._samplerNativePtr = null;
        }

        if (sampler._samplerNativeInterfacePtr != null)
        {
            Marshal.FreeHGlobal((IntPtr)sampler._samplerNativeInterfacePtr);
            sampler._samplerNativeInterfacePtr = null;
        }

        if (sampler._samplerNamePtr != null)
        {
            Marshal.FreeHGlobal((IntPtr)sampler._samplerNamePtr);
            sampler._samplerNamePtr = null;
        }

        sampler._gcHandle.Free();

        sampler._sampler.Dispose();
    }
}

/// <summary>
/// A custom sampler stage for modifying logits or selecting a token
/// </summary>
public interface ICustomSampler
    : IDisposable
{
    /// <summary>
    /// The human readable name of this stage
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Apply this stage to a set of logits.
    /// This can modify logits or select a token (or both).
    /// If logits are modified the Sorted flag <b>must</b> be set to false.
    /// </summary>
    /// <remarks>
    /// If the logits are no longer sorted after the custom sampler has run it is <b>critically</b> important to
    /// set <i>Sorted=false</i>. If unsure, always set it to false, this is a safe default.
    /// </remarks>
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
}