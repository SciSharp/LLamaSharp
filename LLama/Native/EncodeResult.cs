namespace LLama.Native;

/// <summary>
/// Return codes from llama_encode
/// </summary>
public enum EncodeResult
{
    /// <summary>
    /// An unspecified error
    /// </summary>
    Error = -1,

    /// <summary>
    /// Ok.
    /// </summary>
    Ok = 0
}