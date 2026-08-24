namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_load_mode</remarks>
public enum LLamaLoadMode
{
    /// <summary>
    /// no special loading mode
    /// </summary>
    None = 0,

    /// <summary>
    /// memory map the model
    /// </summary>
    MemoryMap = 1,

    /// <summary>
    /// force system to keep model in RAM rather than swapping or compressing
    /// </summary>
    MemoryLock = 2,

    /// <summary>
    /// mmap + force system to keep model in RAM rather than swapping or compressing
    /// </summary>
    MemoryMapAndLock = 3,

    /// <summary>
    /// Use direct I/O if available
    /// </summary>
    DirectIO = 4,
}

public static partial class NativeApi
{
    /// <summary>
    /// Get the canonical name of a particular load mode
    /// </summary>
    /// <param name="load_mode"></param>
    /// <returns></returns>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern string llama_load_mode_name(LLamaLoadMode load_mode);

    /// <summary>
    /// Parse a load mode from a string
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern LLamaLoadMode llama_load_mode_from_str(string str);
}