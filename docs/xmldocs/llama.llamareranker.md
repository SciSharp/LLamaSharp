[`< Back`](./)

---

# LLamaReranker

Namespace: LLama

Get rank scores between prompt and documents

```csharp
public sealed class LLamaReranker : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaReranker](./llama.llamareranker.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Context**

LLama Context

```csharp
public LLamaContext Context { get; }
```

#### Property Value

[LLamaContext](./llama.llamacontext.md)<br>

## Constructors

### **LLamaReranker(LLamaWeights, IContextParams, ILogger)**

Create a new reranker, using the given LLamaWeights

```csharp
public LLamaReranker(LLamaWeights weights, IContextParams params, ILogger logger)
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

### **GetRelevanceScores(String, IReadOnlyList&lt;String&gt;, Boolean, CancellationToken)**

Retrieve relevance scores for input and documents by reranking, execute once.

```csharp
public Task<IReadOnlyList<float>> GetRelevanceScores(string input, IReadOnlyList<string> documents, bool normalize, CancellationToken cancellationToken)
```

#### Parameters

`input` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`documents` [IReadOnlyList&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

`normalize` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to normalize the score to the range (0, 1)

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;IReadOnlyList&lt;Single&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

[NotSupportedException](https://docs.microsoft.com/en-us/dotnet/api/system.notsupportedexception)<br>

### **GetRelevanceScoreWithTokenCount(String, String, Boolean, CancellationToken)**

Retrieve relevance score for input and document by reranking

```csharp
public Task<ValueTuple<float, int>> GetRelevanceScoreWithTokenCount(string input, string document, bool normalize, CancellationToken cancellationToken)
```

#### Parameters

`input` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`document` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`normalize` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to normalize the score to the range (0, 1)

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;ValueTuple&lt;Single, Int32&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

[NotSupportedException](https://docs.microsoft.com/en-us/dotnet/api/system.notsupportedexception)<br>

---

[`< Back`](./)
