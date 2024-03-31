# LLamaEmbedder

Namespace: LLama

The embedder for LLama, which supports getting embeddings from text.

```csharp
public class LLamaEmbedder
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaEmbedder](./llama.llamaembedder.md)

## Constructors

### **LLamaEmbedder(LLamaParams)**

```csharp
public LLamaEmbedder(LLamaParams params)
```

#### Parameters

`params` [LLamaParams](./llama.llamaparams.md)<br>

## Methods

### **GetEmbeddings(String, Int32, Boolean)**

```csharp
public Single[] GetEmbeddings(string text, int n_thread, bool add_bos)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`n_thread` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>
