# ModelParams

Namespace: LLama.Common

The parameters for initializing a LLama model.

```csharp
public class ModelParams : LLama.Abstractions.IModelParams, System.IEquatable`1[[LLama.Common.ModelParams, LLamaSharp, Version=0.5.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ModelParams](./llama.common.modelparams.md)<br>
Implements [IModelParams](./llama.abstractions.imodelparams.md), [IEquatable&lt;ModelParams&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **ContextSize**

Model context size (n_ctx)

```csharp
public int ContextSize { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MainGpu**

the GPU that is used for scratch and small tensors

```csharp
public int MainGpu { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LowVram**

if true, reduce VRAM usage at the cost of performance

```csharp
public bool LowVram { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GpuLayerCount**

Number of layers to run in VRAM / GPU memory (n_gpu_layers)

```csharp
public int GpuLayerCount { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Seed**

Seed for the random number generator (seed)

```csharp
public int Seed { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **UseFp16Memory**

Use f16 instead of f32 for memory kv (memory_f16)

```csharp
public bool UseFp16Memory { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UseMemorymap**

Use mmap for faster loads (use_mmap)

```csharp
public bool UseMemorymap { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UseMemoryLock**

Use mlock to keep model in memory (use_mlock)

```csharp
public bool UseMemoryLock { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Perplexity**

Compute perplexity over the prompt (perplexity)

```csharp
public bool Perplexity { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ModelPath**

Model path (model)

```csharp
public string ModelPath { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ModelAlias**

model alias

```csharp
public string ModelAlias { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **LoraAdapter**

lora adapter path (lora_adapter)

```csharp
public string LoraAdapter { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **LoraBase**

base model path for the lora adapter (lora_base)

```csharp
public string LoraBase { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Threads**

Number of threads (-1 = autodetect) (n_threads)

```csharp
public int Threads { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BatchSize**

batch size for prompt processing (must be &gt;=32 to use BLAS) (n_batch)

```csharp
public int BatchSize { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ConvertEosToNewLine**

Whether to convert eos to newline during the inference.

```csharp
public bool ConvertEosToNewLine { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **EmbeddingMode**

Whether to use embedding mode. (embedding) Note that if this is set to true, 
 The LLamaModel won't produce text response anymore.

```csharp
public bool EmbeddingMode { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **TensorSplits**

how split tensors should be distributed across GPUs

```csharp
public Single[] TensorSplits { get; set; }
```

#### Property Value

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RopeFrequencyBase**

RoPE base frequency

```csharp
public float RopeFrequencyBase { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RopeFrequencyScale**

RoPE frequency scaling factor

```csharp
public float RopeFrequencyScale { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **MulMatQ**

Use experimental mul_mat_q kernels

```csharp
public bool MulMatQ { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Encoding**

The encoding to use to convert text for the model

```csharp
public Encoding Encoding { get; set; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

## Constructors

### **ModelParams(String)**



```csharp
public ModelParams(string modelPath)
```

#### Parameters

`modelPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The model path.

### **ModelParams(String, Int32, Int32, Int32, Boolean, Boolean, Boolean, Boolean, String, String, Int32, Int32, Boolean, Boolean, Single, Single, Boolean, String)**

#### Caution

Use object initializer to set all optional parameters

---



```csharp
public ModelParams(string modelPath, int contextSize, int gpuLayerCount, int seed, bool useFp16Memory, bool useMemorymap, bool useMemoryLock, bool perplexity, string loraAdapter, string loraBase, int threads, int batchSize, bool convertEosToNewLine, bool embeddingMode, float ropeFrequencyBase, float ropeFrequencyScale, bool mulMatQ, string encoding)
```

#### Parameters

`modelPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The model path.

`contextSize` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Model context size (n_ctx)

`gpuLayerCount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of layers to run in VRAM / GPU memory (n_gpu_layers)

`seed` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Seed for the random number generator (seed)

`useFp16Memory` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to use f16 instead of f32 for memory kv (memory_f16)

`useMemorymap` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to use mmap for faster loads (use_mmap)

`useMemoryLock` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to use mlock to keep model in memory (use_mlock)

`perplexity` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Thether to compute perplexity over the prompt (perplexity)

`loraAdapter` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Lora adapter path (lora_adapter)

`loraBase` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Base model path for the lora adapter (lora_base)

`threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of threads (-1 = autodetect) (n_threads)

`batchSize` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Batch size for prompt processing (must be &gt;=32 to use BLAS) (n_batch)

`convertEosToNewLine` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to convert eos to newline during the inference.

`embeddingMode` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to use embedding mode. (embedding) Note that if this is set to true, The LLamaModel won't produce text response anymore.

`ropeFrequencyBase` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
RoPE base frequency.

`ropeFrequencyScale` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
RoPE frequency scaling factor

`mulMatQ` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Use experimental mul_mat_q kernels

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The encoding to use to convert text for the model

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **PrintMembers(StringBuilder)**

```csharp
protected bool PrintMembers(StringBuilder builder)
```

#### Parameters

`builder` [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(ModelParams)**

```csharp
public bool Equals(ModelParams other)
```

#### Parameters

`other` [ModelParams](./llama.common.modelparams.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public ModelParams <Clone>$()
```

#### Returns

[ModelParams](./llama.common.modelparams.md)<br>
