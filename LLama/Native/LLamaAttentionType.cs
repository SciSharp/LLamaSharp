namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_attention_type</remarks>
public enum LLamaAttentionType
{
    /// <summary>
    /// Unspecified attention type. The library will attempt to find the best fit
    /// </summary>
    Unspecified = -1,

    /// <summary>
    /// The causal mask will be applied, causing tokens to only see previous tokens in the same sequence, and not future ones
    /// </summary>
    Causal = 0,

    /// <summary>
    /// The causal mask will not be applied, and tokens of the same sequence will be able to see each other
    /// </summary>
    NonCausal = 1,
}