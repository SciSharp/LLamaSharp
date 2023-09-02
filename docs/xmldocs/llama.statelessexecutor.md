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

### **Context**

The context used by the executor when running the inference.

```csharp
public LLamaContext Context { get; private set; }
```

#### Property Value

[LLamaContext](./llama.llamacontext.md)<br>

## Constructors

### **StatelessExecutor(LLamaWeights, IModelParams)**

Create a new stateless executor which will use the given model

```csharp
public StatelessExecutor(LLamaWeights weights, IModelParams params)
```

#### Parameters

`weights` [LLamaWeights](./llama.llamaweights.md)<br>

`params` [IModelParams](./llama.abstractions.imodelparams.md)<br>

### **StatelessExecutor(LLamaContext)**

#### Caution

Use the constructor which automatically creates contexts using the LLamaWeights

---

Create a new stateless executor which will use the model used to create the given context

```csharp
public StatelessExecutor(LLamaContext context)
```

#### Parameters

`context` [LLamaContext](./llama.llamacontext.md)<br>

## Methods

### **Infer(String, IInferenceParams, CancellationToken)**

```csharp
public IEnumerable<string> Infer(string text, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **InferAsync(String, IInferenceParams, CancellationToken)**

```csharp
public IAsyncEnumerable<string> InferAsync(string text, IInferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>
