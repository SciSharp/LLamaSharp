[`< Back`](./)

---

# LLamaModelParams

Namespace: LLama.Native

A C# representation of the llama.cpp `llama_model_params` struct

```csharp
public struct LLamaModelParams
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/en-us/dotnet/api/system.valuetype) → [LLamaModelParams](./llama.native.llamamodelparams.md)

## Fields

### **devices**

NULL-terminated list of devices to use for offloading (if NULL, all available devices are used).
 Each element is a `ggml_backend_dev_t` as returned by [NativeApi.ggml_backend_dev_get(UIntPtr)](./llama.native.nativeapi.md#ggml_backend_dev_getuintptr).

```csharp
public IntPtr* devices;
```

### **tensor_buft_overrides**

NULL-terminated list of buffer types to use for tensors that match a pattern

```csharp
public LLamaModelTensorBufferOverride* tensor_buft_overrides;
```

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

### **load_mode**

How to load the model

```csharp
public LLamaLoadMode load_mode;
```

### **main_gpu**

the GPU that is used for the entire model when split_mode is LLAMA_SPLIT_MODE_NONE

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
public LlamaProgressCallback? progress_callback;
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

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **check_tensors**

validate model tensor data

```csharp
public bool check_tensors { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **use_extra_bufts**

use extra buffer types (used for weight repacking)

```csharp
public bool use_extra_bufts { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **no_host**

bypass host buffer allowing extra buffers to be used

```csharp
public bool no_host { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **no_alloc**

only load metadata and simulate memory allocations

```csharp
public bool no_alloc { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **load_mtp**

whether to load MTP layers

```csharp
public bool load_mtp { get; set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **Default()**

Create a LLamaModelParams with default values

```csharp
public static LLamaModelParams Default()
```

#### Returns

[LLamaModelParams](./llama.native.llamamodelparams.md)<br>

---

[`< Back`](./)
