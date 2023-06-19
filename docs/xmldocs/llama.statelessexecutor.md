# StatelessExecutor

Namespace: LLama

This executor infer the input as one-time job. Previous inputs won't impact on the 
 response to current input.

```csharp
public class StatelessExecutor : LLama.Abstractions.ILLamaExecutor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [StatelessExecutor](./llama.statelessexecutor.md)<br>
Implements [ILLamaExecutor](./llama.abstractions.illamaexecutor.md)

## Properties

### **Model**

The mode used by the executor when running the inference.

```csharp
public LLamaModel Model { get; }
```

#### Property Value

[LLamaModel](./llama.llamamodel.md)<br>

## Constructors

### **StatelessExecutor(LLamaModel)**



```csharp
public StatelessExecutor(LLamaModel model)
```

#### Parameters

`model` [LLamaModel](./llama.llamamodel.md)<br>
The LLama model.

## Methods

### **Infer(String, InferenceParams, CancellationToken)**

```csharp
public IEnumerable<string> Infer(string text, InferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inferenceParams` [InferenceParams](./llama.common.inferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **InferAsync(String, InferenceParams, CancellationToken)**

```csharp
public IAsyncEnumerable<string> InferAsync(string text, InferenceParams inferenceParams, CancellationToken token)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inferenceParams` [InferenceParams](./llama.common.inferenceparams.md)<br>

`token` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>
