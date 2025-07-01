namespace LLama.Native;

/// <summary>
/// Return codes from llama_decode
/// </summary>
public enum DecodeResult
{
    /// <summary>
    /// Input batch was invalid
    /// </summary>
    InvalidInputBatch = -1,

    /// <summary>
    /// Ok.
    /// </summary>
    Ok = 0,

    /// <summary>
    /// Could not find a KV slot for the batch (try reducing the size of the batch or increase the context)
    /// </summary>
    NoKvSlot = 1,

    /// <summary>
    /// Compute was aborted (e.g. due to callback request or timeout)
    /// </summary>
    ComputeAborted = 2,

    /// <summary>
    /// Failed to allocate memory or reserve output space
    /// </summary>
    AllocationFailed = -2,

    /// <summary>
    /// General failure during decode (e.g. internal error, slot failure)
    /// </summary>
    DecodeFailed = -3,
}