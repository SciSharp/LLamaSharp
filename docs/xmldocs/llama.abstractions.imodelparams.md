# IModelParams

Namespace: LLama.Abstractions

The parameters for initializing a LLama model.

```csharp
public interface IModelParams
```

## Properties

### **ContextSize**

Model context size (n_ctx)

```csharp
public abstract int ContextSize { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MainGpu**

the GPU that is used for scratch and small tensors

```csharp
public abstract int MainGpu { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **LowVram**

if true, reduce VRAM usage at the cost of performance

```csharp
public abstract bool LowVram { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GpuLayerCount**

Number of layers to run in VRAM / GPU memory (n_gpu_layers)

```csharp
public abstract int GpuLayerCount { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Seed**

Seed for the random number generator (seed)

```csharp
public abstract int Seed { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **UseFp16Memory**

Use f16 instead of f32 for memory kv (memory_f16)

```csharp
public abstract bool UseFp16Memory { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UseMemorymap**

Use mmap for faster loads (use_mmap)

```csharp
public abstract bool UseMemorymap { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UseMemoryLock**

Use mlock to keep model in memory (use_mlock)

```csharp
public abstract bool UseMemoryLock { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Perplexity**

Compute perplexity over the prompt (perplexity)

```csharp
public abstract bool Perplexity { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ModelPath**

Model path (model)

```csharp
public abstract string ModelPath { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ModelAlias**

model alias

```csharp
public abstract string ModelAlias { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **LoraAdapter**

lora adapter path (lora_adapter)

```csharp
public abstract string LoraAdapter { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **LoraBase**

base model path for the lora adapter (lora_base)

```csharp
public abstract string LoraBase { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Threads**

Number of threads (-1 = autodetect) (n_threads)

```csharp
public abstract int Threads { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BatchSize**

batch size for prompt processing (must be &gt;=32 to use BLAS) (n_batch)

```csharp
public abstract int BatchSize { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ConvertEosToNewLine**

Whether to convert eos to newline during the inference.

```csharp
public abstract bool ConvertEosToNewLine { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **EmbeddingMode**

Whether to use embedding mode. (embedding) Note that if this is set to true, 
 The LLamaModel won't produce text response anymore.

```csharp
public abstract bool EmbeddingMode { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **TensorSplits**

how split tensors should be distributed across GPUs

```csharp
public abstract Single[] TensorSplits { get; set; }
```

#### Property Value

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RopeFrequencyBase**

RoPE base frequency

```csharp
public abstract float RopeFrequencyBase { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **RopeFrequencyScale**

RoPE frequency scaling factor

```csharp
public abstract float RopeFrequencyScale { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **MulMatQ**

Use experimental mul_mat_q kernels

```csharp
public abstract bool MulMatQ { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Encoding**

The encoding to use for models

```csharp
public abstract Encoding Encoding { get; set; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>
