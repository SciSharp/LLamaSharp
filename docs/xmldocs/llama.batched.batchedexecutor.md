[`< Back`](./)

---

# BatchedExecutor

Namespace: LLama.Batched

A batched executor that can infer multiple separate "conversations" simultaneously.

```csharp
public sealed class BatchedExecutor : System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [BatchedExecutor](./llama.batched.batchedexecutor.md)<br>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

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

### **ClipModel**

The optional [MtmdWeights](./llama.mtmdweights.md) this executor is using

```csharp
public MtmdWeights? ClipModel { get; }
```

#### Property Value

[MtmdWeights](./llama.mtmdweights.md)<br>

### **BatchedTokenCount**

Get the number of tokens in the batch, waiting for [BatchedExecutor.Infer(CancellationToken)](./llama.batched.batchedexecutor.md#infercancellationtoken) to be called

```csharp
public int BatchedTokenCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BatchQueueCount**

Number of batches in the queue, waiting for [BatchedExecutor.Infer(CancellationToken)](./llama.batched.batchedexecutor.md#infercancellationtoken) to be called

```csharp
public int BatchQueueCount { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **IsDisposed**

Check if this executor has been disposed.

```csharp
public bool IsDisposed { get; private set; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **BatchedExecutor(LLamaWeights, IContextParams, MtmdWeights)**

Create a new batched executor

```csharp
public BatchedExecutor(LLamaWeights model, IContextParams contextParams, MtmdWeights? clipModel = null)
```

#### Parameters

`model` [LLamaWeights](./llama.llamaweights.md)<br>
The model to use

`contextParams` [IContextParams](./llama.abstractions.icontextparams.md)<br>
Parameters to create a new context

`clipModel` [MtmdWeights](./llama.mtmdweights.md)?<br>
Clip model to use for multimodal capabilities

## Methods

### **Create()**

Start a new [Conversation](./llama.batched.conversation.md)

```csharp
public Conversation Create()
```

#### Returns

[Conversation](./llama.batched.conversation.md)<br>

### **Load(String)**

Load a conversation that was previously saved to a file. Once loaded the conversation will
 need to be prompted.

```csharp
public Conversation Load(string filepath)
```

#### Parameters

`filepath` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Conversation](./llama.batched.conversation.md)<br>

#### Exceptions

[ObjectDisposedException](https://learn.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

### **Load(State)**

Load a conversation that was previously saved into memory. Once loaded the conversation will need to be prompted.

```csharp
public Conversation Load(State state)
```

#### Parameters

`state` [State](./llama.batched.conversation.state.md)<br>

#### Returns

[Conversation](./llama.batched.conversation.md)<br>

#### Exceptions

[ObjectDisposedException](https://learn.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

### **Infer(CancellationToken)**

Run inference for all conversations in the batch which have pending tokens.
 
 If the result is `NoKvSlot` then there is not enough memory for inference, try disposing some conversation
 threads and running inference again.

```csharp
public async Task<DecodeResult> Infer(CancellationToken cancellation = null)
```

#### Parameters

`cancellation` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;DecodeResult&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **Dispose()**

```csharp
public void Dispose()
```

---

[`< Back`](./)
