[`< Back`](./)

---

# SafeLLamaContextHandle

Namespace: LLama.Native

A safe wrapper around a llama_context

```csharp
public sealed class SafeLLamaContextHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

## Fields

### **handle**

```csharp
protected IntPtr handle;
```

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

### **BatchSize**

Get the maximum batch size for this context

```csharp
public uint BatchSize { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **UBatchSize**

Get the physical maximum batch size for this context

```csharp
public uint UBatchSize { get; }
```

#### Property Value

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **GenerationThreads**

Get or set the number of threads used for generation of a single token.

```csharp
public int GenerationThreads { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BatchThreads**

Get or set the number of threads used for prompt and batch processing (multiple token).

```csharp
public int BatchThreads { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **PoolingType**

Get the pooling type for this context

```csharp
public LLamaPoolingType PoolingType { get; }
```

#### Property Value

[LLamaPoolingType](./llama.native.llamapoolingtype.md)<br>

### **ModelHandle**

Get the model which this context is using

```csharp
public SafeLlamaModelHandle ModelHandle { get; }
```

#### Property Value

[SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

### **Vocab**

Get the vocabulary for the model this context is using

```csharp
public Vocabulary Vocab { get; }
```

#### Property Value

[Vocabulary](./llama.native.safellamamodelhandle.vocabulary.md)<br>

### **KvCacheCanShift**

Check if the context supports KV cache shifting

```csharp
public bool KvCacheCanShift { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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

### **SafeLLamaContextHandle()**

```csharp
public SafeLLamaContextHandle()
```

## Methods

### **ReleaseHandle()**

```csharp
protected bool ReleaseHandle()
```

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Create(SafeLlamaModelHandle, LLamaContextParams)**

Create a new llama_state for the given model

```csharp
public static SafeLLamaContextHandle Create(SafeLlamaModelHandle model, LLamaContextParams lparams)
```

#### Parameters

`model` [SafeLlamaModelHandle](./llama.native.safellamamodelhandle.md)<br>

`lparams` [LLamaContextParams](./llama.native.llamacontextparams.md)<br>

#### Returns

[SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **AddLoraAdapter(LoraAdapter, Single)**

Add a LoRA adapter to this context

```csharp
public void AddLoraAdapter(LoraAdapter lora, float scale)
```

#### Parameters

`lora` [LoraAdapter](./llama.native.loraadapter.md)<br>

`scale` [Single](https://docs.microsoft.com/en-us/dotnet/api/system.single)<br>

#### Exceptions

[ArgumentException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **RemoveLoraAdapter(LoraAdapter)**

Remove a LoRA adapter from this context

```csharp
public bool RemoveLoraAdapter(LoraAdapter lora)
```

#### Parameters

`lora` [LoraAdapter](./llama.native.loraadapter.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Indicates if the lora was in this context and was remove

### **ClearLoraAdapters()**

Remove all LoRA adapters from this context

```csharp
public void ClearLoraAdapters()
```

### **GetLogits(Int32)**

Token logits obtained from the last call to llama_decode.
 The logits for the last token are stored in the last row.
 Only tokens with `logits = true` requested are present.<br>
 Can be mutated in order to change the probabilities of the next token.<br>
 Rows: n_tokens<br>
 Cols: n_vocab

```csharp
public Span<float> GetLogits(int numTokens)
```

#### Parameters

`numTokens` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
The amount of tokens whose logits should be retrieved, in [numTokens X n_vocab] format.<br>
 Tokens' order is based on their order in the LlamaBatch (so, first tokens are first, etc).<br>
 This is helpful when requesting logits for many tokens in a sequence, or want to decode multiple sequences in one go.

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **GetLogitsIth(Int32)**

Logits for the ith token. Equivalent to: llama_get_logits(ctx) + i*n_vocab

```csharp
public Span<float> GetLogitsIth(int i)
```

#### Parameters

`i` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **GetEmbeddingsIth(LLamaPos)**

Get the embeddings for the ith sequence.
 Equivalent to: llama_get_embeddings(ctx) + ctx-&gt;output_ids[i]*n_embd

```csharp
public Span<float> GetEmbeddingsIth(LLamaPos pos)
```

#### Parameters

`pos` [LLamaPos](./llama.native.llamapos.md)<br>

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A pointer to the first float in an embedding, length = ctx.EmbeddingSize

### **GetEmbeddingsSeq(LLamaSeqId)**

Get the embeddings for the a specific sequence.
 Equivalent to: llama_get_embeddings(ctx) + ctx-&gt;output_ids[i]*n_embd

```csharp
public Span<float> GetEmbeddingsSeq(LLamaSeqId seq)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

#### Returns

[Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A pointer to the first float in an embedding, length = ctx.EmbeddingSize

### **Tokenize(String, Boolean, Boolean, Encoding)**

Convert the given text into tokens

```csharp
public LLamaToken[] Tokenize(string text, bool add_bos, bool special, Encoding encoding)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The text to tokenize

`add_bos` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether the "BOS" token should be added

`special` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>
Encoding to use for the text

#### Returns

[LLamaToken[]](./llama.native.llamatoken.md)<br>

#### Exceptions

[RuntimeError](./llama.exceptions.runtimeerror.md)<br>

### **TokenToSpan(LLamaToken, Span&lt;Byte&gt;)**

Convert a single llama token into bytes

```csharp
public uint TokenToSpan(LLamaToken token, Span<byte> dest)
```

#### Parameters

`token` [LLamaToken](./llama.native.llamatoken.md)<br>
Token to decode

`dest` [Span&lt;Byte&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A span to attempt to write into. If this is too small nothing will be written

#### Returns

[UInt32](https://docs.microsoft.com/en-us/dotnet/api/system.uint32)<br>
The size of this token. **nothing will be written** if this is larger than `dest`

### **Synchronize()**

Wait until all computations are finished. This is automatically done when using any of the functions to obtain computation results
 and is not necessary to call it explicitly in most cases.

```csharp
public void Synchronize()
```

### **Encode(LLamaBatch)**

Processes a batch of tokens with the encoder part of the encoder-decoder model. Stores the encoder output
 internally for later use by the decoder cross-attention layers.

```csharp
public DecodeResult Encode(LLamaBatch batch)
```

#### Parameters

`batch` [LLamaBatch](./llama.native.llamabatch.md)<br>

#### Returns

[DecodeResult](./llama.native.decoderesult.md)<br>
0 = success <br>&lt; 0 = error (the KV cache state is restored to the state before this call)

### **Decode(LLamaBatch)**



```csharp
public DecodeResult Decode(LLamaBatch batch)
```

#### Parameters

`batch` [LLamaBatch](./llama.native.llamabatch.md)<br>

#### Returns

[DecodeResult](./llama.native.decoderesult.md)<br>
Positive return values does not mean a fatal error, but rather a warning:<br>
 - 0: success<br>
 - 1: could not find a KV slot for the batch (try reducing the size of the batch or increase the context)<br>
 - &lt; 0: error (the KV cache state is restored to the state before this call)<br>

### **Decode(LLamaBatchEmbeddings)**



```csharp
public DecodeResult Decode(LLamaBatchEmbeddings batch)
```

#### Parameters

`batch` [LLamaBatchEmbeddings](./llama.native.llamabatchembeddings.md)<br>

#### Returns

[DecodeResult](./llama.native.decoderesult.md)<br>
Positive return values does not mean a fatal error, but rather a warning:<br>
 - 0: success<br>
 - 1: could not find a KV slot for the batch (try reducing the size of the batch or increase the context)<br>
 - &lt; 0: error<br>

### **GetStateSize()**

Get the size of the state, when saved as bytes

```csharp
public UIntPtr GetStateSize()
```

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

### **GetStateSize(LLamaSeqId)**

Get the size of the KV cache for a single sequence ID, when saved as bytes

```csharp
public UIntPtr GetStateSize(LLamaSeqId sequence)
```

#### Parameters

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

### **GetState(Byte*, UIntPtr)**

Get the raw state of this context, encoded as bytes. Data is written into the `dest` pointer.

```csharp
public UIntPtr GetState(Byte* dest, UIntPtr size)
```

#### Parameters

`dest` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>
Destination to write to

`size` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes available to write to in dest (check required size with `GetStateSize()`)

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
The number of bytes written to dest

#### Exceptions

[ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
Thrown if dest is too small

### **GetState(Byte*, UIntPtr, LLamaSeqId)**

Get the raw state of a single sequence from this context, encoded as bytes. Data is written into the `dest` pointer.

```csharp
public UIntPtr GetState(Byte* dest, UIntPtr size, LLamaSeqId sequence)
```

#### Parameters

`dest` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>
Destination to write to

`size` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes available to write to in dest (check required size with `GetStateSize()`)

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>
The sequence to get state data for

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
The number of bytes written to dest

### **SetState(Byte*, UIntPtr)**

Set the raw state of this context

```csharp
public UIntPtr SetState(Byte* src, UIntPtr size)
```

#### Parameters

`src` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>
The pointer to read the state from

`size` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes that can be safely read from the pointer

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes read from the src pointer

### **SetState(Byte*, UIntPtr, LLamaSeqId)**

Set the raw state of a single sequence

```csharp
public UIntPtr SetState(Byte* src, UIntPtr size, LLamaSeqId sequence)
```

#### Parameters

`src` [Byte*](https://docs.microsoft.com/en-us/dotnet/api/system.byte*)<br>
The pointer to read the state from

`size` [UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes that can be safely read from the pointer

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>
Sequence ID to set

#### Returns

[UIntPtr](https://docs.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes read from the src pointer

### **GetTimings()**

Get performance information

```csharp
public LLamaPerfContextTimings GetTimings()
```

#### Returns

[LLamaPerfContextTimings](./llama.native.llamaperfcontexttimings.md)<br>

### **ResetTimings()**

Reset all performance information for this context

```csharp
public void ResetTimings()
```

### **KvCacheUpdate()**

Apply KV cache updates (such as K-shifts, defragmentation, etc.)

```csharp
public void KvCacheUpdate()
```

### **KvCacheDefrag()**

Defragment the KV cache. This will be applied:
 - lazily on next llama_decode()
 - explicitly with llama_kv_self_update()

```csharp
public void KvCacheDefrag()
```

### **KvCacheGetDebugView(Int32)**

Get a new KV cache view that can be used to debug the KV cache

```csharp
public LLamaKvCacheViewSafeHandle KvCacheGetDebugView(int maxSequences)
```

#### Parameters

`maxSequences` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[LLamaKvCacheViewSafeHandle](./llama.native.llamakvcacheviewsafehandle.md)<br>

### **KvCacheCountCells()**

Count the number of used cells in the KV cache (i.e. have at least one sequence assigned to them)

```csharp
public int KvCacheCountCells()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **KvCacheCountTokens()**

Returns the number of tokens in the KV cache (slow, use only for debug)
 If a KV cell has multiple sequences assigned to it, it will be counted multiple times

```csharp
public int KvCacheCountTokens()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **KvCacheClear()**

Clear the KV cache - both cell info is erased and KV data is zeroed

```csharp
public void KvCacheClear()
```

### **KvCacheRemove(LLamaSeqId, LLamaPos, LLamaPos)**

Removes all tokens that belong to the specified sequence and have positions in [p0, p1)

```csharp
public void KvCacheRemove(LLamaSeqId seq, LLamaPos p0, LLamaPos p1)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

### **KvCacheSequenceCopy(LLamaSeqId, LLamaSeqId, LLamaPos, LLamaPos)**

Copy all tokens that belong to the specified sequence to another sequence. Note that
 this does not allocate extra KV cache memory - it simply assigns the tokens to the
 new sequence

```csharp
public void KvCacheSequenceCopy(LLamaSeqId src, LLamaSeqId dest, LLamaPos p0, LLamaPos p1)
```

#### Parameters

`src` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`dest` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

### **KvCacheSequenceKeep(LLamaSeqId)**

Removes all tokens that do not belong to the specified sequence

```csharp
public void KvCacheSequenceKeep(LLamaSeqId seq)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

### **KvCacheSequenceAdd(LLamaSeqId, LLamaPos, LLamaPos, Int32)**

Adds relative position "delta" to all tokens that belong to the specified sequence
 and have positions in [p0, p1. If the KV cache is RoPEd, the KV data is updated
 accordingly

```csharp
public void KvCacheSequenceAdd(LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

`delta` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **KvCacheSequenceDivide(LLamaSeqId, LLamaPos, LLamaPos, Int32)**

Integer division of the positions by factor of `d &gt; 1`.
 If the KV cache is RoPEd, the KV data is updated accordingly.<br>
 p0 &lt; 0 : [0, p1]<br>
 p1 &lt; 0 : [p0, inf)

```csharp
public void KvCacheSequenceDivide(LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int divisor)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

`divisor` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **KvCacheMaxPosition(LLamaSeqId)**

Returns the largest position present in the KV cache for the specified sequence

```csharp
public LLamaPos KvCacheMaxPosition(LLamaSeqId seq)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

#### Returns

[LLamaPos](./llama.native.llamapos.md)<br>

---

[`< Back`](./)
