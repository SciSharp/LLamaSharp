# LLamaModel Parameters

When initializing a `LLamaModel` object, there're three parameters, `ModelParams Params, string encoding = "UTF-8", ILLamaLogger? logger = null`.

The usage of `logger` will be further introduced in [logger doc](../More/log.md). The `encoding` is the encoding you want to use when dealing with text via this model.

The most important of all, is the `ModelParams`, which is defined as below. We'll explain the parameters step by step in this document.

```cs
public class ModelParams
{
    public int ContextSize { get; set; } = 512;
    public int GpuLayerCount { get; set; } = 20;
    public int Seed { get; set; } = 1686349486;
    public bool UseFp16Memory { get; set; } = true;
    public bool UseMemorymap { get; set; } = true;
    public bool UseMemoryLock { get; set; } = false;
    public bool Perplexity { get; set; } = false;
    public string ModelPath { get; set; }
    public string LoraAdapter { get; set; } = string.Empty;
    public string LoraBase { get; set; } = string.Empty;
    public int Threads { get; set; } = Math.Max(Environment.ProcessorCount / 2, 1);
    public int BatchSize { get; set; } = 512;
    public bool ConvertEosToNewLine { get; set; } = false;
}
```


# ModelParams

Namespace: LLama.Common

```csharp
public class ModelParams
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ModelParams]()

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
