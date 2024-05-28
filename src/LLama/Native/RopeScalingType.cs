namespace LLama.Native
{
    /// <summary>
    /// RoPE scaling type.
    /// </summary>
    /// <remarks>C# equivalent of llama_rope_scaling_type</remarks>
    public enum RopeScalingType
        : int
    {
        /// <summary>
        /// No particular scaling type has been specified
        /// </summary>
        Unspecified = -1,

        /// <summary>
        /// Do not apply any RoPE scaling
        /// </summary>
        None = 0,

        /// <summary>
        /// Positional linear interpolation, as described by kaikendev: https://kaiokendev.github.io/til#extending-context-to-8k
        /// </summary>
        Linear = 1,

        /// <summary>
        /// YaRN scaling: https://arxiv.org/pdf/2309.00071.pdf
        /// </summary>
        Yarn = 2,
    }
}
