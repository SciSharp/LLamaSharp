# StatefulExecutorBase

Namespace: LLama

The base class for stateful LLama executors.

```csharp
public abstract class StatefulExecutorBase : LLama.Abstractions.ILLamaExecutor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [StatefulExecutorBase](./llama.statefulexecutorbase.md)<br>
Implements [ILLamaExecutor](./llama.abstractions.illamaexecutor.md)

## Properties

### **Model**

The mode used by the executor.

```csharp
public LLamaModel Model { get; }
```

#### Property Value

[LLamaModel](./llama.llamamodel.md)<br>

## Methods

### **WithSessionFile(String)**

This API is currently not verified.

```csharp
public StatefulExecutorBase WithSessionFile(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[StatefulExecutorBase](./llama.statefulexecutorbase.md)<br>

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **SaveSessionFile(String)**

This API has not been verified currently.

```csharp
public void SaveSessionFile(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **HandleRunOutOfContext(Int32)**

After running out of the context, take some tokens from the original prompt and recompute the logits in batches.

```csharp
protected void HandleRunOutOfContext(int tokensToKeep)
```

#### Parameters

`tokensToKeep` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **TryReuseMathingPrefix()**

Try to reuse the matching prefix from the session file.

```csharp
protected void TryReuseMathingPrefix()
```

### **GetLoopCondition(InferStateArgs)**

Decide whether to continue the loop.

```csharp
protected abstract bool GetLoopCondition(InferStateArgs args)
```

#### Parameters

`args` [InferStateArgs](./llama.statefulexecutorbase.inferstateargs.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **PreprocessInputs(String, InferStateArgs)**

Preprocess the inputs before the inference.

```csharp
protected abstract void PreprocessInputs(string text, InferStateArgs args)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`args` [InferStateArgs](./llama.statefulexecutorbase.inferstateargs.md)<br>

### **PostProcess(InferenceParams, InferStateArgs, IEnumerable`1&)**

Do some post processing after the inference.

```csharp
protected abstract bool PostProcess(InferenceParams inferenceParams, InferStateArgs args, IEnumerable`1& extraOutputs)
```

#### Parameters

`inferenceParams` [InferenceParams](./llama.common.inferenceparams.md)<br>

`args` [InferStateArgs](./llama.statefulexecutorbase.inferstateargs.md)<br>

`extraOutputs` [IEnumerable`1&](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **InferInternal(InferenceParams, InferStateArgs)**

The core inference logic.

```csharp
protected abstract void InferInternal(InferenceParams inferenceParams, InferStateArgs args)
```

#### Parameters

`inferenceParams` [InferenceParams](./llama.common.inferenceparams.md)<br>

`args` [InferStateArgs](./llama.statefulexecutorbase.inferstateargs.md)<br>

### **SaveState(String)**

Save the current state to a file.

```csharp
public abstract void SaveState(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetStateData()**

Get the current state data.

```csharp
public abstract ExecutorBaseState GetStateData()
```

#### Returns

[ExecutorBaseState](./llama.statefulexecutorbase.executorbasestate.md)<br>

### **LoadState(ExecutorBaseState)**

Load the state from data.

```csharp
public abstract void LoadState(ExecutorBaseState data)
```

#### Parameters

`data` [ExecutorBaseState](./llama.statefulexecutorbase.executorbasestate.md)<br>

### **LoadState(String)**

Load the state from a file.

```csharp
public abstract void LoadState(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Infer(String, InferenceParams, CancellationToken)**

Execute the inference.

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

Execute the inference asynchronously.

```csharp
public IAsyncEnumerable<string> InferAsync(string text, InferenceParams inferenceParams, CancellationToken cancellationToken)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`inferenceParams` [InferenceParams](./llama.common.inferenceparams.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[IAsyncEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)<br>
