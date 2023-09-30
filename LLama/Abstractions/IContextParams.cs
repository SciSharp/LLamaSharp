using System.Text;

namespace LLama.Abstractions;

/// <summary>
/// The parameters for initializing a LLama context from a model.
/// </summary>
public interface IContextParams
{
    /// <summary>
    /// Model context size (n_ctx)
    /// </summary>
    uint ContextSize { get; set; }

    /// <summary>
    /// batch size for prompt processing (must be >=32 to use BLAS) (n_batch)
    /// </summary>
    uint BatchSize { get; set; }

    /// <summary>
    /// Seed for the random number generator (seed)
    /// </summary>
    uint Seed { get; set; }

    /// <summary>
    /// Use f16 instead of f32 for memory kv (memory_f16)
    /// </summary>
    bool UseFp16Memory { get; set; }

    /// <summary>
    /// Compute perplexity over the prompt (perplexity)
    /// </summary>
    bool Perplexity { get; set; }

    /// <summary>
    /// Whether to use embedding mode. (embedding) Note that if this is set to true, 
    /// The LLamaModel won't produce text response anymore.
    /// </summary>
    bool EmbeddingMode { get; set; }

    /// <summary>
    /// RoPE base frequency
    /// </summary>
    float RopeFrequencyBase { get; set; }

    /// <summary>
    /// RoPE frequency scaling factor
    /// </summary>
    float RopeFrequencyScale { get; set; }

    /// <summary>
    /// Use experimental mul_mat_q kernels
    /// </summary>
    bool MulMatQ { get; set; }

    /// <summary>
    /// The encoding to use for models
    /// </summary>
    Encoding Encoding { get; set; }
}