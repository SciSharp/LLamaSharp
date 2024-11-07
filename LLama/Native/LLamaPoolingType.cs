using LLama.Abstractions;

namespace LLama.Native;

/// <summary>
/// 
/// </summary>
/// <remarks>llama_pooling_type</remarks>
public enum LLamaPoolingType
{
    /// <summary>
    /// No specific pooling type. Use the model default if this is specific in <see cref="IContextParams.PoolingType"/>
    /// </summary>
    Unspecified = -1,

    /// <summary>
    /// Do not pool embeddings (per-token embeddings)
    /// </summary>
    None = 0,

    /// <summary>
    /// Take the mean of every token embedding
    /// </summary>
    Mean = 1,

    /// <summary>
    /// Return the embedding for the special "CLS" token
    /// </summary>
    CLS = 2,

    Last = 3,

    /// <summary>
    /// Used by reranking models to attach the classification head to the graph
    /// </summary>
    Rank,
}