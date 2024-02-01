namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_split_mode</remarks>
public enum GPUSplitMode
{
    /// <summary>
    /// Single GPU
    /// </summary>
    None = 0,

    /// <summary>
    /// Split layers and KV across GPUs
    /// </summary>
    Layer = 1,

    /// <summary>
    /// split rows across GPUs
    /// </summary>
    Row = 2,
}