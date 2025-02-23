using System;
using LLama.Native;

namespace LLama.Sampling;

/// <inheritdoc />
public abstract class BaseSamplingPipeline
    : ISamplingPipeline
{
    private SafeLLamaSamplerChainHandle? _chain;

    /// <summary>
    /// Create a new sampler wrapping a llama.cpp sampler chain
    /// </summary>
    public BaseSamplingPipeline()
    {
    }

    /// <summary>
    /// Create a sampling chain. This will be called once, the base class will automatically dispose the chain.
    /// </summary>
    /// <returns></returns>
    protected abstract SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context);

    /// <inheritdoc />
    public virtual void Dispose()
    {
        _chain?.Dispose();
        _chain = null;

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual LLamaToken Sample(SafeLLamaContextHandle ctx, int index)
    {
        _chain ??= CreateChain(ctx);

        return _chain.Sample(ctx, index);
    }

    /// <inheritdoc />
    public virtual void Apply(SafeLLamaContextHandle ctx, LLamaTokenDataArray data)
    {
        _chain ??= CreateChain(ctx);
        using (LLamaTokenDataArrayNative.Create(data, out var native))
            _chain.Apply(ref native);
    }

    /// <summary>
    /// Apply this sampling chain to a LLamaTokenDataArrayNative
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="data"></param>
    public virtual void Apply(SafeLLamaContextHandle ctx, ref LLamaTokenDataArrayNative data)
    {
        _chain ??= CreateChain(ctx);
        _chain.Apply(ref data);
    }

    /// <inheritdoc />
    public virtual void Reset()
    {
        _chain?.Reset();
    }

    /// <inheritdoc />
    public virtual void Accept(LLamaToken token)
    {
        _chain?.Accept(token);
    }
}