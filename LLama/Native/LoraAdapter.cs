using System;

namespace LLama.Native;

/// <summary>
/// A LoRA adapter which can be applied to a context for a specific model
/// </summary>
public class LoraAdapter
{
    /// <summary>
    /// The model which this LoRA adapter was loaded with.
    /// </summary>
    public SafeLlamaModelHandle Model { get; }

    /// <summary>
    /// The full path of the file this adapter was loaded from
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Native pointer of the loaded adapter, will be automatically freed when the model is unloaded
    /// </summary>
    internal IntPtr Pointer { get; }

    internal LoraAdapter(SafeLlamaModelHandle model, string path, IntPtr nativePtr)
    {
        Model = model;
        Path = path;
        Pointer = nativePtr;
    }
}