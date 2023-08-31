using static LLama.LLamaTransforms;

namespace Microsoft.SemanticKernel.Connectors.AI.LLama.ChatCompletion;

/// <summary>
/// Default HistoryTransform Patch
/// </summary>
public class HistoryTransform : DefaultHistoryTransform
{
    /// <inheritdoc/>
    public override string HistoryToText(global::LLama.Common.ChatHistory history)
    {
        var prompt = base.HistoryToText(history);
        return prompt + "\nAssistant:";

    }
}
