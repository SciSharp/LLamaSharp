# LLamaEmbedder

Namespace: LLama

The embedder for LLama, which supports getting embeddings from text.

```csharp
public class LLamaEmbedder : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaEmbedder](./llama.llamaembedder.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Constructors

### **LLamaEmbedder(ModelParams)**



```csharp
public LLamaEmbedder(ModelParams params)
```

#### Parameters

`params` [ModelParams](./llama.common.modelparams.md)<br>

## Methods

### **GetEmbeddings(String, Int32, Boolean, String)**

Get the embeddings of the text.

```csharp
public Single[] GetEmbeddings(string text, int threads, bool addBos, string encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`threads` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Threads used for inference.

`addBos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Add bos to the text.

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **Dispose()**



```csharp
public void Dispose()
```
