using System.Runtime.InteropServices;

namespace LLama.Native;

/// <summary>
/// A single token along with probability of this token being selected
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct LLamaTokenData
{
    /// <summary>
    /// token id
    /// </summary>
    public LLamaToken id;

    /// <summary>
    /// log-odds of the token
    /// </summary>
    public float logit;

    /// <summary>
    /// probability of the token
    /// </summary>
    public float p;

    /// <summary>
    /// Create a new LLamaTokenData
    /// </summary>
    /// <param name="id"></param>
    /// <param name="logit"></param>
    /// <param name="p"></param>
    public LLamaTokenData(LLamaToken id, float logit, float p)
    {
        this.id = id;
        this.logit = logit;
        this.p = p;
    }
}