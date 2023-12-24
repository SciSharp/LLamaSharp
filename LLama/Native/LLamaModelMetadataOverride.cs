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