using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// Override a key/value pair in the llama model metadata
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct LLamaModelKvOverride
{
    /// <summary>
    /// Key to override
    /// </summary>
    [FieldOffset(0)]
    public fixed char key[128];

    /// <summary>
    /// Type of value
    /// </summary>
    [FieldOffset(128)]
    public LLamaModelKvOverrideType Tag;

    /// <summary>
    /// Value, **must** only be used if Tag == LLAMA_KV_OVERRIDE_INT
    /// </summary>
    [FieldOffset(132)]
    public long IntValue;

    /// <summary>
    /// Value, **must** only be used if Tag == LLAMA_KV_OVERRIDE_FLOAT
    /// </summary>
    [FieldOffset(132)]
    public double FloatValue;

    /// <summary>
    /// Value, **must** only be used if Tag == LLAMA_KV_OVERRIDE_BOOL
    /// </summary>
    [FieldOffset(132)]
    public int BoolValue;
}

/// <summary>
/// Specifies what type of value is being overridden by LLamaModelKvOverride
/// </summary>
public enum LLamaModelKvOverrideType
{
    /// <summary>
    /// Overriding an int value
    /// </summary>
    LLAMA_KV_OVERRIDE_INT = 0,

    /// <summary>
    /// Overriding a float value
    /// </summary>
    LLAMA_KV_OVERRIDE_FLOAT = 1,

    /// <summary>
    /// Overriding a bool value
    /// </summary>
    LLAMA_KV_OVERRIDE_BOOL = 2,
}