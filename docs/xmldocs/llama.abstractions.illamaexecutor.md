# ILLamaExecutor

Namespace: LLama.Abstractions

A high level interface for LLama models.

```csharp
public interface ILLamaExecutor
```

## Properties

### **Model**

The loaded model for this executor.

```csharp
public abstract LLamaModel Model { get; }
```

#### Property Value

[LLamaModel](./llama.llamamodel.md)<br>

## Methods

### **Infer(String, InferenceParams, CancellationToken)**

Infers a response from the model.

```csharp
IEnumerable<string> Infer(string text, InferenceParams inferenceParams, CancellationToken token)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Your prompt

`inferenceParams` [InferenceParams](./llama.common.inferenceparams.md)<br>
Any additional parameters

`token` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
A cancellation token.

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **InferAsync(String, InferenceParams, CancellationToken)**

```csharp
IAsyncEnumerable<string> InferAsync(string text, InferenceParams inferenceParams, CancellationToken token)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inferenceParams` [InferenceParams](./llama.common.inferenceparams.md)<br>

`token` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>
