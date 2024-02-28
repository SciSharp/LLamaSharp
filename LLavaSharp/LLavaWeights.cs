using LLama;
using LLava.Native;
using NativeApi = LLama.Native.NativeApi;

namespace LLava;

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

    public bool EmbedImage(LLamaContext ctxLlama, Byte[] Image, out int n_past )
    {
        return NativeHandle.EmbedImage(ctxLlama, Image, out n_past );
    }
    
    public void Dispose()
    {
        NativeHandle.Dispose();
    }    
    
}