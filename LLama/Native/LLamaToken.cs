using System.Diagnostics;

namespace LLama.Native;

/// <summary>
/// A single token
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("{Value}")]
public readonly record struct LLamaToken
{
    /// <summary>
    /// Token Value used when token is inherently null
    /// </summary>
    public static readonly LLamaToken InvalidToken = -1;

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

    /// <summary>
    /// Get attributes for this token
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public LLamaTokenAttr GetAttributes(SafeLlamaModelHandle model)
    {
        return model.Tokens.GetAttributes(this);
    }

    /// <summary>
    /// Check if this is a control token
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool IsControl(SafeLlamaModelHandle model)
    {
        return model.Tokens.IsControl(this);
    }

    /// <summary>
    /// Check if this token should end generation
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool IsEndOfGeneration(SafeLlamaModelHandle model)
    {
        return model.Tokens.IsEndOfGeneration(this);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString();
    }
}