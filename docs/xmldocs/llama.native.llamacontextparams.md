# LLamaContextParams

Namespace: LLama.Native

A C# representation of the llama.cpp `llama_context_params` struct

```csharp
public struct LLamaContextParams
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaContextParams](./llama.native.llamacontextparams.md)

## Fields

### **seed**

RNG seed, -1 for random

```csharp
public int seed;
```

### **n_ctx**

text context

```csharp
public int n_ctx;
```

### **n_batch**

prompt processing batch size

```csharp
public int n_batch;
```

### **n_gpu_layers**

number of layers to store in VRAM

```csharp
public int n_gpu_layers;
```

### **main_gpu**

the GPU that is used for scratch and small tensors

```csharp
public int main_gpu;
```

### **tensor_split**

how to split layers across multiple GPUs

```csharp
public IntPtr tensor_split;
```

### **rope_freq_base**

ref: https://github.com/ggerganov/llama.cpp/pull/2054
 RoPE base frequency

```csharp
public float rope_freq_base;
```

### **rope_freq_scale**

ref: https://github.com/ggerganov/llama.cpp/pull/2054
 RoPE frequency scaling factor

```csharp
public float rope_freq_scale;
```

### **progress_callback**

called with a progress value between 0 and 1, pass NULL to disable

```csharp
public IntPtr progress_callback;
```

### **progress_callback_user_data**

context pointer passed to the progress callback

```csharp
public IntPtr progress_callback_user_data;
```

## Properties

### **low_vram**

if true, reduce VRAM usage at the cost of performance

```csharp
public bool low_vram { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **mul_mat_q**

if true, use experimental mul_mat_q kernels

```csharp
public bool mul_mat_q { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **f16_kv**

use fp16 for KV cache

```csharp
public bool f16_kv { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **logits_all**

the llama_eval() call computes all logits, not just the last one

```csharp
public bool logits_all { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **vocab_only**

only load the vocabulary, no weights

```csharp
public bool vocab_only { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **use_mmap**

use mmap if possible

```csharp
public bool use_mmap { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **use_mlock**

force system to keep model in RAM

```csharp
public bool use_mlock { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **embedding**

embedding mode only

```csharp
public bool embedding { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
