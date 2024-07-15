namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_chat_message</remarks>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct LLamaChatMessage
{
    /// <summary>
    /// Pointer to the null terminated bytes that make up the role string
    /// </summary>
    public byte* role;

    /// <summary>
    /// Pointer to the null terminated bytes that make up the content string
    /// </summary>
    public byte* content;
}