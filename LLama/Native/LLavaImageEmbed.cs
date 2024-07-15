namespace LLama.Native;

/// <summary>
/// LLaVa Image embeddings 
/// </summary>
/// <remarks>llava_image_embed</remarks>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct LLavaImageEmbed
{
    public float* embed;
    public int n_image_pos;
}