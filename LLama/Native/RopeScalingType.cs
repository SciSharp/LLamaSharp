namespace LLama.Native
{
    /// <summary>
    /// RoPE scaling type. C# equivalent of llama_rope_scaling_type
    /// </summary>
    public enum RopeScalingType
        : sbyte
    {
        LLAMA_ROPE_SCALING_UNSPECIFIED = -1,

        LLAMA_ROPE_SCALING_NONE = 0,

        LLAMA_ROPE_SCALING_LINEAR = 1,

        LLAMA_ROPE_SCALING_YARN = 2,
    }
}
