# ModelParams

Namespace: LLama.Common

```csharp
public class ModelParams
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ModelParams](./llama.common.modelparams.md)

## Properties

### **ContextSize**

Model context size (n_ctx)

```csharp
public int ContextSize { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

## Constructors

### **ModelParams(String, Int32, Int32, Int32, Boolean, Boolean, Boolean, Boolean, String, String, Int32, Int32, Boolean, Boolean)**



```csharp
public ModelParams(string modelPath, int contextSize, int gpuLayerCount, int seed, bool useFp16Memory, bool useMemorymap, bool useMemoryLock, bool perplexity, string loraAdapter, string loraBase, int threads, int batchSize, bool convertEosToNewLine, bool embeddingMode)
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
