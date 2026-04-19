[`< Back`](./)

---

# IModelParams

Namespace: LLama.Abstractions

The parameters for initializing a LLama model.

```csharp
public interface IModelParams
```

Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Properties

### **MainGpu**

main_gpu interpretation depends on split_mode:

- **None** - The GPU that is used for the entire mode.
- **Row** - The GPU that is used for small tensors and intermediate results.
- **Layer** - Ignored.

```csharp
public abstract int MainGpu { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SplitMode**

How to split the model across multiple GPUs

```csharp
public abstract Nullable<GPUSplitMode> SplitMode { get; }
```

#### Property Value

[Nullable&lt;GPUSplitMode&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TensorBufferOverrides**

Buffer type overrides for specific tensor patterns, allowing you to specify hardware devices to use for individual tensors or sets of tensors.
 Equivalent to --override-tensor or -ot on the llama.cpp command line or tensor_buft_overrides internally.

```csharp
public abstract List<TensorBufferOverride> TensorBufferOverrides { get; }
```

#### Property Value

[List&lt;TensorBufferOverride&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **GpuLayerCount**

Number of layers to run in VRAM / GPU memory (n_gpu_layers)

```csharp
public abstract int GpuLayerCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **UseMemorymap**

Use mmap for faster loads (use_mmap)

```csharp
public abstract bool UseMemorymap { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UseMemoryLock**

Use mlock to keep model in memory (use_mlock)

```csharp
public abstract bool UseMemoryLock { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ModelPath**

Model path (model)

```csharp
public abstract string ModelPath { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **TensorSplits**

how split tensors should be distributed across GPUs

```csharp
public abstract TensorSplitsCollection TensorSplits { get; }
```

#### Property Value

[TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)<br>

### **VocabOnly**

Load vocab only (no weights)

```csharp
public abstract bool VocabOnly { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **CheckTensors**

Validate model tensor data before loading

```csharp
public abstract bool CheckTensors { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **MetadataOverrides**

Override specific metadata items in the model

```csharp
public abstract List<MetadataOverride> MetadataOverrides { get; }
```

#### Property Value

[List&lt;MetadataOverride&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

---

[`< Back`](./)
