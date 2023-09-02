# LLamaEmbedder

Namespace: LLama

The embedder for LLama, which supports getting embeddings from text.

```csharp
public sealed class LLamaEmbedder : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaEmbedder](./llama.llamaembedder.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Constructors

### **LLamaEmbedder(IModelParams)**



```csharp
public LLamaEmbedder(IModelParams params)
```

#### Parameters

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

### **LLamaEmbedder(LLamaWeights, IModelParams)**

```csharp
public LLamaEmbedder(LLamaWeights weights, IModelParams params)
```

#### Parameters

`weights` [LLamaWeights](./llama.llamaweights.md)<br>

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

## Methods

### **GetEmbeddings(String, Int32, Boolean, String)**

#### Caution

'threads' and 'encoding' parameters are no longer used

---

Get the embeddings of the text.

```csharp
public Single[] GetEmbeddings(string text, int threads, bool addBos, string encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
unused

`addBos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Add bos to the text.

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
unused

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **GetEmbeddings(String)**

Get the embeddings of the text.

```csharp
public Single[] GetEmbeddings(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **GetEmbeddings(String, Boolean)**

Get the embeddings of the text.

```csharp
public Single[] GetEmbeddings(string text, bool addBos)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`addBos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Add bos to the text.

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Dispose()**



```csharp
public void Dispose()
```
