# ILLamaExecutor

Namespace: LLama.Abstractions

A high level interface for LLama models.

```csharp
public interface ILLamaExecutor
```

## Properties

### **Context**

The loaded context for this executor.

```csharp
public abstract LLamaContext Context { get; }
```

#### Property Value

[LLamaContext](./llama.llamacontext.md)<br>

## Methods

### **Infer(String, IInferenceParams, CancellationToken)**

Infers a response from the model.

```csharp
IEnumerable<string> Infer(string text, IInferenceParams inferenceParams, CancellationToken token)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Your prompt

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>
Any additional parameters

`token` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
A cancellation token.

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **InferAsync(String, IInferenceParams, CancellationToken)**

Asynchronously infers a response from the model.

```csharp
IAsyncEnumerable<string> InferAsync(string text, IInferenceParams inferenceParams, CancellationToken token)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Your prompt

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>
Any additional parameters

`token` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
A cancellation token.

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>
