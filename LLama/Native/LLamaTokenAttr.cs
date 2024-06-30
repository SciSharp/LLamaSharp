using System;

namespace LLama.Native;

/// <summary>
/// Token attributes
/// </summary>
/// <remarks>C# equivalent of llama_token_attr</remarks>
[Flags]
public enum LLamaTokenAttr
{
    Undefined = 0,
    Unknown = 1 << 0,
    Unused = 1 << 1,
    Normal = 1 << 2,
    Control = 1 << 3,
    UserDefined = 1 << 4,
    Byte = 1 << 5,
    Normalized = 1 << 6,
    LStrip = 1 << 7,
    RStrip = 1 << 8,
    SingleWord = 1 << 9,
}