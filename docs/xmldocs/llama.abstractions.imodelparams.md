# IModelParams

Namespace: LLama.Abstractions

The parameters for initializing a LLama model.

```csharp
public interface IModelParams
```

## Properties

### **MainGpu**

main_gpu interpretation depends on split_mode:
 NoneThe GPU that is used for the entire mode.RowThe GPU that is used for small tensors and intermediate results.LayerIgnored.

```csharp
public abstract int MainGpu { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SplitMode**

How to split the model across multiple GPUs

```csharp
public abstract GPUSplitMode SplitMode { get; }
```

#### Property Value

[GPUSplitMode](./llama.native.gpusplitmode.md)<br>

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

### **LoraAdapters**

List of LoRA adapters to apply

```csharp
public abstract AdapterCollection LoraAdapters { get; }
```

#### Property Value

[AdapterCollection](./llama.abstractions.adaptercollection.md)<br>

### **LoraBase**

base model path for the lora adapter (lora_base)

```csharp
public abstract string LoraBase { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **MetadataOverrides**

Override specific metadata items in the model

```csharp
public abstract List<MetadataOverride> MetadataOverrides { get; }
```

#### Property Value

[List&lt;MetadataOverride&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>
