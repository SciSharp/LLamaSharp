namespace LLama.Native;

/// <summary>
/// LLaVa Image embeddings 
/// </summary>
/// <remarks>llava_image_embed</remarks>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct LLavaImageEmbed
{
    /// <summary>
    /// The embeddings of the embedded image.
    /// </summary>
    public float* embed;

    /// <summary>
    /// The position of the image's tokens.
    /// </summary>
    public int n_image_pos;
}