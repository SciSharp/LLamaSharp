using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// LLaVa Image embeddings 
/// </summary>
[StructLayout(LayoutKind.Sequential)]
unsafe public struct LLavaImageEmbed
{
    public float* embed;
    public int n_image_pos;
}