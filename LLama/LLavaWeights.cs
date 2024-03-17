using System;
using LLama.Native;

namespace LLama;

/// <summary>
/// A set of llava model weights (mmproj), loaded into memory.
/// </summary>
public sealed class LLavaWeights : IDisposable
{
    /// <summary>
    /// The native handle, which is used in the native APIs
    /// </summary>
    /// <remarks>Be careful how you use this!</remarks>
    public SafeLlavaModelHandle NativeHandle { get; }   
    
    internal LLavaWeights(SafeLlavaModelHandle weights)
    {
        NativeHandle = weights;
    }

    /// <summary>
    /// Load weights into memory
    /// </summary>
    /// <param name="mmProject">path to the "mmproj" model file</param>
    /// <returns></returns>
    public static LLavaWeights LoadFromFile(string mmProject)
    {
        var weights = SafeLlavaModelHandle.LoadFromFile(mmProject, 1);
        return new LLavaWeights(weights);
    }

    /// <summary>
    /// Embed the image from file into llama context
    /// </summary>
    /// <param name="ctxLlama"></param>
    /// <param name="Image"></param>
    /// <param name="n_past"></param>
    /// <returns></returns>
    public bool EmbedImage(LLamaContext ctxLlama, string Image, ref int n_past )
    {
        return NativeHandle.EmbedImage(ctxLlama, Image, ref n_past );
    }

    /// <summary>
    /// Embed the image from binary into llama context.
    /// </summary>
    /// <param name="ctxLlama"></param>
    /// <param name="Image"></param>
    /// <param name="n_past"></param>
    /// <returns></returns>
    public bool EmbedImage(LLamaContext ctxLlama, byte[] Image, ref int n_past )
    {
        return NativeHandle.EmbedImage(ctxLlama, Image, ref n_past );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        NativeHandle.Dispose();
    }    
    
}