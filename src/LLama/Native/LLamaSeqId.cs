using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// ID for a sequence in a batch
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct LLamaSeqId
{
    /// <summary>
    /// LLamaSeqId with value 0
    /// </summary>
    public static readonly LLamaSeqId Zero = new LLamaSeqId(0);

    /// <summary>
    /// The raw value
    /// </summary>
    public int Value;

    /// <summary>
    /// Create a new LLamaSeqId 
    /// </summary>
    /// <param name="value"></param>
    private LLamaSeqId(int value)
    {
        Value = value;
    }

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