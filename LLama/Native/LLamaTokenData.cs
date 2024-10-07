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
    public LLamaToken ID;

    /// <summary>
    /// log-odds of the token
    /// </summary>
    public float Logit;

    /// <summary>
    /// probability of the token
    /// </summary>
    public float Probability;

    /// <summary>
    /// Create a new LLamaTokenData
    /// </summary>
    /// <param name="id"></param>
    /// <param name="logit"></param>
    /// <param name="probability"></param>
    public LLamaTokenData(LLamaToken id, float logit, float probability)
    {
        ID = id;
        Logit = logit;
        Probability = probability;
    }
}