[`< Back`](./)

---

# IContextParams

Namespace: LLama.Abstractions

The parameters for initializing a LLama context from a model.

```csharp
public interface IContextParams
```

Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Properties

### **ContextSize**

Model context size (n_ctx)

```csharp
uint? ContextSize { get; }
```

#### Property Value

[UInt32?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **ContextType**

The type of context

```csharp
LLamaContextType ContextType { get; }
```

#### Property Value

[LLamaContextType](./llama.native.llamacontexttype.md)<br>

### **BatchSize**

maximum batch size that can be submitted at once (must be &gt;=32 to use BLAS) (n_batch)

```csharp
uint BatchSize { get; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **UBatchSize**

Physical batch size

```csharp
uint UBatchSize { get; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **SeqMax**

max number of sequences (i.e. distinct states for recurrent models)

```csharp
uint SeqMax { get; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **RecurrentRollbackSnapshots**

The number of recurrent-state snapshots per seq for rollback

```csharp
uint RecurrentRollbackSnapshots { get; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **Embeddings**

If true, extract embeddings (together with logits).

```csharp
bool Embeddings { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **RopeFrequencyBase**

RoPE base frequency (null to fetch from the model)

```csharp
float? RopeFrequencyBase { get; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **RopeFrequencyScale**

RoPE frequency scaling factor (null to fetch from the model)

```csharp
float? RopeFrequencyScale { get; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **Encoding**

The encoding to use for models

```csharp
Encoding Encoding { get; }
```

#### Property Value

[Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

### **Threads**

Number of threads (null = autodetect) (n_threads)

```csharp
int? Threads { get; }
```

#### Property Value

[Int32?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **BatchThreads**

Number of threads to use for batch processing (null = autodetect) (n_threads)

```csharp
int? BatchThreads { get; }
```

#### Property Value

[Int32?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnExtrapolationFactor**

YaRN extrapolation mix factor (null = from model)

```csharp
float? YarnExtrapolationFactor { get; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnAttentionFactor**

YaRN magnitude scaling factor (null = from model)

```csharp
float? YarnAttentionFactor { get; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnBetaFast**

YaRN low correction dim (null = from model)

```csharp
float? YarnBetaFast { get; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnBetaSlow**

YaRN high correction dim (null = from model)

```csharp
float? YarnBetaSlow { get; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnOriginalContext**

YaRN original context length (null = from model)

```csharp
uint? YarnOriginalContext { get; }
```

#### Property Value

[UInt32?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnScalingType**

YaRN scaling method to use.

```csharp
RopeScalingType? YarnScalingType { get; }
```

#### Property Value

[RopeScalingType?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TypeK**

Override the type of the K cache

```csharp
GGMLType? TypeK { get; }
```

#### Property Value

[GGMLType?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TypeV**

Override the type of the V cache

```csharp
GGMLType? TypeV { get; }
```

#### Property Value

[GGMLType?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **NoKqvOffload**

Whether to disable offloading the KQV cache to the GPU

```csharp
bool NoKqvOffload { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **FlashAttention**

Whether to use flash attention

```csharp
bool? FlashAttention { get; }
```

#### Property Value

[Boolean?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **DefragThreshold**

defragment the KV cache if holes/size &gt; defrag_threshold, Set to &lt;= 0 to disable (default)

```csharp
float? DefragThreshold { get; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **PoolingType**

How to pool (sum) embedding results by sequence id (ignored if no pooling layer)

```csharp
LLamaPoolingType PoolingType { get; }
```

#### Property Value

[LLamaPoolingType](./llama.native.llamapoolingtype.md)<br>

### **AttentionType**

Attention type to use for embeddings

```csharp
LLamaAttentionType AttentionType { get; }
```

#### Property Value

[LLamaAttentionType](./llama.native.llamaattentiontype.md)<br>

### **OpOffload**

Offload host tensor operations to device

```csharp
bool? OpOffload { get; }
```

#### Property Value

[Boolean?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **KVUnified**

use a unified buffer across the input sequences when computing the attention.
 try to disable when n_seq_max &gt; 1 for improved performance when the sequences do not share a large prefix
 <br>
 ref: https://github.com/ggml-org/llama.cpp/pull/14363

```csharp
bool? KVUnified { get; }
```

#### Property Value

[Boolean?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **SwaFull**

Use full-size SWA cache (https://github.com/ggml-org/llama.cpp/pull/13194#issuecomment-2868343055)

```csharp
bool? SwaFull { get; }
```

#### Property Value

[Boolean?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

**Remarks:**

Setting to false when n_seq_max &gt; 1 can cause bad performance in some cases
 ref: https://github.com/ggml-org/llama.cpp/pull/13845#issuecomment-2924800573

---

[`< Back`](./)
