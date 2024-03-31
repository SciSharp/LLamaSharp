# Understand LLamaSharp context

`LLamaContext` is the most important component as a link between native APIs and higher-level APIs. It contains the basic settings for model inference and holds the kv-cache, which could significantly accelerate the model inference. Since `LLamaContext` is not coupled with `LLamaWeights`, it's possible to create multiple context based on one piece of model weight. Each `ILLamaExecutor` will hold a `LLamaContext` instance, but it's possible to switch to different context in an executor.

If your application has multiple sessions, please take care of managing `LLamaContext`.

`LLamaContext` takes the following parameters as its settings. Note that the parameters could not be changed once the context has been created.

```cs
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

    /// <summary>
    /// defragment the KV cache if holes/size &gt; defrag_threshold, Set to &lt; 0 to disable (default)
    /// </summary>
    float DefragThreshold { get; }

    /// <summary>
    /// Whether to pool (sum) embedding results by sequence id (ignored if no pooling layer)
    /// </summary>
    bool DoPooling { get; }
}
```


`LLamaContext` has its state, which could be saved and loaded.

```cs
LLamaContext.SaveState(string filename)
LLamaContext.GetState()
```

