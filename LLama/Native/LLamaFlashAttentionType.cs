namespace LLama.Native;
/// <summary>
/// flash_attn_type
/// </summary>
public enum LLamaFlashAttentionType
{
    /// <summary>
    /// attention type auto
    /// </summary>
    LLAMA_FLASH_ATTENTION_TYPE_AUTO = -1,
    /// <summary>
    /// attention disabled
    /// </summary>
    LLAMA_FLASH_ATTENTION_TYPE_DISABLED = 0,
    /// <summary>
    /// attention enabled
    /// </summary>
    LLAMA_FLASH_ATTENTION_TYPE_ENABLED = 1,
}