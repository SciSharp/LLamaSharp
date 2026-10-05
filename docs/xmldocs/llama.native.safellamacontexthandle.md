[`< Back`](./)

---

# SafeLLamaContextHandle

Namespace: LLama.Native

A safe wrapper around a llama_context

```csharp
public sealed class SafeLLamaContextHandle : SafeLLamaHandleBase, System.IDisposable
```

Inheritance [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object) → [CriticalFinalizerObject](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.constrainedexecution.criticalfinalizerobject) → [SafeHandle](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.safehandle) → [SafeLLamaHandleBase](./llama.native.safellamahandlebase.md) → [SafeLLamaContextHandle](./llama.native.safellamacontexthandle.md)<br>
Implements [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable)<br>
Attributes [NullableContextAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullablecontextattribute), [NullableAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.nullableattribute)

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

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **EmbeddingSize**

Dimension of embedding vectors

```csharp
public int EmbeddingSize { get; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BatchSize**

Get the maximum batch size for this context

```csharp
public uint BatchSize { get; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **UBatchSize**

Get the physical maximum batch size for this context

```csharp
public uint UBatchSize { get; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **MaxSeq**

Get the number of maximum sequences allowed

```csharp
public uint MaxSeq { get; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **RecurrentRollbackSnapshots**

Get the number of recurrent-state snapshots per seq for rollback

```csharp
public uint RecurrentRollbackSnapshots { get; }
```

#### Property Value

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>

### **GenerationThreads**

Get or set the number of threads used for generation of a single token.

```csharp
public int GenerationThreads { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **BatchThreads**

Get or set the number of threads used for prompt and batch processing (multiple token).

```csharp
public int BatchThreads { get; set; }
```

#### Property Value

[Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

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

### **MemoryCanShift**

Check if the context supports memory shifting

```csharp
public bool MemoryCanShift { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsInvalid**

```csharp
public override bool IsInvalid { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsClosed**

```csharp
public bool IsClosed { get; }
```

#### Property Value

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **SafeLLamaContextHandle()**

```csharp
public SafeLLamaContextHandle()
```

## Methods

### **ReleaseHandle()**

```csharp
protected override bool ReleaseHandle()
```

#### Returns

[Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

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

### **SetCausalAttention(Boolean)**

Set whether to use causal attention or not. If set to true, the model will only attend to the past tokens

```csharp
public void SetCausalAttention(bool value)
```

#### Parameters

`value` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **SetEmbeddings(Boolean)**

Set whether the context outputs embeddings or not

```csharp
public void SetEmbeddings(bool value)
```

#### Parameters

`value` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If true, embeddings will be returned but logits will not

### **SetLoraAdapters(Span&lt;(LoraAdapter, Single)&gt;)**

Set the LoRa adapters on the context

```csharp
public void SetLoraAdapters(Span<(LoraAdapter Adapter, float Scale)> adapters)
```

#### Parameters

`adapters` [Span&lt;(LoraAdapter, Single)&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>

#### Exceptions

[ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception)<br>

### **GetLogits(Int32)**

Token logits obtained from the last call to llama_decode.
 The logits for the last token are stored in the last row.
 Only tokens with `logits = true` requested are present.<br>
 Can be mutated in order to change the probabilities of the next token.<br>
 Rows: n_tokens<br>
 Cols: n_vocab

```csharp
public Span<float> GetLogits(int numTokens = 1)
```

#### Parameters

`numTokens` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>
The amount of tokens whose logits should be retrieved, in [numTokens X n_vocab] format.<br>
 Tokens' order is based on their order in the LlamaBatch (so, first tokens are first, etc).<br>
 This is helpful when requesting logits for many tokens in a sequence, or want to decode multiple sequences in one go.

#### Returns

[Span&lt;Single&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **GetLogitsIth(Int32)**

Logits for the ith token. Equivalent to: llama_get_logits(ctx) + i*n_vocab

```csharp
public Span<float> GetLogitsIth(int i)
```

#### Parameters

`i` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

#### Returns

[Span&lt;Single&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>

### **GetEmbeddingsIth(LLamaPos)**

Get the embeddings for the ith sequence.
 Equivalent to: llama_get_embeddings(ctx) + ctx-&gt;output_ids[i]*n_embd

```csharp
public Span<float> GetEmbeddingsIth(LLamaPos pos)
```

#### Parameters

`pos` [LLamaPos](./llama.native.llamapos.md)<br>

#### Returns

[Span&lt;Single&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>
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

[Span&lt;Single&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A pointer to the first float in an embedding, length = ctx.EmbeddingSize

### **Tokenize(String, Boolean, Boolean, Encoding)**

Convert the given text into tokens

```csharp
public LLamaToken[] Tokenize(string text, bool add_bos, bool special, Encoding encoding)
```

#### Parameters

`text` [String](https://learn.microsoft.com/en-us/dotnet/api/system.string)<br>
The text to tokenize

`add_bos` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether the "BOS" token should be added

`special` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Allow tokenizing special and/or control tokens which otherwise are not exposed and treated as plaintext.

`encoding` [Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>
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

`dest` [Span&lt;Byte&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.span-1)<br>
A span to attempt to write into. If this is too small nothing will be written

#### Returns

[UInt32](https://learn.microsoft.com/en-us/dotnet/api/system.uint32)<br>
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
0 = success <br>&lt; 0 = error (the memory state is restored to the state before this call)

### **Decode(LLamaBatch)**

Process a batch of tokens.
 Requires the context to have a memory.
 For encode-decoder contexts, processes the batch using the decoder.
 Positive return values does not mean a fatal error, but rather a warning.
 Upon fatal-error or abort, the ubatches that managed to be been processed will remain in the memory state of the context
 To handle this correctly, query the memory state using llama_memory_seq_pos_min() and llama_memory_seq_pos_max()
 Upon other return values, the memory state is restored to the state before this call
 0 - success
 1 - could not find a memory slot for the batch (try reducing the size of the batch or increase the context)
 2 - aborted (processed ubatches will remain in the context's memory)
 -1 - invalid input batch
 &lt; -1 - fatal error (processed ubatches will remain in the context's memory)

```csharp
public DecodeResult Decode(LLamaBatch batch)
```

#### Parameters

`batch` [LLamaBatch](./llama.native.llamabatch.md)<br>

#### Returns

[DecodeResult](./llama.native.decoderesult.md)<br>

### **Decode(LLamaBatchEmbeddings)**

Process a batch of tokens.
 Requires the context to have a memory.
 For encode-decoder contexts, processes the batch using the decoder.
 Positive return values does not mean a fatal error, but rather a warning.
 Upon fatal-error or abort, the ubatches that managed to be been processed will remain in the memory state of the context
 To handle this correctly, query the memory state using llama_memory_seq_pos_min() and llama_memory_seq_pos_max()
 Upon other return values, the memory state is restored to the state before this call
 0 - success
 1 - could not find a memory slot for the batch (try reducing the size of the batch or increase the context)
 2 - aborted (processed ubatches will remain in the context's memory)
 -1 - invalid input batch
 &lt; -1 - fatal error (processed ubatches will remain in the context's memory)

```csharp
public DecodeResult Decode(LLamaBatchEmbeddings batch)
```

#### Parameters

`batch` [LLamaBatchEmbeddings](./llama.native.llamabatchembeddings.md)<br>

#### Returns

[DecodeResult](./llama.native.decoderesult.md)<br>

### **GetStateSize()**

Get the size of the state, when saved as bytes

```csharp
public UIntPtr GetStateSize()
```

#### Returns

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

### **GetStateSize(LLamaSeqId)**

Get the size of the memory state for a single sequence ID, when saved as bytes

```csharp
public UIntPtr GetStateSize(LLamaSeqId sequence)
```

#### Parameters

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

#### Returns

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>

### **GetState(Byte*, UIntPtr)**

Get the raw state of this context, encoded as bytes. Data is written into the `dest` pointer.

```csharp
public UIntPtr GetState(Byte* dest, UIntPtr size)
```

#### Parameters

`dest` [Byte*](https://learn.microsoft.com/en-us/dotnet/api/system.byte*)<br>
Destination to write to

`size` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes available to write to in dest (check required size with `GetStateSize()`)

#### Returns

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
The number of bytes written to dest

#### Exceptions

[ArgumentOutOfRangeException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception)<br>
Thrown if dest is too small

### **GetState(Byte*, UIntPtr, LLamaSeqId)**

Get the raw state of a single sequence from this context, encoded as bytes. Data is written into the `dest` pointer.

```csharp
public UIntPtr GetState(Byte* dest, UIntPtr size, LLamaSeqId sequence)
```

#### Parameters

`dest` [Byte*](https://learn.microsoft.com/en-us/dotnet/api/system.byte*)<br>
Destination to write to

`size` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes available to write to in dest (check required size with `GetStateSize()`)

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>
The sequence to get state data for

#### Returns

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
The number of bytes written to dest

### **SetState(Byte*, UIntPtr)**

Set the raw state of this context

```csharp
public UIntPtr SetState(Byte* src, UIntPtr size)
```

#### Parameters

`src` [Byte*](https://learn.microsoft.com/en-us/dotnet/api/system.byte*)<br>
The pointer to read the state from

`size` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes that can be safely read from the pointer

#### Returns

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes read from the src pointer

### **SetState(Byte*, UIntPtr, LLamaSeqId)**

Set the raw state of a single sequence

```csharp
public UIntPtr SetState(Byte* src, UIntPtr size, LLamaSeqId sequence)
```

#### Parameters

`src` [Byte*](https://learn.microsoft.com/en-us/dotnet/api/system.byte*)<br>
The pointer to read the state from

`size` [UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
Number of bytes that can be safely read from the pointer

`sequence` [LLamaSeqId](./llama.native.llamaseqid.md)<br>
Sequence ID to set

#### Returns

[UIntPtr](https://learn.microsoft.com/en-us/dotnet/api/system.uintptr)<br>
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

### **MemoryClear(Boolean)**

Clear the memory

```csharp
public void MemoryClear(bool data = true)
```

#### Parameters

`data` [Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean)<br>
If true, the data buffers will also be cleared together with the metadata

### **MemorySequenceRemove(LLamaSeqId, LLamaPos, LLamaPos)**

Removes all tokens that belong to the specified sequence and have positions in [p0, p1)

```csharp
public void MemorySequenceRemove(LLamaSeqId seq, LLamaPos p0, LLamaPos p1)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

### **MemorySequenceCopy(LLamaSeqId, LLamaSeqId, LLamaPos, LLamaPos)**

Copy all tokens that belong to the specified sequence to another sequence. Note that
 this does not allocate extra memory - it simply assigns the tokens to the
 new sequence

```csharp
public void MemorySequenceCopy(LLamaSeqId src, LLamaSeqId dest, LLamaPos p0, LLamaPos p1)
```

#### Parameters

`src` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`dest` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

### **MemorySequenceKeep(LLamaSeqId)**

Removes all tokens that do not belong to the specified sequence

```csharp
public void MemorySequenceKeep(LLamaSeqId seq)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

### **MemorySequenceAdd(LLamaSeqId, LLamaPos, LLamaPos, Int32)**

Adds relative position "delta" to all tokens that belong to the specified sequence
 and have positions in [p0, p1)

```csharp
public void MemorySequenceAdd(LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int delta)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

`delta` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MemorySequenceDivide(LLamaSeqId, LLamaPos, LLamaPos, Int32)**

Integer division of the positions by factor of `d &gt; 1`.<br>
 p0 &lt; 0 : [0, p1]<br>
 p1 &lt; 0 : [p0, inf)

```csharp
public void MemorySequenceDivide(LLamaSeqId seq, LLamaPos p0, LLamaPos p1, int divisor)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

`p0` [LLamaPos](./llama.native.llamapos.md)<br>

`p1` [LLamaPos](./llama.native.llamapos.md)<br>

`divisor` [Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32)<br>

### **MemorySequenceMinPosition(LLamaSeqId)**

Returns the smallest position present in memory for the specified sequence

```csharp
public LLamaPos MemorySequenceMinPosition(LLamaSeqId seq)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

#### Returns

[LLamaPos](./llama.native.llamapos.md)<br>

### **MemorySequenceMaxPosition(LLamaSeqId)**

Returns the largest position present in memory for the specified sequence

```csharp
public LLamaPos MemorySequenceMaxPosition(LLamaSeqId seq)
```

#### Parameters

`seq` [LLamaSeqId](./llama.native.llamaseqid.md)<br>

#### Returns

[LLamaPos](./llama.native.llamapos.md)<br>

---

[`< Back`](./)
