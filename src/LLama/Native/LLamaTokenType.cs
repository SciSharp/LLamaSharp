namespace LLama.Native;

/// <summary>
/// Token Types
/// </summary>
/// <remarks>C# equivalent of llama_token_get_type</remarks>
public enum LLamaTokenType
{
    /// <summary>
    /// No specific type has been set for this token
    /// </summary>
    LLAMA_TOKEN_TYPE_UNDEFINED = 0,

    /// <summary>
    /// This is a "normal" token
    /// </summary>
    LLAMA_TOKEN_TYPE_NORMAL = 1,

    /// <summary>
    /// An "unknown" character/text token e.g. &lt;unk&gt;
    /// </summary>
    LLAMA_TOKEN_TYPE_UNKNOWN = 2,

    /// <summary>
    /// A special control token e.g. &lt;/s&gt;
    /// </summary>
    LLAMA_TOKEN_TYPE_CONTROL = 3,

    LLAMA_TOKEN_TYPE_USER_DEFINED = 4,

    LLAMA_TOKEN_TYPE_UNUSED = 5,

    LLAMA_TOKEN_TYPE_BYTE = 6,
}