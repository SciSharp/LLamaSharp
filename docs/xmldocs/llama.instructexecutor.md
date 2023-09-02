# InstructExecutor

Namespace: LLama

The LLama executor for instruct mode.

```csharp
public class InstructExecutor : StatefulExecutorBase, LLama.Abstractions.ILLamaExecutor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [StatefulExecutorBase](./llama.statefulexecutorbase.md) → [InstructExecutor](./llama.instructexecutor.md)<br>
Implements [ILLamaExecutor](./llama.abstractions.illamaexecutor.md)

## Properties

### **Context**

The context used by the executor.

```csharp
public LLamaContext Context { get; }
```

#### Property Value

[LLamaContext](./llama.llamacontext.md)<br>

## Constructors

### **InstructExecutor(LLamaContext, String, String)**



```csharp
public InstructExecutor(LLamaContext context, string instructionPrefix, string instructionSuffix)
```

#### Parameters

`context` [LLamaContext](./llama.llamacontext.md)<br>

`instructionPrefix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`instructionSuffix` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **GetStateData()**

```csharp
public ExecutorBaseState GetStateData()
```

#### Returns

[ExecutorBaseState](./llama.statefulexecutorbase.executorbasestate.md)<br>

### **LoadState(ExecutorBaseState)**

```csharp
public void LoadState(ExecutorBaseState data)
```

#### Parameters

`data` [ExecutorBaseState](./llama.statefulexecutorbase.executorbasestate.md)<br>

### **SaveState(String)**

```csharp
public void SaveState(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **LoadState(String)**

```csharp
public void LoadState(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetLoopCondition(InferStateArgs)**

```csharp
protected bool GetLoopCondition(InferStateArgs args)
```

#### Parameters

`args` [InferStateArgs](./llama.statefulexecutorbase.inferstateargs.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **PreprocessInputs(String, InferStateArgs)**

```csharp
protected void PreprocessInputs(string text, InferStateArgs args)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`args` [InferStateArgs](./llama.statefulexecutorbase.inferstateargs.md)<br>

### **PostProcess(IInferenceParams, InferStateArgs, IEnumerable`1&)**

```csharp
protected bool PostProcess(IInferenceParams inferenceParams, InferStateArgs args, IEnumerable`1& extraOutputs)
```

#### Parameters

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`args` [InferStateArgs](./llama.statefulexecutorbase.inferstateargs.md)<br>

`extraOutputs` [IEnumerable`1&](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **InferInternal(IInferenceParams, InferStateArgs)**

```csharp
protected void InferInternal(IInferenceParams inferenceParams, InferStateArgs args)
```

#### Parameters

`inferenceParams` [IInferenceParams](./llama.abstractions.iinferenceparams.md)<br>

`args` [InferStateArgs](./llama.statefulexecutorbase.inferstateargs.md)<br>
