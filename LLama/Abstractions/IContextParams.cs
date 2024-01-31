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
    uint? ContextSize { get; }

    /// <summary>
    /// batch size for prompt processing (must be >=32 to use BLAS) (n_batch)
    /// </summary>
    uint BatchSize { get; }

    /// <summary>
    /// Seed for the random number generator (seed)
    /// </summary>
    uint Seed { get; }

    /// <summary>
    /// Whether to use embedding mode. (embedding) Note that if this is set to true, 
    /// The LLamaModel won't produce text response anymore.
    /// </summary>
    bool EmbeddingMode { get; }

    /// <summary>
    /// RoPE base frequency (null to fetch from the model)
    /// </summary>
    float? RopeFrequencyBase { get; }

    /// <summary>
    /// RoPE frequency scaling factor (null to fetch from the model)
    /// </summary>
    float? RopeFrequencyScale { get; }

    /// <summary>
    /// The encoding to use for models
    /// </summary>
    Encoding Encoding { get; }

    /// <summary>
    /// Number of threads (null = autodetect) (n_threads)
    /// </summary>
    uint? Threads { get; }

    /// <summary>
    /// Number of threads to use for batch processing (null = autodetect) (n_threads)
    /// </summary>
    uint? BatchThreads { get; }

    /// <summary>
    /// YaRN extrapolation mix factor (null = from model)
    /// </summary>
    float? YarnExtrapolationFactor { get; }

    /// <summary>
    /// YaRN magnitude scaling factor (null = from model)
    /// </summary>
    float? YarnAttentionFactor { get; }

    /// <summary>
    /// YaRN low correction dim (null = from model)
    /// </summary>
    float? YarnBetaFast { get; }

    /// <summary>
    /// YaRN high correction dim (null = from model)
    /// </summary>
    float? YarnBetaSlow { get; }

    /// <summary>
    /// YaRN original context length (null = from model)
    /// </summary>
    uint? YarnOriginalContext { get; }

    /// <summary>
    /// YaRN scaling method to use.
    /// </summary>
    RopeScalingType? YarnScalingType { get; }

    /// <summary>
    /// Override the type of the K cache
    /// </summary>
    GGMLType? TypeK { get; }

    /// <summary>
    /// Override the type of the V cache
    /// </summary>
    GGMLType? TypeV { get; }

    /// <summary>
    /// Whether to disable offloading the KQV cache to the GPU
    /// </summary>
    bool NoKqvOffload { get; }
}