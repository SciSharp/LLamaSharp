namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_rope_type</remarks>
public enum LLamaRopeType
{
    None = -1,
    Norm = 0,
    NEOX = 2,//GGML_ROPE_TYPE_NEOX,
}