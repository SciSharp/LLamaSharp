# LLamaContextParams

Namespace: LLama.Native

```csharp
public struct LLamaContextParams
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaContextParams](./llama.native.llamacontextparams.md)

## Fields

### **n_ctx**

text context

```csharp
public int n_ctx;
```

### **n_gpu_layers**

number of layers to store in VRAM

```csharp
public int n_gpu_layers;
```

### **seed**

RNG seed, -1 for random

```csharp
public int seed;
```

### **f16_kv**

use fp16 for KV cache

```csharp
public bool f16_kv;
```

### **logits_all**

the llama_eval() call computes all logits, not just the last one

```csharp
public bool logits_all;
```

### **vocab_only**

only load the vocabulary, no weights

```csharp
public bool vocab_only;
```

### **use_mmap**

use mmap if possible

```csharp
public bool use_mmap;
```

### **use_mlock**

force system to keep model in RAM

```csharp
public bool use_mlock;
```

### **embedding**

embedding mode only

```csharp
public bool embedding;
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
