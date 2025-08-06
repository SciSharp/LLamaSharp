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
    /// maximum batch size that can be submitted at once (must be >=32 to use BLAS) (n_batch)
    /// </summary>
    uint BatchSize { get; }

    /// <summary>
    /// Physical batch size
    /// </summary>
    uint UBatchSize { get; }

    /// <summary>
    /// max number of sequences (i.e. distinct states for recurrent models)
    /// </summary>
    uint SeqMax { get; }

    /// <summary>
    /// If true, extract embeddings (together with logits).
    /// </summary>
    bool Embeddings { get; }

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
    int? Threads { get; }

    /// <summary>
    /// Number of threads to use for batch processing (null = autodetect) (n_threads)
    /// </summary>
    int? BatchThreads { get; }

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

    /// <summary>
    /// Whether to use flash attention
    /// </summary>
    bool FlashAttention { get; }

    /// <summary>
    /// defragment the KV cache if holes/size &gt; defrag_threshold, Set to &lt;= 0 to disable (default)
    /// </summary>
    float? DefragThreshold { get; }

    /// <summary>
    /// How to pool (sum) embedding results by sequence id (ignored if no pooling layer)
    /// </summary>
    LLamaPoolingType PoolingType { get; }

    /// <summary>
    /// Attention type to use for embeddings
    /// </summary>
    LLamaAttentionType AttentionType { get; }

    /// <summary>
    /// Offload host tensor operations to device
    /// </summary>
    bool? OpOffload { get; }

    /// <summary>
    /// use a unified buffer across the input sequences when computing the attention.
    /// try to disable when n_seq_max > 1 for improved performance when the sequences do not share a large prefix
    /// <br />
    /// ref: <a href="https://github.com/ggml-org/llama.cpp/pull/14363">https://github.com/ggml-org/llama.cpp/pull/14363</a>
    /// </summary>
    bool? KVUnified { get; }

    /// <summary>
    /// Use full-size SWA cache (https://github.com/ggml-org/llama.cpp/pull/13194#issuecomment-2868343055)
    /// </summary>
    /// <remarks>Setting to false when n_seq_max > 1 can cause bad performance in some cases
    ///       ref: https://github.com/ggml-org/llama.cpp/pull/13845#issuecomment-2924800573
    /// </remarks>
    bool? SwaFull { get; }
}