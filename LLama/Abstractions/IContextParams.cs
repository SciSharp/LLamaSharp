using System.Text;
using LLama.Native;

namespace LLama.Abstractions;

/// <summary>
/// The parameters for initializing a LLama context from a model.
/// </summary>
public interface IContextParams
{
    /// <summary>
    /// Model context size (n_ctx)
    /// </summary>
    uint? ContextSize { get; set; }

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
    /// RoPE base frequency (null to fetch from the model)
    /// </summary>
    float? RopeFrequencyBase { get; set; }

    /// <summary>
    /// RoPE frequency scaling factor (null to fetch from the model)
    /// </summary>
    float? RopeFrequencyScale { get; set; }

    /// <summary>
    /// Use experimental mul_mat_q kernels
    /// </summary>
    bool MulMatQ { get; set; }

    /// <summary>
    /// The encoding to use for models
    /// </summary>
    Encoding Encoding { get; set; }

    /// <summary>
    /// Number of threads (null = autodetect) (n_threads)
    /// </summary>
    uint? Threads { get; set; }

    /// <summary>
    /// Number of threads to use for batch processing (null = autodetect) (n_threads)
    /// </summary>
    uint? BatchThreads { get; set; }

    /// <summary>
    /// YaRN extrapolation mix factor
    /// </summary>
    float? YarnExtrapolationFactor { get; set; }

    /// <summary>
    /// YaRN magnitude scaling factor
    /// </summary>
    float? YarnAttentionFactor { get; set; }

    /// <summary>
    /// YaRN low correction dim
    /// </summary>
    float? YarnBetaFast { get; set; }

    /// <summary>
    /// YaRN high correction dim
    /// </summary>
    float? YarnBetaSlow { get; set; }

    /// <summary>
    /// YaRN original context length
    /// </summary>
    uint? YarnOriginalContext { get; set; }

    /// <summary>
    /// YaRN scaling method to use.
    /// </summary>
    RopeScalingType? YarnScalingType { get; set; }
}