namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_attention_type</remarks>
public enum LLamaAttentionType
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Unspecified = -1,
    Causal = 0,
    NonCausal = 1,
#pragma warning restore CS1591  // Missing XML comment for publicly visible type or member
}