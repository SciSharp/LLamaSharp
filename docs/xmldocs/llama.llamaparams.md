# LLamaParams

Namespace: LLama

```csharp
public struct LLamaParams
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaParams](./llama.llamaparams.md)

## Fields

### **seed**

```csharp
public int seed;
```

### **n_threads**

```csharp
public int n_threads;
```

### **n_predict**

```csharp
public int n_predict;
```

### **n_parts**

```csharp
public int n_parts;
```

### **n_ctx**

```csharp
public int n_ctx;
```

### **n_batch**

```csharp
public int n_batch;
```

### **n_keep**

```csharp
public int n_keep;
```

### **logit_bias**

```csharp
public Dictionary<int, float> logit_bias;
```

### **top_k**

```csharp
public int top_k;
```

### **top_p**

```csharp
public float top_p;
```

### **tfs_z**

```csharp
public float tfs_z;
```

### **typical_p**

```csharp
public float typical_p;
```

### **temp**

```csharp
public float temp;
```

### **repeat_penalty**

```csharp
public float repeat_penalty;
```

### **repeat_last_n**

```csharp
public int repeat_last_n;
```

### **frequency_penalty**

```csharp
public float frequency_penalty;
```

### **presence_penalty**

```csharp
public float presence_penalty;
```

### **mirostat**

```csharp
public int mirostat;
```

### **mirostat_tau**

```csharp
public float mirostat_tau;
```

### **mirostat_eta**

```csharp
public float mirostat_eta;
```

### **model**

```csharp
public string model;
```

### **prompt**

```csharp
public string prompt;
```

### **path_session**

```csharp
public string path_session;
```

### **input_prefix**

```csharp
public string input_prefix;
```

### **input_suffix**

```csharp
public string input_suffix;
```

### **antiprompt**

```csharp
public List<string> antiprompt;
```

### **lora_adapter**

```csharp
public string lora_adapter;
```

### **lora_base**

```csharp
public string lora_base;
```

### **memory_f16**

```csharp
public bool memory_f16;
```

### **random_prompt**

```csharp
public bool random_prompt;
```

### **use_color**

```csharp
public bool use_color;
```

### **interactive**

```csharp
public bool interactive;
```

### **embedding**

```csharp
public bool embedding;
```

### **interactive_first**

```csharp
public bool interactive_first;
```

### **instruct**

```csharp
public bool instruct;
```

### **penalize_nl**

```csharp
public bool penalize_nl;
```

### **perplexity**

```csharp
public bool perplexity;
```

### **use_mmap**

```csharp
public bool use_mmap;
```

### **use_mlock**

```csharp
public bool use_mlock;
```

### **mem_test**

```csharp
public bool mem_test;
```

### **verbose_prompt**

```csharp
public bool verbose_prompt;
```

## Constructors

### **LLamaParams(Int32, Int32, Int32, Int32, Int32, Int32, Int32, Dictionary&lt;Int32, Single&gt;, Int32, Single, Single, Single, Single, Single, Int32, Single, Single, Int32, Single, Single, String, String, String, String, String, List&lt;String&gt;, String, String, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean)**

```csharp
LLamaParams(int seed, int n_threads, int n_predict, int n_parts, int n_ctx, int n_batch, int n_keep, Dictionary<int, float> logit_bias, int top_k, float top_p, float tfs_z, float typical_p, float temp, float repeat_penalty, int repeat_last_n, float frequency_penalty, float presence_penalty, int mirostat, float mirostat_tau, float mirostat_eta, string model, string prompt, string path_session, string input_prefix, string input_suffix, List<string> antiprompt, string lora_adapter, string lora_base, bool memory_f16, bool random_prompt, bool use_color, bool interactive, bool embedding, bool interactive_first, bool instruct, bool penalize_nl, bool perplexity, bool use_mmap, bool use_mlock, bool mem_test, bool verbose_prompt)
```

#### Parameters

`seed` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_predict` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_parts` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_ctx` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_batch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`n_keep` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`logit_bias` [Dictionary&lt;Int32, Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

`top_k` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`top_p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`tfs_z` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`typical_p` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`temp` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`repeat_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`repeat_last_n` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`frequency_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`presence_penalty` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`mirostat` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`mirostat_tau` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`mirostat_eta` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

`model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`path_session` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`input_prefix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`input_suffix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`antiprompt` [List&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

`lora_adapter` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`lora_base` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`memory_f16` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`random_prompt` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`use_color` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`interactive` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`embedding` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`interactive_first` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`instruct` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`penalize_nl` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`perplexity` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`use_mmap` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`use_mlock` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`mem_test` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`verbose_prompt` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
