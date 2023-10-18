namespace LLama.Native;

/// <summary>
/// Indicates position in a sequence
/// </summary>
public readonly record struct LLamaPos(int Value)
{
    /// <summary>
    /// The raw value
    /// </summary>
    public readonly int Value = Value;

    /// <summary>
    /// Convert a LLamaPos into an integer (extract the raw value)
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static explicit operator int(LLamaPos pos) => pos.Value;

    /// <summary>
    /// Convert an integer into a LLamaPos
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator LLamaPos(int value) => new(value);
}