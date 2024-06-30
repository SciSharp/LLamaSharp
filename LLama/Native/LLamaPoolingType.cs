namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_pooling_type</remarks>
public enum LLamaPoolingType
{
    Unspecified = -1,
    None = 0,
    Mean = 1,
    CLS = 2,

    Last = 3,
}