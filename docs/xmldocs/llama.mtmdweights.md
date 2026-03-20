[`< Back`](./)

---

# MtmdWeights

Namespace: LLama

Lightweight wrapper around the MTMD native context and its helpers.

```csharp
public sealed class MtmdWeights : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [MtmdWeights](./llama.mtmdweights.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Properties

### **NativeHandle**

The native handle, which is used in the native APIs

```csharp
public SafeMtmdModelHandle NativeHandle { get; }
```

#### Property Value

[SafeMtmdModelHandle](./llama.native.safemtmdmodelhandle.md)<br>

**Remarks:**

Be careful how you use this!

### **SupportsVision**

Indicates whether the model supports vision inputs.

```csharp
public bool SupportsVision { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SupportsAudio**

Indicates whether the model supports audio inputs.

```csharp
public bool SupportsAudio { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UsesNonCausalAttention**

Indicates whether the model decodes using the non-causal path.

```csharp
public bool UsesNonCausalAttention { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UsesMRope**

Indicates whether the model decodes using multi-scale RoPE.

```csharp
public bool UsesMRope { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **AudioBitrate**

Gets the audio bitrate advertised by the model.

```csharp
public int AudioBitrate { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **LoadFromFile(String, LLamaWeights, MtmdContextParams)**

Load weights into memory

```csharp
public static MtmdWeights LoadFromFile(string mmProject, LLamaWeights textModel, MtmdContextParams mtmdCtxParams)
```

#### Parameters

`mmProject` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the mmproj file

`textModel` [LLamaWeights](./llama.llamaweights.md)<br>
The text model

`mtmdCtxParams` [MtmdContextParams](./llama.native.mtmdcontextparams.md)<br>
Parameters for MTMD context creation

#### Returns

[MtmdWeights](./llama.mtmdweights.md)<br>

### **LoadFromFileAsync(String, LLamaWeights, MtmdContextParams, CancellationToken)**

Load weights into memory

```csharp
public static Task<MtmdWeights> LoadFromFileAsync(string mmProject, LLamaWeights textModel, MtmdContextParams mtmdCtxParams, CancellationToken token)
```

#### Parameters

`mmProject` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the mmproj file

`textModel` [LLamaWeights](./llama.llamaweights.md)<br>
The text model

`mtmdCtxParams` [MtmdContextParams](./llama.native.mtmdcontextparams.md)<br>
Parameters for MTMD context creation

`token` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;MtmdWeights&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **LoadMedia(String)**

Load media from disk and keep it pending for the next tokenize call.

```csharp
public SafeMtmdEmbed LoadMedia(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>

### **LoadMedia(ReadOnlySpan&lt;Byte&gt;)**

Load media from an in-memory buffer and keep it pending for the next tokenize call.

```csharp
public SafeMtmdEmbed LoadMedia(ReadOnlySpan<byte> data)
```

#### Parameters

`data` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>

### **LoadMediaStandalone(String)**

Load media from disk as a standalone embedding without touching the shared pending-media queue.

```csharp
public SafeMtmdEmbed LoadMediaStandalone(string path)
```

#### Parameters

`path` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>

### **LoadMediaStandalone(ReadOnlySpan&lt;Byte&gt;)**

Load media from an in-memory buffer as a standalone embedding without touching the shared pending-media queue.

```csharp
public SafeMtmdEmbed LoadMediaStandalone(ReadOnlySpan<byte> data)
```

#### Parameters

`data` [ReadOnlySpan&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>

### **ClearMedia()**

Clear any pending media buffers before or after tokenization.

```csharp
public void ClearMedia()
```

### **Tokenize(String, Boolean, Boolean, SafeMtmdInputChunks&)**

Tokenize text (with optional special tokens) against the pending media buffers.

```csharp
public int Tokenize(string text, bool addSpecial, bool parseSpecial, SafeMtmdInputChunks& chunks)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`addSpecial` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`parseSpecial` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`chunks` [SafeMtmdInputChunks&](./llama.native.safemtmdinputchunks&.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Tokenize(String, Boolean, Boolean, ReadOnlySpan&lt;SafeMtmdEmbed&gt;, SafeMtmdInputChunks&)**

Tokenize text (with optional special tokens) against explicit media embeddings.
 The caller retains ownership of `embeds`.

```csharp
public int Tokenize(string text, bool addSpecial, bool parseSpecial, ReadOnlySpan<SafeMtmdEmbed> embeds, SafeMtmdInputChunks& chunks)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`addSpecial` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`parseSpecial` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`embeds` [ReadOnlySpan&lt;SafeMtmdEmbed&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`chunks` [SafeMtmdInputChunks&](./llama.native.safemtmdinputchunks&.md)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **EvaluateChunks(SafeMtmdInputChunks, SafeLLamaContextHandle, Int32&, Int32, Int32, Boolean)**

Evaluate a chunk batch using the helper that performs mtmd encode + llama decode.

```csharp
public int EvaluateChunks(SafeMtmdInputChunks chunks, SafeLLamaContextHandle llamaContext, Int32& nPast, int seqId, int nBatch, bool logitsLast)
```

#### Parameters

`chunks` [SafeMtmdInputChunks](./llama.native.safemtmdinputchunks.md)<br>

`llamaContext` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`nPast` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`seqId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`nBatch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`logitsLast` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **EvaluateChunk(IntPtr, SafeLLamaContextHandle, Int32&, Int32, Int32, Boolean)**

```csharp
public int EvaluateChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, Int32& nPast, int seqId, int nBatch, bool logitsLast)
```

#### Parameters

`chunkPtr` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`llamaContext` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`nPast` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`seqId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`nBatch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`logitsLast` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **DecodeImageChunk(IntPtr, SafeLLamaContextHandle, IntPtr, Int32&, Int32, Int32)**

```csharp
public int DecodeImageChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, IntPtr encodedEmbeddings, Int32& nPast, int seqId, int nBatch)
```

#### Parameters

`chunkPtr` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`llamaContext` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`encodedEmbeddings` [IntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`nPast` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/system.int32&)<br>

`seqId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

`nBatch` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Dispose()**

```csharp
public void Dispose()
```

---

[`< Back`](./)
