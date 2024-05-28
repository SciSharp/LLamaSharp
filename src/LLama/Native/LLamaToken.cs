using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// A single token
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("{Value}")]
public readonly record struct LLamaToken
{
    /// <summary>
    /// The raw value
    /// </summary>
    private readonly int Value;

    /// <summary>
    /// Create a new LLamaToken
    /// </summary>
    /// <param name="value"></param>
    private LLamaToken(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Convert a LLamaToken into an integer (extract the raw value)
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static explicit operator int(LLamaToken pos) => pos.Value;

    /// <summary>
    /// Convert an integer into a LLamaToken
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator LLamaToken(int value) => new(value);

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}