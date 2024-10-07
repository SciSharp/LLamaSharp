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
    public void Dispose()
    {
        _chain?.Dispose();
        _chain = null;

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public LLamaToken Sample(SafeLLamaContextHandle ctx, int index)
    {
        _chain ??= CreateChain(ctx);

        return _chain.Sample(ctx, index);
    }

    /// <inheritdoc />
    public void Reset()
    {
        _chain?.Reset();
    }

    /// <inheritdoc />
    public void Accept(LLamaToken token)
    {
        _chain?.Accept(token);
    }
}