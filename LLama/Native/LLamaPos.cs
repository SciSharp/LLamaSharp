using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// Indicates position in a sequence
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct LLamaPos
{
    /// <summary>
    /// The raw value
    /// </summary>
    public int Value;

    /// <summary>
    /// Create a new LLamaPos
    /// </summary>
    /// <param name="value"></param>
    private LLamaPos(int value)
    {
        Value = value;
    }

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

    /// <summary>
    /// Increment this position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static LLamaPos operator ++(LLamaPos pos)
    {
        return new LLamaPos(pos.Value + 1);
    }

    /// <summary>
    /// Increment this position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static LLamaPos operator --(LLamaPos pos)
    {
        return new LLamaPos(pos.Value - 1);
    }
}