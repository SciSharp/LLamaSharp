[`< Back`](./)

---

# ModelParams

Namespace: LLama.Common

The parameters for initializing a LLama model.

```csharp
public record class ModelParams : LLama.Abstractions.ILLamaParams, LLama.Abstractions.IModelParams, LLama.Abstractions.IContextParams, System.IEquatable`1[[LLama.Common.ModelParams, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ModelParams](./llama.common.modelparams.md)<br>
Implements [ILLamaParams](./llama.abstractions.illamaparams.md), [IModelParams](./llama.abstractions.imodelparams.md), [IContextParams](./llama.abstractions.icontextparams.md), [IEquatable&lt;ModelParams&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **ContextSize**

Model context size (n_ctx)

```csharp
public uint? ContextSize { get; set; }
```

#### Property Value

[UInt32?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **ContextType**

The type of context

```csharp
public LLamaContextType ContextType { get; set; }
```

#### Property Value

[LLamaContextType](./llama.native.llamacontexttype.md)<br>

### **MainGpu**

main_gpu interpretation depends on split_mode:

- **None** - The GPU that is used for the entire mode.
- **Row** - The GPU that is used for small tensors and intermediate results.
- **Layer** - Ignored.

```csharp
public int MainGpu { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SplitMode**

How to split the model across multiple GPUs

```csharp
public GPUSplitMode? SplitMode { get; set; }
```

#### Property Value

[GPUSplitMode?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TensorBufferOverrides**

Buffer type overrides for specific tensor patterns, allowing you to specify hardware devices to use for individual tensors or sets of tensors.
 Equivalent to --override-tensor or -ot on the llama.cpp command line or tensor_buft_overrides internally.

```csharp
public List<TensorBufferOverride> TensorBufferOverrides { get; set; }
```

#### Property Value

[List&lt;TensorBufferOverride&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **Devices**

Names of the backend devices the model may use, in priority order, e.g. "Vulkan1" or "CUDA0" (see [NativeApi.ggml_backend_dev_name(IntPtr)](./llama.native.nativeapi.md#ggml_backend_dev_nameintptr)).
 Equivalent to --device on the llama.cpp command line or `devices` in `llama_model_params`.

```csharp
public List<string> Devices { get; set; }
```

#### Property Value

[List&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

**Remarks:**

When empty, llama.cpp picks the devices itself: all discrete GPUs, or the first integrated GPU when there is no discrete GPU.
 Setting this list explicitly bypasses that selection, which is the only way to run on an integrated GPU in a machine that also has a discrete GPU.
 [IModelParams.MainGpu](./llama.abstractions.imodelparams.md#maingpu) is an index into this list when it is non-empty. Names are matched case insensitively; a name that does not match any available device throws [UnknownDeviceException](./llama.exceptions.unknowndeviceexception.md) when the model is loaded.

### **GpuLayerCount**

Number of layers to run in VRAM / GPU memory (n_gpu_layers)

```csharp
public int GpuLayerCount { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SeqMax**

max number of sequences (i.e. distinct states for recurrent models)

```csharp
public uint SeqMax { get; set; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **RecurrentRollbackSnapshots**

The number of recurrent-state snapshots per seq for rollback

```csharp
public uint RecurrentRollbackSnapshots { get; set; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **ModelPath**

Model path (model)

```csharp
public string ModelPath { get; set; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Threads**

Number of threads (null = autodetect) (n_threads)

```csharp
public int? Threads { get; set; }
```

#### Property Value

[Int32?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **BatchThreads**

Number of threads to use for batch processing (null = autodetect) (n_threads)

```csharp
public int? BatchThreads { get; set; }
```

#### Property Value

[Int32?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **BatchSize**

maximum batch size that can be submitted at once (must be &gt;=32 to use BLAS) (n_batch)

```csharp
public uint BatchSize { get; set; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **UBatchSize**

Physical batch size

```csharp
public uint UBatchSize { get; set; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **Embeddings**

If true, extract embeddings (together with logits).

```csharp
public bool Embeddings { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **TensorSplits**

how split tensors should be distributed across GPUs

```csharp
public TensorSplitsCollection TensorSplits { get; set; }
```

#### Property Value

[TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)<br>

### **CheckTensors**

Validate model tensor data before loading

```csharp
public bool CheckTensors { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **MetadataOverrides**

Override specific metadata items in the model

```csharp
public List<MetadataOverride> MetadataOverrides { get; set; }
```

#### Property Value

[List&lt;MetadataOverride&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **LoadMode**

How the load this model

```csharp
public LLamaLoadMode LoadMode { get; set; }
```

#### Property Value

[LLamaLoadMode](./llama.native.llamaloadmode.md)<br>

### **LoadMTP**

Whether to load MTP layers

```csharp
public bool LoadMTP { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **RopeFrequencyBase**

RoPE base frequency (null to fetch from the model)

```csharp
public float? RopeFrequencyBase { get; set; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **RopeFrequencyScale**

RoPE frequency scaling factor (null to fetch from the model)

```csharp
public float? RopeFrequencyScale { get; set; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnExtrapolationFactor**

YaRN extrapolation mix factor (null = from model)

```csharp
public float? YarnExtrapolationFactor { get; set; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnAttentionFactor**

YaRN magnitude scaling factor (null = from model)

```csharp
public float? YarnAttentionFactor { get; set; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnBetaFast**

YaRN low correction dim (null = from model)

```csharp
public float? YarnBetaFast { get; set; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnBetaSlow**

YaRN high correction dim (null = from model)

```csharp
public float? YarnBetaSlow { get; set; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnOriginalContext**

YaRN original context length (null = from model)

```csharp
public uint? YarnOriginalContext { get; set; }
```

#### Property Value

[UInt32?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnScalingType**

YaRN scaling method to use.

```csharp
public RopeScalingType? YarnScalingType { get; set; }
```

#### Property Value

[RopeScalingType?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TypeK**

Override the type of the K cache

```csharp
public GGMLType? TypeK { get; set; }
```

#### Property Value

[GGMLType?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TypeV**

Override the type of the V cache

```csharp
public GGMLType? TypeV { get; set; }
```

#### Property Value

[GGMLType?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **NoKqvOffload**

Whether to disable offloading the KQV cache to the GPU

```csharp
public bool NoKqvOffload { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **FlashAttention**

Whether to use flash attention

```csharp
public bool? FlashAttention { get; set; }
```

#### Property Value

[Boolean?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **DefragThreshold**

#### Caution

This member is obsolete.

---

defragment the KV cache if holes/size &gt; defrag_threshold, Set to &lt;= 0 to disable (default)

```csharp
public float? DefragThreshold { get; set; }
```

#### Property Value

[Single?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **PoolingType**

How to pool (sum) embedding results by sequence id (ignored if no pooling layer)

```csharp
public LLamaPoolingType PoolingType { get; set; }
```

#### Property Value

[LLamaPoolingType](./llama.native.llamapoolingtype.md)<br>

### **AttentionType**

Attention type to use for embeddings

```csharp
public LLamaAttentionType AttentionType { get; set; }
```

#### Property Value

[LLamaAttentionType](./llama.native.llamaattentiontype.md)<br>

### **VocabOnly**

Load vocab only (no weights)

```csharp
public bool VocabOnly { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **OpOffload**

Offload host tensor operations to device

```csharp
public bool? OpOffload { get; set; }
```

#### Property Value

[Boolean?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **SwaFull**

Use full-size SWA cache (https://github.com/ggml-org/llama.cpp/pull/13194#issuecomment-2868343055)

```csharp
public bool? SwaFull { get; set; }
```

#### Property Value

[Boolean?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

**Remarks:**

Setting to false when n_seq_max &gt; 1 can cause bad performance in some cases
 ref: https://github.com/ggml-org/llama.cpp/pull/13845#issuecomment-2924800573

### **KVUnified**

use a unified buffer across the input sequences when computing the attention.
 try to disable when n_seq_max &gt; 1 for improved performance when the sequences do not share a large prefix
 <br>
 ref: https://github.com/ggml-org/llama.cpp/pull/14363

```csharp
public bool? KVUnified { get; set; }
```

#### Property Value

[Boolean?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **Encoding**

The encoding to use for models

```csharp
public Encoding Encoding { get; set; }
```

#### Property Value

[Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

## Constructors

### **ModelParams(String)**



```csharp
public ModelParams(string modelPath)
```

#### Parameters

`modelPath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
The model path.

### **ModelParams(ModelParams)**

```csharp
protected ModelParams(ModelParams original)
```

#### Parameters

`original` [ModelParams](./llama.common.modelparams.md)<br>

---

[`< Back`](./)
