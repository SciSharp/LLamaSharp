namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_chat_message</remarks>
public unsafe struct LLamaChatMessage
{
    public byte* role;
    public byte* content;
}