
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

    public SafeLlavaImageEmbedHandle CreateImageEmbeddings(LLamaContext ctxLlama, Byte[] image )
    {
        return NativeHandle.CreateImageEmbeddings(ctxLlama, image  );
    }

    public SafeLlavaImageEmbedHandle CreateImageEmbeddings(LLamaContext ctxLlama, string image )
    {
        return NativeHandle.CreateImageEmbeddings(ctxLlama, image  );
    }

    public bool EvalImageEmbed(LLamaContext ctxLlama, SafeLlavaImageEmbedHandle imageEmbed, ref int n_past)
    {
        return NativeHandle.EvalImageEmbed( ctxLlama, imageEmbed,  ref n_past );
    }

    public void Dispose()
    {
        NativeHandle.Dispose();
    }    
    
}