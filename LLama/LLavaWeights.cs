
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
    /// Create the Image Embeddings from the bytes of an image.
    /// </summary>
    /// <param name="ctxLlama"></param>
    /// <param name="image">Image bytes. Supported formats:
    /// <list type="bullet">
    ///     <item>JPG</item>
    ///     <item>PNG</item>
    ///     <item>BMP</item>
    ///     <item>TGA</item>
    /// </list>
    /// </param>
    /// <returns></returns>
    public SafeLlavaImageEmbedHandle CreateImageEmbeddings(LLamaContext ctxLlama, byte[] image )
    {
        return NativeHandle.CreateImageEmbeddings(ctxLlama, image  );
    }

    /// <summary>
    /// Create the Image Embeddings from the bytes of an image.
    /// </summary>
    /// <param name="ctxLlama"></param>
    /// <param name="image">Path to the image file. Supported formats:
    /// <list type="bullet">
    ///     <item>JPG</item>
    ///     <item>PNG</item>
    ///     <item>BMP</item>
    ///     <item>TGA</item>
    /// </list>
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception> 
    public SafeLlavaImageEmbedHandle CreateImageEmbeddings(LLamaContext ctxLlama, string image )
    {
        return NativeHandle.CreateImageEmbeddings(ctxLlama, image  );
    }

    /// <summary>
    /// Eval the image embeddings
    /// </summary>
    /// <param name="ctxLlama"></param>
    /// <param name="imageEmbed"></param>
    /// <param name="n_past"></param>
    /// <returns></returns>
    public bool EvalImageEmbed(LLamaContext ctxLlama, SafeLlavaImageEmbedHandle imageEmbed, ref int n_past)
    {
        return NativeHandle.EvalImageEmbed( ctxLlama, imageEmbed,  ref n_past );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        NativeHandle.Dispose();
    }    
    
}