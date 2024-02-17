namespace LLama.Native
{
    /// <summary>
    /// RoPE scaling type.
    /// </summary>
    /// <remarks>C# equivalent of llama_rope_scaling_type</remarks>
    public enum RopeScalingType
        : sbyte
    {
        /// <summary>
        /// No particular scaling type has been specified
        /// </summary>
        LLAMA_ROPE_SCALING_UNSPECIFIED = -1,

        /// <summary>
        /// Do not apply any RoPE scaling
        /// </summary>
        LLAMA_ROPE_SCALING_NONE = 0,

        /// <summary>
        /// Positional linear interpolation, as described by kaikendev: https://kaiokendev.github.io/til#extending-context-to-8k
        /// </summary>
        LLAMA_ROPE_SCALING_LINEAR = 1,

        /// <summary>
        /// YaRN scaling: https://arxiv.org/pdf/2309.00071.pdf
        /// </summary>
        LLAMA_ROPE_SCALING_YARN = 2,
    }
}
