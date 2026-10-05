[`< Back`](./)

---

# ILLamaExecutor

Namespace: LLama.Abstractions

A high level interface for LLama models.

```csharp
public interface ILLamaExecutor
```

Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute)

## Properties

### **Context**

The loaded context for this executor.

```csharp
LLamaContext Context { get; }
```

#### Property Value

[LLamaContext](./llama.llamacontext.md)<br>

### **IsMultiModal**

Identify if it's a multi-modal model and there is a image to process.

```csharp
bool IsMultiModal { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **ClipModel**

Multi-Modal Projections / Clip Model weights

```csharp
MtmdWeights? ClipModel { get; }
```

#### Property Value

[MtmdWeights](./llama.mtmdweights.md)<br>

### **Embeds**

List of media: List of media for Multi-Modal models.

```csharp
List<SafeMtmdEmbed> Embeds { get; }
```

#### Property Value

[List&lt;SafeMtmdEmbed&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

## Methods

### **InferAsync(String, IInferenceParams, CancellationToken)**

Asynchronously infers a response from the model.

```csharp
IAsyncEnumerable<string> InferAsync(string text, IInferenceParams? inferenceParams = null, CancellationToken token = null)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Your prompt

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)?<br>
Any additional parameters

`token` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
A cancellation token.

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>

---

[`< Back`](./)
