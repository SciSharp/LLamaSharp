namespace LLama.Native;

/// <summary>
/// Return codes from llama_decode
/// </summary>
public enum DecodeResult
{
    /// <summary>
    /// An unspecified error
    /// </summary>
    Error = -1,

    /// <summary>
    /// Ok.
    /// </summary>
    Ok = 0,

    /// <summary>
    /// Could not find a KV slot for the batch (try reducing the size of the batch or increase the context)
    /// </summary>
    NoKvSlot = 1,
}