using LLama.Native;

namespace LLama.Sampling;

/// <summary>
/// A sampling pipeline which always selects the most likely token
/// </summary>
public class GreedySamplingPipeline
    : BaseSamplingPipeline
{
    /// <summary>
    /// Grammar to apply to constrain possible tokens
    /// </summary>
    public Grammar? Grammar { get; init; }

    /// <inheritdoc />
    protected override SafeLLamaSamplerChainHandle CreateChain(SafeLLamaContextHandle context)
    {
        var chain = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());

        if (Grammar != null)
            chain.AddGrammar(context.ModelHandle, Grammar.Gbnf, Grammar.Root);

        chain.AddGreedySampler();

        return chain;
    }
}