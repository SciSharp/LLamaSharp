namespace LLama.Native;

/// <summary>
/// ID for a sequence in a batch
/// </summary>
/// <param name="Value"></param>
public record struct LLamaSeqId(int Value)
{
    /// <summary>
    /// The raw value
    /// </summary>
    public int Value = Value;

    /// <summary>
    /// Convert a LLamaSeqId into an integer (extract the raw value)
    /// </summary>
    /// <param name="pos"></param>
    public static explicit operator int(LLamaSeqId pos) => pos.Value;

    /// <summary>
    /// Convert an integer into a LLamaSeqId
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static explicit operator LLamaSeqId(int value) => new(value);
}