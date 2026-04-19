[`< Back`](./)

---

# LLamaEmbedder

Namespace: LLama

Generate high dimensional embedding vectors from text

```csharp
public sealed class LLamaEmbedder : System.IDisposable, Microsoft.Extensions.AI.IEmbeddingGenerator`2[[System.String, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e],[Microsoft.Extensions.AI.Embedding`1[[System.Single, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], Microsoft.Extensions.AI.Abstractions, Version=9.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]], Microsoft.Extensions.AI.IEmbeddingGenerator
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaEmbedder](./llama.llamaembedder.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable), IEmbeddingGenerator&lt;String, Embedding&lt;Single&gt;&gt;, IEmbeddingGenerator<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; private set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Context**

LLama Context

```csharp
public LLamaContext Context { get; private set; }
```

#### Property Value

[LLamaContext](./llama.llamacontext.md)<br>

## Constructors

### **LLamaEmbedder(LLamaWeights, IContextParams, ILogger)**

Create a new embedder, using the given LLamaWeights

```csharp
public LLamaEmbedder(LLamaWeights weights, IContextParams params, ILogger logger)
```

#### Parameters

`weights` [LLamaWeights](./llama.llamaweights.md)<br>

`params` [IContextParams](./llama.abstractions.icontextparams.md)<br>

`logger` ILogger<br>

## Methods

### **Dispose()**

```csharp
public void Dispose()
```

### **GetEmbeddings(String, CancellationToken)**

Get high dimensional embedding vectors for the given text. Depending on the pooling type used when constructing
 this [LLamaEmbedder](./llama.llamaembedder.md) this may return an embedding vector per token, or one single embedding vector for the entire string.

```csharp
public Task<IReadOnlyList<Single[]>> GetEmbeddings(string input, CancellationToken cancellationToken)
```

#### Parameters

`input` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;IReadOnlyList&lt;Single[]&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

[NotSupportedException](https://docs.microsoft.com/en-us/dotnet/api/system.notsupportedexception)<br>

**Remarks:**

Embedding vectors are not normalized, consider using one of the extensions in [SpanNormalizationExtensions](./llama.extensions.spannormalizationextensions.md).

---

[`< Back`](./)
