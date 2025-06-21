[`< Back`](./)

---

# LLamaContext

Namespace: LLama

A llama_context, which holds all the context required to interact with a model

```csharp
public sealed class LLamaContext : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLamaContext](./llama.llamacontext.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **ContextSize**

Total number of tokens in the context

```csharp
public uint ContextSize { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Params**

The context params set for this context

```csharp
public IContextParams Params { get; }
```

#### Property Value

[IContextParams](./llama.abstractions.icontextparams.md)<br>

### **NativeHandle**

The native handle, which is used to be passed to the native APIs

```csharp
public SafeLLamaContextHandle NativeHandle { get; }
```

#### Property Value

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

**Remarks:**

Be careful how you use this!

### **Encoding**

The encoding set for this model to deal with text input.

```csharp
public Encoding Encoding { get; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

### **GenerationThreads**

Get or set the number of threads to use for generation

```csharp
public int GenerationThreads { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BatchThreads**

Get or set the number of threads to use for batch processing

```csharp
public int BatchThreads { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BatchSize**

Get the maximum batch size for this context

```csharp
public uint BatchSize { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **Vocab**

Get the special tokens for the model associated with this context

```csharp
public Vocabulary Vocab { get; }
```

#### Property Value

[Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

## Constructors

### **LLamaContext(LLamaWeights, IContextParams, ILogger)**

Create a new LLamaContext for the given LLamaWeights

```csharp
public LLamaContext(LLamaWeights model, IContextParams params, ILogger logger)
```

#### Parameters

`model` [LLamaWeights](./llama.llamaweights.md)<br>

`params` [IContextParams](./llama.abstractions.icontextparams.md)<br>

`logger` ILogger<br>

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>

## Methods

### **Tokenize(String, Boolean, Boolean)**

Tokenize a string.

```csharp
public LLamaToken[] Tokenize(string text, bool addBos, bool special)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`addBos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to add a bos to the text.

`special` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.

#### Returns

[LLamaToken[]](./llama.native.llamatoken.md)<br>

### **DeTokenize(IReadOnlyList&lt;LLamaToken&gt;)**

#### Caution

Use a `StreamingTokenDecoder` instead

---

Detokenize the tokens to text.

```csharp
public string DeTokenize(IReadOnlyList<LLamaToken> tokens)
```

#### Parameters

`tokens` [IReadOnlyList&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **SaveState(String)**

Save the state to specified path.

```csharp
public void SaveState(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **SaveState(String, LLamaSeqId)**

Save the state of a particular sequence to specified path.

```csharp
public void SaveState(string filename, LLamaSeqId sequence)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

### **GetState()**

Get the state data as an opaque handle, which can be loaded later using [LLamaContext.LoadState(String)](./llama.llamacontext.md#loadstatestring)

```csharp
public State GetState()
```

#### Returns

[State](./llama.llamacontext.state.md)<br>

**Remarks:**

Use [LLamaContext.SaveState(String)](./llama.llamacontext.md#savestatestring) if you intend to save this state to disk.

### **GetState(LLamaSeqId)**

Get the state data as an opaque handle, which can be loaded later using [LLamaContext.LoadState(String)](./llama.llamacontext.md#loadstatestring)

```csharp
public SequenceState GetState(LLamaSeqId sequence)
```

#### Parameters

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

#### Returns

[SequenceState](./llama.llamacontext.sequencestate.md)<br>

**Remarks:**

Use [LLamaContext.SaveState(String, LLamaSeqId)](./llama.llamacontext.md#savestatestring-llamaseqid) if you intend to save this state to disk.

### **LoadState(String)**

Load the state from specified path.

```csharp
public void LoadState(string filename)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **LoadState(String, LLamaSeqId)**

Load the state from specified path into a particular sequence

```csharp
public void LoadState(string filename, LLamaSeqId sequence)
```

#### Parameters

`filename` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

### **LoadState(State)**

Load the state from memory.

```csharp
public void LoadState(State state)
```

#### Parameters

`state` [State](./llama.llamacontext.state.md)<br>

### **LoadState(SequenceState, LLamaSeqId)**

Load the state from memory into a particular sequence

```csharp
public void LoadState(SequenceState state, LLamaSeqId sequence)
```

#### Parameters

`state` [SequenceState](./llama.llamacontext.sequencestate.md)<br>

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

### **Encode(LLamaBatch)**



```csharp
public EncodeResult Encode(LLamaBatch batch)
```

#### Parameters

`batch` [LLamaBatch](./llama.native.llamabatch.md)<br>

#### Returns

[EncodeResult](./llama.native.encoderesult.md)<br>

### **EncodeAsync(LLamaBatch, CancellationToken)**



```csharp
public Task<EncodeResult> EncodeAsync(LLamaBatch batch, CancellationToken cancellationToken)
```

#### Parameters

`batch` [LLamaBatch](./llama.native.llamabatch.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;EncodeResult&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **Decode(LLamaBatch)**



```csharp
public DecodeResult Decode(LLamaBatch batch)
```

#### Parameters

`batch` [LLamaBatch](./llama.native.llamabatch.md)<br>

#### Returns

[DecodeResult](./llama.native.decoderesult.md)<br>

### **DecodeAsync(LLamaBatch, CancellationToken)**



```csharp
public Task<DecodeResult> DecodeAsync(LLamaBatch batch, CancellationToken cancellationToken)
```

#### Parameters

`batch` [LLamaBatch](./llama.native.llamabatch.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;DecodeResult&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **Decode(LLamaBatchEmbeddings)**



```csharp
public DecodeResult Decode(LLamaBatchEmbeddings batch)
```

#### Parameters

`batch` [LLamaBatchEmbeddings](./llama.native.llamabatchembeddings.md)<br>

#### Returns

[DecodeResult](./llama.native.decoderesult.md)<br>

### **DecodeAsync(LLamaBatchEmbeddings, CancellationToken)**



```csharp
public Task<DecodeResult> DecodeAsync(LLamaBatchEmbeddings batch, CancellationToken cancellationToken)
```

#### Parameters

`batch` [LLamaBatchEmbeddings](./llama.native.llamabatchembeddings.md)<br>

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;DecodeResult&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **DecodeAsync(List&lt;LLamaToken&gt;, LLamaSeqId, LLamaBatch, Int32)**



```csharp
public Task<ValueTuple<DecodeResult, int, int>> DecodeAsync(List<LLamaToken> tokens, LLamaSeqId id, LLamaBatch batch, int n_past)
```

#### Parameters

`tokens` [List&lt;LLamaToken&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

`id` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`batch` [LLamaBatch](./llama.native.llamabatch.md)<br>

`n_past` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Task&lt;ValueTuple&lt;DecodeResult, Int32, Int32&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
A tuple, containing the decode result, the number of tokens that have not been decoded yet and the total number of tokens that have been decoded.

### **Dispose()**

```csharp
public void Dispose()
```

---

[`< Back`](./)
