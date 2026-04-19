[`< Back`](./)

---

# IContextParams

Namespace: LLama.Abstractions

The parameters for initializing a LLama context from a model.

```csharp
public interface IContextParams
```

Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Properties

### **ContextSize**

Model context size (n_ctx)

```csharp
public abstract Nullable<uint> ContextSize { get; }
```

#### Property Value

[Nullable&lt;UInt32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **BatchSize**

maximum batch size that can be submitted at once (must be &gt;=32 to use BLAS) (n_batch)

```csharp
public abstract uint BatchSize { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **UBatchSize**

Physical batch size

```csharp
public abstract uint UBatchSize { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **SeqMax**

max number of sequences (i.e. distinct states for recurrent models)

```csharp
public abstract uint SeqMax { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **Embeddings**

If true, extract embeddings (together with logits).

```csharp
public abstract bool Embeddings { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **RopeFrequencyBase**

RoPE base frequency (null to fetch from the model)

```csharp
public abstract Nullable<float> RopeFrequencyBase { get; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **RopeFrequencyScale**

RoPE frequency scaling factor (null to fetch from the model)

```csharp
public abstract Nullable<float> RopeFrequencyScale { get; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **Encoding**

The encoding to use for models

```csharp
public abstract Encoding Encoding { get; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

### **Threads**

Number of threads (null = autodetect) (n_threads)

```csharp
public abstract Nullable<int> Threads { get; }
```

#### Property Value

[Nullable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **BatchThreads**

Number of threads to use for batch processing (null = autodetect) (n_threads)

```csharp
public abstract Nullable<int> BatchThreads { get; }
```

#### Property Value

[Nullable&lt;Int32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnExtrapolationFactor**

YaRN extrapolation mix factor (null = from model)

```csharp
public abstract Nullable<float> YarnExtrapolationFactor { get; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnAttentionFactor**

YaRN magnitude scaling factor (null = from model)

```csharp
public abstract Nullable<float> YarnAttentionFactor { get; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnBetaFast**

YaRN low correction dim (null = from model)

```csharp
public abstract Nullable<float> YarnBetaFast { get; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnBetaSlow**

YaRN high correction dim (null = from model)

```csharp
public abstract Nullable<float> YarnBetaSlow { get; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnOriginalContext**

YaRN original context length (null = from model)

```csharp
public abstract Nullable<uint> YarnOriginalContext { get; }
```

#### Property Value

[Nullable&lt;UInt32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnScalingType**

YaRN scaling method to use.

```csharp
public abstract Nullable<RopeScalingType> YarnScalingType { get; }
```

#### Property Value

[Nullable&lt;RopeScalingType&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TypeK**

Override the type of the K cache

```csharp
public abstract Nullable<GGMLType> TypeK { get; }
```

#### Property Value

[Nullable&lt;GGMLType&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TypeV**

Override the type of the V cache

```csharp
public abstract Nullable<GGMLType> TypeV { get; }
```

#### Property Value

[Nullable&lt;GGMLType&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **NoKqvOffload**

Whether to disable offloading the KQV cache to the GPU

```csharp
public abstract bool NoKqvOffload { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **FlashAttention**

Whether to use flash attention

```csharp
public abstract bool FlashAttention { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **DefragThreshold**

defragment the KV cache if holes/size &gt; defrag_threshold, Set to &lt; 0 to disable (default)
 defragment the KV cache if holes/size &gt; defrag_threshold, Set to  or &lt; 0 to disable (default)

```csharp
public abstract Nullable<float> DefragThreshold { get; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **PoolingType**

How to pool (sum) embedding results by sequence id (ignored if no pooling layer)

```csharp
public abstract LLamaPoolingType PoolingType { get; }
```

#### Property Value

[LLamaPoolingType](./llama.native.llamapoolingtype.md)<br>

### **AttentionType**

Attention type to use for embeddings

```csharp
public abstract LLamaAttentionType AttentionType { get; }
```

#### Property Value

[LLamaAttentionType](./llama.native.llamaattentiontype.md)<br>

---

[`< Back`](./)
