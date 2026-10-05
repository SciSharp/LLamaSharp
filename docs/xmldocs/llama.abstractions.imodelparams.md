[`< Back`](./)

---

# IModelParams

Namespace: LLama.Abstractions

The parameters for initializing a LLama model.

```csharp
public interface IModelParams
```

Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Properties

### **MainGpu**

main_gpu interpretation depends on split_mode:

- **None** - The GPU that is used for the entire mode.
- **Row** - The GPU that is used for small tensors and intermediate results.
- **Layer** - Ignored.

```csharp
int MainGpu { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SplitMode**

How to split the model across multiple GPUs

```csharp
GPUSplitMode? SplitMode { get; }
```

#### Property Value

[GPUSplitMode?](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TensorBufferOverrides**

Buffer type overrides for specific tensor patterns, allowing you to specify hardware devices to use for individual tensors or sets of tensors.
 Equivalent to --override-tensor or -ot on the llama.cpp command line or tensor_buft_overrides internally.

```csharp
List<TensorBufferOverride> TensorBufferOverrides { get; }
```

#### Property Value

[List&lt;TensorBufferOverride&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **Devices**

Names of the backend devices the model may use, in priority order, e.g. "Vulkan1" or "CUDA0" (see [NativeApi.ggml_backend_dev_name(IntPtr)](./llama.native.nativeapi.md#ggml_backend_dev_nameintptr)).
 Equivalent to --device on the llama.cpp command line or `devices` in `llama_model_params`.

```csharp
List<string> Devices { get; }
```

#### Property Value

[List&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

**Remarks:**

When empty, llama.cpp picks the devices itself: all discrete GPUs, or the first integrated GPU when there is no discrete GPU.
 Setting this list explicitly bypasses that selection, which is the only way to run on an integrated GPU in a machine that also has a discrete GPU.
 [IModelParams.MainGpu](./llama.abstractions.imodelparams.md#maingpu) is an index into this list when it is non-empty. Names are matched case insensitively; a name that does not match any available device throws [UnknownDeviceException](./llama.exceptions.unknowndeviceexception.md) when the model is loaded.

### **GpuLayerCount**

Number of layers to run in VRAM / GPU memory (n_gpu_layers)

```csharp
int GpuLayerCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ModelPath**

Model path (model)

```csharp
string ModelPath { get; }
```

#### Property Value

[String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TensorSplits**

how split tensors should be distributed across GPUs

```csharp
TensorSplitsCollection TensorSplits { get; }
```

#### Property Value

[TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)<br>

### **VocabOnly**

Load vocab only (no weights)

```csharp
bool VocabOnly { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **CheckTensors**

Validate model tensor data before loading

```csharp
bool CheckTensors { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **MetadataOverrides**

Override specific metadata items in the model

```csharp
List<MetadataOverride> MetadataOverrides { get; }
```

#### Property Value

[List&lt;MetadataOverride&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **LoadMode**

How the load this model

```csharp
LLamaLoadMode LoadMode { get; }
```

#### Property Value

[LLamaLoadMode](./llama.native.llamaloadmode.md)<br>

### **LoadMTP**

Whether to load MTP layers

```csharp
bool LoadMTP { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

---

[`< Back`](./)
