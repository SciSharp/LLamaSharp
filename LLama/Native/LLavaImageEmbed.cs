namespace LLama.Native;

/// <summary>
/// LLaVa Image embeddings 
/// </summary>
/// <remarks>llava_image_embed</remarks>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct LLavaImageEmbed
{
    /// <summary>
    /// The embeddings of the embedded image. Should be a float[]
    /// </summary>
    public float* embed;

    /// <summary>
    /// 
    /// </summary>
    public int n_image_pos;
}