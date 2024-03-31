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
public uint seed;
```

### **n_ctx**

text context, 0 = from model

```csharp
public uint n_ctx;
```

### **n_batch**

prompt processing batch size

```csharp
public uint n_batch;
```

### **n_threads**

number of threads to use for generation

```csharp
public uint n_threads;
```

### **n_threads_batch**

number of threads to use for batch processing

```csharp
public uint n_threads_batch;
```

### **rope_scaling_type**

RoPE scaling type, from `enum llama_rope_scaling_type`

```csharp
public RopeScalingType rope_scaling_type;
```

### **rope_freq_base**

RoPE base frequency, 0 = from model

```csharp
public float rope_freq_base;
```

### **rope_freq_scale**

RoPE frequency scaling factor, 0 = from model

```csharp
public float rope_freq_scale;
```

### **yarn_ext_factor**

YaRN extrapolation mix factor, negative = from model

```csharp
public float yarn_ext_factor;
```

### **yarn_attn_factor**

YaRN magnitude scaling factor

```csharp
public float yarn_attn_factor;
```

### **yarn_beta_fast**

YaRN low correction dim

```csharp
public float yarn_beta_fast;
```

### **yarn_beta_slow**

YaRN high correction dim

```csharp
public float yarn_beta_slow;
```

### **yarn_orig_ctx**

YaRN original context size

```csharp
public uint yarn_orig_ctx;
```

### **defrag_threshold**

defragment the KV cache if holes/size &gt; defrag_threshold, Set to &lt; 0 to disable (default)

```csharp
public float defrag_threshold;
```

### **cb_eval**

ggml_backend_sched_eval_callback

```csharp
public IntPtr cb_eval;
```

### **cb_eval_user_data**

User data passed into cb_eval

```csharp
public IntPtr cb_eval_user_data;
```

### **type_k**

data type for K cache

```csharp
public GGMLType type_k;
```

### **type_v**

data type for V cache

```csharp
public GGMLType type_v;
```

## Properties

### **embedding**

embedding mode only

```csharp
public bool embedding { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **offload_kqv**

whether to offload the KQV ops (including the KV cache) to GPU

```csharp
public bool offload_kqv { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **do_pooling**

Whether to pool (sum) embedding results by sequence id (ignored if no pooling layer)

```csharp
public bool do_pooling { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
