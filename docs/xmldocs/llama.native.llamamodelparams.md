# LLamaModelParams

Namespace: LLama.Native

A C# representation of the llama.cpp `llama_model_params` struct

```csharp
public struct LLamaModelParams
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaModelParams](./llama.native.llamamodelparams.md)

## Fields

### **n_gpu_layers**

// number of layers to store in VRAM

```csharp
public int n_gpu_layers;
```

### **split_mode**

how to split the model across multiple GPUs

```csharp
public GPUSplitMode split_mode;
```

### **main_gpu**

the GPU that is used for scratch and small tensors

```csharp
public int main_gpu;
```

### **tensor_split**

how to split layers across multiple GPUs (size: [NativeApi.llama_max_devices()](./llama.native.nativeapi.md#llama_max_devices))

```csharp
public Single* tensor_split;
```

### **progress_callback**

called with a progress value between 0 and 1, pass NULL to disable. If the provided progress_callback
 returns true, model loading continues. If it returns false, model loading is immediately aborted.

```csharp
public LlamaProgressCallback progress_callback;
```

### **progress_callback_user_data**

context pointer passed to the progress callback

```csharp
public Void* progress_callback_user_data;
```

### **kv_overrides**

override key-value pairs of the model meta data

```csharp
public LLamaModelMetadataOverride* kv_overrides;
```

## Properties

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
