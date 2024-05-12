using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// Override a key/value pair in the llama model metadata (llama_model_kv_override)
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct LLamaModelMetadataOverride
{
    /// <summary>
    /// Key to override
    /// </summary>
    [FieldOffset(0)]
    public fixed byte key[128];

    /// <summary>
    /// Type of value
    /// </summary>
    [FieldOffset(128)]
    public LLamaModelKvOverrideType Tag;

    /// <summary>
    /// Add 4 bytes of padding, to align the next fields to 8 bytes
    /// </summary>
    [FieldOffset(132)]
    private readonly int PADDING;

    /// <summary>
    /// Value, **must** only be used if Tag == LLAMA_KV_OVERRIDE_INT
    /// </summary>
    [FieldOffset(136)]
    public long IntValue;

    /// <summary>
    /// Value, **must** only be used if Tag == LLAMA_KV_OVERRIDE_FLOAT
    /// </summary>
    [FieldOffset(136)]
    public double FloatValue;

    /// <summary>
    /// Value, **must** only be used if Tag == LLAMA_KV_OVERRIDE_BOOL
    /// </summary>
    [FieldOffset(136)]
    public long BoolValue;

    /// <summary>
    /// Value, **must** only be used if Tag == String
    /// </summary>
    [FieldOffset(136)]
    public fixed byte StringValue[128];
}

/// <summary>
/// Specifies what type of value is being overridden by LLamaModelKvOverride
/// </summary>
/// <remarks>llama_model_kv_override_type</remarks>
public enum LLamaModelKvOverrideType
{
    /// <summary>
    /// Overriding an int value
    /// </summary>
    Int = 0,

    /// <summary>
    /// Overriding a float value
    /// </summary>
    Float = 1,

    /// <summary>
    /// Overriding a bool value
    /// </summary>
    Bool = 2,

    /// <summary>
    /// Overriding a string value
    /// </summary>
    String = 3,
}