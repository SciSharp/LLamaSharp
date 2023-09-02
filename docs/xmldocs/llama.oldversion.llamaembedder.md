# LLamaEmbedder

Namespace: LLama.OldVersion

#### Caution

The entire LLama.OldVersion namespace will be removed

---

```csharp
public class LLamaEmbedder : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaEmbedder](./llama.oldversion.llamaembedder.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Constructors

### **LLamaEmbedder(LLamaParams)**

```csharp
public LLamaEmbedder(LLamaParams params)
```

#### Parameters

`params` [LLamaParams](./llama.oldversion.llamaparams.md)<br>

## Methods

### **GetEmbeddings(String, Int32, Boolean, String)**

```csharp
public Single[] GetEmbeddings(string text, int n_thread, bool add_bos, string encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`n_thread` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`encoding` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Single[]](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

### **Dispose()**

```csharp
public void Dispose()
```
