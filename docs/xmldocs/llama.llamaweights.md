# LLamaWeights

Namespace: LLama

A set of model weights, loaded into memory.

```csharp
public sealed class LLamaWeights : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaWeights](./llama.llamaweights.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **NativeHandle**

The native handle, which is used in the native APIs

```csharp
public SafeLlamaModelHandle NativeHandle { get; }
```

#### Property Value

[SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

**Remarks:**

Be careful how you use this!

### **Encoding**

Encoding to use to convert text into bytes for the model

```csharp
public Encoding Encoding { get; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

### **VocabCount**

Total number of tokens in vocabulary of this model

```csharp
public int VocabCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **ContextSize**

Total number of tokens in the context

```csharp
public int ContextSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **LoadFromFile(IModelParams)**

Load weights into memory

```csharp
public static LLamaWeights LoadFromFile(IModelParams params)
```

#### Parameters

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

#### Returns

[LLamaWeights](./llama.llamaweights.md)<br>

### **Dispose()**

```csharp
public void Dispose()
```

### **CreateContext(IModelParams)**

Create a llama_context using this model

```csharp
public LLamaContext CreateContext(IModelParams params)
```

#### Parameters

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

#### Returns

[LLamaContext](./llama.llamacontext.md)<br>
