[`< Back`](./)

---

# SafeMtmdModelHandle

Namespace: LLama.Native

Wrapper to the Multi Modal Weights handle. This wrapper manages the low level
 operations.

```csharp
public sealed class SafeMtmdModelHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeMtmdModelHandle](./llama.native.safemtmdmodelhandle.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

## Properties

### **IsInvalid**

```csharp
public bool IsInvalid { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsClosed**

```csharp
public bool IsClosed { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **SafeMtmdModelHandle()**

```csharp
public SafeMtmdModelHandle()
```

## Methods

### **ReleaseHandle()**

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **LoadFromFile(String, LLamaWeights, MtmdContextParams)**

Load a multimodal projection model from disk and bind it to the supplied text model.

```csharp
public static SafeMtmdModelHandle LoadFromFile(string modelPath, LLamaWeights textModel, MtmdContextParams mtmdCtxParams)
```

#### Parameters

`modelPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the MMP (Multi-Modal Projections) file.

`textModel` [LLamaWeights](./llama.llamaweights.md)<br>
Text model that provides tokenizer weights for the multimodal helper.

`mtmdCtxParams` [MtmdContextParams](./llama.native.mtmdcontextparams.md)<br>
Optional context parameters; defaults are used when `null`.

#### Returns

[SafeMtmdModelHandle](./llama.native.safemtmdmodelhandle.md)<br>
Safe handle for the MTMD model.

#### Exceptions

[InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/system.invalidoperationexception)<br>
The file exists but is not readable by the current process.

[LoadWeightsFailedException](./llama.exceptions.loadweightsfailedexception.md)<br>
The native loader failed to initialize the MTMD model.

### **LoadMediaFromFile(String)**

Load media from disk and queue it for the next tokenize call.

```csharp
public SafeMtmdEmbed LoadMediaFromFile(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Absolute or relative path to the media asset.

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>
Safe handle to the media embedding.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
The model handle has been disposed.

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>
The native loader failed to ingest the file.

### **LoadMediaFromBuffer(ReadOnlySpan&lt;Byte&gt;)**

Load media from an in-memory buffer and queue it for the next tokenize call.

```csharp
public SafeMtmdEmbed LoadMediaFromBuffer(ReadOnlySpan<byte> buffer)
```

#### Parameters

`buffer` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Binary buffer containing the encoded media data.

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>
Safe handle to the media embedding.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
The model handle has been disposed.

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>
The native loader failed to ingest the buffer contents.

### **CreateMediaEmbedFromFile(String)**

Create a standalone media embedding from disk without queueing it for the next tokenize call.

```csharp
public SafeMtmdEmbed CreateMediaEmbedFromFile(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the media file on disk.

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>
Safe handle to the prepared media embedding.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
The model handle has been disposed.

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>
The native loader failed to ingest the file contents.

### **CreateMediaEmbedFromBuffer(ReadOnlySpan&lt;Byte&gt;)**

Create a standalone media embedding from an in-memory buffer without queueing it for the next tokenize call.

```csharp
public SafeMtmdEmbed CreateMediaEmbedFromBuffer(ReadOnlySpan<byte> buffer)
```

#### Parameters

`buffer` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Binary buffer containing the encoded media data.

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>
Safe handle to the prepared media embedding.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
The model handle has been disposed.

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>
The native loader failed to ingest the buffer contents.

### **ClearMedia()**

Disposes and clears any media buffers currently queued for tokenization.

```csharp
public void ClearMedia()
```

### **Tokenize(String, Boolean, Boolean, SafeMtmdInputChunks&)**

Tokenize a prompt alongside the pending media buffers. Pending media is cleared on success.

```csharp
public int Tokenize(string text, bool addSpecial, bool parseSpecial, SafeMtmdInputChunks& chunks)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Prompt text to tokenize.

`addSpecial` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to append special tokens automatically.

`parseSpecial` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether special tokens should be treated as user-provided text.

`chunks` [SafeMtmdInputChunks&](./llama.native.safemtmdinputchunks&.md)<br>
Receives the native chunk collection when tokenization succeeds.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Zero on success; otherwise the native mtmd tokenize error code.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
The model handle has been disposed.

### **Tokenize(String, Boolean, Boolean, ReadOnlySpan&lt;SafeMtmdEmbed&gt;, SafeMtmdInputChunks&)**

Tokenize a prompt alongside the provided media embeddings.
 The caller retains ownership of `embeds`.

```csharp
public int Tokenize(string text, bool addSpecial, bool parseSpecial, ReadOnlySpan<SafeMtmdEmbed> embeds, SafeMtmdInputChunks& chunks)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Prompt text to tokenize.

`addSpecial` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to append special tokens automatically.

`parseSpecial` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether special tokens should be treated as user-provided text.

`embeds` [ReadOnlySpan&lt;SafeMtmdEmbed&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>
Media embeddings to include in the multimodal prompt.

`chunks` [SafeMtmdInputChunks&](./llama.native.safemtmdinputchunks&.md)<br>
Receives the native chunk collection when tokenization succeeds.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Zero on success; otherwise the native mtmd tokenize error code.

#### Exceptions

[ObjectDisposedException](https://docs.microsoft.com/en-us/dotnet/api/system.objectdisposedexception)<br>
The model handle has been disposed.

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>
The native tokenizer failed to allocate output chunks.

### **EvaluateChunks(SafeMtmdInputChunks, SafeLLamaContextHandle, Int32&, Int32, Int32, Boolean)**

Evaluate a batch of chunks using the helper (mirrors mtmd-helper eval logic).

```csharp
public int EvaluateChunks(SafeMtmdInputChunks chunks, SafeLLamaContextHandle llamaContext, Int32& nPast, int seqId, int nBatch, bool logitsLast)
```

#### Parameters

`chunks` [SafeMtmdInputChunks](./llama.native.safemtmdinputchunks.md)<br>
Chunk collection produced by [SafeMtmdModelHandle.Tokenize(String, Boolean, Boolean, SafeMtmdInputChunks&)](./llama.native.safemtmdmodelhandle.md#tokenizestring-boolean-boolean-safemtmdinputchunks&).

`llamaContext` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
Context handle that receives the evaluated tokens.

`nPast` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
Number of past tokens; updated when evaluation succeeds.

`seqId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Sequence identifier used for KV cache management.

`nBatch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Maximum number of tokens to evaluate in a single batch.

`logitsLast` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to request logits for the last token only.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Zero on success; otherwise the native helper error code.

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown when required handles are null.

### **EvaluateChunk(IntPtr, SafeLLamaContextHandle, Int32&, Int32, Int32, Boolean)**

Evaluate a single chunk helper.

```csharp
public int EvaluateChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, Int32& nPast, int seqId, int nBatch, bool logitsLast)
```

#### Parameters

`chunkPtr` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to the chunk to evaluate.

`llamaContext` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
Context handle that receives the evaluated tokens.

`nPast` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
Number of past tokens; updated when evaluation succeeds.

`seqId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Sequence identifier used for KV cache management.

`nBatch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Maximum number of tokens to evaluate in a single batch.

`logitsLast` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether to request logits for the last token only.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Zero on success; otherwise the native helper error code.

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown when required handles are null.

### **DecodeImageChunk(IntPtr, SafeLLamaContextHandle, IntPtr, Int32&, Int32, Int32)**

Decode a prepared image chunk whose embedding is already computed.

```csharp
public int DecodeImageChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, IntPtr encodedEmbeddings, Int32& nPast, int seqId, int nBatch)
```

#### Parameters

`chunkPtr` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to the chunk whose embedding should be decoded.

`llamaContext` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
Context handle used for decoding.

`encodedEmbeddings` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>
Pointer to the pre-computed embedding data.

`nPast` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>
Number of past tokens; updated when evaluation succeeds.

`seqId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Sequence identifier used for KV cache management.

`nBatch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Maximum number of tokens to evaluate in a single batch.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Zero on success; otherwise the native helper error code.

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>
Thrown when required handles are null.

### **Finalize()**

Finalizer to ensure native resources are released if Dispose was not called.

```csharp
protected void Finalize()
```

### **DecodeUseNonCausal()**

Indicates whether the model decodes using the non-causal path.

```csharp
public bool DecodeUseNonCausal()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **DecodeUseMRope()**

Indicates whether the model decodes using multi-scale RoPE.

```csharp
public bool DecodeUseMRope()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SupportVision()**

Indicates whether the model supports vision inputs.

```csharp
public bool SupportVision()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SupportAudio()**

Indicates whether the model supports audio inputs.

```csharp
public bool SupportAudio()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **GetAudioBitrate()**

Gets the audio bitrate advertised by the model.

```csharp
public int GetAudioBitrate()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

---

[`< Back`](./)
