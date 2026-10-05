[`< Back`](./)

---

# MtmdWeights

Namespace: LLama

Lightweight wrapper around the MTMD native context and its helpers.

```csharp
public sealed class MtmdWeights : System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [MtmdWeights](./llama.mtmdweights.md)<br>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

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

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SupportsAudio**

Indicates whether the model supports audio inputs.

```csharp
public bool SupportsAudio { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UsesNonCausalAttention**

Indicates whether the model decodes using the non-causal path.

```csharp
public bool UsesNonCausalAttention { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **UsesMRope**

Indicates whether the model decodes using multi-scale RoPE.

```csharp
public bool UsesMRope { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SampleRate**

Gets the audio sample rate advertised by the model.

```csharp
public int SampleRate { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **LoadFromFile(String, LLamaWeights, MtmdContextParams)**

Load weights into memory

```csharp
public static MtmdWeights LoadFromFile(string mmProject, LLamaWeights textModel, MtmdContextParams mtmdCtxParams)
```

#### Parameters

`mmProject` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
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
public static async Task<MtmdWeights> LoadFromFileAsync(string mmProject, LLamaWeights textModel, MtmdContextParams mtmdCtxParams, CancellationToken token = null)
```

#### Parameters

`mmProject` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
Path to the mmproj file

`textModel` [LLamaWeights](./llama.llamaweights.md)<br>
The text model

`mtmdCtxParams` [MtmdContextParams](./llama.native.mtmdcontextparams.md)<br>
Parameters for MTMD context creation

`token` [CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

#### Returns

[Task&lt;MtmdWeights&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **LoadMedia(String)**

Load media from disk and keep it pending for the next tokenize call.

```csharp
public SafeMtmdEmbed LoadMedia(string path)
```

#### Parameters

`path` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>

### **LoadMedia(ReadOnlySpan&lt;Byte&gt;)**

Load media from an in-memory buffer and keep it pending for the next tokenize call.

```csharp
public SafeMtmdEmbed LoadMedia(ReadOnlySpan<byte> data)
```

#### Parameters

`data` [ReadOnlySpan&lt;Byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

#### Returns

[SafeMtmdEmbed](./llama.native.safemtmdembed.md)<br>

### **ClearMedia()**

Clear any pending media buffers before or after tokenization.

```csharp
public void ClearMedia()
```

### **Tokenize(String, Boolean, Boolean, out SafeMtmdInputChunks)**

Tokenize text (with optional special tokens) against the pending media buffers.

```csharp
public int Tokenize(string text, bool addSpecial, bool parseSpecial, out SafeMtmdInputChunks chunks)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`addSpecial` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`parseSpecial` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`out` `chunks` [SafeMtmdInputChunks](./llama.native.safemtmdinputchunks.md)<br>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Tokenize(String, Boolean, Boolean, ReadOnlySpan&lt;SafeMtmdEmbed&gt;, out SafeMtmdInputChunks)**

Tokenize text (with optional special tokens) against explicit media embeddings.
 The caller retains ownership of `embeds`.

```csharp
public int Tokenize(string text, bool addSpecial, bool parseSpecial, ReadOnlySpan<SafeMtmdEmbed> embeds, out SafeMtmdInputChunks chunks)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>

`addSpecial` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`parseSpecial` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

`embeds` [ReadOnlySpan&lt;SafeMtmdEmbed&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1)<br>

`out` `chunks` [SafeMtmdInputChunks](./llama.native.safemtmdinputchunks.md)<br>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **EvaluateChunks(SafeMtmdInputChunks, SafeLLamaContextHandle, ref Int32, Int32, Int32, Boolean)**

Evaluate a chunk batch using the helper that performs mtmd encode + llama decode.

```csharp
public int EvaluateChunks(SafeMtmdInputChunks chunks, SafeLLamaContextHandle llamaContext, ref int nPast, int seqId, int nBatch, bool logitsLast)
```

#### Parameters

`chunks` [SafeMtmdInputChunks](./llama.native.safemtmdinputchunks.md)<br>

`llamaContext` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`ref` `nPast` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`seqId` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`nBatch` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`logitsLast` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **EvaluateChunk(IntPtr, SafeLLamaContextHandle, ref Int32, Int32, Int32, Boolean)**

```csharp
public int EvaluateChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, ref int nPast, int seqId, int nBatch, bool logitsLast)
```

#### Parameters

`chunkPtr` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`llamaContext` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`ref` `nPast` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`seqId` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`nBatch` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`logitsLast` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **DecodeImageChunk(IntPtr, SafeLLamaContextHandle, IntPtr, ref Int32, Int32, Int32)**

```csharp
public int DecodeImageChunk(IntPtr chunkPtr, SafeLLamaContextHandle llamaContext, IntPtr encodedEmbeddings, ref int nPast, int seqId, int nBatch)
```

#### Parameters

`chunkPtr` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`llamaContext` [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

`encodedEmbeddings` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.intptr)<br>

`ref` `nPast` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`seqId` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

`nBatch` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **Dispose()**

```csharp
public void Dispose()
```

---

[`< Back`](./)
