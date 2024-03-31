# ModelParams

Namespace: LLama.Common

The parameters for initializing a LLama model.

```csharp
public class ModelParams : LLama.Abstractions.ILLamaParams, LLama.Abstractions.IModelParams, LLama.Abstractions.IContextParams, System.IEquatable`1[[LLama.Common.ModelParams, LLamaSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ModelParams](./llama.common.modelparams.md)<br>
Implements [ILLamaParams](./llama.abstractions.illamaparams.md), [IModelParams](./llama.abstractions.imodelparams.md), [IContextParams](./llama.abstractions.icontextparams.md), [IEquatable&lt;ModelParams&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1)

## Properties

### **ContextSize**

```csharp
public Nullable<uint> ContextSize { get; set; }
```

#### Property Value

[Nullable&lt;UInt32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **MainGpu**

```csharp
public int MainGpu { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **SplitMode**

```csharp
public GPUSplitMode SplitMode { get; set; }
```

#### Property Value

[GPUSplitMode](./llama.native.gpusplitmode.md)<br>

### **GpuLayerCount**

```csharp
public int GpuLayerCount { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Seed**

```csharp
public uint Seed { get; set; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **UseMemorymap**

```csharp
public bool UseMemorymap { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UseMemoryLock**

```csharp
public bool UseMemoryLock { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ModelPath**

```csharp
public string ModelPath { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **LoraAdapters**

```csharp
public AdapterCollection LoraAdapters { get; set; }
```

#### Property Value

[AdapterCollection](./llama.abstractions.adaptercollection.md)<br>

### **LoraBase**

```csharp
public string LoraBase { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Threads**

```csharp
public Nullable<uint> Threads { get; set; }
```

#### Property Value

[Nullable&lt;UInt32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **BatchThreads**

```csharp
public Nullable<uint> BatchThreads { get; set; }
```

#### Property Value

[Nullable&lt;UInt32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **BatchSize**

```csharp
public uint BatchSize { get; set; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **EmbeddingMode**

```csharp
public bool EmbeddingMode { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **TensorSplits**

```csharp
public TensorSplitsCollection TensorSplits { get; set; }
```

#### Property Value

[TensorSplitsCollection](./llama.abstractions.tensorsplitscollection.md)<br>

### **MetadataOverrides**

```csharp
public List<MetadataOverride> MetadataOverrides { get; set; }
```

#### Property Value

[List&lt;MetadataOverride&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **RopeFrequencyBase**

```csharp
public Nullable<float> RopeFrequencyBase { get; set; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **RopeFrequencyScale**

```csharp
public Nullable<float> RopeFrequencyScale { get; set; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnExtrapolationFactor**

```csharp
public Nullable<float> YarnExtrapolationFactor { get; set; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnAttentionFactor**

```csharp
public Nullable<float> YarnAttentionFactor { get; set; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnBetaFast**

```csharp
public Nullable<float> YarnBetaFast { get; set; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnBetaSlow**

```csharp
public Nullable<float> YarnBetaSlow { get; set; }
```

#### Property Value

[Nullable&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnOriginalContext**

```csharp
public Nullable<uint> YarnOriginalContext { get; set; }
```

#### Property Value

[Nullable&lt;UInt32&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **YarnScalingType**

```csharp
public Nullable<RopeScalingType> YarnScalingType { get; set; }
```

#### Property Value

[Nullable&lt;RopeScalingType&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TypeK**

```csharp
public Nullable<GGMLType> TypeK { get; set; }
```

#### Property Value

[Nullable&lt;GGMLType&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **TypeV**

```csharp
public Nullable<GGMLType> TypeV { get; set; }
```

#### Property Value

[Nullable&lt;GGMLType&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)<br>

### **NoKqvOffload**

```csharp
public bool NoKqvOffload { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **DefragThreshold**

```csharp
public float DefragThreshold { get; set; }
```

#### Property Value

[Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **DoPooling**

```csharp
public bool DoPooling { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **VocabOnly**

```csharp
public bool VocabOnly { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Encoding**

```csharp
public Encoding Encoding { get; set; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

## Constructors

### **ModelParams(String)**



```csharp
public ModelParams(string modelPath)
```

#### Parameters

`modelPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The model path.

## Methods

### **ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **PrintMembers(StringBuilder)**

```csharp
protected bool PrintMembers(StringBuilder builder)
```

#### Parameters

`builder` [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Equals(ModelParams)**

```csharp
public bool Equals(ModelParams other)
```

#### Parameters

`other` [ModelParams](./llama.common.modelparams.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **&lt;Clone&gt;$()**

```csharp
public ModelParams <Clone>$()
```

#### Returns

[ModelParams](./llama.common.modelparams.md)<br>
