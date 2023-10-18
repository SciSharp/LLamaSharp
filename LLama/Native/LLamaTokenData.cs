using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// A single token along with probability of this token being selected
/// </summary>
/// <param name="id"></param>
/// <param name="logit"></param>
/// <param name="p"></param>
[StructLayout(LayoutKind.Sequential)]
public record struct LLamaTokenData(int id, float logit, float p)
{
    /// <summary>
    /// token id
    /// </summary>
    public int id = id;

    /// <summary>
    /// log-odds of the token
    /// </summary>
    public float logit = logit;

    /// <summary>
    /// probability of the token
    /// </summary>
    public float p = p;
}