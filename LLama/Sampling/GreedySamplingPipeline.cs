using LLama.Native;

namespace LLama.Sampling;

/// <summary>
/// A sampling pipeline which always selects the most likely token
/// </summary>
public class GreedySamplingPipeline
    : BaseSamplingPipeline
{
    /// <inheritdoc />
    protected override SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
    {
        var chain = SafeLLamaSamplerHandle.CreateChain(LLamaSamplerChainParams.Default());
        chain.Add(SafeLLamaSamplerHandle.CreateGreedySampler());
        return chain;
    }
}