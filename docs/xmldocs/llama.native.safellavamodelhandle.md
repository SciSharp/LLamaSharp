# SafeLlavaModelHandle

Namespace: LLama.Native

A reference to a set of llava model weights.

```csharp
public sealed class SafeLlavaModelHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **IsInvalid**

```csharp
public bool IsInvalid { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsClosed**

```csharp
public bool IsClosed { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **ReleaseHandle()**

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **LoadFromFile(String, Int32)**

Load a model from the given file path into memory

```csharp
public static SafeLlavaModelHandle LoadFromFile(string modelPath, int verbosity)
```

#### Parameters

`modelPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
MMP File (Multi-Modal Projections)

`verbosity` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Verbosity level

#### Returns

[SafeLlavaModelHandle](./llama.native.safellavamodelhandle.md)<br>
SafeHandle of the Clip Model

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **CreateImageEmbeddings(LLamaContext, String)**

Create the Image Embeddings.

```csharp
public SafeLlavaImageEmbedHandle CreateImageEmbeddings(LLamaContext ctxLlama, string image)
```

#### Parameters

`ctxLlama` [LLamaContext](./llama.llamacontext.md)<br>
LLama Context

`image` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Image filename (it supports jpeg format only)

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>
return the SafeHandle of these embeddings

### **CreateImageEmbeddings(LLamaContext, Byte[])**

Create the Image Embeddings.

```csharp
public SafeLlavaImageEmbedHandle CreateImageEmbeddings(LLamaContext ctxLlama, Byte[] image)
```

#### Parameters

`ctxLlama` [LLamaContext](./llama.llamacontext.md)<br>
LLama Context

`image` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
Image in binary format (it supports jpeg format only)

#### Returns

[SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>
return the SafeHandle of these embeddings

### **EvalImageEmbed(LLamaContext, SafeLlavaImageEmbedHandle, Int32&)**

Evaluates the image embeddings.

```csharp
public bool EvalImageEmbed(LLamaContext ctxLlama, SafeLlavaImageEmbedHandle imageEmbed, Int32& n_past)
```

#### Parameters

`ctxLlama` [LLamaContext](./llama.llamacontext.md)<br>
Llama Context

`imageEmbed` [SafeLlavaImageEmbedHandle](./llama.native.safellavaimageembedhandle.md)<br>
The current embeddings to evaluate

`n_past` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True on success
