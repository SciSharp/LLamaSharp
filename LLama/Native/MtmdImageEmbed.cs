using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// Representation of the native <c>llava_image_embed</c> structure used to return image embeddings.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct MtmdImageEmbed
{
    /// <summary>
    /// Pointer to the embedding buffer for the decoded image.
    /// </summary>
    public float* embed;

    /// <summary>
    /// Number of sequence positions consumed by the image tokens associated with the embedding.
    /// </summary>
    public int n_image_pos;
}
