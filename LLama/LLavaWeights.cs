
using System;
using LLama.Native;

namespace LLama;

public sealed class LLavaWeights : IDisposable
{
    public SafeLlavaModelHandle NativeHandle { get; }   
    
    internal LLavaWeights(SafeLlavaModelHandle weights)
    {
        NativeHandle = weights;
    }
    
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
    public bool EmbedImage(LLamaContext ctxLlama, Byte[] Image, ref int n_past )
    {
        return NativeHandle.EmbedImage(ctxLlama, Image, ref n_past );
    }
    
    public void Dispose()
    {
        NativeHandle.Dispose();
    }    
    
}