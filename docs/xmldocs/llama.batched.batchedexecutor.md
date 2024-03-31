# BatchedExecutor

Namespace: LLama.Batched

A batched executor that can infer multiple separate "conversations" simultaneously.

```csharp
public sealed class BatchedExecutor : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BatchedExecutor](./llama.batched.batchedexecutor.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

## Properties

### **Context**

The [LLamaContext](./llama.llamacontext.md) this executor is using

```csharp
public LLamaContext Context { get; }
```

#### Property Value

[LLamaContext](./llama.llamacontext.md)<br>

### **Model**

The [LLamaWeights](./llama.llamaweights.md) this executor is using

```csharp
public LLamaWeights Model { get; }
```

#### Property Value

[LLamaWeights](./llama.llamaweights.md)<br>

### **BatchedTokenCount**

Get the number of tokens in the batch, waiting for [BatchedExecutor.Infer(CancellationToken)](./llama.batched.batchedexecutor.md#infercancellationtoken) to be called

```csharp
public int BatchedTokenCount { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IsDisposed**

Check if this executor has been disposed.

```csharp
public bool IsDisposed { get; private set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **BatchedExecutor(LLamaWeights, IContextParams)**

Create a new batched executor

```csharp
public BatchedExecutor(LLamaWeights model, IContextParams contextParams)
```

#### Parameters

`model` [LLamaWeights](./llama.llamaweights.md)<br>
The model to use

`contextParams` [IContextParams](./llama.abstractions.icontextparams.md)<br>
Parameters to create a new context

## Methods

### **Prompt(String)**

#### Caution

Use BatchedExecutor.Create instead

---

Start a new [Conversation](./llama.batched.conversation.md) with the given prompt

```csharp
public Conversation Prompt(string prompt)
```

#### Parameters

`prompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Conversation](./llama.batched.conversation.md)<br>

### **Create()**

Start a new [Conversation](./llama.batched.conversation.md)

```csharp
public Conversation Create()
```

#### Returns

[Conversation](./llama.batched.conversation.md)<br>

### **Infer(CancellationToken)**

Run inference for all conversations in the batch which have pending tokens.
 
 If the result is `NoKvSlot` then there is not enough memory for inference, try disposing some conversation
 threads and running inference again.

```csharp
public Task<DecodeResult> Infer(CancellationToken cancellation)
```

#### Parameters

`cancellation` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;DecodeResult&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **Dispose()**

```csharp
public void Dispose()
```

### **GetNextSequenceId()**

```csharp
internal LLamaSeqId GetNextSequenceId()
```

#### Returns

[LLamaSeqId](./llama.native.llamaseqid.md)<br>
